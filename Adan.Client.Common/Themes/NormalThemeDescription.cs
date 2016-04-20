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
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;
    using System.Linq;

    /// <summary>
    /// Represents "normal" theme.
    /// </summary>
    public class NormalThemeDescription : ThemeDescription
    {
        private readonly IList<string> _dictionariesToMerge = new List<string>();
        private readonly IDictionary<TextColor, SolidColorBrush> _textBrushes = new Dictionary<TextColor, SolidColorBrush>();
        private readonly IDictionary<TextColor, Color> _textColors = new Dictionary<TextColor, Color>();

        private readonly SolidColorBrush _defaultTextColor = new SolidColorBrush(Color.FromRgb(128, 128, 128));

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalThemeDescription"/> class.
        /// </summary>
        public NormalThemeDescription()
            : base("Normal", "Normal")
        {
            _dictionariesToMerge.Add(@"Resources/Developer.xaml");

            _dictionariesToMerge.Add(@"/AvalonDock.Themes;component/themes/dev2010.xaml");

            _textBrushes[TextColor.Black] = Brushes.White;
            _textBrushes[TextColor.Blue] = new SolidColorBrush(Color.FromRgb(43, 145, 175));
            _textBrushes[TextColor.BrightBlack] = new SolidColorBrush(Color.FromRgb(128, 128, 128));
            _textBrushes[TextColor.BrightBlue] = new SolidColorBrush(Color.FromRgb(0, 52, 255));
            _textBrushes[TextColor.BrightCyan] = new SolidColorBrush(Color.FromRgb(0, 255, 255));
            _textBrushes[TextColor.BrightGreen] = new SolidColorBrush(Color.FromRgb(0, 200, 0));
            _textBrushes[TextColor.BrightMagenta] = new SolidColorBrush(Color.FromRgb(255, 0, 255));
            _textBrushes[TextColor.BrightRed] = new SolidColorBrush(Color.FromRgb(255, 0, 99));
            _textBrushes[TextColor.BrightWhite] = Brushes.Black;
            _textBrushes[TextColor.BrightYellow] = new SolidColorBrush(Color.FromRgb(128, 128, 0));
            _textBrushes[TextColor.Cyan] = new SolidColorBrush(Color.FromRgb(0, 128, 128));
            _textBrushes[TextColor.Green] = new SolidColorBrush(Color.FromRgb(0, 100, 0));
            _textBrushes[TextColor.Magenta] = new SolidColorBrush(Color.FromRgb(128, 0, 128));
            _textBrushes[TextColor.Red] = new SolidColorBrush(Color.FromRgb(163, 21, 21));
            _textBrushes[TextColor.RepeatCommandTextColor] = new SolidColorBrush(Color.FromRgb(128, 128, 0));
            _textBrushes[TextColor.Yellow] = new SolidColorBrush(Color.FromRgb(90, 90, 0));
            _textBrushes[TextColor.White] = new SolidColorBrush(Color.FromRgb(128, 128, 128));

            _textColors[TextColor.Black] = Color.FromRgb(255, 255, 255);
            _textColors[TextColor.Blue] = Color.FromRgb(43, 145, 175);
            _textColors[TextColor.BrightBlack] = Color.FromRgb(128, 128, 128);
            _textColors[TextColor.BrightBlue] = Color.FromRgb(0, 52, 255);
            _textColors[TextColor.BrightCyan] = Color.FromRgb(0, 255, 255);
            _textColors[TextColor.BrightGreen] = Color.FromRgb(0, 200, 0);
            _textColors[TextColor.BrightMagenta] = Color.FromRgb(255, 0, 255);
            _textColors[TextColor.BrightRed] = Color.FromRgb(255, 0, 99);
            _textColors[TextColor.BrightWhite] = Color.FromRgb(0, 0, 0);
            _textColors[TextColor.BrightYellow] = Color.FromRgb(128, 128, 0);
            _textColors[TextColor.Cyan] = Color.FromRgb(0, 128, 128);
            _textColors[TextColor.Green] = Color.FromRgb(0, 100, 0);
            _textColors[TextColor.Magenta] = Color.FromRgb(128, 0, 128);
            _textColors[TextColor.Red] = Color.FromRgb(163, 21, 21);
            _textColors[TextColor.RepeatCommandTextColor] = Color.FromRgb(128, 128, 0);
            _textColors[TextColor.Yellow] = Color.FromRgb(90, 90, 0);
            _textColors[TextColor.White] = Color.FromRgb(128, 128, 128);
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

            return _textBrushes[color];
        }

        /// <summary>
        /// Gets the text color by brush.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="isBackground">if set to <c>true</c> [is back ground].</param>
        /// <returns></returns>
        public override TextColor GetTextColorByBrush(SolidColorBrush color, bool isBackground)
        {
            return _textBrushes.FirstOrDefault(x => x.Value == color).Key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isBackground"></param>
        /// <returns></returns>
        public override SolidColorBrush GetSelectionBrushByTextColor(bool isBackground)
        {
            return isBackground ? _textBrushes[TextColor.White] : _textBrushes[TextColor.Black];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="isBackground"></param>
        /// <returns></returns>
        public override Color GetColorByTextColor(TextColor color, bool isBackground)
        {
            if (color == TextColor.None)
            {
                return isBackground ? DefaultBackGroundColor : DefaultTextColor;
            }

            return _textColors[color];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isBackground"></param>
        /// <returns></returns>
        public override Color GetSelectionColorByTextColor(bool isBackground)
        {
            return isBackground ? _textColors[TextColor.White] : _textColors[TextColor.Black];
        }
    }
}
