using System;

namespace Kingsland.PiFaceSharp.Remote
{

    public sealed class ResponseSentEventArgs : EventArgs
    {

        public ResponseSentEventArgs(byte[] data)
        {
            this.Data = data;
        }

        public byte[] Data
        {
            get; 
            set; 
        }

    }

}
