﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomTextRunProperties.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Provides a set of properties, such as typeface or foreground brush, that can be applied to a System.Windows.Media.TextFormatting.TextRun object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Controls.TextFormatting
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.TextFormatting;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Provides a set of properties, such as typeface or foreground brush, that can be applied to a System.Windows.Media.TextFormatting.TextRun object.
    /// </summary>
    public class CustomTextRunProperties : TextRunProperties
    {
        // private readonly Typeface _typeface = new Typeface(new FontFamily(new Uri("pack://application:,,,/"), "/Resources/consola.ttf#Consolas"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        private readonly Typeface _typeface = new Typeface("Consolas, Lucida Console");
        private readonly TextDecorationCollection _textDecorations = new TextDecorationCollection();
        private readonly TextEffectCollection _textEffects = new TextEffectCollection();
        private readonly Brush _foregroundBrush;
        private readonly Brush _backgroundBrush;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTextRunProperties"/> class.
        /// </summary>
        /// <param name="foregroundBrush">The foreground brush.</param>
        /// <param name="backgroundBrush">The background brush.</param>
        public CustomTextRunProperties([NotNull] Brush foregroundBrush, [NotNull] Brush backgroundBrush)
        {
            Assert.ArgumentNotNull(foregroundBrush, "foregroundBrush");
            Assert.ArgumentNotNull(backgroundBrush, "backgroundBrush");
            _foregroundBrush = foregroundBrush;
            _backgroundBrush = backgroundBrush;
        }

        /// <summary>
        /// Gets the typeface for the text run.
        /// </summary>
        /// <returns>
        /// A value of <see cref="T:System.Windows.Media.Typeface"/>.
        /// </returns>
        [NotNull]
        public override Typeface Typeface
        {
            get
            {
                return _typeface;
            }
        }

        /// <summary>
        /// Gets the text size in points for the text run.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Double"/> that represents the text size in DIPs (Device Independent Pixels). The default is 12 DIP.
        /// </returns>
        public override double FontRenderingEmSize
        {
            get
            {
                return 14.0;
            }
        }

        /// <summary>
        /// Gets the text size in points, which is then used for font hinting.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Double"/> that represents the text size in points. The default is 12 pt.
        /// </returns>
        public override double FontHintingEmSize
        {
            get
            {
                return 14.0;
            }
        }

        /// <summary>
        /// Gets the collection of  <see cref="T:System.Windows.TextDecoration"/> objects used for the text run.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.TextDecorationCollection"/> value.
        /// </returns>
        [NotNull]
        public override TextDecorationCollection TextDecorations
        {
            get
            {
                return _textDecorations;
            }
        }

        /// <summary>
        /// Gets the brush that is used to paint the foreground color of the text run.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Media.Brush"/> value that represents the foreground color.
        /// </returns>
        [NotNull]
        public override Brush ForegroundBrush
        {
            get
            {
                return _foregroundBrush;
            }
        }

        /// <summary>
        /// Gets the brush that is used to paint the background color of the text run.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Media.Brush"/> value that represents the background color.
        /// </returns>
        [NotNull]
        public override Brush BackgroundBrush
        {
            get
            {
                return _backgroundBrush;
            }
        }

        /// <summary>
        /// Gets the culture information for the text run.
        /// </summary>
        /// <returns>
        /// A value of <see cref="T:System.Globalization.CultureInfo"/> that represents the culture of the text run.
        /// </returns>
        [NotNull]
        public override CultureInfo CultureInfo
        {
            get
            {
                return CultureInfo.CurrentUICulture;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="T:System.Windows.Media.TextEffect"/> objects used for the text run.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Media.TextEffectCollection"/> value.
        /// </returns>
        public override TextEffectCollection TextEffects
        {
            get
            {
                return _textEffects;
            }
        }
    }
}
