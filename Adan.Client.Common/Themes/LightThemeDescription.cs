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
    using System.Linq;

    /// <summary>
    /// Represents expression dark theme.
    /// </summary>
    public class LightThemeDescription : ThemeDescription
    {
        private readonly IList<string> _dictionariesToMerge = new List<string>();
        private readonly IDictionary<TextColor, SolidColorBrush> _textBrushes = new Dictionary<TextColor, SolidColorBrush>();
        private readonly IDictionary<TextColor, Color> _textColors = new Dictionary<TextColor, Color>();

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public LightThemeDescription()
            : base("Light", "Light")
        {
            _dictionariesToMerge.Add(@"Resources/ExpressionDark.xaml");

            _textColors[TextColor.Black] = Color.FromRgb(218, 199, 158);
            _textColors[TextColor.Red] = Color.FromRgb(192, 0, 0);
            _textColors[TextColor.Green] = Color.FromRgb(0, 100, 0);
            _textColors[TextColor.Yellow] = Color.FromRgb(192, 192, 0);
            _textColors[TextColor.Blue] = Color.FromRgb(0, 0, 64);
            _textColors[TextColor.Cyan] = Color.FromRgb(0, 64, 64);
            _textColors[TextColor.Magenta] = Color.FromRgb(192, 0, 192);
            _textColors[TextColor.White] = Color.FromRgb(0, 0, 0);
            _textColors[TextColor.BrightBlack] = Color.FromRgb(73, 53, 35);
            _textColors[TextColor.BrightRed] = Color.FromRgb(168, 0, 0);
            _textColors[TextColor.BrightGreen] = Color.FromRgb(44, 84, 51);
            _textColors[TextColor.BrightYellow] = Color.FromRgb(55, 94, 136);
            _textColors[TextColor.BrightBlue] = Color.FromRgb(97, 142, 248);
            _textColors[TextColor.BrightCyan] = Color.FromRgb(0, 136, 136);
            _textColors[TextColor.BrightMagenta] = Color.FromRgb(255, 0, 255);
            _textColors[TextColor.BrightWhite] = Color.FromRgb(13, 31, 56);

            _textColors[TextColor.RepeatCommandTextColor] = Color.FromRgb(64, 64, 64);

            foreach (TextColor color in System.Enum.GetValues(typeof(TextColor)))
            {
                if (color == TextColor.None)
                    continue;
                _textBrushes[color] = new SolidColorBrush(_textColors[color]);
            }

            foreach(var brush in _textBrushes)
            {
                if (brush.Value.CanFreeze)
                    brush.Value.Freeze();
            }

            DefaultTextBrush = new SolidColorBrush(_textColors[TextColor.White]);
            if (DefaultTextBrush.CanFreeze)
                DefaultTextBrush.Freeze();

            DefaultBackGroundBrush = new SolidColorBrush(_textColors[TextColor.Black]);
            if (DefaultBackGroundBrush.CanFreeze)
                DefaultBackGroundBrush.Freeze();

            DefaultTextColor = _textColors[TextColor.White];
            DefaultBackGroundColor = _textColors[TextColor.Black];
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
                //return isBackground ? Brushes.Black : _defaultTextColor;
                return isBackground ? DefaultBackGroundBrush : DefaultTextBrush;
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
