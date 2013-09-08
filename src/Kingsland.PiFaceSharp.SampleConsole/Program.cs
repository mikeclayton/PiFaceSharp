using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kingsland.PiFaceSharp.Spi;

namespace Kingsland.PiFaceSharp.SampleConsole
{

    class Program
    {

        static void Main(string[] args)
        {

            // get reference to default piface device
            var piface = new PiFaceDevice();

            // toggle each output pin on and off 50 times
            for (byte pin = 0; pin < 8; pin++)
            {
                var state = true;
                for (var i = 0; i < 50; i++)
                {
                    piface.SetOutputPinState(pin, state);
                    System.Threading.Thread.Sleep(25);
                    state = !state;
                }
            }

            // read the current state of each input pin
            for (byte pin = 0; pin < 8; pin++)
            {
                Console.Write(piface.GetInputPinState(pin) + " ");
            }

        }

    }

}
