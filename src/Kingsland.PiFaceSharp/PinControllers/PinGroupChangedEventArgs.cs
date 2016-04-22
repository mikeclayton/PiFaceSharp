using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingsland.PiFaceSharp.PinControllers
{
    public class PinGroupChangedEventArgs : EventArgs
    {

        private byte _pinMask;
        private bool _state;
        private byte _interruptMask;

        public PinGroupChangedEventArgs(byte pinMask, bool state, byte interruptMask)
        {
            this._pinMask = pinMask;
            this._state = state;
            this._interruptMask = interruptMask;
        }

        public byte PinMask
        {
            get
            {
                return _pinMask;
            }
        }

        public bool State
        {
            get
            {
                return _state;
            }
        }

        public byte InterruptMask
        {
            get
            {
                return _interruptMask;
            }
        }

    }
}
