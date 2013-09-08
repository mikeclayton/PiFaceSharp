using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Kingsland.PiFaceSharp.Remote
{

    public static class PiFaceTcpHelper
    {

        /// <summary>
        /// Reads a fixed number of bytes from a stream, and doesn't
        /// return until it's got them all.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static byte[] ReadBytes(Stream stream, byte length)
        {
            var count = 0;
            var buffer = new byte[length];
            while (count < buffer.Length)
            {
                var bytesRead = stream.Read(buffer, count, buffer.Length - count);
                if (bytesRead == 0) { throw new System.InvalidOperationException(); }
                count += bytesRead;
            }
            return buffer;
        }

        /// <summary>
        /// Returns the IP Address on first available local network card.
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetLocalIPAddress()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList
                       .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

    }

}
