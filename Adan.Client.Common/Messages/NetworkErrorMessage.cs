// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkErrorMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the NetworkErrorMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Messages
{
    using System.Net.Sockets;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Message that appears when some network occurs.
    /// </summary>
    public class NetworkErrorMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkErrorMessage"/> class.
        /// </summary>
        /// <param name="socketException">The socket exception.</param>
        public NetworkErrorMessage([NotNull] SocketException socketException)
        {
            Assert.ArgumentNotNull(socketException, "socketException");
            SocketException = socketException;
        }

        /// <summary>
        /// Gets the socket exception.
        /// </summary>
        public SocketException SocketException
        {
            get;
            private set;
        }
    }
}
