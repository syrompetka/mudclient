using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Adan.Client.Common.Controls.AvalonEdit.VisualLineElements;
using ICSharpCode.AvalonEdit.Rendering;

namespace Adan.Client.Common.Controls.AvalonEdit
{
    /// <summary>
    /// 
    /// </summary>
    public class AdanLineTransformer : IVisualLineTransformer
    {
#if DEBUG
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler RenderTimeChanged;

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan RenderTime
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get;
            set;
        }
#endif

        private IAdanVisualLineElement _savedEscapeElement;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="elements"></param>
        public void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements)
        {

#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            IAdanVisualLineElement escapeElement = null;
            foreach (var element in elements)
            {
                if (element is AdanColorVisualLineElement)
                    escapeElement = (IAdanVisualLineElement)element;
                else if (element is AdanResetVisualLineElement)
                    escapeElement = null;
                else if (element is AdanStartHighlightVisualElement)
                {
                    _savedEscapeElement = escapeElement;
                    escapeElement = (IAdanVisualLineElement)element;
                }
                else if (element is AdanStopHighlightVisualElement)
                {
                    escapeElement = _savedEscapeElement;
                }
                else if (escapeElement != null)
                {
                    element.TextRunProperties.SetForegroundBrush(escapeElement.Foreground);
                    element.TextRunProperties.SetBackgroundBrush(escapeElement.Background);
                }
            }

#if DEBUG
            sw.Stop();
            if (RenderTimeChanged != null)
            {
                RenderTime += sw.Elapsed;
                RenderTimeChanged(this, EventArgs.Empty);
            }
#endif
        }
    }
}
