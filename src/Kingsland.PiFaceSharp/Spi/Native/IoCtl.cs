using System;
using System.Runtime.InteropServices;

namespace Kingsland.PiFaceSharp.Spi.Native
{

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="/usr/include/asm-generic/ioctl.h"/>
    public static class IoCtl
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="cmd"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <see cref="/usr/include/asm-generic/ioctl.h"/>
        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        public static extern int ioctl(uint fd, uint cmd, ref uint arg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="cmd"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <see cref="/usr/include/asm-generic/ioctl.h"/>
        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        public static extern int ioctl(uint fd, uint cmd, IntPtr arg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="cmd"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <see cref="/usr/include/asm-generic/ioctl.h"/>
        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        public static extern int ioctl(uint fd, uint cmd, ref SpiDev.spi_ioc_transfer arg);

        public const int IOC_NRBITS = 8;
        public const int IOC_TYPEBITS = 8;

        public const int IOC_SIZEBITS = 14;
        public const int IOC_DIRBITS = 2;

        public const uint IOC_NRMASK = (1 << IOC_NRBITS) - 1;
        public const uint IOC_TYPEMASK = (1 << IOC_TYPEBITS) - 1;
        public const uint IOC_SIZEMASK = (1 << IOC_SIZEBITS) - 1;
        public const uint IOC_DIRMASK = (1 << IOC_DIRBITS) - 1;

        public const int IOC_NRSHIFT = 0;
        public const int IOC_TYPESHIFT = IOC_NRSHIFT + IOC_NRBITS;
        public const int IOC_SIZESHIFT = IOC_TYPESHIFT + IOC_TYPEBITS;
        public const int IOC_DIRSHIFT = IOC_SIZESHIFT + IOC_SIZEBITS;

        public const uint IOC_NONE = 0;
        public const uint IOC_WRITE = 1;
        public const uint IOC_READ = 2;

        public static uint IOC(uint dir, uint type, uint nr, uint size)
        {
            return (dir << IOC_DIRSHIFT) |
                   (type << IOC_TYPESHIFT) |
                   (nr << IOC_NRSHIFT) |
                   (size << IOC_SIZESHIFT);
        }

        public static uint IO(uint type, uint nr)
        {
            return IOC(IOC_NONE, type, nr, 0);
        }
        public static uint IOR(uint type, uint nr, uint size)
        {
            return IOC(IOC_READ, type, nr, size);
        }
        public static uint IOW(uint type, uint nr, uint size)
        {
            return IOC(IOC_WRITE, type, nr, size);
        }
        public static uint IOWR(uint type, uint nr, uint size)
        {
            return IOC(IOC_READ | IOC_WRITE, type, nr, size);
        }

        public static uint IOC_DIR(uint nr)
        {
            return (nr >> IOC_DIRSHIFT) & IOC_DIRMASK;
        }
        public static uint IOC_TYPE(uint nr)
        {
            return (nr >> IOC_TYPESHIFT) & IOC_TYPEMASK;
        }
        public static uint IOC_NR(uint nr)
        {
            return (nr >> IOC_NRSHIFT) & IOC_NRMASK;
        }
        public static uint IOC_SIZE(uint nr)
        {
            return (nr >> IOC_SIZESHIFT) & IOC_SIZEMASK;
        }

    }

}
