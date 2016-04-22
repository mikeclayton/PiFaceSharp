using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingsland.PiFaceSharp
{
    public interface IISRPiFaceDevice : IPiFaceDevice
    {

        bool IsISREnabled { get; }

        event EventHandler<InputsChangedEventArgs> InputsChanged;

    }
}
