// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataReceivedEventArgs.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Event arguments for <see cref="TelnetClient.DataReceived" /> event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Networking
{
    using System;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Event arguments for <see cref="TelnetClient.DataReceived"/> event.
    /// </summary>
    public sealed class DataReceivedEventArgs : EventArgs
    {
        private readonly byte[] _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="buffer">The buffer.</param>
        internal DataReceivedEventArgs(int count, int offset, [NotNull] byte[] buffer)
        {
            Validate.ArgumentNotNull(buffer, "buffer");

            Offset = offset;
            BytesReceived = count;
            _data = buffer;
        }

        /// <summary>
        /// Gets the bytes received.
        /// </summary>
        /// <value>The bytes received.</value>
        public int BytesReceived { get; private set; }

        /// <summary>
        /// Gets the offset in buffer.
        /// </summary>
        public int Offset
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the received data.
        /// </summary>
        /// <returns>An array of bytes that contains received data.</returns>
        public byte[] GetData()
        {
            return _data;
        }
    }
}