using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingsland.PiFaceSharp
{
    /// <summary>
    /// Event class for signaling input state changes on interrupt service routine.
    /// </summary>
    public class InputsChangedEventArgs : EventArgs
    {

        private byte _InterruptLatch;
        private byte _InputPinStates;

        /// <summary>
        /// Creates an event args for signaling changed input states.
        /// </summary>
        /// <param name="InterruptLatch">Byte mask specifying which pin caused the interrupt.</param>
        /// <param name="InputPinStates">Byte mask specifying the pin states at interrupt.</param>
        public InputsChangedEventArgs(byte InterruptLatch, byte InputPinStates)
        {
            this._InterruptLatch = InterruptLatch;
            this._InputPinStates = InputPinStates;
        }

        /// <summary>
        /// Gets the byte mask specifying which pin caused the interrupt.
        /// </summary>
        public byte InterruptLatch
        {
            get
            {
                return _InterruptLatch;
            }
        }

        /// <summary>
        /// Gets the byte mask specifying the pin states at interrupt.
        /// </summary>
        public byte InputPinStates
        {
            get
            {
                return _InputPinStates;
            }
        }

    }
}
