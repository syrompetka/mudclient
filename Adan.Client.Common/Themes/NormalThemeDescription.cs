// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NormalThemeDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the NormalThemeDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Themes
{
    using System.Collections.Generic;
    using System.Windows.Media;
    using System.Linq;

    /// <summary>
    /// Represents "normal" theme.
    /// </summary>
    public class NormalThemeDescription : ThemeDescription
    {
        private readonly IList<string> _dictionariesToMerge = new List<string>();
        private readonly IDictionary<TextColor, SolidColorBrush> _textColors = new Dictionary<TextColor, SolidColorBrush>();
        private readonly SolidColorBrush _defaultTextColor = new SolidColorBrush(Color.FromRgb(128, 128, 128));

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalThemeDescription"/> class.
        /// </summary>
        public NormalThemeDescription()
            : base("Normal", "Normal")
        {
            _dictionariesToMerge.Add(@"Resources/Developer.xaml");

            _dictionariesToMerge.Add(@"/AvalonDock.Themes;component/themes/dev2010.xaml");

            _textColors[TextColor.Black] = Brushes.White;
            _textColors[TextColor.Blue] = new SolidColorBrush(Color.FromRgb(43, 145, 175));
            _textColors[TextColor.BrightBlack] = new SolidColorBrush(Color.FromRgb(128, 128, 128));
            _textColors[TextColor.BrightBlue] = new SolidColorBrush(Color.FromRgb(0, 52, 255));
            _textColors[TextColor.BrightCyan] = new SolidColorBrush(Color.FromRgb(0, 255, 255));
            _textColors[TextColor.BrightGreen] = new SolidColorBrush(Color.FromRgb(0, 200, 0));
            _textColors[TextColor.BrightMagenta] = new SolidColorBrush(Color.FromRgb(255, 0, 255));
            _textColors[TextColor.BrightRed] = new SolidColorBrush(Color.FromRgb(255, 0, 99));
            _textColors[TextColor.BrightWhite] = Brushes.Black;
            _textColors[TextColor.BrightYellow] = new SolidColorBrush(Color.FromRgb(128, 128, 0));
            _textColors[TextColor.Cyan] = new SolidColorBrush(Color.FromRgb(0, 128, 128));
            _textColors[TextColor.Green] = new SolidColorBrush(Color.FromRgb(0, 100, 0));
            _textColors[TextColor.Magenta] = new SolidColorBrush(Color.FromRgb(128, 0, 128));
            _textColors[TextColor.Red] = new SolidColorBrush(Color.FromRgb(163, 21, 21));
            _textColors[TextColor.RepeatCommandTextColor] = new SolidColorBrush(Color.FromRgb(128, 128, 0));
            _textColors[TextColor.Yellow] = new SolidColorBrush(Color.FromRgb(90, 90, 0));
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
                return isBackground ? Brushes.White : _defaultTextColor;
            }

            return _textColors[color];
        }

        /// <summary>
        /// Gets the text color by brush.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="isBackground">if set to <c>true</c> [is back ground].</param>
        /// <returns></returns>
        public override TextColor GetTextColorByBrush(SolidColorBrush color, bool isBackground)
        {
            return _textColors.FirstOrDefault(x => x.Value == color).Key;
        }
    }
}
