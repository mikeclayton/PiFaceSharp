using System;

namespace Kingsland.PiFaceSharp.Remote
{

    public sealed class MessageReceivedEventArgs : EventArgs
    {

        public MessageReceivedEventArgs(PacketType packetType, byte[] data)
        {
            this.PacketType = packetType;
            this.Data = data;
        }

        public PacketType PacketType
        {
            get; 
            set; 
        }

        public byte[] Data
        {
            get; 
            set; 
        }

    }

}
