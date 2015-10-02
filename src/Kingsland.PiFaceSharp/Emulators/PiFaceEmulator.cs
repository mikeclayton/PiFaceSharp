using System;

namespace Kingsland.PiFaceSharp.Emulators
{

    /// <summary>
    /// Implements an emulator for the PiFaceDevice that can be used during development
    /// and testing as a replacement for a physical device. It has very limited functionality
    /// and only contains a buffer for the input and output pin states.
    /// </summary>
    public sealed class PiFaceEmulator : IPiFaceDevice
    {

        #region Events

        public event EventHandler OutputPinStateChanged;
        public event EventHandler InputPinStateChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the state of the output pins.
        /// </summary>
        private byte OutputPinStates
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the state of the input pins.
        /// </summary>
        private byte InputPinStates
        {
            get;
            set;
        }

        #endregion

        #region Pin State Methods

        /// <summary>
        /// Gets the state of a single output pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public bool GetOutputPinState(byte pin)
        {
            if (pin > 7)
            {
                throw new System.ArgumentOutOfRangeException("pin", "pin must be in the range 0-7");
            }
            var mask = (byte)(1 << pin);
            var state = this.OutputPinStates;
            return (state & mask) == mask;
        }

        /// <summary>
        /// Gets the bitmask containing the state of all output pins.
        /// </summary>
        /// <returns>
        /// A bitmask containing true for each output pin that is HIGH, and false if it is LOW.
        /// </returns>
        public byte GetOutputPinStates()
        {
            var state = this.OutputPinStates;
            return state;
        }

        /// <summary>
        /// Update the state of a single output pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="enabled"></param>
        public void SetOutputPinState(byte pin, bool enabled)
        {
            var mask = (byte)(1 << pin);
            if (enabled)
            {
                this.OutputPinStates |= mask;
            }
            else
            {
                this.OutputPinStates &= (byte)~mask;
            }
            this.OnOutputPinStateChanged();
        }

        /// <summary>
        /// Update the state of all output pins using a bitmask
        /// containing the state for each pin.
        /// </summary>
        /// <param name="bitMask"></param>
        public void SetOutputPinStates(byte bitMask)
        {
            this.OutputPinStates = bitMask;
            this.OnOutputPinStateChanged();
        }

        /// <summary>
        /// Gets the state of a single input pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <returns>
        /// True if the specified pin is HIGH, or false if it is LOW.
        /// </returns>
        public bool GetInputPinState(byte pin)
        {
            if (pin > 7)
            {
                throw new ArgumentOutOfRangeException("pin", "pin must be in the range 0-7");
            }
            var mask = (byte)(1 << pin);
            var state = this.InputPinStates;
            return ((state & mask) == 0);

        }

        /// <summary>
        /// Gets a bitmask containing the state of all input pins.
        /// </summary>
        /// <returns>
        /// A bitmask containing true for each input pin that is HIGH, and false if it is LOW.
        /// </returns>
        public byte GetInputPinStates()
        {
            return this.InputPinStates;
        }

        /// <summary>
        /// Update the state of a single input pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="enabled"></param>
        /// <remarks>
        /// This method is provided to support the PiFaceEmulator, and will 
        /// throw an exception if called on a physical PiFaceDevice.
        /// </remarks>
        public void SetInputPinState(byte pin, bool enabled)
        {
            if (pin > 7)
            {
                throw new ArgumentOutOfRangeException("pin", "pin must be in the range 0-7");
            }
            var mask = (byte)(1 << pin);
            if (enabled)
            {
                this.InputPinStates |= mask;
            }
            else
            {
                this.InputPinStates &= (byte)~mask;
            }
            this.OnInputPinStateChanged();
        }

        /// <summary>
        /// Update the state of all input pins using a bitmask
        /// containing the state for each pin.
        /// </summary>
        /// <param name="bitMask"></param>
        /// <remarks>
        /// This method is provided to support the PiFaceEmulator, and will 
        /// throw an exception if called on a physical PiFaceDevice.
        /// </remarks>
        public void SetInputPinStates(byte bitMask)
        {
            this.InputPinStates = bitMask;
            this.OnInputPinStateChanged();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        private void OnOutputPinStateChanged()
        {
            var handler = this.OutputPinStateChanged;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnInputPinStateChanged()
        {
            var handler = this.InputPinStateChanged;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        #endregion

    }

}
