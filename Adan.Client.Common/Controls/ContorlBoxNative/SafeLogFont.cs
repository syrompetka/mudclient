using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Adan.Client.Common.Controls.ContorlBoxNative
{
    /// <summary>
    /// 
    /// </summary>
    /// TODO: Переделать на SafeHandle
    public class SafeLogFont : IDisposable
    {
        private IntPtr _logFont = IntPtr.Zero;

        /// <summary>
        /// 
        /// </summary>
        public SafeLogFont(string faceName) 
        {
            var logfont = new LOGFONT()
            {
                lfFaceName = faceName,
            };           

            _logFont = CreateFontIndirect(ref logfont);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeface"></param>
        /// <param name="size"></param>
        public SafeLogFont(Typeface typeface, int size)
        {
            var logfont = new LOGFONT()
            {
                lfFaceName = typeface.FontFamily.Source,
                lfHeight = - (int) (typeface.FontFamily.LineSpacing * 3/4 * size),
                lfPitchAndFamily = FontPitchAndFamily.FIXED_PITCH,
                lfCharSet = FontCharSet.ANSI_CHARSET,
            };

            if (typeface.Weight == FontWeights.Thin)
                logfont.lfWeight = FontWeight.FW_THIN;
            else if (typeface.Weight == FontWeights.ExtraLight || typeface.Weight == FontWeights.UltraLight)
                logfont.lfWeight = FontWeight.FW_EXTRALIGHT;
            else if (typeface.Weight == FontWeights.Light)
                logfont.lfWeight = FontWeight.FW_LIGHT;
            else if (typeface.Weight == FontWeights.Normal || typeface.Weight == FontWeights.Regular)
                logfont.lfWeight = FontWeight.FW_NORMAL;
            else if (typeface.Weight == FontWeights.Medium)
                logfont.lfWeight = FontWeight.FW_MEDIUM;
            else if (typeface.Weight == FontWeights.SemiBold || typeface.Weight == FontWeights.DemiBold)
                logfont.lfWeight = FontWeight.FW_SEMIBOLD;
            else if (typeface.Weight == FontWeights.Bold)
                logfont.lfWeight = FontWeight.FW_BOLD;
            else if (typeface.Weight == FontWeights.ExtraBold || typeface.Weight == FontWeights.UltraBold)
                logfont.lfWeight = FontWeight.FW_EXTRABOLD;
            else if (typeface.Weight == FontWeights.Black || typeface.Weight == FontWeights.ExtraBlack
                || typeface.Weight == FontWeights.UltraBlack || typeface.Weight == FontWeights.Heavy)
                logfont.lfWeight = FontWeight.FW_HEAVY;

            if (typeface.Style == FontStyles.Italic)
                logfont.lfItalic = true;

            //TODO: Реализовать подчеркивание и зачеркивание

            _logFont = CreateFontIndirect(ref logfont);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="font"></param>
        public SafeLogFont(System.Drawing.Font font)
        {
            var logfont = new LOGFONT()
            {
                lfFaceName = font.Name,
                lfHeight = -font.Height,
                lfPitchAndFamily = FontPitchAndFamily.FIXED_PITCH,
                lfCharSet = (FontCharSet) font.GdiCharSet,
                lfItalic = font.Italic,
                lfWeight = font.Bold ? FontWeight.FW_BOLD : FontWeight.FW_NORMAL,
                lfUnderline = font.Underline,
                lfStrikeOut = font.Strikeout,
            };

            _logFont = CreateFontIndirect(ref logfont);
        }

        /// <summary>
        /// 
        /// </summary>
        public IntPtr Source
        {
            get
            {
                if (_logFont == IntPtr.Zero)
                {
                    var logfont = new LOGFONT();
                    _logFont = CreateFontIndirect(ref logfont);
                }

                return _logFont;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.ReleaseLogFont();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        ~SafeLogFont()
        {
            this.ReleaseLogFont();
        }

        private void ReleaseLogFont()
        {
            if (_logFont != IntPtr.Zero)
            {
                try
                {
                    IntPtr logfont = _logFont;
                    _logFont = IntPtr.Zero;
                    DeleteObject(logfont);
                }
                catch (Exception)
                { }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public FontWeight lfWeight;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfItalic;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfUnderline;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfStrikeOut;
            public FontCharSet lfCharSet;
            public FontPrecision lfOutPrecision;
            public FontClipPrecision lfClipPrecision;
            public FontQuality lfQuality;
            public FontPitchAndFamily lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName;
        }

        enum FontWeight : int
        {
            FW_DONTCARE = 0,
            FW_THIN = 100,
            FW_EXTRALIGHT = 200,
            FW_LIGHT = 300,
            FW_NORMAL = 400,
            FW_MEDIUM = 500,
            FW_SEMIBOLD = 600,
            FW_BOLD = 700,
            FW_EXTRABOLD = 800,
            FW_HEAVY = 900,
        }

        enum FontCharSet : byte
        {
            ANSI_CHARSET = 0,
            DEFAULT_CHARSET = 1,
            SYMBOL_CHARSET = 2,
            SHIFTJIS_CHARSET = 128,
            HANGEUL_CHARSET = 129,
            HANGUL_CHARSET = 129,
            GB2312_CHARSET = 134,
            CHINESEBIG5_CHARSET = 136,
            OEM_CHARSET = 255,
            JOHAB_CHARSET = 130,
            HEBREW_CHARSET = 177,
            ARABIC_CHARSET = 178,
            GREEK_CHARSET = 161,
            TURKISH_CHARSET = 162,
            VIETNAMESE_CHARSET = 163,
            THAI_CHARSET = 222,
            EASTEUROPE_CHARSET = 238,
            RUSSIAN_CHARSET = 204,
            MAC_CHARSET = 77,
            BALTIC_CHARSET = 186,
        }

        enum FontPrecision : byte
        {
            OUT_DEFAULT_PRECIS = 0,
            OUT_STRING_PRECIS = 1,
            OUT_CHARACTER_PRECIS = 2,
            OUT_STROKE_PRECIS = 3,
            OUT_TT_PRECIS = 4,
            OUT_DEVICE_PRECIS = 5,
            OUT_RASTER_PRECIS = 6,
            OUT_TT_ONLY_PRECIS = 7,
            OUT_OUTLINE_PRECIS = 8,
            OUT_SCREEN_OUTLINE_PRECIS = 9,
            OUT_PS_ONLY_PRECIS = 10,
        }

        enum FontClipPrecision : byte
        {
            CLIP_DEFAULT_PRECIS = 0,
            CLIP_CHARACTER_PRECIS = 1,
            CLIP_STROKE_PRECIS = 2,
            CLIP_MASK = 0xf,
            CLIP_LH_ANGLES = (1 << 4),
            CLIP_TT_ALWAYS = (2 << 4),
            CLIP_DFA_DISABLE = (4 << 4),
            CLIP_EMBEDDED = (8 << 4),
        }

        enum FontQuality : byte
        {
            DEFAULT_QUALITY = 0,
            DRAFT_QUALITY = 1,
            PROOF_QUALITY = 2,
            NONANTIALIASED_QUALITY = 3,
            ANTIALIASED_QUALITY = 4,
            CLEARTYPE_QUALITY = 5,
            CLEARTYPE_NATURAL_QUALITY = 6,
        }

        [Flags]
        enum FontPitchAndFamily : byte
        {
            DEFAULT_PITCH = 0,
            FIXED_PITCH = 1,
            VARIABLE_PITCH = 2,
            FF_DONTCARE = (0 << 4),
            FF_ROMAN = (1 << 4),
            FF_SWISS = (2 << 4),
            FF_MODERN = (3 << 4),
            FF_SCRIPT = (4 << 4),
            FF_DECORATIVE = (5 << 4),
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr CreateFontIndirect(ref LOGFONT lplf);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DeleteObject(IntPtr hObject);
    }
}
