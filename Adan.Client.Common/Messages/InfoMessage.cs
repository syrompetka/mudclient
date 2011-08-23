// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Information message that should be displayed to end user.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Messages
{
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Themes;

    /// <summary>
    /// Information message that should be displayed to end user.
    /// </summary>
    public class InfoMessage : OutputToMainWindowMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        public InfoMessage([NotNull] string text)
            : base(text, TextColor.BrightWhite)
        {
            Assert.ArgumentNotNullOrWhiteSpace(text, "text");
            SkipProcessing = true;
        }
    }
}
