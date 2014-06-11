using System.Runtime.InteropServices;

namespace Kingsland.PiFaceSharp.Spi.Native
{

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="/usr/include/fcntl.h"/>
    public static class FCntl
    {

        /// <summary>
        /// See http://man7.org/linux/man-pages/man2/open.2.html
        /// </summary>
        /// <param name="path"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        [DllImport("libc", EntryPoint = "open", SetLastError = true)]
        public static extern int open(string path, int flag);

        /// <summary>
        /// See http://man7.org/linux/man-pages/man2/close.2.html
        /// </summary>
        /// <param name="fildes"></param>
        /// <returns></returns>
        [DllImport("libc", EntryPoint = "close", SetLastError = true)]
        public static extern int close(int fildes);

        public const int O_ACCMODE = 00000003;
        public const int O_RDONLY = 00000000;
        public const int O_WRONLY = 00000001;
        public const int O_RDWR = 00000002;

    }

}
