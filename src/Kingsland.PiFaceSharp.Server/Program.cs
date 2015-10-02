using System;
using System.Net;
using Kingsland.PiFaceSharp.Emulators;
using Kingsland.PiFaceSharp.Remote;
using Kingsland.PiFaceSharp.Spi;

namespace Kingsland.PiFaceSharp.Server
{

    class Program
    {

        static void Main(string[] args)
        {

            var settings = Properties.Settings.Default;

            // initialise the PiFace device to bind the server to
            var device = default(IPiFaceDevice);
            switch(settings.DeviceType)
            {
                case "Physical":
                    device = new PiFaceDevice();
                    break;
                case "Emulated":
                    device = new PiFaceEmulator();
                    break;
                default:
                    throw new System.Configuration.ConfigurationErrorsException("The 'DeviceType' setting in app.config must be either 'Physical' or 'Emulated'.");
            }
            
            // get the address to run the server on
            var address = default(IPAddress);
            if (string.IsNullOrEmpty(settings.ServerAddress))
            {
                address = PiFaceTcpHelper.GetLocalIPAddress();
            }
            else
            {
                address = IPAddress.Parse(settings.ServerAddress);
            }

            // get the port to run the server on
            var port = (uint)0;
            if (!uint.TryParse(settings.ServerPort, out port))
            {
                throw new System.Configuration.ConfigurationErrorsException("The 'ServerPort' setting in app.config must be an unsigned integer.");
            }

            // start the server
            var endpoint = new IPEndPoint(address, (int)port);
            var server = new PiFaceTcpServer(device, endpoint);
            server.MessageReceived += Program.PiFaceTcpServer_MessageReceived;
            server.ResponseSent += Program.PiFaceTcpServer_ResponseSent;
            server.Start();

            // wait around while the server runs
            while(true)
            {
                System.Threading.Thread.Sleep(1000);
            }

        }

        #region Server Event handlers

        private static void PiFaceTcpServer_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Program.LogEvent(string.Format("message received - {0}", e.PacketType));
        }

        private static void PiFaceTcpServer_ResponseSent(object sender, ResponseSentEventArgs e)
        {
            Program.LogEvent("response sent");
        }

        #endregion

        private static void LogEvent(string message)
        {
            Console.WriteLine("{0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), message);
        }

    }

}
