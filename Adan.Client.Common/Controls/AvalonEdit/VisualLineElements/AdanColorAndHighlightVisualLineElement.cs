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
    public class AdanColorAndHighlightVisualLineElement : AdanStartHighlightVisualElement
    {
        /// <summary>
        /// 
        /// </summary>
        public IAdanVisualLineElement ColorElement
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
        /// <param name="colorElement"></param>
        public AdanColorAndHighlightVisualLineElement(TextColor foreground, TextColor background, int length, VisualLineElement colorElement)
            : base(foreground, background, length)
        {
            ColorElement = ColorElement;
        }
    }
}
