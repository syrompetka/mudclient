// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionDarkThemeDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ExpressionDarkThemeDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Themes
{
    using System.Collections.Generic;
    using System.Windows.Media;

    /// <summary>
    /// Represents expression dark theme.
    /// </summary>
    public class ExpressionDarkThemeDescription : ThemeDescription
    {
        private readonly IList<string> _dictionariesToMerge = new List<string>();
        private readonly IDictionary<TextColor, SolidColorBrush> _textColors = new Dictionary<TextColor, SolidColorBrush>();
        private readonly SolidColorBrush _defaultTextColor = new SolidColorBrush(Color.FromRgb(192, 192, 192));

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionDarkThemeDescription"/> class.
        /// </summary>
        public ExpressionDarkThemeDescription()
            : base("ExpressionDark", "Expression dark")
        {
            _dictionariesToMerge.Add(@"/AvalonDock.Themes;component/themes/ExpressionDark.xaml");
            _dictionariesToMerge.Add(@"Resources/ExpressionDark.xaml");

            _textColors[TextColor.Black] = Brushes.Black;
            _textColors[TextColor.Blue] = new SolidColorBrush(Color.FromRgb(0, 0, 128));
            _textColors[TextColor.BrightBlack] = new SolidColorBrush(Color.FromRgb(96, 96, 96));
            _textColors[TextColor.BrightBlue] = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            _textColors[TextColor.BrightCyan] = new SolidColorBrush(Color.FromRgb(0, 255, 255));
            _textColors[TextColor.BrightGreen] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            _textColors[TextColor.BrightMagenta] = new SolidColorBrush(Color.FromRgb(255, 0, 255));
            _textColors[TextColor.BrightRed] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            _textColors[TextColor.BrightWhite] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            _textColors[TextColor.BrightYellow] = new SolidColorBrush(Color.FromRgb(255, 255, 0));
            _textColors[TextColor.Cyan] = new SolidColorBrush(Color.FromRgb(0, 128, 128));
            _textColors[TextColor.Green] = new SolidColorBrush(Color.FromRgb(0, 128, 0));
            _textColors[TextColor.Magenta] = new SolidColorBrush(Color.FromRgb(128, 0, 128));
            _textColors[TextColor.Red] = new SolidColorBrush(Color.FromRgb(128, 0, 0));
            _textColors[TextColor.RepeatCommandTextColor] = new SolidColorBrush(Color.FromRgb(128, 128, 0));
            _textColors[TextColor.Yellow] = new SolidColorBrush(Color.FromRgb(128, 128, 0));
            _textColors[TextColor.White] = new SolidColorBrush(Color.FromRgb(128, 128, 128));
        }

        /// <summary>
        /// Gets the dictionaries to merge.
        /// </summary>
        public override IEnumerable<string> DictionariesToMerge
        {
            get
            {
                return _dictionariesToMerge;
            }
        }

        /// <summary>
        /// Gets the brush by text color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="isBackground">if set to <c>true</c> [is back ground].</param>
        /// <returns>
        /// A <see cref="Brush"/> instance to use to draw specified text color.
        /// </returns>
        public override SolidColorBrush GetBrushByTextColor(TextColor color, bool isBackground)
        {
            if (color == TextColor.None)
            {
                return isBackground ? Brushes.Black : _defaultTextColor;
            }

            return _textColors[color];
        }
    }
}
