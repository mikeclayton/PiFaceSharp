using System.ComponentModel;

namespace Kingsland.PiFaceSharp.Spi
{

    [Browsable(false)]
    public interface ISpiDevice
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
