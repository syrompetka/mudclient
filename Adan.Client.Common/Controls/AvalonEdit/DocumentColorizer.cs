using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Adan.Client.Common.Themes;
using Adan.Client.Common.Utils;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace Adan.Client.Common.Controls.AvalonEdit
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentColorizer : DocumentColorizingTransformer  
    {

        /// <summary>
        /// 
        /// </summary>
        public ColorsQueue<TextColorHolder> ColorsQueue
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
        {
            if (ColorsQueue == null)
                throw new ArgumentNullException();

            int startOffset = line.Offset;
            int endOffset = startOffset + line.Length;
            int currentOffset = startOffset;
            var colors = ColorsQueue;

            while (currentOffset < endOffset)
            {
                var index = colors.GetIndexForElement(currentOffset);

                if (index < 0)
                    throw new Exception("Not found color for line");

                var endIndex = Math.Min(colors.GetRange(index).High + 1, endOffset);

                var curColor = colors.GetValue(index);

                this.ChangeLinePart(currentOffset, endIndex, visual =>
                    {
                        visual.TextRunProperties.SetBackgroundBrush(ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(curColor.Background, true));
                        visual.TextRunProperties.SetForegroundBrush(ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(curColor.Foreground, false));
                    });

                currentOffset = endIndex;
            }
        }
    }
}