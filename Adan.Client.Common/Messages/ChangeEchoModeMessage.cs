// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeEchoModeMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ChangeEchoModeMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Messages
{
    /// <summary>
    /// Message to change echo mode.
    /// </summary>
    public class ChangeEchoModeMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeEchoModeMessage"/> class.
        /// </summary>
        /// <param name="displayTypedCharacters">if set to <c>true</c> the typed characters will be displayed; otherwise - not.</param>
        public ChangeEchoModeMessage(bool displayTypedCharacters)
        {
            DisplayTypedCharacters = displayTypedCharacters;
        }

        /// <summary>
        /// Gets a value indicating whether to display typed characters or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if typed characters should be displayed; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayTypedCharacters
        {
            get;
            private set;
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
                return BuiltInMessageTypes.EchoModeMessage;
            }
        }
    }
}
