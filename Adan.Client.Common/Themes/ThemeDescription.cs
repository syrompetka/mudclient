// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemeDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ThemeDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Themes
{
    using System.Collections.Generic;
    using System.Windows.Media;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A description of a single theme.
    /// </summary>
    public abstract class ThemeDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeDescription"/> class.
        /// </summary>
        /// <param name="name">The name of created theme.</param>
        /// <param name="description">The description of created theme.</param>
        protected ThemeDescription([NotNull] string name, [NotNull] string description)
        {
            Assert.ArgumentNotNullOrWhiteSpace(name, "name");
            Assert.ArgumentNotNullOrWhiteSpace(description, "description");

            Name = name;
            Description = description;
        }

        /// <summary>
        /// Gets the dictionaries to merge.
        /// </summary>
        [NotNull]
        public abstract IEnumerable<string> DictionariesToMerge
        {
            get;
        }

        /// <summary>
        /// Gets the name of this theme.
        /// </summary>
        [NotNull]
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the description of this theme.
        /// </summary>
        [NotNull]
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// Default text color
        /// </summary>
        public SolidColorBrush DefaultTextBrush
        {
            get;
            protected set;
        }

        /// <summary>
        /// Default background color
        /// </summary>
        public SolidColorBrush DefaultBackGroundBrush
        {
            get;
            protected set;
        }

        /// <summary>
        /// Default text color
        /// </summary>
        public Color DefaultTextColor
        {
            get;
            protected set;
        }

        /// <summary>
        /// Default background color
        /// </summary>
        public Color DefaultBackGroundColor
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the brush by text color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="isBackground">if set to <c>true</c> [is back ground].</param>
        /// <returns>A <see cref="Brush"/> instance to use to draw specified text color.</returns>
        [NotNull]
        public abstract SolidColorBrush GetBrushByTextColor(TextColor color, bool isBackground);

        /// <summary>
        /// Gets the text color by brush.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="isBackground">if set to <c>true</c> [is back ground].</param>
        /// <returns></returns>
        [NotNull]
        public abstract TextColor GetTextColorByBrush(SolidColorBrush color, bool isBackground);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isBackground"></param>
        /// <returns></returns>
        [NotNull]
        public abstract SolidColorBrush GetSelectionBrushByTextColor(bool isBackground);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="isBackground"></param>
        /// <returns></returns>
        [NotNull]
        public abstract Color GetColorByTextColor(TextColor color, bool isBackground);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isBackground"></param>
        /// <returns></returns>
        [NotNull]
        public abstract Color GetSelectionColorByTextColor(bool isBackground);
    }
}
