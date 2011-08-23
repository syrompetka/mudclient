// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandSerializer.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the CommandProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.CommandSerializers
{
    using Commands;

    using Conveyor;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Base class for all 
    /// </summary>
    public abstract class CommandSerializer
    {
        #region Constants and Fields

        private readonly MessageConveyor _messageConveyor;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSerializer"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        protected CommandSerializer([NotNull] MessageConveyor messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");

            _messageConveyor = messageConveyor;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes the and sends specified command (if possible).
        /// </summary>
        /// <param name="command">The command.</param>
        public abstract void SerializeAndSendCommand([NotNull] Command command);

        #endregion

        #region Methods

        /// <summary>
        /// Sends the raw data to server.
        /// </summary>
        /// <param name="offset">The offset if <paramref name="data"/> array to start.</param>
        /// <param name="bytesToSend">The bytes to send.</param>
        /// <param name="data">The array of bytes to send.</param>
        protected void SendRawDataToServer(int offset, int bytesToSend, [NotNull] byte[] data)
        {
            Assert.ArgumentNotNull(data, "data");

            _messageConveyor.SendRawDataToServer(offset, bytesToSend, data);
        }

        #endregion
    }
}