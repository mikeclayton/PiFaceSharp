using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingsland.PiFaceSharp.PinControllers
{
    /// <summary>
    /// Class for tracking the state of an PiFace input pin via interrupt service routines.
    /// </summary>
    /// <remarks>
    /// PiFace device must be ISR enabled.
    /// </remarks>
    class InputPinController : PinControllerBase
    {

        #region fields
        
        private byte _inputPin;
        private bool _state;
        private int _gateDuration;
        private DateTime _lastChangeAt = DateTime.MinValue;

        #endregion

        #region events

        public event EventHandler<PinChangedEventArgs> PinChanged;

        #endregion

        #region constructors

        /// <summary>
        /// Creates an instance of InputPinController.
        /// </summary>
        /// <param name="piface">PiFace device. Must be ISR enabled.</param>
        /// <param name="inputPin">Input pin to track changes.</param>
        /// <param name="gateDuration">Gate time in miliseconds in within further changes are ignored (antibeat function, default 20ms)</param>
        public InputPinController(IISRPiFaceDevice piface, byte inputPin, int gateDuration = 20) 
            : base(piface) 
        {
            if (inputPin > 7)
            {
                throw new ArgumentOutOfRangeException("inputPin", "inputPin must be in the range 0-7");
            }
            if (gateDuration < 0)
            {
                throw new ArgumentOutOfRangeException("gateDuration", "gateDuration must be positive");
            }
            if (!piface.IsISREnabled)
            {
                throw new ArgumentOutOfRangeException("piface", "piface must be ISR enabled");
            }
            this._inputPin = inputPin;
            this._gateDuration = gateDuration;
            this.PiFace.InputsChanged += PiFace_InputsChanged;
        }

        #endregion

        #region properties

        public IISRPiFaceDevice PiFace
        {
            get
            {
                return (IISRPiFaceDevice)base.PiFace;
            }
        }

        public byte InputPin
        {
            get
            {
                return _inputPin;
            }
        }

        public int GateDuration
        {
            get
            {
                return _gateDuration;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("GateDuration", "GateDuration must be positive");
                }
                _gateDuration = value;
            }
        }

        public bool State
        {
            get
            {
                return _state;
            }
            private set
            {
                _state = value;
            }
        }

        #endregion

        #region methods

        private void PiFace_InputsChanged(object sender, InputsChangedEventArgs e)
        {
            int pinMask = (1 << this.InputPin);
            bool state = Convert.ToBoolean(e.InputPinStates & pinMask);
            DateTime now = DateTime.Now;
            // if this pin caused interrupt or pin value changed
            if (((e.InterruptLatch & pinMask) == pinMask || state != this.State) &&
                now.Subtract(_lastChangeAt).TotalMilliseconds >= this.GateDuration)
            {
                this.State = state;
                _lastChangeAt = now;
                OnPinChanged(state);
            }
        }

        protected void OnPinChanged(bool state)
        {
            if (PinChanged != null)
            {
                PinChanged(this, new PinChangedEventArgs(this.InputPin, state));
            }
        }

        #endregion

    }
}
