using Adan.Client.Common.Themes;
using Adan.Client.Common.Utils;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Adan.Client.Common.Controls.ContorlBoxNative
{
    /// <summary>
    /// 
    /// </summary>
    public class TextView : UserControl
    {      
        private readonly TextBoxNative _parent;
        private SafeLogFont _logFont;

        /// <summary>
        /// 
        /// </summary>
        public TextView(TextBoxNative parent)
        {
            _parent = parent;
            BackColor = Color.Black;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque | ControlStyles.UserPaint, true);
            
            FontChanged += TextView_FontChanged;
        }

        private void TextView_FontChanged(object sender, EventArgs e)
        {
            _logFont = new SafeLogFont(Font);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_parent.Messages.Count == 0 || _parent.ViewportHeight == 0)
                return;

            IntPtr hdc = e.Graphics.GetHdc();
            IntPtr pOldFont = IntPtr.Zero;

            try
            {
                pOldFont = SelectObject(hdc, _logFont.Source);
                SetBkMode(hdc, BK_OPAQUE);

                int nCount = (int)_parent.VerticalOffset;
                int nLine = Math.Min(_parent.Messages.Count, (int)_parent.ViewportHeight);
                int nH = (int)_parent.ViewportHeight * this.FontHeight - this.FontHeight;

                if (nCount < 0 || nLine < 0 || nCount + nLine > _parent.Messages.Count)
                    return;

                var rect = new RECT();
                for (int i = nCount + nLine - 1; i >= nCount; i--)
                {
                    int xShift = 0;
                    var text = _parent.Messages[i];

                    foreach (var block in text.MessageBlocks)
                    {
                        DrawText(hdc, block.Text, -1, ref rect, DTOptions.DT_LEFT | DTOptions.DT_SINGLELINE | DTOptions.DT_NOCLIP | DTOptions.DT_CALCRECT);

                        var color = ThemeManager.Instance.ActiveTheme.GetColorByTextColor(block.Foreground, false);
                        SetTextColor(hdc, (int)color.R | (int)color.G << 8 | (int)color.B << 16);

                        color = ThemeManager.Instance.ActiveTheme.GetColorByTextColor(block.Background, true);
                        SetBkColor(hdc, (int)color.R | (int)color.G << 8 | (int)color.B << 16);

                        ExtTextOut(hdc, xShift, nH, ETOOptions.ETO_OPAQUE, ref rect, block.Text, block.Text.Length, null);
                        xShift += rect.Right;
                    }

                    nH -= this.FontHeight;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Instance.Write(string.Format("Error paint main output window: {0}\r\n{1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                try
                {
                    if (pOldFont != IntPtr.Zero)
                        SelectObject(hdc, pOldFont);
                }
                catch { }

                e.Graphics.ReleaseHdc();
            }
        }

        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ExtTextOut(
            IntPtr hdc,
            int X,
            int Y,
            ETOOptions fuOptions,
            IntPtr lprc,
            [MarshalAs(UnmanagedType.LPWStr)]
            string lpString,
            int cbCount,
            int[] lpDx
        );
             
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ExtTextOut(
            IntPtr hdc,
            int X,
            int Y,
            ETOOptions fuOptions,
            ref RECT lprc,
            [MarshalAs(UnmanagedType.LPWStr)]
            string lpString,
            int cbCount,
            int[] lpDx
        );

        [Flags]
        enum ETOOptions : uint
        {
            ETO_CLIPPED = 0x4,
            ETO_GLYPH_INDEX = 0x10,
            ETO_IGNORELANGUAGE = 0x1000,
            ETO_NUMERICSLATIN = 0x800,
            ETO_NUMERICSLOCAL = 0x400,
            ETO_OPAQUE = 0x2,
            ETO_PDY = 0x2000,
            ETO_RTLREADING = 0x800,
        }

        [DllImport("user32.dll")]
        static extern int DrawText(IntPtr hDC, string lpString, int nCount, ref RECT lpRect, DTOptions uFormat);

        [Flags]
        enum DTOptions : uint
        {
            DT_TOP = 0x00000000,
            DT_LEFT = 0x00000000,
            DT_CENTER = 0x00000001,
            DT_RIGHT = 0x00000002,
            DT_VCENTER = 0x00000004,
            DT_BOTTOM = 0x00000008,
            DT_WORDBREAK = 0x00000010,
            DT_SINGLELINE = 0x00000020,
            DT_EXPANDTABS = 0x00000040,
            DT_TABSTOP = 0x00000080,
            DT_NOCLIP = 0x00000100,
            DT_EXTERNALLEADING = 0x00000200,
            DT_CALCRECT = 0x00000400,
            DT_NOPREFIX = 0x00000800,
            DT_INTERNAL = 0x00001000,
        }

        struct RECT
        {
            /// <summary>
            /// 
            /// </summary>
            public int Left, Top, Right, Bottom;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="left"></param>
            /// <param name="top"></param>
            /// <param name="right"></param>
            /// <param name="bottom"></param>
            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="r"></param>
            public RECT(Rectangle r)
            {
                this.Left = r.Left;
                this.Top = r.Top;
                this.Bottom = r.Bottom;
                this.Right = r.Right;
            }
        }

        [DllImport("gdi32.dll")]
        static extern int SetBkMode(IntPtr hdc, int iBkMode);

        const int BK_TRANSPARENT = 1;
        const int BK_OPAQUE = 2;

        [DllImport("gdi32.dll")]
        static extern int SetTextColor(IntPtr hdc, int crColor);
            
        [DllImport("gdi32.dll")]
        static extern uint SetBkColor(IntPtr hdc, int crColor);
    }
}
