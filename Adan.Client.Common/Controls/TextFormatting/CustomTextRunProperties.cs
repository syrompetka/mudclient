// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomTextRunProperties.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Provides a set of properties, such as typeface or foreground brush, that can be applied to a System.Windows.Media.TextFormatting.TextRun object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Controls.TextFormatting
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.TextFormatting;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Settings;
    /// <summary>
    /// Provides a set of properties, such as typeface or foreground brush, that can be applied to a System.Windows.Media.TextFormatting.TextRun object.
    /// </summary>
    public class CustomTextRunProperties : TextRunProperties
    {
        // private readonly Typeface _typeface = new Typeface(new FontFamily(new Uri("pack://application:,,,/"), "/Resources/consola.ttf#Consolas"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        private Typeface _typeface;
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
            //_typeface = new Typeface(SettingsHolder.Instance.Settings.MUDFontName);
            var weight = GetFontWeightFromString(SettingsHolder.Instance.Settings.MudFontWeight);
            _typeface = new Typeface(new FontFamily(SettingsHolder.Instance.Settings.MUDFontName), FontStyles.Normal, weight, FontStretches.Normal);
            SettingsHolder.Instance.Settings.OnSettingsChanged += (s, e) =>
            {
                if (e.Name == "MUDFontName" || e.Name == "MUDFontSize" || e.Name == "MUDFontWeight" || e.Name == "ColorTheme")
                {
                    //_typeface = new Typeface(SettingsHolder.Instance.Settings.MUDFontName);
                    var fontWeight = GetFontWeightFromString(SettingsHolder.Instance.Settings.MudFontWeight);
                    _typeface = new Typeface(new FontFamily(SettingsHolder.Instance.Settings.MUDFontName), FontStyles.Normal, fontWeight, FontStretches.Normal);
                }
            };
        }

        private FontWeight GetFontWeightFromString(string mudFontWeight)
        {
            switch(mudFontWeight)
            {
                //"Thin", "ExtraLight", "Light", "Normal", "Medium", "DemiBold", "Bold", "ExtraBold", "Black", "ExtraBlack" 
                case "Thin":
                    return FontWeights.Thin;
                case "ExtraLight":
                    return FontWeights.ExtraLight;
                case "Light":
                    return FontWeights.Light;
                case "Medium":
                    return FontWeights.Medium;
                case "DemiBold":
                    return FontWeights.DemiBold;
                case "ExtraBold":
                    return FontWeights.ExtraBold;
                case "Black":
                    return FontWeights.Black;
                case "ExtraBlack":
                    return FontWeights.ExtraBlack;
                case "Normal":
                default:
                    return FontWeights.Normal;
            }
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
                return FontHintingEmSize * 96 / 72;
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
                return SettingsHolder.Instance.Settings.MUDFontSize;
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
