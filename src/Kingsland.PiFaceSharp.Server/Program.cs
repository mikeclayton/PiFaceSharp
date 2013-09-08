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
            var device = (IPiFaceDevice)null;
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
            var address = (IPAddress)null;
            if (string.IsNullOrEmpty(settings.ServerAddress))
            {
                address = PiFaceTcpHelper.GetLocalIPAddress();
            }
            else
            {
                address = System.Net.IPAddress.Parse(settings.ServerAddress);
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
            server.Start();

            // wait around while the server runs
            while(true)
            {
                System.Threading.Thread.Sleep(1000);
            }

        }

    }

}
