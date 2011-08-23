// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageDeserializer.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the MessageDeserializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.MessageDeserializers
{
    using Conveyor;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Messages;

    /// <summary>
    /// Base class for all message processors.
    /// </summary>
    public abstract class MessageDeserializer
    {
        #region Constants and Fields

        private readonly MessageConveyor _messageConveyor;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDeserializer"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        protected MessageDeserializer([NotNull] MessageConveyor messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");

            _messageConveyor = messageConveyor;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Processes the data from server.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="bytesReceived">The bytes received.</param>
        /// <param name="data">The get data.</param>
        /// <param name="isGagReceived">indicates whether <c>GAG</c> sequence was recieved or not.</param>
        /// <returns>Number of bytes that were read from <paramref name="data"/> buffer.</returns>
        public abstract int DeserializeDataFromServer(
            int offset, int bytesReceived, [NotNull] byte[] data, bool isGagReceived);

        #endregion

        #region Methods

        /// <summary>
        /// Pushes the message to conveyor.
        /// </summary>
        /// <param name="message">The message to push to conveyor.</param>
        protected void PushMessageToConveyor([NotNull] Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            _messageConveyor.PushMessage(message);
        }

        #endregion
    }
}