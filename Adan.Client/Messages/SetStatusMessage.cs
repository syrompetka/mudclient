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
    public class SetStatusMessage : Message
    {
        /// <summary>
        /// 
        /// </summary>
        public SetStatusMessage(string id, string msg, string color = "")
        {
            Id = id;
            Msg = msg;
            Color = color;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Msg
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Color
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
                return BuiltInMessageTypes.SetStatusMessage;
            }
        }
    }
}
