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
    using System;
    using System.Net.Sockets;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Message that appears when some network occurs.
    /// </summary>
    public class NetworkErrorMessageEx : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkErrorMessageEx"/> class.
        /// </summary>
        /// <param name="socketException">The socket exception.</param>
        public NetworkErrorMessageEx([NotNull] Exception socketException)
        {
            Assert.ArgumentNotNull(socketException, "socketException");
            Exception = socketException;
        }

        /// <summary>
        /// Gets the socket exception.
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of this message.
        /// </summary>
        /// <value>
        /// The type of this message.
        /// </value>
        public override int MessageType
        {
            get
            {
                return BuiltInMessageTypes.ConnectionMessages;
            }
        }
    }
}
