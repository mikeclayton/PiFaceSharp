using Kingsland.PiFaceSharp.Isr;
using Kingsland.PiFaceSharp.Spi.Native;
using System;
using System.ComponentModel;

namespace Kingsland.PiFaceSharp.Spi
{

    /// <summary>
    /// Implements a wrapper around a physical PiFace device attached to a Raspberry Pi.
    /// </summary>
    /// <see cref="https://github.com/WiringPi/WiringPi/blob/master/wiringPi/wiringPiFace.c"/>
    public sealed class PiFaceDevice : IISRPiFaceDevice, IDisposable
    {

        #region Constants

        private const byte CMD_WRITE = 0x40;
        private const byte CMD_READ = 0x41;

        #endregion

        #region Fields

        private PiFacePullUpMode _portBPullUpMode;
        private GpioEdgeDetector _EdgeDetector;

        #endregion

        #region Events

        public event EventHandler<InputsChangedEventArgs> InputsChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new PiFace object using the default device name.
        /// </summary>
        public PiFaceDevice()
            : this("/dev/spidev0.0")
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
            this.SpiDevice = new HardwareSpiDevice(0, 0, deviceName);
            this.Initialize();
        }

        /// <summary>
        /// Creates a new PiFace object using the specified spi device.
        /// </summary>
        /// <param name="spiDevice">
        /// The spi device to connect to.
        /// </param>
        [Browsable(false)]
        public PiFaceDevice(ISpiDevice spiDevice)
        {
            this.SpiDevice = spiDevice;
            this.Initialize();
        }

        /// <summary>
        /// Creates a new PiFace object using the default device name and registers an interrupt service routine on PiFace inputs.
        /// </summary>
        /// <param name="enableInputInterruptMask">
        /// Byte mask for input pins to enable interrupt.
        /// </param>
        /// <param name="interruptGpioPin">
        /// Raspberry Pi GPIO Pin which is connected to PiFace interrupt signaling pin (default 25).
        /// </param>
        /// <param name="edge">
        /// <see cref="EdgeDetectionMode"/> (rising/falling/both) specifies which signal edge should be detected (default falling).
        /// </param>
        /// <remarks>
        /// Interrupt handling is only enabled if enableInputInterruptMask is > 0 and edge is not None.
        /// interruptGpioPin must be preconfigured as an input on Raspberry Pi GPIO.
        /// </remarks>
        public PiFaceDevice(byte enableInputInterruptMask, byte interruptGpioPin = 25, EdgeDetectionMode edge = EdgeDetectionMode.falling)
            : this()
        {
            if (enableInputInterruptMask > 0 && edge != EdgeDetectionMode.none)
            {
                InitializeEdgeDetection(enableInputInterruptMask);
                this.EdgeDetector = new GpioEdgeDetector(interruptGpioPin, edge);
            }
        }

        /// <summary>
        /// Creates a new PiFace object using the specified device name and registers an interrupt service routine on PiFace inputs.
        /// </summary>
        /// <param name="deviceName">
        /// The name of the device to connect to.
        /// </param>
        /// <param name="enableInputInterruptMask">
        /// Byte mask for input pins to enable interrupt.
        /// </param>
        /// <param name="interruptGpioPin">
        /// Raspberry Pi GPIO Pin which is connected to PiFace interrupt signaling pin (default 25).
        /// </param>
        /// <param name="edge">
        /// <see cref="EdgeDetectionMode"/> (rising/falling/both) specifies which signal edge should be detected (default falling).
        /// </param>
        /// <remarks>
        /// Interrupt handling is only enabled if enableInputInterruptMask is > 0 and edge is not None.
        /// interruptGpioPin must be preconfigured as an input on Raspberry Pi GPIO.
        /// </remarks>
        public PiFaceDevice(string deviceName, byte enableInputInterruptMask, byte interruptGpioPin = 25, EdgeDetectionMode edge = EdgeDetectionMode.falling)
            : this(deviceName)
        {
            if (enableInputInterruptMask > 0 && edge != EdgeDetectionMode.none)
            {
                InitializeEdgeDetection(enableInputInterruptMask);
                this.EdgeDetector = new GpioEdgeDetector(interruptGpioPin, edge);
            }
        }

