// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelnetClient.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Networking
{
    using System;
    using System.Net.Sockets;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Basic wrapper of <see cref="Socket"/> to perform asynchronous input/output.
    /// </summary>
    public class TelnetClient : IDisposable
    {
        #region Constants and Fields

        private readonly byte[] _buffer;
        private readonly AsyncCallback _receiveDataCallback;
        private readonly AsyncCallback _connectedCallback;
        private readonly AsyncCallback _dataSentCallback;
        private Socket _theSocket;
        private SocketError _error;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TelnetClient"/> class.
        /// </summary>
        public TelnetClient()
        {
            _buffer = new byte[32767];
            _receiveDataCallback = new AsyncCallback(ReceiveData);
            _connectedCallback = new AsyncCallback(OnConnected);
            _dataSentCallback = new AsyncCallback(OnDataSent);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when data is received.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Occurs when some network error happens.
        /// </summary>
        public event EventHandler<NetworkErrorEventArgs> NetworkError;

        /// <summary>
        /// Occurs when client is connected to server.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Occurs when connection to server is lost.
        /// </summary>
        public event EventHandler Disconnected;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the last error.
        /// </summary>
        /// <value>The last error.</value>
        public SocketError LastError
        {
            get { return _error; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Connects to the specified host and port.
        /// </summary>
        /// <param name="host">
        /// The host to connect to.
        /// </param>
        /// <param name="port">
        /// The port to use.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "It's ok here.")]
        public virtual void Connect([NotNull] string host, int port)
        {
            Validate.ArgumentNotNull(host, "host");

            _theSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _theSocket.BeginConnect(host, port, _connectedCallback, null);
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public virtual void Disconnect()
        {
            if (_theSocket != null)
            {
                Dispose(true);
                OnDisconnected();
            }
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer containing data to send.</param>
        /// <param name="offset">The zero-based position in <paramref name="buffer"/> at which to begin sending data.</param>
        /// <param name="length">The number of bytes to send.</param>
        public virtual void Send([NotNull] byte[] buffer, int offset, int length)
        {
            Validate.ArgumentNotNull(buffer, "buffer");
            try
            {
                if (_theSocket != null)
                {
                    _theSocket.BeginSend(buffer, offset, length, SocketFlags.None, out _error, _dataSentCallback, null);
                }
            }
            catch (SocketException exception)
            {
                HandleSockedException(exception);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method is called each time when some data is received from server.
        /// </summary>
        /// <param name="sender">
        /// The source of event.
        /// </param>
        /// <param name="e">
        /// Arguments of event.
        /// </param>
        protected virtual void OnDataReceived([NotNull] object sender, [NotNull] DataReceivedEventArgs e)
        {
            Validate.ArgumentNotNull(sender, "sender");
            Validate.ArgumentNotNull(e, "e");

            if (DataReceived != null)
            {
                DataReceived(sender, e);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var socket = _theSocket;
                _theSocket = null;
                if (socket != null)
                {
                    socket.Dispose();
                }
            }
        }

        /// <summary>
        /// Called when this client looses connection.
        /// </summary>
        protected virtual void OnDisconnected()
        {
            if (Disconnected != null)
            {
                Disconnected(this, EventArgs.Empty);
            }
        }

        private void OnDataReceived(int count)
        {
            OnDataReceived(this, new DataReceivedEventArgs(count, 0, _buffer));
        }

        private void Initialize()
        {
            if (_theSocket != null)
            {
                _theSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, out _error, _receiveDataCallback, null);
            }
        }

        private void ReceiveData([NotNull] IAsyncResult ar)
        {
            Validate.ArgumentNotNull(ar, "ar");
            try
            {
                if (_theSocket != null)
                {
                    var bytesRecieved = _theSocket.EndReceive(ar);
                    if (bytesRecieved > 0)
                    {
                        OnDataReceived(bytesRecieved);
                        Initialize();
                    }
                    else
                    {
                        OnDisconnected();
                    }
                }
            }
            catch (SocketException exception)
            {
                HandleSockedException(exception);
            }
        }

        private void OnConnected([NotNull] IAsyncResult ar)
        {
            Assert.ArgumentNotNull(ar, "ar");
            try
            {
                if (_theSocket != null)
                {
                    _theSocket.EndConnect(ar);

                    Initialize();
                    if (Connected != null)
                    {
                        Connected(this, EventArgs.Empty);
                    }
                }
            }
            catch (SocketException exception)
            {
                HandleSockedException(exception);
            }
        }

        private void OnDataSent([NotNull] IAsyncResult ar)
        {
            Assert.ArgumentNotNull(ar, "ar");

            try
            {
                if (_theSocket != null)
                {
                    _theSocket.EndSend(ar);
                }
            }
            catch (SocketException exception)
            {
                HandleSockedException(exception);
            }
        }

        private void HandleSockedException([NotNull] SocketException exception)
        {
            Assert.ArgumentNotNull(exception, "exception");
            Dispose(true);
            if (NetworkError != null)
            {
                NetworkError(this, new NetworkErrorEventArgs(exception));
            }
        }

        #endregion
    }
}