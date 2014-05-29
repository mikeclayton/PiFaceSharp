using System;
using System.Threading;

namespace Kingsland.PiFaceSharp.PinControllers
{

    /// <summary>
    /// Implements a Pulse Width Modulation pin controller that can be used to
    /// drive a DC motor at a variable rate. Since the PiFace doesn't have any
    /// analog output pins we can't vary the voltage to control the motor speed
    /// so the PWM controller rapidly turns the output pin on and off, with the
    /// proportion of the time spent "on" defining the motor speed.
    /// </summary>
    /// <remarks>
    /// See https://en.wikipedia.org/wiki/Pulse-width_modulation
    /// </remarks>
    public sealed class PwmPinController : BackgroundPinController
    {

        #region Fields

        private byte _mOutputPin;

        private int _mPeriod;
        private float _mDuty;
        private int _mDutyHighMs;
        private int _mDutyLowMs;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a PwmController bound to the specified pin on a PiFace Digital
        /// device, with the specified cycle period. The initial duty load is set to zero,
        /// and the "Start" method must be called to start the controller.
        /// </summary>
        /// <param name="piface"></param>
        /// <param name="outputPin"></param>
        /// <param name="period"></param>
        public PwmPinController(IPiFaceDevice piface, byte outputPin, int period)
            : this(piface, outputPin, period, 0)
        {
        }

        /// <summary>
        /// Initialises a PwmController bound to the specified pin on a PiFace Digital
        /// device, with the specified cycle period. The initial duty load is also set
        /// to the specified value, although the "Start" method must be called to
        /// start the controller.
        /// </summary>
        /// <param name="piface"></param>
        /// <param name="outputPin"></param>
        /// <param name="period"></param>
        /// <param name="duty"></param>
        public PwmPinController(IPiFaceDevice piface, byte outputPin, int period, float duty)
            : base(piface)
        {
            // copy the parameters locally
            this.OutputPin = outputPin;
            this.SetTimers(period, duty);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the index of the output pin this controller is bound to.
        /// </summary>
        public byte OutputPin
        {
            get
            {
                return _mOutputPin;
            }
            private set
            {
                if (_mOutputPin > 7)
                {
                    throw new System.ArgumentOutOfRangeException("value", "Value must be 7 or less.");
                }
                _mOutputPin = value;
            }
        }

        /// <summary>
        /// Gets the duration of a full cycle.
        /// E.g. "period 100ms, duty 20%" repeats "80ms off, 20ms on".
        /// </summary>
        public int Period
        {
            get
            {
                return _mPeriod;
            }
            private set
            {
                this.SetTimers(value, this.Duty);
            }
        }

        /// <summary>
        /// Gets or sets the percentage of time the output pin is HIGH.
        /// E.g. "period 100ms, duty 20%" repeats "80ms off, 20ms on".
        /// </summary>
        public float Duty
        {
            get
            {
                return _mDuty;
            }
            set
            {
                this.SetTimers(this.Period, Math.Max(0, Math.Min(1, value)));
            }
        }

        #endregion

        #region Methods

        private void SetTimers(int period, float duty)
        {
            // validate the parameters
            if (period < 10)
            {
                // there's a practical limit to how fast the output pins can be updated
                throw new System.ArgumentOutOfRangeException("period", "Value must be 10 or less.");
            }
            if (duty < 0 || duty > 1)
            {
                // can't be on a negative amount, or more than 100%
                throw new System.ArgumentOutOfRangeException("duty", "Value must be between 0 and 1.");
            }
            // copy the parameters locally
            _mPeriod = period;
            _mDuty = duty;
            // calculate the on and off intervals so we don't have to keep doing it in the Execute method
            _mDutyHighMs = (int)(_mDuty * _mPeriod);
            _mDutyLowMs = period - _mDutyHighMs;
        }
               
        /// <summary>
        /// 
        /// </summary>
        protected override void Execute()
        {
            // run the main loop
            while (this.Status == BackgroundPinControllerStatus.Running)
            {
                if (_mDutyLowMs > 0)
                {
                    this.PiFace.SetOutputPinState(this.OutputPin, false);
                    Thread.Sleep(_mDutyLowMs);
                }
                if (_mDutyHighMs > 0)
                {
                    this.PiFace.SetOutputPinState(this.OutputPin, true);
                    Thread.Sleep(_mDutyHighMs);
                }
            }
        }

        #endregion

    }

}
