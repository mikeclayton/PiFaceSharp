using System;
using System.Runtime.InteropServices;
using Kingsland.PiFaceSharp.Spi.Native;
using System.IO;

namespace Kingsland.PiFaceSharp.Spi
{

    public sealed class HardwareSpiDevice : ISpiDevice, IDisposable
    {

        #region Fields

        private const int TxRxBufferLength = 3;
        private IntPtr _txBufferPtr = IntPtr.Zero;
        private IntPtr _rxBufferPtr = IntPtr.Zero;

        private byte SPI_SLAVE_ID = 0x40;
        private const byte SPI_SLAVE_ADDR = 0;
        private const byte SPI_SLAVE_MSG_END = 0x0E;
        private const byte SPI_SLAVE_READ = 1;
        private const byte SPI_SLAVE_WRITE = 0;

        #endregion

        #region Constructors

        public HardwareSpiDevice(uint bus, uint chipSelect, string deviceName, byte slaveId = 0x40)
        {
            this.Bus = bus;
            this.ChipSelect = chipSelect;
            this.SpiDelay = 0;
            this.DeviceName = deviceName;
            this.SPI_SLAVE_ID = slaveId;
        }

        ~HardwareSpiDevice()
        {
            this.Dispose(false);
        }
        #endregion

        #region Properties

        public uint Bus
        {
            get;
            private set;
        }

        public uint ChipSelect
        {
            get;
            private set;
        }

        public string DeviceName
        {
            get;
            private set;
        }

        private int DeviceHandle
        {
            get;
            set;
        }

        private uint Mode
        {
            get;
            set;
        }

        private byte BitsPerWord
        {
            get;
            set;
        }

        private uint MaxSpeedHz
        {
            get;
            set;
        }

        private ushort SpiDelay
        {
            get;
            set;
        }

        #endregion

        #region Methods

        private void InitTxRxBuffers()
        {
            // create unmanaged transmit and receive buffers
            this._txBufferPtr = Marshal.AllocHGlobal(3);
            this._rxBufferPtr = Marshal.AllocHGlobal(3);
        }

