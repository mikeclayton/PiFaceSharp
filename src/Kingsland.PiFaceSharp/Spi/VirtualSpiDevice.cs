using System.Collections.Generic;
using System.IO;

namespace Kingsland.PiFaceSharp.Spi
{

    public sealed class VirtualSpiDevice : ISpiDevice
    {

        #region Fields

        private Dictionary<byte, byte> _registers;

        #endregion

        #region Properties

        private bool IsOpen
        {
            get;
            set;
        }

        private Dictionary<byte, byte> Registers
        {
            get
            {
                if (_registers == null)
                {
                    _registers = new Dictionary<byte, byte>();
                }
                return _registers;
            }
        }

        #endregion

        #region ISpiDevice Interface

        public void Open(int flags)
        {
            if (this.IsOpen)
            {
                throw new IOException("Device is already open.");
            }
            this.IsOpen = true;
        }

        public void Close()
        {
            if (this.IsOpen)
            {
                throw new IOException("Device is already closed.");
            }
            this.IsOpen = false;
        }

        public byte ReadByte(byte address)
        {
            if (this.Registers.ContainsKey(address))
            {
                return this.Registers[address];
            }
            else
            {
                return 0;
            }
        }

        public void WriteByte(byte address, byte value)
        {
            if (this.Registers.ContainsKey(address))
            {
                this.Registers[address] = value;
            }
            else
            {
                this.Registers.Add(address, value);
            }
        }

        public void SetMode(uint mode)
        {
        }

        public void SetBitsPerWord(byte bitsPerWord)
        {
        }

        public void SetMaxSpeedHz(uint maxSpeedHz)
        {
        }

        #endregion

    }

}
