using System;
using System.Runtime.InteropServices;

namespace Kingsland.PiFaceSharp.Spi.Native
{

    /// <summary>
    ///
    /// </summary>
    /// <see cref="/usr/include/linux/spi/spidev.h"/>
    public static class SpiDev
    {

        public const byte SPI_CPHA = 0x01;
        public const byte SPI_CPOL = 0x02;

        public const byte SPI_MODE_0 = (0 | 0);
        public const byte SPI_MODE_1 = (0 | SPI_CPHA);
        public const byte SPI_MODE_2 = (SPI_CPOL | 0);
        public const byte SPI_MODE_3 = (SPI_CPOL | SPI_CPHA);

        public const byte SPI_CS_HIGH = 0x04;
        public const byte SPI_LSB_FIRST = 0x08;
        public const byte SPI_3WIRE = 0x10;
        public const byte SPI_LOOP = 0x20;
        public const byte SPI_NO_CS = 0x40;
        public const byte SPI_READY = 0x80;

        public const char SPI_IOC_MAGIC = 'k';

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct spi_ioc_transfer
        {
            public UInt64 tx_buf;
            public UInt64 rx_buf;
            public UInt32 len;
            public UInt32 speed_hz;
            public UInt16 delay_usecs;
            public byte bits_per_word;
            public byte cs_change;
            public UInt32 pad;
        };

        public static uint SPI_MSGSIZE(uint N)
        {
            var size = Marshal.SizeOf(typeof(spi_ioc_transfer));
            return (N * size < (1 << IoCtl.IOC_SIZEBITS)) ? (uint)(N * size) : 0;
        }

        public static uint SPI_IOC_MESSAGE(uint N)
        {
            return IoCtl.IOW(SPI_IOC_MAGIC, 0, SPI_MSGSIZE(N));
        }

        public static readonly uint SPI_IOC_RD_MODE = IoCtl.IOR(SPI_IOC_MAGIC, 1, sizeof(byte));
        public static readonly uint SPI_IOC_WR_MODE = IoCtl.IOW(SPI_IOC_MAGIC, 1, sizeof(byte));

        public static readonly uint SPI_IOC_RD_LSB_FIRST = IoCtl.IOR(SPI_IOC_MAGIC, 2, sizeof(byte));
        public static readonly uint SPI_IOC_WR_LSB_FIRST = IoCtl.IOW(SPI_IOC_MAGIC, 2, sizeof(byte));

        public static readonly uint SPI_IOC_RD_BITS_PER_WORD = IoCtl.IOR(SPI_IOC_MAGIC, 3, sizeof(byte));
        public static readonly uint SPI_IOC_WR_BITS_PER_WORD = IoCtl.IOW(SPI_IOC_MAGIC, 3, sizeof(byte));

        public static readonly uint SPI_IOC_RD_MAX_SPEED_HZ = IoCtl.IOR(SPI_IOC_MAGIC, 4, sizeof(UInt32));
        public static readonly uint SPI_IOC_WR_MAX_SPEED_HZ = IoCtl.IOW(SPI_IOC_MAGIC, 4, sizeof(UInt32));
        
    }

}
