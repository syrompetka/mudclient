using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Adan.Client.Common.Themes;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace Adan.Client.Common.Controls.AvalonEdit
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectionColorizer : ColorizingTransformer
    {
        private TextArea _textArea;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="textArea"></param>
        public SelectionColorizer(Brush foreground, Brush background, TextArea textArea)
        {
            ForegroundBrush = foreground;
            BackgroundBrush = background;
            _textArea = textArea;
        }

        /// <summary>
        /// 
        /// </summary>
        public Brush BackgroundBrush
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Brush ForegroundBrush
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        protected override void Colorize(ITextRunConstructionContext context)
        {
            int lineStartOffset = context.VisualLine.FirstDocumentLine.Offset;
            int lineEndOffset = context.VisualLine.LastDocumentLine.Offset + context.VisualLine.LastDocumentLine.TotalLength;

            foreach (SelectionSegment segment in _textArea.Selection.Segments)
            {
                int segmentStart = segment.StartOffset;
                int segmentEnd = segment.EndOffset;
                if (segmentEnd <= lineStartOffset)
                    continue;
                if (segmentStart >= lineEndOffset)
                    continue;
                int startColumn;
                if (segmentStart < lineStartOffset)
                    startColumn = 0;
                else
                    startColumn = context.VisualLine.ValidateVisualColumn(segment.StartOffset, segment.StartVisualColumn, _textArea.Selection.EnableVirtualSpace);

                int endColumn;
                if (segmentEnd > lineEndOffset)
                    endColumn = _textArea.Selection.EnableVirtualSpace ? int.MaxValue : context.VisualLine.VisualLengthWithEndOfLineMarker;
                else
                    endColumn = context.VisualLine.ValidateVisualColumn(segment.EndOffset, segment.EndVisualColumn, _textArea.Selection.EnableVirtualSpace);

                ChangeVisualElements(
                    startColumn, endColumn,
                    element =>
                    {
                        element.TextRunProperties.SetForegroundBrush(ForegroundBrush);
                        element.TextRunProperties.SetBackgroundBrush(BackgroundBrush);
                    });
            }
        }
    }
}