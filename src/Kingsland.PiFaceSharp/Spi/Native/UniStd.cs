using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public static extern uint read (int __fd, IntPtr __buf, uint __nbytes);

    }

}
