using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Kingsland.PiFaceSharp.Spi.Native;

namespace Kingsland.PiFaceSharp.Remote
{

    /// <summary>
    /// Implements a PiFace device that with a remote server via a TCP/IP network.
    /// This can be used to send commands to a PiFace device running on a remote
    /// computer. (For example, a Windows Forms application running on a PC
    /// controlling the output pins on a PiFace connected to a Raspberry Pi).
    /// </summary>
    public sealed class PiFaceTcpClient : IPiFaceDevice
    {

        #region Constructors

        /// <summary>
        /// Creates a new PiFaceTcpClient that connects to a PiFaceTcpServer
        /// over a TCP/IP network to control a remote PiFace device.
        /// </summary>
        public PiFaceTcpClient(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint)
            : base()
        {
            this.LocalEndPoint = localEndPoint;
            this.RemoteEndPoint = remoteEndPoint;
            this.TcpClient = new TcpClient(localEndPoint);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The endpoint on the local computer to use for making the
        /// outbound connection.
        /// </summary>
        private IPEndPoint LocalEndPoint
        {
            get;
            set;
        }

        /// <summary>
        /// The endpoint to send commands to on the remote computer.
        /// </summary>
        private IPEndPoint RemoteEndPoint
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        private TcpClient TcpClient
        {
            get;
            set;
        }

        #endregion

        #region Connection Methods

        /// <summary>
        /// 
        /// </summary>
        public void Connect()
        {
            this.TcpClient.Connect(this.RemoteEndPoint);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            this.TcpClient.Close();
        }

        #endregion

        #region IO Methods
        
        private byte[] SendMessage(PacketType type)
        {
            return this.SendMessage(type, null);
        }

        /// <summary>
        /// Sends a message of the specified type, with an optional data payload.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] SendMessage(PacketType type, byte[] data)
        {
            // send the message
            var stream = this.TcpClient.GetStream();
            var length = (byte)0;
            if (data != null)
            {
                length += (byte)data.Length;
            }
            stream.WriteByte((byte)type);
            stream.WriteByte(length);
            if (length > 0)
            {
                stream.Write(data, 0, data.Length);
            }
            stream.Flush();
            // read the response
            var result = (byte[])null;
            length = (byte)stream.ReadByte();
            if (length > 0)
            {
                result = PiFaceTcpHelper.ReadBytes(stream, length);
            }
            return result;
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
            var data = new byte[] { pin };
            var result = this.SendMessage(PacketType.GetOutputPinState, data);
            throw new System.InvalidOperationException();
        }

        /// <summary>
        /// Gets the bitmask containing the state of all output pins.
        /// </summary>
        /// <param name="pin"></param>
        /// <returns>
        /// A bitmask containing true for each output pin that is HIGH, and false if it is LOW.
        /// </returns>
        public byte GetOutputPinStates()
        {
            var result = this.SendMessage(PacketType.GetOutputPinStates);
            return result[0];
        }

        /// <summary>
        /// Update the state of a single output pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="value"></param>
        public void SetOutputPinState(byte pin, bool enabled)
        {
            var data = new byte[] { pin, (byte)(enabled ? 1 : 0) };
            var result = this.SendMessage(PacketType.SetOutputPinState, data);
        }

        /// <summary>
        /// Update the state of all output pins using a bitmask
        /// containing the state for each pin.
        /// </summary>
        /// <param name="bitMask"></param>
        public void SetOutputPinStates(byte bitMask)
        {
            var data = new byte[] { bitMask };
            this.SendMessage(PacketType.SetOutputPinStates, data);
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
            var data = new byte[] { pin };
            var result = this.SendMessage(PacketType.GetInputPinState, data);
            throw new System.InvalidOperationException();
        }

        /// <summary>
        /// Gets a bitmask containing the state of all input pins.
        /// </summary>
        /// <param name="pin"></param>
        /// <returns>
        /// A bitmask containing true for each input pin that is HIGH, and false if it is LOW.
        /// </returns>
        public byte GetInputPinStates()
        {
            var result = this.SendMessage(PacketType.GetInputPinStates);
            throw new System.InvalidOperationException();
        }

        /// <summary>
        /// Update the state of a single input pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="value"></param>
        /// <remarks>
        /// This method is provided to support the PiFaceEmulator, and will 
        /// throw an exception if called on a physical PiFaceDevice.
        /// </remarks>
        public void SetInputPinState(byte pin, bool enabled)
        {
            var data = new byte[] { pin, (byte)(enabled ? 1 : 0) };
            var result = this.SendMessage(PacketType.SetInputPinState, data);
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
            var data = new byte[] { bitMask };
            this.SendMessage(PacketType.SetInputPinStates, data);
            throw new System.InvalidOperationException();
        }

        #endregion

    }

}
