using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingsland.PiFaceSharp.Spi
{

    internal interface ISpiDevice
    {

        void Open(uint mode);
        void Close();

        byte ReadByte(byte address);
        void WriteByte(byte address, byte value);

        void SetMode(uint mode);
        void SetBitsPerWord(byte bitsPerWord);
        void SetMaxSpeedHz(uint maxSpeedHz);

    }

}
