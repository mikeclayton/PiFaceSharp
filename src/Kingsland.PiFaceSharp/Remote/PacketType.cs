﻿namespace Kingsland.PiFaceSharp.Remote
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
