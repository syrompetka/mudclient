using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;

namespace Adan.Client.Fonts
{
    /// <summary>
    /// 
    /// </summary>
    public class FontFamilyListItem : TextBlock, IComparable
    {
        private string _displayName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fontFamily"></param>
        public FontFamilyListItem(FontFamily fontFamily)
        {
            _displayName = GetDisplayName(fontFamily);

            this.FontFamily = fontFamily;
            this.Text = _displayName;
            this.ToolTip = _displayName;

            // In the case of symbol font, apply the default message font to the text so it can be read.
            if (IsSymbolFont(fontFamily))
            {
                TextRange range = new TextRange(this.ContentStart, this.ContentEnd);
                range.ApplyPropertyValue(TextBlock.FontFamilyProperty, SystemFonts.MessageFontFamily);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _displayName;
        }

        int IComparable.CompareTo(object obj)
        {
            return string.Compare(_displayName, obj.ToString(), true, CultureInfo.CurrentCulture);
        }

        internal static bool IsSymbolFont(FontFamily fontFamily)
        {
            foreach (Typeface typeface in fontFamily.GetTypefaces())
            {
                GlyphTypeface face;
                if (typeface.TryGetGlyphTypeface(out face))
                {
                    return face.Symbol;
                }
            }
            return false;
        }

        internal static string GetDisplayName(FontFamily family)
        {
            return NameDictionaryHelper.GetDisplayName(family.FamilyNames);
        }
    }
}
