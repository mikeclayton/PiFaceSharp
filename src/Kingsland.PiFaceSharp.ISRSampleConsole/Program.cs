using Kingsland.PiFaceSharp.Spi;
using Kingsland.PiFaceSharp.PinControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRSampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // get reference to default piface device with all inputs enabled for ISR
            var piface = new PiFaceDevice(255);

            var pinDetector = new InputPinController(piface, 2);
            pinDetector.PinChanged += pinDetector_PinChanged;

            while (Console.KeyAvailable)
                Console.ReadKey(true);
            Console.WriteLine("Detectin changes on PiFace input pin 2. Press <Enter> key to exit..");
            var ki = Console.ReadKey(true);
            while (ki.Key != ConsoleKey.Enter)
            {
                System.Threading.Thread.Sleep(100);
                ki = Console.ReadKey(true);
            }
        }

        static void pinDetector_PinChanged(object sender, PinChangedEventArgs e)
        {
            Console.WriteLine("Pin {0} changed to {1}", e.Pin, e.State);
        }
    }
}
