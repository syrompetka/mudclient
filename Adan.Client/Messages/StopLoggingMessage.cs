// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopLoggingMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the StopLoggingMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Messages
{
    using Common.Messages;

    /// <summary>
    /// Message to stop logging.
    /// </summary>
    public class StopLoggingMessage : Message
    {
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
                return BuiltInMessageTypes.LoggingMessage;
            }
        }
    }
}
