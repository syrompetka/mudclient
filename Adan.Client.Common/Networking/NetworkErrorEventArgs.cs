// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkErrorEventArgs.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the NetworkErrorEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Networking
{
    using System;
    using System.Net.Sockets;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Event arguments for <see cref="TelnetClient.NetworkError"/> event.
    /// </summary>
    public sealed class NetworkErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkErrorEventArgs"/> class.
        /// </summary>
        /// <param name="socketException">The socket exception.</param>
        public NetworkErrorEventArgs([NotNull] Exception socketException)
        {
            Exception = socketException;
            Assert.ArgumentNotNull(socketException, "socketException");
        }

        /// <summary>
        /// Gets the socket exception.
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }
    }
}
