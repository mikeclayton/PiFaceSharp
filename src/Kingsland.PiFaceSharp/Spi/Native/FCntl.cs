using System.Runtime.InteropServices;

namespace Kingsland.PiFaceSharp.Spi.Native
{

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="/usr/include/fcntl.h"/>
    public static class FCntl
    {

        [DllImport("libc", EntryPoint = "open", SetLastError = true)]
        public static extern uint open(string __file, uint __oflag);

        public const uint O_ACCMODE = 00000003;
        public const uint O_RDONLY = 00000000;
        public const uint O_WRONLY = 00000001;
        public const uint O_RDWR = 00000002;

    }

}
