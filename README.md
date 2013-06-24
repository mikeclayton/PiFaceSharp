PiFaceSharp
===========

PiFaceSharp is a C# library for controlling the inputs and outputs on a Pi-Face Digital device connected to a Raspberry Pi, and is based loosely around Gordon Henderson's WiringPiFace project (see https://projects.drogon.net/raspberry-pi/wiringpiface/).

The library can be used to write C# applications that run using Mono on the Raspberry Pi.


Example
=======

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

Pin Controllers
===============
There's now some additional helper classes for controlling output pins in a background thread. For example, if you want to build a simple project to make a blinking LED light you can use something like the following:

Blinking LED Example
--------------------

    // get reference to default piface device
    var piface = new PiFace();

    // create a new background-thread pin controller that turns pin 3 on and
    // off every 500 milliseconds
    var controller = new BlinkingPinController(piface, 3, 500);

    // start the controller. it will run in the background regardless of what
    // the main program thread is doing
    controller.Start();

    // do something else here while the LED blinks away
    System.Threading.Thread.Sleep(10000);

    // stop the controller
    controller.Stop();

PWM Motor Example
-----------------

There's also a PWM controller which can be used to vary the speed of DC motors, or even as a brightness controller for a simple LED. For example:

    // get reference to default piface device
    var piface = new PiFace();

    // create a new background-thread PWM controller on pin 5 which has a 50 millisecond
    // cycle period
    var controller = new PwmPinController(piface, 5, 50);

    // start the controller. it will drive the motor in the background regardless of what
    // the main program thread is doing
    controller.Duty = 0.50; // run at 50% power
    controller.Start();

    // do something else here (e.g. wait for user input) while the Pwm controller 
    System.Threading.Thread.Sleep(10000);

    // now run at 25% power
    controller.Duty = 0.25;
    System.Threading.Thread.Sleep(10000);

    // stop the controller
    controller.Stop();


License
=======

This project is licensed under the GNU Lesser General Public License (LGPL) v3. See LICENSE.txt for details.


