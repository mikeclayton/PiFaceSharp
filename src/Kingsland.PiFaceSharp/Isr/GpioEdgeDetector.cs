using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kingsland.PiFaceSharp.Spi.Native;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Kingsland.PiFaceSharp.Isr
{
    public class GpioEdgeDetector : IDisposable
    {

        public event EventHandler InterruptOccured;

        private String deviceName;
        private int pollTimeout = -1;
        private int deviceHandle = 0;
        private System.Threading.CancellationTokenSource cancelTokenSource;


        private struct pollParams
        {
            public int deviceHandle;
            public int pollTimeout;
            public System.Threading.CancellationToken cancelToken;
        }

        public GpioEdgeDetector(byte pin, EdgeDetectionMode edge, int pollTimeout = -1)
        {
            this.deviceName = String.Format("/sys/class/gpio/gpio{0}/value", pin);
            this.pollTimeout = pollTimeout;

            // set edge detection and export pin via gpio command
            Process proc = new Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = "gpio";
            proc.StartInfo.Arguments = String.Format("edge {0} {1}", pin, edge.ToString());
            Console.WriteLine("calling gpio with {0}..", proc.StartInfo.Arguments);
            proc.Start();
            proc.WaitForExit();
            if (proc.ExitCode != 0)
            {
                throw new IOException("could not set gpio edge detection - error {0}.", proc.ExitCode);
            }

            var result = FCntl.open(this.deviceName, FCntl.O_RDONLY);
            if (result < 0)
            {
                throw new IOException(string.Format("Failed to open device - error {0}.", result));
            }
            this.deviceHandle = result;

            // clear pending interrupts
            readBufs(this.deviceHandle);

            // start async polling task
            cancelTokenSource = new System.Threading.CancellationTokenSource();
            pollParams p = new pollParams();
            p.cancelToken = cancelTokenSource.Token;
            p.deviceHandle = this.deviceHandle;
            p.pollTimeout = this.pollTimeout;
            Task.Factory.StartNew(new Action<Object>(pollInterrupt), p, cancelTokenSource.Token);
        }

        private void readBufs(int handle)
        {
            byte[] buf = new byte[4096];
            int bytesread = 0;
            do
            {
                bytesread = UniStd.read(handle, ref buf, Convert.ToUInt32(buf.Length));
                if (bytesread < 0)
                {
                    throw new IOException(string.Format("Failed to read - error {0}.", bytesread));
                }
            } while (bytesread > 0);
            if (UniStd.lseek(handle, 0, 0) < 0)
            {
                throw new IOException("Failed to seek.");
            }
        }

        private void pollInterrupt(Object p)
        {
            pollParams pp = (pollParams)p;
            while (!pp.cancelToken.IsCancellationRequested)
            {
                if (poll(pp.deviceHandle, pp.pollTimeout))
                {
                    OnInterruptOccured();
                }
            }
        }

        private void OnInterruptOccured()
        {
            if (InterruptOccured != null)
            {
                InterruptOccured(this, EventArgs.Empty);
            }
        }

        private bool poll(int fd, int timeout)
        {
            Mono.Unix.Native.Pollfd[] pollFDs = new Mono.Unix.Native.Pollfd[1];
            pollFDs[0] = new Mono.Unix.Native.Pollfd();
            pollFDs[0].fd = fd;
            pollFDs[0].events = Mono.Unix.Native.PollEvents.POLLPRI | Mono.Unix.Native.PollEvents.POLLERR;
            pollFDs[0].revents = 0;

            // poll 
            int result = Mono.Unix.Native.Syscall.poll(pollFDs, timeout);
            if (result < 0)
            {
                throw new IOException(string.Format("Failed to poll - error {0}.", result));
            }
            else if (result > 0)
            {
                // clear interrupt
                readBufs(fd);

                return (pollFDs[0].revents & Mono.Unix.Native.PollEvents.POLLPRI) == Mono.Unix.Native.PollEvents.POLLPRI;
            }
            return false;
        }


        #region IDisposable Interface

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(Boolean disposing)
        {
            if (cancelTokenSource != null)
            {
                cancelTokenSource.Cancel();
                if (this.pollTimeout >= 0)
                {
                    // only wait for task to finish if poll timeout is not infinite
                    cancelTokenSource.Token.WaitHandle.WaitOne();
                }                
                cancelTokenSource = null;
            }
            if (this.deviceHandle != 0)
            {
                try
                {
                    FCntl.close(this.deviceHandle);
                }
                finally
                {
                    this.deviceHandle = 0;
                }
            }
        }

        #endregion
    }
}
