using System;
using Kingsland.PiFaceSharp.Spi.Native;

namespace Kingsland.PiFaceSharp.Spi
{

    /// <summary>
    /// Implements a wrapper around a physical PiFace device attached to a Raspberry Pi.
    /// </summary>
    /// <see cref="https://github.com/WiringPi/WiringPi/blob/master/wiringPi/wiringPiFace.c"/>
    public sealed class PiFaceDevice : IPiFaceDevice
    {

        #region Constants

        /// <summary>
        /// The default name of the PiFace device.
        /// </summary>
        public const string DefaultDeviceName = "/dev/spidev0.0";

        private const byte CMD_WRITE = 0x40;
        private const byte CMD_READ = 0x41;

        // register addresses
        // see https://github.com/piface/pifacecommon/blob/master/pifacecommon/mcp23s17.py
        private enum RegisterAddress : byte
        {
            IODIRA = 0x00,    // I/O direction A
            IODIRB = 0x01,    // I/O direction B
            IPOLA = 0x02,     // I/O polarity A
            IPOLB = 0x03,     // I/O polarity B
            GPINTENA = 0x04,  // interupt enable A
            GPINTENB = 0x05,  // interupt enable B
            DEFVALA = 0x06,   // register default value A (interupts)
            DEFVALB = 0x07,   // register default value B (interupts)
            INTCONA = 0x08,   // interupt control A
            INTCONB = 0x09,   // interupt control B
            IOCON = 0x0A,     // I/O config (also 0xB)
            GPPUA = 0x0C,     // port A pullups
            GPPUB = 0x0D,     // port B pullups
            INTFA = 0x0E,     // interupt flag A (where the interupt came from)
            INTFB = 0x0F,     // interupt flag B
            INTCAPA = 0x10,   // interupt capture A (value at interupt is saved here)
            INTCAPB = 0x11,   // interupt capture B
            GPIOA = 0x12,     // port A
            GPIOB = 0x13,     // port B
            OLATA = 0x14,     // output latch A
            OLATB = 0x15      // output latch B
        }

        // i/o config
        // from https://github.com/piface/pifacecommon/blob/master/pifacecommon/mcp23s17.py
        [Flags]
        private enum IoConfig : byte
        {
            BANK_OFF = 0x00,        // addressing mode
            BANK_ON = 0x80,
            INT_MIRROR_ON = 0x40,   // interupt mirror (INTa|INTb)
            INT_MIRROR_OFF = 0x00,
            SEQOP_OFF = 0x20,       // incrementing address pointer
            SEQOP_ON = 0x00,
            DISSLW_ON = 0x10,       // slew rate
            DISSLW_OFF = 0x00,
            HAEN_ON = 0x08,         // hardware addressing
            HAEN_OFF = 0x00,
            ODR_ON = 0x04,          // open drain for interupts
            ODR_OFF = 0x00,
            INTPOL_HIGH = 0x02,     // interupt polarity
            INTPOL_LOW = 0x00
        }


        private enum PullUpMode
        {
            Off = 0,
            PullDown = 1,
            PullUp = 2
        }

        #endregion

        #region Fields

        private PullUpMode _mPortBPullUpMode;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new PiFace object using the default device name.
        /// </summary>
        public PiFaceDevice()
            : this(PiFaceDevice.DefaultDeviceName)
        {
        }