        private void FreeTxRxBuffers()
        {
            // release unmanaged transmit and receive buffers
            if (this._txBufferPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this._txBufferPtr);
            }
            if (this._txBufferPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this._rxBufferPtr);
            }
        }

        #endregion

        #region ISpiDevice Interface

        public void Open(int flags)
        {
            if (this.DeviceHandle != 0)
            {
                throw new IOException("Device is already open.");
            }
            var result = FCntl.open(this.DeviceName, flags);
            if (result < 0)
            {
                throw new IOException(string.Format("Failed to open device - error {0}.", result));
            }
            this.DeviceHandle = result;
            this.InitTxRxBuffers();
        }

        public void Close()
        {
            if (this.DeviceHandle == 0)
            {
                throw new IOException("Device is already closed.");
            }
            var result = FCntl.close(this.DeviceHandle);
            if (result < 0)
            {
                throw new IOException(string.Format("Failed to close device - error {0}.", result));
            }
            this.DeviceHandle = 0;
            this.FreeTxRxBuffers();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public byte ReadByte(byte address)
        {
            byte buffer = SPI_SLAVE_ID;
            buffer |= (SPI_SLAVE_ADDR << 1 & SPI_SLAVE_MSG_END) | SPI_SLAVE_READ;
            //Console.WriteLine("    spiBufTx = {0} {1} {2}", CMD_READ, address, 0);
            Marshal.Copy(new byte[] { buffer, address, 0 }, 0, this._txBufferPtr, HardwareSpiDevice.TxRxBufferLength);
            Marshal.Copy(new byte[] { 0, 0, 0 }, 0, this._rxBufferPtr, HardwareSpiDevice.TxRxBufferLength);
            // build the command
            var cmd = SpiDev.SPI_IOC_MESSAGE(1);
            // build the spi transfer structure
            var spi = new SpiDev.spi_ioc_transfer
            {
                tx_buf = (UInt64)this._txBufferPtr.ToInt64(),
                rx_buf = (UInt64)this._rxBufferPtr.ToInt64(),
                len = HardwareSpiDevice.TxRxBufferLength,
                delay_usecs = this.SpiDelay,
                speed_hz = this.MaxSpeedHz,
                bits_per_word = (byte)this.BitsPerWord
            };
            // call the native method
            var result = IoCtl.ioctl(this.DeviceHandle, cmd, ref spi);
            if (result < 0)
            {
                var error = Mono.Unix.Native.Stdlib.GetLastError();
            }
            // return the result. every byte transmitted results in a
            // data or dummy byte received, so we have to skip the
            // leading dummy bytes to read out actual data bytes.
            var bufOut = new byte[HardwareSpiDevice.TxRxBufferLength];
            Marshal.Copy(this._txBufferPtr, bufOut, 0, bufOut.Length);
            Marshal.Copy(this._rxBufferPtr, bufOut, 0, bufOut.Length);
            return bufOut[2];
        }

        /// <summary>
        /// Write a value to the SPI bus.
        /// </summary><
        /// <param name="address"></param>
        /// <param name="value"></param>
        public void WriteByte(byte address, byte value)
        {
            byte buffer = SPI_SLAVE_ID;
            buffer |= (SPI_SLAVE_ADDR << 1 & SPI_SLAVE_MSG_END) | SPI_SLAVE_WRITE;
            Marshal.Copy(new byte[] { buffer, address, value }, 0, this._txBufferPtr, HardwareSpiDevice.TxRxBufferLength);
            Marshal.Copy(new byte[] { 0, 0, 0 }, 0, this._rxBufferPtr, HardwareSpiDevice.TxRxBufferLength);
            // build the command
            var cmd = SpiDev.SPI_IOC_MESSAGE(1);
            // build the spi transfer structure
            var spi = new SpiDev.spi_ioc_transfer
            {
                tx_buf = (UInt64)this._txBufferPtr.ToInt64(),
                rx_buf = (UInt64)this._rxBufferPtr.ToInt64(),
                len = HardwareSpiDevice.TxRxBufferLength,
                delay_usecs = this.SpiDelay,
                speed_hz = this.MaxSpeedHz,
                bits_per_word = (byte)this.BitsPerWord
            };
            // call the native method
            var result = IoCtl.ioctl(this.DeviceHandle, cmd, ref spi);
            if (result < 0)
            {
                var error = Mono.Unix.Native.Stdlib.GetLastError();
            }
        }

        public void SetMode(uint mode)
        {
            var value = mode;
            var result = 0;
            result = IoCtl.ioctl(this.DeviceHandle, SpiDev.SPI_IOC_WR_MODE, ref value);
            if (result < 0)
            {
                throw new InvalidOperationException(string.Format("Failed to set mode - error {0}", result));
            }
            // every tx results in an rx, so we have to read after every write)
            result = IoCtl.ioctl(this.DeviceHandle, SpiDev.SPI_IOC_WR_MODE, ref value);
            if (result < 0)
            {
                throw new InvalidOperationException(string.Format("Failed to get mode - error {0}", result));
            }
            this.Mode = mode;
        }

        public void SetBitsPerWord(byte bitsPerWord)
        {
            var value = (uint)bitsPerWord;
            var result = 0;
            result = IoCtl.ioctl(this.DeviceHandle, SpiDev.SPI_IOC_WR_BITS_PER_WORD, ref value);
            if (result < 0)
            {
                throw new InvalidOperationException(string.Format("Failed to set bits per word - error {0}", result));
            }
            // every tx results in an rx, so we have to read after every write
            result = IoCtl.ioctl(this.DeviceHandle, SpiDev.SPI_IOC_RD_BITS_PER_WORD, ref value);
            if (result < 0)
            {
                throw new InvalidOperationException(string.Format("Failed to get bits per word - error {0}", result));
            }
            this.BitsPerWord = bitsPerWord;
        }

        public void SetMaxSpeedHz(uint maxSpeedHz)
        {
            var value = maxSpeedHz;
            var result = 0;
            result = IoCtl.ioctl(this.DeviceHandle, SpiDev.SPI_IOC_WR_MAX_SPEED_HZ, ref value);
            if (result < 0)
            {
                throw new InvalidOperationException(string.Format("Failed to set max speed hz - error {0}", result));
            }
            // every tx results in an rx, so we have to read after every write
            result = IoCtl.ioctl(this.DeviceHandle, SpiDev.SPI_IOC_RD_MAX_SPEED_HZ, ref value);
            if (result < 0)
            {
                throw new System.InvalidOperationException(string.Format("Failed to get max speed hz - error {0}.", result));
            }
            this.MaxSpeedHz = maxSpeedHz;
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(Boolean disposing)
        {
            if (this.DeviceHandle != 0)
            {
                this.Close();
            }
        }

        #endregion

    }

}
