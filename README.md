PiFaceSharp
===========

PiFaceSharp is a C# library for controlling the inputs and outputs on a Pi-Face Digital device connected to a Raspberry Pi, and is based loosely around Gordon Henderson's WiringPiFace project (see https://projects.drogon.net/raspberry-pi/wiringpiface/).

The library can be used to write C# applications that run using Mono on the Raspberry Pi.

Follow the links below to read about some of the features of PiFaceSharp:

* [Getting Started](https://github.com/mikeclayton/PiFaceSharp/wiki/Getting-Started)

* [Pin Controllers](https://github.com/mikeclayton/PiFaceSharp/wiki/Pin-Controllers) - background workers that will manage pin state automatically

* [PiFace Emulator](https://github.com/mikeclayton/PiFaceSharp/wiki/PiFace-Emulator) - a virtual PiFace for Windows that you can program against even if you don't have a Raspberry Pi!

* [PiFace Remoting](https://github.com/mikeclayton/PiFaceSharp/wiki/PiFace-Remoting) - control the pins on your PiFace over a network using exactly the same C# code as when running it on the Raspberry Pi.

Example
=======

```c#
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
```

See the Kingsland.PiFaceSharp.SampleConsole project for the full source.

Enable Interrupt Listeners
=======
The PiFace connects an internal interrupt wire to GPIO25, so changes on
PiFace inputs can be detected by enabling interrupts on both the PiFace inputs and the Raspberry Pi GPIO25.
PiFaceSharp does this automatically - just pass the enable interrupt byte mask
to the constructor overload.

Afterwards one can use some of the new `InputPinController`, `InputPinGroupController` and `ButtonInputController` classes to benefit from pin change detection.

Example (see `ISRSampleConsole` demo project)
-------

```c#
class Program
    {
        static void Main(string[] args)
        {
            // get reference to default piface device with all inputs enabled for ISR
            var piface = new PiFaceDevice(255);

            var pinDetector = new InputPinController(piface, 0);
            pinDetector.PinChanged += pinDetector_PinChanged;

            var buttonDetector = new ButtonInputController(piface, 1);
            buttonDetector.ButtonClicked += buttonDetector_ButtonClicked;

            while (Console.KeyAvailable)
                Console.ReadKey(true);
            Console.WriteLine("Detectin changes on PiFace input pin 0 and Button clicks on pin 1. Press <Enter> key to exit..");
            var ki = Console.ReadKey(true);
            while (ki.Key != ConsoleKey.Enter)
            {
                System.Threading.Thread.Sleep(100);
                ki = Console.ReadKey(true);
            }
        }

        static void buttonDetector_ButtonClicked(object sender, InputButtonEventArgs e)
        {
            if (e.IsDoubleClick) {
                Console.WriteLine("Pin {0} double click", e.Pin);
            } else if (e.IsHold)
            {
                Console.WriteLine("Pin {0} hold", e.Pin);
            }
            else
            {
                Console.WriteLine("Pin {0} click", e.Pin);
            }
        }

        static void pinDetector_PinChanged(object sender, PinChangedEventArgs e)
        {
            Console.WriteLine("Pin {0} changed to {1}", e.Pin, e.State);
        }
    }

```


License
=======

This project is licensed under the GNU Lesser General Public License (LGPL) v3. See LICENSE.txt for details.


