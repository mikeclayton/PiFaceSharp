using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingsland.PiFaceSharp.PinControllers
{
    public class InputPinGroupController : PinControllerBase
    {

        public enum PinGroupMode : byte
        {
            Any,
            All
        }

         #region fields
        
        private byte _inputPinMask;
        private PinGroupMode _pinGroupMode; 
        private bool _state;
        private int _gateDuration;
        private DateTime _lastChangeAt = DateTime.MinValue;

        #endregion

        #region events

        public event EventHandler<PinGroupChangedEventArgs> PinGroupChanged;

        #endregion

        #region constructors

        /// <summary>
        /// Creates an instance of InputPinController.
        /// </summary>
        /// <param name="piface">PiFace device. Must be ISR enabled.</param>
        /// <param name="inputPinMask">Input pin mask to track changes.</param>
        /// <param name="PinGroupMode">Any: at least one pin means group is set; All: all pins of the group must be set.</param>
        /// <param name="gateDuration">Gate time in miliseconds in within further changes are ignored (antibeat function, default 20ms)</param>
        public InputPinGroupController(IISRPiFaceDevice piface, byte inputPinMask, PinGroupMode pinGroupMode, int gateDuration = 20) 
            : base(piface) 
        {
            if (gateDuration < 0)
            {
                throw new ArgumentOutOfRangeException("gateDuration", "gateDuration must be positive");
            }
            if (!piface.IsISREnabled)
            {
                throw new ArgumentOutOfRangeException("piface", "piface must be ISR enabled");
            }
            this._inputPinMask = inputPinMask;
            this._pinGroupMode = pinGroupMode;
            this._gateDuration = gateDuration;
            byte currentStates = piface.GetInputPinStates();
            this._state = getState(currentStates);
            piface.InputsChanged += PiFace_InputsChanged;
        }

        #endregion

        #region properties

        public new IISRPiFaceDevice PiFace
        {
            get
            {
                return (IISRPiFaceDevice)base.PiFace;
            }
        }

        public byte InputPinMask
        {
            get
            {
                return _inputPinMask;
            }
        }

        public PinGroupMode GroupMode
        {
            get
            {
                return _pinGroupMode;
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

        private bool getState(byte currentStates)
        {
            if (this.GroupMode == PinGroupMode.All)
            {
                return ((currentStates & this.InputPinMask) == this.InputPinMask);
            }
            else
            {
                return ((currentStates & this.InputPinMask) != 0);
            }
        }

        private void PiFace_InputsChanged(object sender, InputsChangedEventArgs e)
        {
            bool state = getState(e.InputPinStates);
            DateTime now = DateTime.Now;
            // if state is changed and time since last change is greater than GateDuration
            if (state != this.State &&
                now.Subtract(_lastChangeAt).TotalMilliseconds >= this.GateDuration)
            {
                this.State = state;
                _lastChangeAt = now;
                // raise event
                OnPinGroupChanged(state, (byte)(e.InterruptLatch & this.InputPinMask));
            }
        }

        protected virtual void OnPinGroupChanged(bool state, byte interruptMask)
        {
            if (PinGroupChanged != null)
            {
                PinGroupChanged(this, new PinGroupChangedEventArgs(this.InputPinMask, state, interruptMask));
            }
        }

        #endregion

    }
}
