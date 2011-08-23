using System;

using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;


namespace ConsoleApplication3
{
    /// <summary>
    /// Event arguments for <see cref="TelnetClient.DataReceived"/> event.
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="buffer">The buffer.</param>
        internal DataReceivedEventArgs(int count, [NotNull] byte[] buffer)
        {
            Validate.ArgumentNotNull(buffer, "buffer");

            BytesReceived = count;
            Data = buffer;
        }

        /// <summary>
        /// Gets the bytes received.
        /// </summary>
        /// <value>The bytes received.</value>
        public int BytesReceived
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets received data.
        /// </summary>
        /// <value>The received data.</value>
        public byte[] Data
        {
            get;
            private set;
        }
    }
}
