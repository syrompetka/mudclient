// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdditionalOutputWindow.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for AdditionalOutputWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using Adan.Client.Plugins.OutputWindow.Messages;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using Adan.Client.Common.Controls.AvalonEdit;
using System.Windows.Input;
using System.Windows;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;
using Adan.Client.Common.Utils;
using System.Windows.Controls;
using Adan.Client.Common.Themes;
using System.Text;
using ICSharpCode.AvalonEdit.Rendering;

namespace Adan.Client.Plugins.OutputWindow
{

    /// <summary>
    /// Interaction logic for AdditionalOutputWindow.xaml
    /// </summary>
    public partial class AdditionalOutputWindow : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionalOutputWindow"/> class.
        /// </summary>
        public AdditionalOutputWindow()
        {
            InitializeComponent();

            var options = new TextEditorOptions();

            options.AllowScrollBelowDocument = false;
            options.CutCopyWholeLine = false;
            options.EnableEmailHyperlinks = true;
            options.EnableHyperlinks = true;
            options.EnableImeSupport = false;
            options.EnableRectangularSelection = false;
            options.EnableTextDragDrop = false;
            options.EnableVirtualSpace = false;
            options.RequireControlModifierForHyperlinkClick = false;
            options.ShowBoxForControlCharacters = false;

            additionalOutputWindow.Options = options;
            var elementGenerator = new AdanElementGenerator();
            var adanLineTransformer = new AdanLineTransformer();

            var selectionColorizer = new SelectionColorizer(
                ThemeManager.Instance.ActiveTheme.GetSelectionBrushByTextColor(false),
                ThemeManager.Instance.ActiveTheme.GetSelectionBrushByTextColor(true),
                additionalOutputWindow.TextArea);

            additionalOutputWindow.Foreground = ThemeManager.Instance.ActiveTheme.DefaultTextColor;
            additionalOutputWindow.Background = ThemeManager.Instance.ActiveTheme.DefaultBackGroundColor;
            additionalOutputWindow.TextArea.TextView.ElementGenerators.Add(elementGenerator);
            additionalOutputWindow.TextArea.TextView.LineTransformers.Clear();
            additionalOutputWindow.TextArea.TextView.LineTransformers.Add(adanLineTransformer);
            additionalOutputWindow.TextArea.TextView.LineTransformers.Add(selectionColorizer);
            additionalOutputWindow.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            additionalOutputWindow.TextArea.Caret.Hide();
            additionalOutputWindow.TextArea.LostMouseCapture += TextArea_LostMouseCapture;
            additionalOutputWindow.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;
            additionalOutputWindow.Document.UndoStack.SizeLimit = 0;
        }

        void TextView_ScrollOffsetChanged(object sender, EventArgs e)
        {
            var textView = sender as TextView;
            if (textView.DocumentHeight - (additionalOutputWindow.ViewportHeight + textView.VerticalOffset) < 0.01)
                return;
        }

        private void TextArea_LostMouseCapture(object sender, MouseEventArgs e)
        {
            var textArea = sender as TextArea;
            if (!textArea.Selection.IsEmpty)
            {
                string selectedText = textArea.Selection.GetText();
                int offset = 0;
                int startIndex = 0;
                StringBuilder sb = new StringBuilder(selectedText.Length);

                while (offset < selectedText.Length)
                {
                    if (selectedText[offset] == '\x1B')
                    {
                        if (offset > 0)
                            sb.Append(selectedText.Substring(startIndex, offset - startIndex));

                        offset += 2;

                        while (offset < selectedText.Length && selectedText[offset] != 'm')
                            ++offset;

                        if (offset == selectedText.Length)
                            break;

                        startIndex = offset + 1;
                    }

                    ++offset;
                }

                if (startIndex != selectedText.Length)
                    sb.Append(selectedText.Substring(startIndex, offset - startIndex));

                Clipboard.SetText(sb.ToString());
                textArea.ClearSelection();
            }
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            ((Caret)sender).Hide();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        public void ChangeOutputWindow(TextDocument document)
        {
            additionalOutputWindow.Document = document;
            additionalOutputWindow.ScrollToEnd();
        }
    }
}
