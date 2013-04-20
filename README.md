PiFaceSharp
===========

PiFaceSharp is a C# library for controlling the inputs and outputs on a Pi-Face Digital device connected to a Raspberry Pi, and is based loosely around Gordon Henderson's WiringPiFace project (see https://projects.drogon.net/raspberry-pi/wiringpiface/).

The library can be used to write C# applications that run using Mono on the Raspberry Pi.


Example
=======

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
    for(byte i = 0; i < 8; i++)
    {
        Console.Write(piface.GetInputPinState(i) + " ");
    }


License
=======

This project is licensed under the GNU Lesser General Public License (LGPL) v3. See LICENSE.txt for details.


