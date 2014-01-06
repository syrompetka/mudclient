using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using Adan.Client.Common.Themes;
using ICSharpCode.AvalonEdit.Rendering;

namespace Adan.Client.Common.Controls.AvalonEdit.VisualLineElements
{
    /// <summary>
    /// 
    /// </summary>
    public class AdanStartHighlightVisualElement : VisualLineElement, IAdanVisualLineElement
    {

        private static TextHidden _cashedTextHidden = new TextHidden(1);

        /// <summary>
        /// 
        /// </summary>
        public Brush Foreground
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Brush Background
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="length"></param>
        public AdanStartHighlightVisualElement(TextColor foreground, TextColor background, int length)
            : base(1, length)
        {
            Foreground = ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(foreground, false);
            Background = ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(background, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startVisualColumn"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            return _cashedTextHidden;
        }
    }
}
