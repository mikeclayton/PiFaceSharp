using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingsland.PiFaceSharp.PinControllers
{
    class PinChangedEventArgs : EventArgs
    {

        private byte _pin;
        private bool _state;

        public PinChangedEventArgs(byte pin, bool state)
        {
            this._pin = pin;
            this._state = state;
        }

        public byte Pin
        {
            get
            {
                return _pin;
            }
        }

        public bool State
        {
            get
            {
                return _state;
            }
        }

    }
}
