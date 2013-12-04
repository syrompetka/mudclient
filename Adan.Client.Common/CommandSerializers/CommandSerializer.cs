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

        #endregion

        #region Constructors and Destructors

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public MessageConveyor Conveyor
        {
            get;
            set;
        }

        #region Public Methods

        /// <summary>
        /// Serializes the and sends specified command (if possible).
        /// </summary>
        /// <param name="command">The command.</param>
        public abstract void SerializeAndSendCommand([NotNull] Command command);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract CommandSerializer Clone();

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

            Conveyor.SendRawDataToServer(offset, bytesToSend, data);
        }

        #endregion
    }
}