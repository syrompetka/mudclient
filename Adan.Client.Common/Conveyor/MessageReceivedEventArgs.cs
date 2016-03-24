// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageReceivedEventArgs.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the MessageReceivedEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Conveyor
{
    #region Namespace Imports

    using System;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Messages;

    #endregion

    /// <summary>
    /// Event arguments for <see cref="MessageConveyor.MessageReceived"/> event.
    /// </summary>
    public sealed class MessageReceivedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessageReceivedEventArgs([NotNull] Message message)
        {
            Validate.ArgumentNotNull(message, "message");

            Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the message that was received.
        /// </summary>
        [NotNull]
        public Message Message
        {
            get;
            private set;
        }

        #endregion
    }
}