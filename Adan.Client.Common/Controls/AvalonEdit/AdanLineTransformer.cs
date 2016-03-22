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
        private IAdanVisualLineElement escapeElement = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="elements"></param>
        public void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements)
        {
            Stack<IAdanVisualLineElement> _savedElement = new Stack<IAdanVisualLineElement>();

            foreach (var element in elements)
            {
                if (element is AdanColorVisualLineElement)
                {
                    escapeElement = (IAdanVisualLineElement) element;
                }
                else if (element is AdanResetVisualLineElement)
                {
                    escapeElement = null;
                }
                else if (element is AdanColorAndHighlightVisualLineElement)
                {
                    _savedElement.Push((IAdanVisualLineElement)((AdanColorAndHighlightVisualLineElement)element).ColorElement);
                    escapeElement = (IAdanVisualLineElement)element;
                }
                else if (element is AdanStartHighlightVisualElement)
                {
                    _savedElement.Push(escapeElement);
                    escapeElement = (IAdanVisualLineElement)element;
                }
                else if (element is AdanStopHighlightVisualElement)
                {
                    if (_savedElement.Count > 0)
                        escapeElement = _savedElement.Pop();
                    else
                        escapeElement = null;
                }
                else
                {
                    if (escapeElement != null)
                    {
                        element.TextRunProperties.SetForegroundBrush(escapeElement.Foreground);
                        element.TextRunProperties.SetBackgroundBrush(escapeElement.Background);
                    }
                }
            }
        }
    }
}
