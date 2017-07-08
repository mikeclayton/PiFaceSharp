using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Kingsland.PiFaceSharp.PinControllers
{
    public class ButtonInputController : InputPinController
    {

        private Timer _holdTimer;
        private DateTime _lastClick = DateTime.MinValue;
        private bool _hold;

        public event EventHandler<InputButtonEventArgs> ButtonClicked;

        public ButtonInputController(IISRPiFaceDevice piface, byte inputPin, int gateDuration = 20, int maxDoubleClickDuration = 300, int holdTime = 1000) 
            : base(piface, inputPin, gateDuration) 
        {
            _holdTimer = new Timer(holdTime) { AutoReset = false, Enabled = false };
            _holdTimer.Elapsed += _holdTimer_Elapsed;
            this.MaxDoubleClickDuration = TimeSpan.FromMilliseconds(maxDoubleClickDuration);
        }

        public TimeSpan MaxDoubleClickDuration { get; set; }

        void _holdTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.State)
            {
                _hold = true;
                OnButtonClicked(false, true);
            }
        }

        protected override void OnPinChanged(bool state)
        {
            base.OnPinChanged(state);
            if (state)
            {
                _holdTimer.Start();
            }
            else
            {
                var now = DateTime.Now;
                _holdTimer.Stop();
                if (now.Subtract(_lastClick) <= MaxDoubleClickDuration)
                {
                    _lastClick = DateTime.MinValue;
                    if (!_hold)
                    {
                        OnButtonClicked(true);
                    }                    
                }
                else
                {
                    _lastClick = now;
                    if (!_hold)
                    {
                        OnButtonClicked();
                    }                    
                }
                _hold = false;
            }
        }

        protected virtual void OnButtonClicked(bool isDoubleClick = false, bool isHold = false)
        {
            if (ButtonClicked != null)
            {
                ButtonClicked(this, new InputButtonEventArgs(this.InputPin, isDoubleClick, isHold));
            }
        }

    }
}
