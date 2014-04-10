using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Rendering;

namespace Adan.Client.Common.Controls.AvalonEdit.VisualLineElements
{
    /// <summary>
    /// 
    /// </summary>
    public class AdanResetVisualLineElement : VisualLineElement
    {

        private static TextHidden _cachedTextHidden = new TextHidden(1);

        /// <summary>
        /// 
        /// </summary>
        public AdanResetVisualLineElement(int length)
            : base(1, length)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startVisualColumn"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            return _cachedTextHidden;
        }
    }
}
