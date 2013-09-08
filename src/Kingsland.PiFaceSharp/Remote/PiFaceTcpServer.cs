using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public enum PiFaceTcpServerStatus
        {
            Stopped,
            Stopping,
            Running
        }

        #region Fields

        private object m_LockObject = new object();

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
            : base()
        {
            // validate the parameters
            if (device == null) { throw new System.ArgumentNullException("device"); }
            if (localEndPoint == null) { throw new System.ArgumentNullException("localEndPoint"); }
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
            lock (m_LockObject)
            {
                this.WorkerThread = new Thread(this.ExecuteMainLoop);
                this.WorkerThread.IsBackground = true;
                this.WorkerThread.Start();
                this.Status = PiFaceTcpServerStatus.Running;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            lock (m_LockObject)
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
                            catch (Exception ex)
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

        private void ProcessMessage(NetworkStream stream)
        {
            // read the next message type
            var type = (PacketType)stream.ReadByte();
            // read the byte count
            var dataLength = (byte)stream.ReadByte();
            // read the message data
            var data = (byte[])null;
            if (dataLength > 0)
            {
                data = PiFaceTcpHelper.ReadBytes(stream, dataLength);
            }
            // process the message
            var result = new List<Byte>();
            switch (type)
            {
                case PacketType.GetOutputPinState:
                    var state = this.PiFaceDevice.GetOutputPinState(data[0]);
                    result.Add((byte)(state ? 0 : 1));
                    break;
                case PacketType.GetOutputPinStates:
                    result.Add(this.PiFaceDevice.GetOutputPinStates());
                    break;
                case PacketType.SetOutputPinState:
                    this.PiFaceDevice.SetOutputPinState(data[0], (data[1] != 0));
                    break;
                case PacketType.SetOutputPinStates:
                    this.PiFaceDevice.SetOutputPinStates(data[0]);
                    break;
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
            stream.WriteByte((byte)result.Count);
            if (result.Count > 0)
            {
                stream.Write(result.ToArray(), 0, result.Count);
            }
            stream.Flush();
        }
        
        #endregion

    }

}
