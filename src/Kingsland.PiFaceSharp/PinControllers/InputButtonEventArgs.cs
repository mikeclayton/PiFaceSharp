using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingsland.PiFaceSharp.PinControllers
{
    public class InputButtonEventArgs : EventArgs
    {

        private byte _pin;
        private bool _isDoubleClick;
        private bool _isHold;

        public InputButtonEventArgs(byte pin, bool isDoubleClick, bool isHold)
        {
            this._pin = pin;
            this._isDoubleClick = isDoubleClick;
            this._isHold = isHold;
        }

        public byte Pin
        {
            get
            {
                return _pin;
            }
        }

        public bool IsDoubleClick
        {
            get
            {
                return _isDoubleClick;
            }
        }

        public bool IsHold
        {
            get
            {
                return _isHold;
            }
        }

    }
}
