// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectCommand.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ConnectCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Commands
{
    using Common.Commands;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Command to connect to server.
    /// </summary>
    public class ConnectCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectCommand"/> class.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to user.</param>
        public ConnectCommand([NotNull] string host, int port)
        {
            Host = host;
            Port = port;
            Assert.ArgumentNotNullOrWhiteSpace(host, "host");
        }

        /// <summary>
        /// Gets the host to connect to.
        /// </summary>
        public string Host
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the port to use.
        /// </summary>
        public int Port
        {
            get;
            private set;
        }
    }
}
