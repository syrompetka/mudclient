// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextMessageBlock.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Plain text message block that defines string and color.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Messages
{
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Themes;

    /// <summary>
    /// Plain text message block that defines string and color.
    /// </summary>
    /// TODO: 
    public class TextMessageBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessageBlock"/> class.
        /// </summary>
        /// <param name="text">The text of created block.</param>
        /// <param name="foreground">The foreground.</param>
        /// <param name="background">The background.</param>
        public TextMessageBlock([NotNull] string text, [NotNull] TextColor foreground, [NotNull] TextColor background)
        {
            Assert.ArgumentNotNull(text, "text");
            Assert.ArgumentNotNull(foreground, "foreground");
            Assert.ArgumentNotNull(background, "background");

            Text = text;
            Foreground = foreground;
            Background = background;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessageBlock"/> class.
        /// </summary>
        /// <param name="text">The text of created block.</param>
        /// <param name="foreground">The fore ground.</param>
        public TextMessageBlock([NotNull] string text, [NotNull] TextColor foreground)
            : this(text, foreground, TextColor.None)
        {
            Assert.ArgumentNotNull(text, "text");
            Assert.ArgumentNotNull(foreground, "foreground");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessageBlock"/> class.
        /// </summary>
        /// <param name="text">The text of created block.</param>
        public TextMessageBlock([NotNull] string text)
            : this(text, TextColor.None, TextColor.None)
        {
            Assert.ArgumentNotNull(text, "text");
        }

        /// <summary>
        /// Gets the text of this block.
        /// </summary>
        public string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the text foreground color of this block.
        /// </summary>
        public TextColor Foreground
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the text background color of this block.
        /// </summary>
        public TextColor Background
        {
            get;
            private set;
        }

        /// <summary>
        /// Changes the inner text.
        /// </summary>
        /// <param name="newInnerText">The new inner text.</param>
        public void ChangeInnerText([NotNull]string newInnerText)
        {
            Assert.ArgumentNotNull(newInnerText, "newInnerText");

            Text = newInnerText;
        }
    }
}