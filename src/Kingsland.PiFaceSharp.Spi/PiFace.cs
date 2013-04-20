using System;
using System.Runtime.InteropServices;
using Kingsland.PiFaceSharp.Spi.Native;

namespace Kingsland.PiFaceSharp.Spi
{

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="https://github.com/WiringPi/WiringPi/blob/master/wiringPi/wiringPiFace.c"/>
    public sealed class PiFace
    {

        #region Constants

        /// <summary>
        /// The default name of the PiFace device.
        /// </summary>
        public const string DefaultDeviceName = "/dev/spidev0.0";

        private const byte IOCON = 0x0A;

        private const byte IODIRA = 0x00;
        private const byte IPOLA = 0x02;
        private const byte GPINTENA = 0x04;
        private const byte DEFVALA = 0x06;
        private const byte INTCONA = 0x08;
        private const byte GPPUA = 0x0C;
        private const byte INTFA = 0x0E;
        private const byte INTCAPA = 0x10;
        private const byte GPIOA = 0x12;
        private const byte OLATA = 0x14;

        private const byte IODIRB = 0x01;
        private const byte IPOLB = 0x03;
        private const byte GPINTENB = 0x05;
        private const byte DEFVALB = 0x07;
        private const byte INTCONB = 0x09;
        private const byte GPPUB = 0x0D;
        private const byte INTFB = 0x0F;
        private const byte INTCAPB = 0x11;
        private const byte GPIOB = 0x13;
        private const byte OLATB = 0x15;

        private const byte IOCON_BANK_MODE = 0x80;
        private const byte IOCON_MIRROR = 0x40;
        private const byte IOCON_SEQOP = 0x20;
        private const byte IOCON_DISSLW = 0x10;
        private const byte IOCON_HAEN = 0x08;
        private const byte IOCON_ODR = 0x04;
        private const byte IOCON_INTPOL = 0x02;
        private const byte IOCON_UNUSED = 0x01;

        private const byte IOCON_INIT = IOCON_SEQOP;

        private const byte CMD_WRITE = 0x40;
        private const byte CMD_READ = 0x41;

        #endregion

        private UInt32 m_SpiMode = 0;
        private UInt32 m_SpiBPW = 8;
        private UInt32 m_SpiSpeed = 5000000;
        private UInt16 m_SpiDelay = 0;

        #region Constructors

        /// <summary>
        /// Creates a new PiFace object using the default device name.
        /// </summary>
        public PiFace()
            : this(PiFace.DefaultDeviceName)
        {
        }

        /// <summary>
        /// Creates a new PiFace object using the specified device name.
        /// </summary>
        /// <param name="deviceName">
        /// The name of the device to connect to.
        /// </param>
        public PiFace(string deviceName)
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
        /// Gets or sets a system handle to the device this object is connected to.
        /// </summary>
        private uint DeviceHandle
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

        #endregion

        #region Write Methods

        /// <summary>
        /// Write a value to the specified register on the PiFace's
        /// MCP23S17 using the SPI bus.
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="value"></param>
        private void WriteByte(byte pin, byte value)
        {
            // create unmanaged transmit and receive buffers
            var spiBufTx = Marshal.AllocHGlobal(3);
            var spiBufRx = Marshal.AllocHGlobal(3);
            Marshal.Copy(new byte[3] { CMD_WRITE, pin, value }, 0, spiBufTx, 3);
            Marshal.Copy(new byte[3] { 0, 0, 0 }, 0, spiBufRx, 3);
            // build the command
            var cmd = SpiDev.SPI_IOC_MESSAGE(1);
            // build the spi transfer structure
            var spi = new SpiDev.spi_ioc_transfer();
            spi.tx_buf = (UInt64)spiBufTx.ToInt64();
            spi.rx_buf = (UInt64)spiBufRx.ToInt64();
            spi.len = 3;
            spi.delay_usecs = this.m_SpiDelay;
            spi.speed_hz = this.m_SpiSpeed;
            spi.bits_per_word = (byte)this.m_SpiBPW;
            // call the native method
            var result = IoCtl.ioctl(this.DeviceHandle, cmd, ref spi);
            if (result < 0)
            {
                var error = Mono.Unix.Native.Stdlib.GetLastError();
            }
            // free up unmanaged buffers
            Marshal.FreeHGlobal(spiBufTx);
            Marshal.FreeHGlobal(spiBufRx);
        }

