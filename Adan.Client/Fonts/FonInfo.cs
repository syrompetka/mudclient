using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Adan.Client.Fonts
{
    /// <summary>
    /// 
    /// </summary>
    public class FontInfo
    {
        /// <summary>
        /// FonFamile
        /// </summary>
        public FontFamily Family { get; set; }
        /// <summary>
        /// Font Size
        /// </summary>
        public double Size { get; set; }
        /// <summary>
        /// Font Style
        /// </summary>
        public FontStyle Style { get; set; }
        /// <summary>
        /// Font Stretch
        /// </summary>
        public FontStretch Stretch { get; set; }
        /// <summary>
        /// Font Weight
        /// </summary>
        public FontWeight Weight { get; set; }
        /// <summary>
        /// Font color brush
        /// </summary>
        public SolidColorBrush BrushColor { get; set; }

        #region Static Utils

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ttf"></param>
        /// <returns></returns>
        public static string TypefaceToString(FamilyTypeface ttf)
        {
            StringBuilder sb = new StringBuilder(ttf.Stretch.ToString());
            sb.Append("-");
            sb.Append(ttf.Weight.ToString());
            sb.Append("-");
            sb.Append(ttf.Style.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="font"></param>
        public static void ApplyFont(Control control, FontInfo font)
        {
            control.FontFamily = font.Family;
            control.FontSize = font.Size;
            control.FontStyle = font.Style;
            control.FontStretch = font.Stretch;
            control.FontWeight = font.Weight;
            control.Foreground = font.BrushColor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static FontInfo GetControlFont(Control control)
        {
            FontInfo font = new FontInfo();
            font.Family = control.FontFamily;
            font.Size = control.FontSize;
            font.Style = control.FontStyle;
            font.Stretch = control.FontStretch;
            font.Weight = control.FontWeight;
            font.BrushColor = (SolidColorBrush)control.Foreground;
            return font;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public FontInfo()
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fam"></param>
        /// <param name="sz"></param>
        /// <param name="style"></param>
        /// <param name="strc"></param>
        /// <param name="weight"></param>
        /// <param name="c"></param>
        public FontInfo(FontFamily fam, double sz, FontStyle style,
                        FontStretch strc, FontWeight weight, SolidColorBrush c)
        {
            this.Family = fam;
            this.Size = sz;
            this.Style = style;
            this.Stretch = strc;
            this.Weight = weight;
            this.BrushColor = c;
        }

        /// <summary>
        /// 
        /// </summary>
        public FontColor Color
        {
            get
            {
                return AvailableColors.GetFontColor(this.BrushColor);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public FamilyTypeface Typeface
        {
            get
            {
                FamilyTypeface ftf = new FamilyTypeface();
                ftf.Stretch = this.Stretch;
                ftf.Weight = this.Weight;
                ftf.Style = this.Style;
                return ftf;
            }
        }

    }
}
