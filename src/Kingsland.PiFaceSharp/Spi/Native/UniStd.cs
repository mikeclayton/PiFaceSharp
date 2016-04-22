using System;
using System.Runtime.InteropServices;

namespace Kingsland.PiFaceSharp.Spi.Native
{

    /// <summary>
    ///
    /// </summary>
    /// <see cref="/usr/include/unistd.h"/>
    public static class UniStd
    {

        [DllImport("libc", EntryPoint = "read", SetLastError = true)]
        public static extern int read(int __fd, [In, Out, MarshalAs(UnmanagedType.SafeArray)] ref byte[] __buf, uint __nbytes);

        [DllImport("libc", EntryPoint = "lseek", SetLastError = true)]
        public static extern int lseek(int __fd, int __offset, int whence);

    }

}
