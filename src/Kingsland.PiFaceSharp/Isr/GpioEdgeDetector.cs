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
    public sealed class GpioEdgeDetector : IDisposable
    {

        public event EventHandler InterruptOccured;

        private String deviceName;
        private byte pin;
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
            this.pin = pin;
            this.deviceName = String.Format("/sys/class/gpio/gpio{0}/value", pin);
            this.pollTimeout = pollTimeout;

            // enable edge detection 
            setEdgeDetection(pin, edge);

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

            // TODO: Error Handling!
            Task.Factory.StartNew(new Action<Object>(pollInterrupt), p, cancelTokenSource.Token).
                ContinueWith((t) => {
                    Console.WriteLine(String.Format("GpioEdgeDetector Error: {0}", (t.Exception != null ? t.Exception.ToString() : String.Empty)));
                }, System.Threading.CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Current);
        }

        private void setEdgeDetection(byte pin, EdgeDetectionMode edge)
        {
            if (edge == EdgeDetectionMode.none)
            {
                // open file handle to gpio export
                var fd = FCntl.open("/sys/class/gpio/unexport", FCntl.O_WRONLY);
                if (fd < 0)
                {
                    throw new IOException(string.Format("Failed to open gpio class - error {0}.", fd));
                }
                // write pin number to export gpio pin
                // (don't check for errors because it raises if already unexported..)
                var buf = System.Text.UTF8Encoding.UTF8.GetBytes(pin.ToString() + Environment.NewLine);
                UniStd.write(fd, buf, Convert.ToUInt32(buf.Length));
                FCntl.close(fd);
            }
            else
            {
                // open file handle to gpio export
                var fd = FCntl.open("/sys/class/gpio/export", FCntl.O_WRONLY);
                if (fd < 0)
                {
                    throw new IOException(string.Format("Failed to open gpio class - error {0}.", fd));
                }
                // write pin number to export gpio pin
                // (don't check for errors because it raises if already exported..)
                var buf = System.Text.UTF8Encoding.UTF8.GetBytes(pin.ToString() + Environment.NewLine);
                UniStd.write(fd, buf, Convert.ToUInt32(buf.Length));
                FCntl.close(fd);

                // wait short delay for export to complete
                System.Threading.Thread.Sleep(50);

                // open file handle to gpio direction
                fd = FCntl.open(String.Format("/sys/class/gpio/gpio{0}/direction", pin), FCntl.O_WRONLY);
                if (fd < 0)
                {
                    throw new IOException(string.Format("Failed to open gpio direction - error {0}.", fd));
                }
                // write pin number to export gpio direction
                buf = System.Text.UTF8Encoding.UTF8.GetBytes("in" + Environment.NewLine);
                UniStd.write(fd, buf, Convert.ToUInt32(buf.Length));
                FCntl.close(fd);

                // open file handle to gpio edge
                fd = FCntl.open(String.Format("/sys/class/gpio/gpio{0}/edge", pin), FCntl.O_WRONLY);
                if (fd < 0)
                {
                    throw new IOException(string.Format("Failed to open gpio edge - error {0}.", fd));
                }
                // write pin number to export gpio direction
                buf = System.Text.UTF8Encoding.UTF8.GetBytes(edge.ToString() + Environment.NewLine);
                UniStd.write(fd, buf, Convert.ToUInt32(buf.Length));
                FCntl.close(fd);
            }
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
            int errCnt = 0;
            while (!pp.cancelToken.IsCancellationRequested)
            {
                try
                {
                    if (poll(pp.deviceHandle, pp.pollTimeout))
                    {
                        OnInterruptOccured();
                    }
                    errCnt = 0;
                } catch (IOException)
                {
                    errCnt++;
                    if (errCnt > 3)
                    {
                        throw;
                    } else
                    {
                        System.Threading.Thread.Sleep(10);
                    }
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

        #region "IDisposable implementation"

        private bool disposed = false;

        void IDisposable.Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
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
                    if (this.pin > 0)
                    {
                        setEdgeDetection(this.pin, EdgeDetectionMode.none);
                    }                    
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion
    }
}