        /// <summary>
        /// Update the state of a single output pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="value"></param>
        public void SetOutputPinState(byte pin, bool enabled)
        {
            byte mask = (byte)(1 << pin);
            if (enabled)
            {
                this.OutputPinBuffer |= mask;
            }
            else
            {
                this.OutputPinBuffer &= (byte)~mask;
            }
            this.WriteByte(GPIOA, this.OutputPinBuffer);
        }

        /// <summary>
        /// Update the state of all output pins using a bitmask
        /// containing the state for each pin.
        /// </summary>
        /// <param name="bitMask"></param>
        public void SetOutputPinStates(byte bitMask)
        {
            this.WriteByte(GPIOA, bitMask);
        }

        #endregion

        #region Read Methods

        public byte ReadByte(byte pin)
        {
            // create unmanaged transmit and receive buffers
            var spiBufTx = Marshal.AllocHGlobal(3);
            var spiBufRx = Marshal.AllocHGlobal(3);
            Marshal.Copy(new byte[3] { CMD_READ, pin, 0 }, 0, spiBufTx, 3);
            Marshal.Copy(new byte[3] { 0, 0, 0 }, 0, spiBufRx, 3);
            // build the command
            var cmd = SpiDev.SPI_IOC_MESSAGE(1);
            // build the spi transfer structure
            var spi = new SpiDev.spi_ioc_transfer();
            spi.tx_buf = (UInt64)spiBufTx.ToInt64();
            spi.rx_buf = (UInt64)spiBufRx.ToInt64();
            spi.len = 3;
            spi.delay_usecs = this.m_SpiDelay;
            spi.speed_hz = this.m_SpiSpeed;
            spi.bits_per_word = (byte)this.m_SpiBPW;
            // call the native method
            var result = IoCtl.ioctl(this.DeviceHandle, cmd, ref spi);
            if (result < 0)
            {
                var error = Mono.Unix.Native.Stdlib.GetLastError();
            }
            // return the result. every byte transmitted results in a
            // data or dummy byte received, so we have to skip the
            // leading dummy bytes to read out actual data bytes.
            var bufOut = new byte[3];
            Marshal.Copy(spiBufRx, bufOut, 0, bufOut.Length);
            return bufOut[2];
        }

        public bool GetInputPinState(byte pin)
        {
            byte mask = (byte)(1 << pin);
            return (this.ReadByte(GPIOB) & mask) == mask;
        }

        #endregion

        #region Setup Methods

        /// <summary>
        /// Setup the SPI interface and initialise the MCP23S17 chip.
        /// </summary>
        /// <returns></returns>
        private int Initialize()
        {
            this.DeviceHandle = FCntl.open(this.DeviceName, FCntl.O_RDWR);
            if (this.DeviceHandle < 0)
            {
                return -1;
            }
            // set the SPI parameters
            // (note - every tx results in an rx, so we have to read after every write) 
            if (IoCtl.ioctl(this.DeviceHandle, SpiDev.SPI_IOC_WR_MODE, ref m_SpiMode) < 0)
            {
                return -1;
            }
            if (IoCtl.ioctl(this.DeviceHandle, SpiDev.SPI_IOC_RD_MODE, ref m_SpiMode) < 0)
            {
                return -1;
            }
            if (IoCtl.ioctl(this.DeviceHandle, SpiDev.SPI_IOC_WR_BITS_PER_WORD, ref m_SpiBPW) < 0)
            {
                return -1;
            }
            if (IoCtl.ioctl(this.DeviceHandle, SpiDev.SPI_IOC_RD_BITS_PER_WORD, ref m_SpiBPW) < 0)
            {
                return -1;
            }
            if (IoCtl.ioctl(this.DeviceHandle, SpiDev.SPI_IOC_WR_MAX_SPEED_HZ, ref m_SpiSpeed) < 0)
            {
                return -1;
            }
            if (IoCtl.ioctl(this.DeviceHandle, SpiDev.SPI_IOC_RD_MAX_SPEED_HZ, ref m_SpiSpeed) < 0)
            {
                return -1;
            }
            // send some initialization controls to the MCP23S17
            this.WriteByte(IOCON, IOCON_INIT);
            this.WriteByte(IODIRA, 0x00); // initialise output pins
            this.WriteByte(IODIRB, 0xFF); // initalise input pins
            return 0;
        }
        
        #endregion

    }

}
