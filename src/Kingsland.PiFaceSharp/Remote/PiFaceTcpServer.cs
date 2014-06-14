using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Kingsland.PiFaceSharp.Remote
{

    /// <summary>
    /// Implements a new PiFaceTcpServer that is bound to the specified IPiFaceDevice.
    /// This allows a PiFaceTcpClient to connect in and control the bound PiFace
    /// device over a TCP/IP network. Note that depending on latency on the network,
    /// performance is unlikely to be real-time.
    /// </summary>
    public sealed class PiFaceTcpServer
    {

        #region Fields

        private readonly object _lockObject = new object();

        #endregion

        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        // <summary>
        // 
        // </summary>
        public event EventHandler<ResponseSentEventArgs> ResponseSent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnMessageReceived(MessageReceivedEventArgs e)
        {
            var handler = this.MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnResponseSent(ResponseSentEventArgs e)
        {
            var handler = this.ResponseSent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new PiFaceTcpServer that is bound to the specified IPiFaceDevice.
        /// </summary>
        /// <param name="device">
        /// The device to bind to. This doesn't have to be a pyhsical device - for example
        /// a PiFaceEmulator device could be used instead.
        /// </param>
        /// <param name="localEndPoint"></param>
        public PiFaceTcpServer(IPiFaceDevice device, IPEndPoint localEndPoint)
        {
            // validate the parameters
            if (device == null)
            {
                throw new System.ArgumentNullException("device");
            }
            if (localEndPoint == null)
            {
                throw new System.ArgumentNullException("localEndPoint");
            }
            // copy the parameters locally
            this.PiFaceDevice = device;
            this.LocalEndPoint = localEndPoint;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        private IPiFaceDevice PiFaceDevice
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        private IPEndPoint LocalEndPoint
        {
            get;
            set;
        }

        private Thread WorkerThread
        {
            get;
            set;
        }

        public PiFaceTcpServerStatus Status
        {
            get;
            private set;
        }

        private ManualResetEvent ClientConnected
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            lock (_lockObject)
            {
                this.WorkerThread = new Thread(this.ExecuteMainLoop)
                {
                    IsBackground = true
                };
                this.WorkerThread.Start();
                this.Status = PiFaceTcpServerStatus.Running;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            lock (_lockObject)
            {
                switch (this.Status)
                {
                    case PiFaceTcpServerStatus.Running:
                        this.Status = PiFaceTcpServerStatus.Stopping;
                        while (this.WorkerThread.IsAlive)
                        {
                            Thread.Sleep(100);
                        }
                        this.WorkerThread = null;
                        this.Status = PiFaceTcpServerStatus.Stopped;
                        break;
                    default:
                        throw new System.InvalidOperationException("Cannot stop a server with the status '" + this.Status.ToString() + "'");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ExecuteMainLoop()
        {
            this.ClientConnected = new ManualResetEvent(false);
            var listener = new TcpListener(this.LocalEndPoint);
            listener.Start();
            while (this.Status == PiFaceTcpServerStatus.Running)
            {
                if (listener.Pending())
                {
                    this.ClientConnected.Reset();
                    this.AcceptTcpClient(listener);
                    while (!this.ClientConnected.WaitOne(250))
                    {
                        if (this.Status != PiFaceTcpServerStatus.Running)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            listener.Stop();
        }

        private void AcceptTcpClient(TcpListener listener)
        {
            using (var client = listener.AcceptTcpClient())
            {
                using (var stream = client.GetStream())
                {
                    while (client.Connected && (this.Status == PiFaceTcpServerStatus.Running))
                    {
                        if (stream.DataAvailable)
                        {
                            try
                            {
                                this.ProcessMessage(stream);
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
            }
            this.ClientConnected.Set();
        }

        private void ProcessMessage(Stream stream)
        {
            // read the next message type
            var type = (PacketType)stream.ReadByte();
            // read the byte count
            var dataLength = (byte)stream.ReadByte();
            // read the message data
            var data = default(byte[]);
            if (dataLength > 0)
            {
                data = PiFaceTcpHelper.ReadBytes(stream, dataLength);
            }
            this.OnMessageReceived(new MessageReceivedEventArgs(type, data));
            // process the message
            var result = new List<Byte>();
            switch (type)
            {
                case PacketType.GetOutputPinState:
                {
                    var state = this.PiFaceDevice.GetOutputPinState(data[0]);
                    result.Add((byte) (state ? 0 : 1));
                    break;
                }
                case PacketType.GetOutputPinStates:
                    result.Add(this.PiFaceDevice.GetOutputPinStates());
                    break;
                case PacketType.SetOutputPinState:
                    this.PiFaceDevice.SetOutputPinState(data[0], (data[1] != 0));
                    break;
                case PacketType.SetOutputPinStates:
                    this.PiFaceDevice.SetOutputPinStates(data[0]);
                    break;
                case PacketType.GetInputPinState:
                {
                    var state = this.PiFaceDevice.GetInputPinState(data[0]);
                    result.Add((byte) (state ? 0 : 1));
                    break;
                }
                case PacketType.GetInputPinStates:
                    result.Add(this.PiFaceDevice.GetInputPinStates());
                    break;
                case PacketType.SetInputPinState:
                    this.PiFaceDevice.SetInputPinState(data[0], (data[1] != 0));
                    break;
                case PacketType.SetInputPinStates:
                    this.PiFaceDevice.SetInputPinStates(data[0]);
                    break;
                default:
                    throw new System.InvalidOperationException();
            }
            // write the result
            data = result.ToArray();
            stream.WriteByte((byte)result.Count);
            if (result.Count > 0)
            {
                stream.Write(data, 0, result.Count);
            }
            stream.Flush();
            this.OnResponseSent(new ResponseSentEventArgs(data));
        }
        
        #endregion

    }

}
