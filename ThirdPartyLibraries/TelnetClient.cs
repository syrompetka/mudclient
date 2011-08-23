#region Namespace Imports

using System;
using System.Net.Sockets;

using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

#endregion


namespace ConsoleApplication3
{
    /// <summary>
    /// Basic wrapper of <see cref="Socket"/> to perform asynchronous input/output.
    /// </summary>
    public class TelnetClient : IDisposable
    {
        #region Constants and Fields

        private readonly byte[] _buffer;
        private readonly AsyncCallback _initalizeCallback;
        private readonly AsyncCallback _receiveDataCallback;
        private readonly Socket _theSocket;
        private SocketError _error;

        #endregion


        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TelnetClient"/> class.
        /// </summary>
        public TelnetClient()
        {
            _theSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _buffer = new byte[32767];
            _receiveDataCallback = new AsyncCallback(ReceiveData);
            _initalizeCallback = new AsyncCallback(Initialize);
        }


        /// <summary>
        /// Finalizes an instance of the <see cref="TelnetClient"/> class. 
        /// <see cref="TelnetClient"/> is reclaimed by garbage collection.
        /// </summary>
        ~TelnetClient()
        {
            Dispose();
        }

        #endregion


        #region Events

        /// <summary>
        /// Occurs when data is received.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        #endregion


        #region Properties

        /// <summary>
        /// Gets the last error.
        /// </summary>
        /// <value>The last error.</value>
        public SocketError LastError
        {
            get
            {
                return _error;
            }
        }

        #endregion


        #region Public Methods


        /// <summary>
        /// Connects to the specified host and port.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to use.</param>
        public void Connect([NotNull] string host, int port)
        {
            Validate.ArgumentNotNull(host, "host");

            _theSocket.Connect(host, port);
            Initialize(null);
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            _theSocket.Disconnect(false);
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _theSocket.Close();
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public void Send([NotNull] byte[] buffer)
        {
            Validate.ArgumentNotNull(buffer, "buffer");

            _theSocket.EndSend(_theSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, out _error, _initalizeCallback, null));
        }

        #endregion


        #region Methods

        private void FireDataReceived([NotNull] object sender, [NotNull] DataReceivedEventArgs e)
        {
            Validate.ArgumentNotNull(sender, "sender");
            Validate.ArgumentNotNull(e, "e");

            if (DataReceived != null)
            {
                DataReceived(sender, e);
            }
        }


        private void FireDataReceived(int count)
        {
            FireDataReceived(this, new DataReceivedEventArgs(count, _buffer));
        }


        private void Initialize([CanBeNull] IAsyncResult ar)
        {
            _theSocket.BeginReceive(
                _buffer, 0, _buffer.Length, SocketFlags.None, out _error, _receiveDataCallback, null);
        }


        private void ReceiveData([NotNull] IAsyncResult ar)
        {
            Validate.ArgumentNotNull(ar, "ar");

            FireDataReceived(_theSocket.EndReceive(ar));
        }

        #endregion
    }
}