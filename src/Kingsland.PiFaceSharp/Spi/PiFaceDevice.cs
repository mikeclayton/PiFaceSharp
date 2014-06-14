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

        #endregion

        #region Fields

        private PiFacePullUpMode _mPortBPullUpMode;

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

        private PiFacePullUpMode PortBPullUpMode
        {
            get
            {
                return _mPortBPullUpMode;
            }
            set
            {
                switch(value)
                {
                    case PiFacePullUpMode.PullUp:
                        this.SpiDevice.WriteByte((byte)PiFaceRegisterAddress.GPPUB, 0xFF);
                        break;
                    case PiFacePullUpMode.PullDown:
                        this.SpiDevice.WriteByte((byte)PiFaceRegisterAddress.GPPUB, 0x00);
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
            var state = this.SpiDevice.ReadByte((byte)PiFaceRegisterAddress.GPIOA);
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
            var state = this.SpiDevice.ReadByte((byte)PiFaceRegisterAddress.GPIOA);
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
            this.SpiDevice.WriteByte((byte)PiFaceRegisterAddress.GPIOA, this.OutputPinBuffer);
        }

        /// <summary>
        /// Update the state of all output pins using a bitmask
        /// containing the state for each pin.
        /// </summary>
        /// <param name="bitMask"></param>
        public void SetOutputPinStates(byte bitMask)
        {
            this.SpiDevice.WriteByte((byte)PiFaceRegisterAddress.GPIOA, bitMask);
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
            var state = this.SpiDevice.ReadByte((byte)PiFaceRegisterAddress.GPIOB);
            switch (this.PortBPullUpMode)
            {
                case PiFacePullUpMode.PullUp:
                    return ((state & mask) == 0);
                case PiFacePullUpMode.PullDown:
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
            var state = this.SpiDevice.ReadByte((byte)PiFaceRegisterAddress.GPIOB);
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
            const PiFaceIoConfigFlags flags = PiFaceIoConfigFlags.HAEN_ON;
            this.SpiDevice.WriteByte((byte)PiFaceRegisterAddress.IOCON, (byte)flags);
            this.SetOutputPinStates(0);
            this.SpiDevice.WriteByte((byte)PiFaceRegisterAddress.IODIRA, 0x00); // initialise Port A pins for output
            this.SpiDevice.WriteByte((byte)PiFaceRegisterAddress.IODIRB, 0xFF); // initialise Port B pins for input
            this.PortBPullUpMode = PiFacePullUpMode.PullUp; // set pull up mode on input pins
            this.SetOutputPinStates(0);
            return 0;
        }

        #endregion

    }

}