        /// <summary>
        /// Creates a new PiFace object using the specified spi device and registers an interrupt service routine on PiFace inputs.
        /// </summary>
        /// <param name="spiDevice">
        /// The spi device to connect to.
        /// </param>
        /// <param name="enableInputInterruptMask">
        /// Byte mask for input pins to enable interrupt.
        /// </param>
        /// <param name="interruptGpioPin">
        /// Raspberry Pi GPIO Pin which is connected to PiFace interrupt signaling pin (default 25).
        /// </param>
        /// <param name="edge">
        /// <see cref="EdgeDetectionMode"/> (rising/falling/both) specifies which signal edge should be detected (default falling).
        /// </param>
        /// <remarks>
        /// Interrupt handling is only enabled if enableInputInterruptMask is > 0 and edge is not None.
        /// interruptGpioPin must be preconfigured as an input on Raspberry Pi GPIO.
        /// </remarks>
        [Browsable(false)]
        public PiFaceDevice(ISpiDevice spiDevice, byte enableInputInterruptMask, byte interruptGpioPin = 25, EdgeDetectionMode edge = EdgeDetectionMode.falling)
            : this(spiDevice)
        {
            if (enableInputInterruptMask > 0 && edge != EdgeDetectionMode.none)
            {
                InitializeEdgeDetection(enableInputInterruptMask);
                this.EdgeDetector = new GpioEdgeDetector(interruptGpioPin, edge);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets an SpiDevice that represents the SPI interface for this device.
        /// </summary>
        private ISpiDevice SpiDevice
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an GpioEdgeDetector to detect 
        /// </summary>
        private GpioEdgeDetector EdgeDetector
        {
            get
            {
                return _EdgeDetector;
            }
            set
            {
                if (_EdgeDetector != null)
                {
                    _EdgeDetector.InterruptOccured -= _EdgeDetector_InterruptOccured;
                }
                _EdgeDetector = value;
                if (value != null)
                {
                    value.InterruptOccured += _EdgeDetector_InterruptOccured;
                }
            }
        }

        /// <summary>
        /// Gets or sets the last-written state of the output pins.
        /// </summary>
        /// <remarks>
        /// We can't write bit states for individual pins on the MCP23S17, so
        /// this property caches the last known value that we wrote. Otherwise
        /// we'd have to read the states back from it every time we want to
        /// change a single pin's state.
        /// </remarks>
        private byte OutputPinBuffer
        {
            get;
            set;
        }

        private PiFacePullUpMode PortBPullUpMode
        {
            get
            {
                return _portBPullUpMode;
            }
            set
            {
                switch (value)
                {
                    case PiFacePullUpMode.PullUp:
                        this.SpiDevice.WriteByte((byte)PiFaceRegisterAddress.GPPUB, 0xFF);
                        break;
                    case PiFacePullUpMode.PullDown:
                        this.SpiDevice.WriteByte((byte)PiFaceRegisterAddress.GPPUB, 0x00);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
                _portBPullUpMode = value;
            }
        }

        public bool IsISREnabled
        {
            get
            {
                return (_EdgeDetector != null);
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
            if (pin > 7)
            {
                throw new ArgumentOutOfRangeException("pin", "pin must be in the range 0-7");
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
                throw new ArgumentOutOfRangeException("pin", "pin must be in the range 0-7");
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
                throw new ArgumentOutOfRangeException("pin", "pin must be in the range 0-7");
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
        #endregion

        #region Interrupt Methods

        /// <summary>
        /// Enables the Interrupts on the sepcified PiFace input pins.
        /// </summary>
        /// <param name="bitMask">Byte mask to enable interrupt on pins.</param>
        private void SetInputInterrupts(byte bitMask)
        {
            this.SpiDevice.WriteByte((byte)PiFaceRegisterAddress.GPINTENB, bitMask);
        }

        /// <summary>
        /// Gets the Interrupt enabled states for PiFace input pins.
        /// </summary>
        /// <returns>Byte mask with interrupt enabled input pins.</returns>
        private byte GetInputInterrupts()
        {
            return this.SpiDevice.ReadByte((byte)PiFaceRegisterAddress.GPINTENB);
        }

        /// <summary>
        /// Gets the input interrupt latch.
        /// </summary>
        /// <returns>Byte mask with input pins that caused the interrupt.</returns>
        private byte GetInputInterruptFlags()
        {
            return this.SpiDevice.ReadByte((byte)PiFaceRegisterAddress.INTFB);
        }

        private void _EdgeDetector_InterruptOccured(object sender, EventArgs e)
        {
            // read Interrupt latch (byte mask which pins caused the interrupt)
            // and current pin states.
            // Important to reset the interrupt flags on PiFace and Raspberry Pi!
            byte latch = GetInputInterruptFlags();
            byte currentStates = GetInputPinStates();
            OnInputStatesChanged(latch, currentStates);
        }

        /// <summary>
        /// Raises the <see cref="InputsChanged"/> event.
        /// </summary>
        /// <param name="InterruptLatch">Byte mask specifying which pin caused the interrupt.</param>
        /// <param name="InputPinStates">Byte mask specifying the pin states at interrupt.</param>
        private void OnInputStatesChanged(byte InterruptLatch, byte InputPinStates)
        {
            if (InputsChanged != null)
            {
                InputsChangedEventArgs ie = new InputsChangedEventArgs(InterruptLatch, InputPinStates);
                InputsChanged(this, ie);
            }
        }

        #endregion

        #region Setup Methods

        /// <summary>
        /// Setup the SPI interface and initialise the MCP23S17 chip.
        /// </summary>
        /// <returns></returns>
        private int Initialize()
        {
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

        private void InitializeEdgeDetection(byte enableInputInterruptMask)
        {
            // set PiFace Interrupt on all inputs
            SetInputInterrupts(enableInputInterruptMask);
        }

        #endregion

        #region "IDisposable implementation"

        private bool disposed = false;

        void IDisposable.Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    if (this._EdgeDetector != null)
                    {
                        try
                        {
                            this.SetInputInterrupts(0);
                            ((IDisposable)this._EdgeDetector).Dispose();
                        }
                        finally
                        {
                            this._EdgeDetector = null;
                        }
                    }

                    if (this.SpiDevice != null)
                    {
                        try
                        {
                            this.SetOutputPinStates(0);
                            this.SpiDevice.Close();
                        }
                        finally
                        {
                            this.SpiDevice = null;
                        }
                    }
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion
    }

}