        /// <summary>
        /// Creates a new PiFace object using the specified device name.
        /// </summary>
        /// <param name="deviceName">
        /// The name of the device to connect to.
        /// </param>
        public PiFaceDevice(string deviceName)
        {
            this.DeviceName = deviceName;
            this.Initialize();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the device this object is connected to.
        /// </summary>
        public string DeviceName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets an SpiDevice that represents the SPI interface.
        /// </summary>
        private SpiDevice SpiDevice
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last-written state of the output pins.
        /// We can't write bit states for individual pins on the MCP23S17, so
        /// this property caches the last known value that we wrote. Otherwise
        /// we'd have to read the states back from it every time we want to
        /// change a single pin's state.
        /// </summary>
        private byte OutputPinBuffer
        {
            get;
            set;
        }

        private PullUpMode PortBPullUpMode
        {
            get
            {
                return _mPortBPullUpMode;
            }
            set
            {
                switch(value)
                {
                    case PullUpMode.PullUp:
                        this.SpiDevice.WriteByte((byte)RegisterAddress.GPPUB, 0xFF);
                        break;
                    case PullUpMode.PullDown:
                        this.SpiDevice.WriteByte((byte)RegisterAddress.GPPUB, 0x00);
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException("value");
                }
                _mPortBPullUpMode = value;
            }
        }

        #endregion
        
        #region Pin State Methods

        /// <summary>
        /// Gets the state of a single output pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public bool GetOutputPinState(byte pin)
        {
            if(pin > 7)
            {
                throw new System.ArgumentOutOfRangeException("pin", "pin must be in the range 0-7");
            }
            var mask = (byte)(1 << pin);
            var state = this.SpiDevice.ReadByte((byte)RegisterAddress.GPIOA);
            this.OutputPinBuffer = state;
            return (state & mask) == mask;
        }

        /// <summary>
        /// Gets the bitmask containing the state of all output pins.
        /// </summary>
        /// <returns>
        /// A bitmask containing true for each output pin that is HIGH, and false if it is LOW.
        /// </returns>
        public byte GetOutputPinStates()
        {
            var state = this.SpiDevice.ReadByte((byte)RegisterAddress.GPIOA);
            this.OutputPinBuffer = state;
            return state;
        }

        /// <summary>
        /// Update the state of a single output pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="enabled"></param>
        public void SetOutputPinState(byte pin, bool enabled)
        {
            if (pin > 7)
            {
                throw new System.ArgumentOutOfRangeException("pin", "pin must be in the range 0-7");
            }
            var mask = (byte)(1 << pin);
            if (enabled)
            {
                this.OutputPinBuffer |= mask;
            }
            else
            {
                this.OutputPinBuffer &= (byte)~mask;
            }
            this.SpiDevice.WriteByte((byte)RegisterAddress.GPIOA, this.OutputPinBuffer);
        }

        /// <summary>
        /// Update the state of all output pins using a bitmask
        /// containing the state for each pin.
        /// </summary>
        /// <param name="bitMask"></param>
        public void SetOutputPinStates(byte bitMask)
        {
            this.SpiDevice.WriteByte((byte)RegisterAddress.GPIOA, bitMask);
        }

        /// <summary>
        /// Gets the state of a single input pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <returns>
        /// True if the specified pin is HIGH, or false if it is LOW.
        /// </returns>
        public bool GetInputPinState(byte pin)
        {
            if (pin > 7)
            {
                throw new System.ArgumentOutOfRangeException("pin", "pin must be in the range 0-7");
            }
            var mask = (byte)(1 << pin);
            var state = this.SpiDevice.ReadByte((byte)RegisterAddress.GPIOB);
            switch (this.PortBPullUpMode)
            {
                case PullUpMode.PullUp:
                    return ((state & mask) == 0);
                case PullUpMode.PullDown:
                default:
                    throw new System.InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets a bitmask containing the state of all input pins.
        /// </summary>
        /// <returns>
        /// A bitmask containing true for each input pin that is HIGH, and false if it is LOW.
        /// </returns>
        public byte GetInputPinStates()
        {
            var state = this.SpiDevice.ReadByte((byte)RegisterAddress.GPIOB);
            return state;
        }

        /// <summary>
        /// Update the state of a single input pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="enabled"></param>
        /// <remarks>
        /// This method is provided to support the PiFaceEmulator, and will 
        /// throw an exception if called on a physical PiFaceDevice.
        /// </remarks>
        public void SetInputPinState(byte pin, bool enabled)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Update the state of all input pins using a bitmask
        /// containing the state for each pin.
        /// </summary>
        /// <param name="bitMask"></param>
        /// <remarks>
        /// This method is provided to support the PiFaceEmulator, and will 
        /// throw an exception if called on a physical PiFaceDevice.
        /// </remarks>
        public void SetInputPinStates(byte bitMask)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Setup Methods

        /// <summary>
        /// Setup the SPI interface and initialise the MCP23S17 chip.
        /// </summary>
        /// <returns></returns>
        private int Initialize()
        {
            this.SpiDevice = new SpiDevice(0, 0, this.DeviceName);
            this.SpiDevice.Open(FCntl.O_RDWR);
            //// set the SPI parameters
            this.SpiDevice.SetMode(SpiDev.SPI_MODE_0);
            this.SpiDevice.SetBitsPerWord(8);
            this.SpiDevice.SetMaxSpeedHz(5000000);
            // send some initialization controls to the MCP23S17. note the PiFace Emulator sends the following
            // commands when it starts up:
            //     cmd: WRITE, port: IOCON,  data: 0x8
            //     cmd: WRITE, port: GPIOA,  data: 0x0
            //     cmd: WRITE, port: IODIRA, data: 0x0
            //     cmd: WRITE, port: IODIRB, data: 0xff
            //     cmd: WRITE, port: GPPUB,  data: 0xff
            //     cmd: WRITE, port: GPIOA,  data: 0x0
            const IoConfig flags = IoConfig.HAEN_ON;
            this.SpiDevice.WriteByte((byte)RegisterAddress.IOCON, (byte)flags);
            this.SetOutputPinStates(0);
            this.SpiDevice.WriteByte((byte)RegisterAddress.IODIRA, 0x00); // initialise Port A pins for output
            this.SpiDevice.WriteByte((byte)RegisterAddress.IODIRB, 0xFF); // initialise Port B pins for input
            this.PortBPullUpMode = PullUpMode.PullUp; // set pull up mode on input pins
            this.SetOutputPinStates(0);
            return 0;
        }

        #endregion

    }

}
