using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Adan.Client.Common.Conveyor;
using Adan.Client.Common.Messages;

namespace Adan.Client.Common.Networking
{
    /// <summary>
    /// 
    /// </summary>
    public class MccpClientEx : MccpClient
    {
        private Thread _thread;
        private AutoResetEvent _receivedDataEvent = new AutoResetEvent(false);

        private ConcurrentQueue<byte[]> _queue = new ConcurrentQueue<byte[]>();

        /// <summary>
        /// 
        /// </summary>
        public MccpClientEx() : base()
        {
            _thread = new Thread(new ThreadStart(ThreadRunning));
            _thread.IsBackground = true;
            _thread.Priority = ThreadPriority.BelowNormal;
            _thread.Start();
        }

        private void ThreadRunning()
        {
            byte[] data;

            while (true)
            {
                _receivedDataEvent.WaitOne();
                while (_queue.TryDequeue(out data))
                {
                    base.OnDataReceived(this, new DataReceivedEventArgs(data.Length, 0, data));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            byte[] data = new byte[e.BytesReceived];
            Array.Copy(e.GetData(), e.Offset, data, 0, e.BytesReceived);
            _queue.Enqueue(data);
            _receivedDataEvent.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            try
            {
                _thread.Abort();
            }
            catch { }

            _receivedDataEvent.Close();
        }
    }
}
