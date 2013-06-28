PiFaceSharp
===========

PiFaceSharp is a C# library for controlling the inputs and outputs on a Pi-Face Digital device connected to a Raspberry Pi, and is based loosely around Gordon Henderson's WiringPiFace project (see https://projects.drogon.net/raspberry-pi/wiringpiface/).

The library can be used to write C# applications that run using Mono on the Raspberry Pi.

Follow the links below to read about some of the features of PiFaceSharp:

* [Getting Started](https://github.com/mikeclayton/PiFaceSharp/wiki/Getting-Started)

* [Pin Controllers](https://github.com/mikeclayton/PiFaceSharp/wiki/Pin-Controllers) - background workers that will manage pin state automatically

* [PiFace Emulator](https://github.com/mikeclayton/PiFaceSharp/wiki/PiFace-Emulator) - a virtual PiFace for Windows that you can program against even if you don't have a Raspberyy Pi!

* [PiFace Remoting](https://github.com/mikeclayton/PiFaceSharp/wiki/PiFace-Remoting) - control the pins on your PiFace over a network using exactly the same C# code as when running it on the Raspberry Pi.

Example
=======

```c#
// get reference to default piface device
var piface = new PiFace();
    
// toggle each output pin on and off 50 times
for (byte pin = 0; pin < 8; pin++)
{
    var state = true;
    for (var i = 0; i < 50; i++ )
    {
        piface.SetOutputPinState(pin, state);
        System.Threading.Thread.Sleep(25);
        state = !state;
    }
}
    
// read the current state of each input pin
for(byte pin = 0; pin < 8; i++)
{
    Console.Write(piface.GetInputPinState(pin) + " ");
}
```


License
=======

This project is licensed under the GNU Lesser General Public License (LGPL) v3. See LICENSE.txt for details.


