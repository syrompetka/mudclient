using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

using Adan.Client.Common.Themes;

namespace Adan.Client.Common.Utils
{
    /// <summary>
    /// Parse TextColor from string
    /// </summary>
    public static class TextColorParser
    {
        /// <summary>
        /// Parse TextColor from string
        /// </summary>
        public static TextColor Parse(string color)
        {
            if (String.IsNullOrEmpty(color))
                return TextColor.None;

            if (String.Compare(color, "red", true) == 0 || String.Compare(color, "b red", true) == 0)
                return TextColor.Red;

            if (String.Compare(color, "green", true) == 0 || String.Compare(color, "b green", true) == 0)
                return TextColor.Green;

            if (String.Compare(color, "brown", true) == 0 || String.Compare(color, "b brown", true) == 0)
                return TextColor.Yellow;

            if (String.Compare(color, "blue", true) == 0 || String.Compare(color, "b blue", true) == 0)
                return TextColor.Blue;

            if (String.Compare(color, "magenta", true) == 0 || String.Compare(color, "b magenta", true) == 0)
                return TextColor.Magenta;

            if (String.Compare(color, "cyan", true) == 0 || String.Compare(color, "b cyan", true) == 0)
                return TextColor.Cyan;

            if (String.Compare(color, "grey", true) == 0 || String.Compare(color, "b grey", true) == 0)
                return TextColor.White;

            if (String.Compare(color, "charcoal", true) == 0 || String.Compare(color, "b charcoal", true) == 0)
                return TextColor.BrightBlack;

            if (String.Compare(color, "light red", true) == 0 || String.Compare(color, "b light red", true) == 0)
                return TextColor.BrightRed;

            if (String.Compare(color, "light green", true) == 0 || String.Compare(color, "b light green", true) == 0)
                return TextColor.BrightGreen;

            if (String.Compare(color, "yellow", true) == 0 || String.Compare(color, "b yellow", true) == 0)
                return TextColor.BrightYellow;

            if (String.Compare(color, "light blue", true) == 0 || String.Compare(color, "b light blue", true) == 0)
                return TextColor.BrightBlue;

            if (String.Compare(color, "light magenta", true) == 0 || String.Compare(color, "b light magenta", true) == 0)
                return TextColor.BrightMagenta;

            if (String.Compare(color, "light cyan", true) == 0 || String.Compare(color, "b light cyan", true) == 0)
                return TextColor.BrightCyan;

            if (String.Compare(color, "white", true) == 0 || String.Compare(color, "b white", true) == 0)
                return TextColor.White;

            if (String.Compare(color, "black", true) == 0 || String.Compare(color, "b black", true) == 0)
                return TextColor.Black;

            return TextColor.None;
        }
    }
}
