using Adan.Client.Common.Messages;
using Adan.Client.Common.Themes;
using Adan.Client.Common.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private SafeLogFontHandle _logFont;

        /// <summary>
        /// 
        /// </summary>
        public TextView()
        {
            this.Messages = new List<TextMessage>();
            this.BackColor = Color.Black;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque | ControlStyles.UserPaint, true);
            this.FontChanged += TextView_FontChanged;
            this.TextView_FontChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        public List<TextMessage> Messages
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSelected
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int StartSelectedLine
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int StartSelectedIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int EndSelectedLine
        {
            get;
            set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public int EndSelectedIndex
        {
            get;
            set;
        }

        private void TextView_FontChanged(object sender, EventArgs e)
        {
            this._logFont = new SafeLogFontHandle(this.Font);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            List<TextMessage> messages = Messages;

            if (messages.Count == 0)
                return;

            IntPtr hdc = e.Graphics.GetHdc();
            IntPtr pOldFont = IntPtr.Zero;

            try
            {
                if (_logFont.IsInvalid)
                    throw new Exception("LogFont is invalid");

                pOldFont = SelectObject(hdc, _logFont.DangerousGetHandle());
                SetBkMode(hdc, BK_OPAQUE);

                RECT rect = new RECT();

                if (IsSelected)
                {
                    for (int i = messages.Count - 1; i >= 0; --i)
                    {
                        int xShift = 0;

                        foreach (var block in messages[i].MessageBlocks)
                        {
                            System.Windows.Media.Color color;

                            DrawText(hdc, block.Text, -1, ref rect, DTOptions.DT_LEFT | DTOptions.DT_SINGLELINE | DTOptions.DT_NOCLIP | DTOptions.DT_CALCRECT);

                            if (i > StartSelectedLine && i < EndSelectedLine)
                            {
                                color = ThemeManager.Instance.ActiveTheme.GetSelectionColorByTextColor(false);
                                SetTextColor(hdc, (int)color.R | (int)color.G << 8 | (int)color.B << 16);

                                color = ThemeManager.Instance.ActiveTheme.GetSelectionColorByTextColor(true);
                                SetBkColor(hdc, (int)color.R | (int)color.G << 8 | (int)color.B << 16);
                            }
                            else if (i == StartSelectedLine)
                            {
                                if (StartSelectedIndex < xShift)
                                {

                                }
                            }

                            color = ThemeManager.Instance.ActiveTheme.GetColorByTextColor(block.Foreground, false);
                            SetTextColor(hdc, (int)color.R | (int)color.G << 8 | (int)color.B << 16);

                            color = ThemeManager.Instance.ActiveTheme.GetColorByTextColor(block.Background, true);
                            SetBkColor(hdc, (int)color.R | (int)color.G << 8 | (int)color.B << 16);

                            var yShift = this.Height - this.FontHeight * ((messages.Count) - i);
                            ExtTextOut(hdc, xShift, yShift, ETOOptions.ETO_OPAQUE & ETOOptions.ETO_CLIPPED, ref rect, block.Text, block.Text.Length, null);
                            xShift += rect.Right;
                        }
                    }
                }
                else
                {
                    for (int i = messages.Count - 1; i >= 0; --i)
                    {
                        int xShift = 0;

                        foreach (var block in messages[i].MessageBlocks)
                        {
                            DrawText(hdc, block.Text, -1, ref rect, DTOptions.DT_LEFT | DTOptions.DT_SINGLELINE | DTOptions.DT_NOCLIP | DTOptions.DT_CALCRECT);

                            var color = ThemeManager.Instance.ActiveTheme.GetColorByTextColor(block.Foreground, false);
                            SetTextColor(hdc, (int)color.R | (int)color.G << 8 | (int)color.B << 16);

                            color = ThemeManager.Instance.ActiveTheme.GetColorByTextColor(block.Background, true);
                            SetBkColor(hdc, (int)color.R | (int)color.G << 8 | (int)color.B << 16);

                            var yShift = this.Height - this.FontHeight * ((messages.Count) - i);
                            ExtTextOut(hdc, xShift, yShift, ETOOptions.ETO_OPAQUE & ETOOptions.ETO_CLIPPED, ref rect, block.Text, block.Text.Length, null);
                            xShift += rect.Right;
                        }
                    }
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
        
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            _logFont.Close();
            base.Dispose(disposing);
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ExtTextOut(
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
        private static extern bool ExtTextOut(
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
        private enum ETOOptions : uint
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
        private static extern int DrawText(IntPtr hDC, string lpString, int nCount, ref RECT lpRect, DTOptions uFormat);

        [Flags]
        private enum DTOptions : uint
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

        private struct RECT
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
        private static extern int SetBkMode(IntPtr hdc, int iBkMode);

        private const int BK_TRANSPARENT = 1;
        private const int BK_OPAQUE = 2;

        [DllImport("gdi32.dll")]
        private static extern int SetTextColor(IntPtr hdc, int crColor);
            
        [DllImport("gdi32.dll")]
        private static extern uint SetBkColor(IntPtr hdc, int crColor);
    }
}
