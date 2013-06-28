using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingsland.PiFaceSharp.Remote
{
    
    public enum PacketType : byte
    {

        GetOutputPinState,
        GetOutputPinStates,
        SetOutputPinState,
        SetOutputPinStates,
        GetInputPinState,
        GetInputPinStates,
        SetInputPinState,
        SetInputPinStates

    }

}
