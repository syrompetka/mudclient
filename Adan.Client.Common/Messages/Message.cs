// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Message.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Message type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Messages
{
    /// <summary>
    /// Base class for all messages.
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Message"/> was handled by some conveyor unit.
        /// </summary>
        /// <value>
        ///   <c>true</c> if handled; otherwise, <c>false</c>.
        /// </value>
        public bool Handled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the type of this message.
        /// </summary>
        /// <value>
        /// The type of this message.
        /// </value>
        public abstract int MessageType
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to skip triggers while processing this message or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if triggers should be skipped; otherwise, <c>false</c>.
        /// </value>
        public bool SkipTriggers
        {
            get;
            set;
        }
    }
}