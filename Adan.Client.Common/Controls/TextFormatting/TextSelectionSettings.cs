// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextSelectionSettings.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TextSelectionSettings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Controls.TextFormatting
{
    using System.Collections.Generic;
    using System.Windows;

    using CSLib.Net.Annotations;

    using Messages;

    /// <summary>
    /// Class to store text selection settings.
    /// </summary>
    public class TextSelectionSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextSelectionSettings"/> class.
        /// </summary>
        public TextSelectionSettings()
        {
            SelectedMessages = new List<TextMessage>();
        }

        /// <summary>
        /// Gets the selected messages.
        /// </summary>
        [NotNull]
        public List<TextMessage> SelectedMessages
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the selection start character number.
        /// </summary>
        /// <value>
        /// The selection start character number.
        /// </value>
        public int SelectionStartCharacterNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the selection end character number.
        /// </summary>
        /// <value>
        /// The selection end character number.
        /// </value>
        public int SelectionEndCharacterNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether currently we are in selection process.
        /// </summary>
        /// <value>
        /// <c>true</c> if dragging; otherwise, <c>false</c>.
        /// </value>
        public bool Dragging
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether update is needed by renderer.
        /// </summary>
        /// <value>
        /// <c>true</c> if update is needed; otherwise, <c>false</c>.
        /// </value>
        public bool NeedUpdate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the selection start position.
        /// </summary>
        public Point SelectionStartPosition
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the selection end position.
        /// </summary>
        public Point SelectionEndPosition
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto scroll before selection started was on or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if auto scroll before selection started was turned on; otherwise, <c>false</c>.
        /// </value>
        public bool AutoScrollBeforeSelectionStarted
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the selection start position.
        /// </summary>
        /// <param name="point">The point.</param>
        public void SetSelectionStartPosition(Point point)
        {
            SelectionStartPosition = point;
            SelectionEndPosition = point;
        }

        /// <summary>
        /// Sets the selection end position.
        /// </summary>
        /// <param name="point">The point.</param>
        public void SetSelectionEndPosition(Point point)
        {
            SelectionEndPosition = point;
        }
    }
}
