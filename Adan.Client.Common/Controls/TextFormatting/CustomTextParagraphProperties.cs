// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomTextParagraphProperties.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the CustomTextParagraphProperties type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Controls.TextFormatting
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.TextFormatting;

    using CSLib.Net.Annotations;

    /// <summary>
    /// Provides a set of properties, such as flow direction, alignment, or indentation, that can be applied to a paragraph.
    /// </summary>
    public class CustomTextParagraphProperties : TextParagraphProperties
    {
        private readonly CustomTextRunProperties _defaultTextRunProperties = new CustomTextRunProperties(Brushes.White, Brushes.Black);

        /// <summary>
        /// Gets a value that specifies whether the primary text advance direction shall be left-to-right, or right-to-left.
        /// </summary>
        /// <returns>
        /// An enumerated value of <see cref="T:System.Windows.FlowDirection"/>.
        /// </returns>
        public override FlowDirection FlowDirection
        {
            get
            {
                return FlowDirection.LeftToRight;
            }
        }

        /// <summary>
        /// Gets a value that describes how an inline content of a block is aligned.
        /// </summary>
        /// <returns>
        /// An enumerated value of <see cref="T:System.Windows.TextAlignment"/>.
        /// </returns>
        public override TextAlignment TextAlignment
        {
            get
            {
                return TextAlignment.Left;
            }
        }

        /// <summary>
        /// Gets the height of a line of text.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Double"/> that represents the height of a line of text.
        /// </returns>
        public override double LineHeight
        {
            get
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the text run is the first line of the paragraph.
        /// </summary>
        /// <returns>
        /// true, if the text run is the first line of the paragraph; otherwise, false.
        /// </returns>
        public override bool FirstLineInParagraph
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the default text run properties, such as typeface or foreground brush.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Media.TextFormatting.TextRunProperties"/> value.
        /// </returns>
        [CanBeNull]
        public override TextRunProperties DefaultTextRunProperties
        {
            get
            {
                return _defaultTextRunProperties;
            }
        }

        /// <summary>
        /// Gets a value that controls whether text wraps when it reaches the flow edge of its containing block box.
        /// </summary>
        /// <returns>
        /// An enumerated value of <see cref="T:System.Windows.TextWrapping"/>.
        /// </returns>
        public override TextWrapping TextWrapping
        {
            get
            {
                return TextWrapping.Wrap;
            }
        }

        /// <summary>
        /// Gets a value that specifies marker characteristics of the first line in the paragraph.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Media.TextFormatting.TextMarkerProperties"/> value.
        /// </returns>
        public override TextMarkerProperties TextMarkerProperties
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the amount of line indentation.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Double"/> that represents the amount of line indentation.
        /// </returns>
        public override double Indent
        {
            get
            {
                return 0.0;
            }
        }
    }
}
