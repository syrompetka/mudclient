// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ErrorMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Messages
{
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Themes;

    /// <summary>
    /// Error that should be displayed to end user.
    /// </summary>
    public class ErrorMessage : OutputToMainWindowMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage"/> class.
        /// </summary>
        /// <param name="errorText">The error text.</param>
        public ErrorMessage([NotNull] string errorText)
            : base(errorText, TextColor.BrightRed)
        {
            Assert.ArgumentNotNullOrWhiteSpace(errorText, "errorText");
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
                return BuiltInMessageTypes.SystemMessage;
            }
        }
    }
}