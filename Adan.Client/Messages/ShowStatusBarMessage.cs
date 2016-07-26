// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShowStatusBarMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ShowStatusBarMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Messages
{
    using Common.Messages;

    /// <summary>
    /// Message to start logging.
    /// </summary>
    public class ShowStatusBarMessage : Message
    {
        /// <summary>
        /// 
        /// </summary>
        public ShowStatusBarMessage(bool state)
        {
            State = state;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool State
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
        public override int MessageType
        {
            get
            {
                return BuiltInMessageTypes.ShowStatusBarMessage;
            }
        }
    }
}
