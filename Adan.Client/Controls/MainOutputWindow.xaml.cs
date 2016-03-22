using Adan.Client.Common.Commands;
using Adan.Client.Common.Controls.AvalonEdit;
using Adan.Client.Common.Messages;
using Adan.Client.Common.Model;
using Adan.Client.Common.Settings;
using Adan.Client.Common.Themes;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Adan.Client.Controls
{
    /// <summary>
    /// Логика взаимодействия для MainOutputWindow.xaml
    /// </summary>
    public partial class MainOutputWindow : MainOutputWindowBase
    {
        private static TextEditorOptions _options;

        private MainWindow _mainWindow;
        private RootModel _rootModel;

        private bool _isMainOutputEndToScroll = true;
        private bool _needMainOutputScrollToEnd = false;

        static MainOutputWindow()
        {
            _options = new TextEditorOptions();

            _options.AllowScrollBelowDocument = false;
            _options.CutCopyWholeLine = false;
            _options.EnableEmailHyperlinks = true;
            _options.EnableHyperlinks = true;
            _options.EnableImeSupport = false;
            _options.EnableRectangularSelection = false;
            _options.EnableTextDragDrop = false;
            _options.EnableVirtualSpace = false;
            _options.RequireControlModifierForHyperlinkClick = false;
            _options.ShowBoxForControlCharacters = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainOutputWindow"/> class.
        /// </summary>
        public MainOutputWindow(MainWindow mainWindow, RootModel rootModel)
        {
            InitializeComponent();

            mainScrollOutput.Options = _options;
            secondScrollOutput.Options = _options;

            _mainWindow = mainWindow;
            _rootModel = rootModel;

            var mainSelectionColorizer = new SelectionColorizer(
                ThemeManager.Instance.ActiveTheme.GetSelectionBrushByTextColor(false),
                ThemeManager.Instance.ActiveTheme.GetSelectionBrushByTextColor(true),
                mainScrollOutput.TextArea);

            var secondSelectionColorizer = new SelectionColorizer(
                ThemeManager.Instance.ActiveTheme.GetSelectionBrushByTextColor(false),
                ThemeManager.Instance.ActiveTheme.GetSelectionBrushByTextColor(true), secondScrollOutput.TextArea);

            var elementGenerator = new AdanElementGenerator();
            var adanLineTransformer = new AdanLineTransformer();

            mainScrollOutput.Foreground = new SolidColorBrush(ThemeManager.Instance.ActiveTheme.DefaultTextColor);
            mainScrollOutput.Background = new SolidColorBrush(ThemeManager.Instance.ActiveTheme.DefaultBackGroundColor);
            mainScrollOutput.TextArea.TextView.ElementGenerators.Add(elementGenerator);
            mainScrollOutput.TextArea.TextView.LineTransformers.Clear();
            mainScrollOutput.TextArea.TextView.LineTransformers.Add(adanLineTransformer);
            mainScrollOutput.TextArea.TextView.LineTransformers.Add(mainSelectionColorizer);
            mainScrollOutput.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            mainScrollOutput.TextArea.Caret.Hide();
            mainScrollOutput.TextArea.GotFocus += TextArea_GotFocus;
            mainScrollOutput.TextArea.TextView.ScrollOffsetChanged += MainOutput_TextView_ScrollOffsetChanged;
            mainScrollOutput.TextArea.LostMouseCapture += TextArea_LostMouseCapture;
            mainScrollOutput.Document.UndoStack.SizeLimit = 0;

            secondScrollOutput.Foreground = new SolidColorBrush(ThemeManager.Instance.ActiveTheme.DefaultTextColor);
            secondScrollOutput.Background = new SolidColorBrush(ThemeManager.Instance.ActiveTheme.DefaultBackGroundColor);
            secondScrollOutput.TextArea.Document = mainScrollOutput.TextArea.Document;
            secondScrollOutput.TextArea.TextView.ElementGenerators.Add(elementGenerator);
            secondScrollOutput.TextArea.TextView.LineTransformers.Clear();
            secondScrollOutput.TextArea.TextView.LineTransformers.Add(adanLineTransformer);
            secondScrollOutput.TextArea.TextView.LineTransformers.Add(secondSelectionColorizer);
            secondScrollOutput.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            secondScrollOutput.TextArea.Caret.Hide();
            secondScrollOutput.TextArea.GotFocus += TextArea_GotFocus;
            secondScrollOutput.TextArea.LostMouseCapture += TextArea_LostMouseCapture;
            secondScrollOutput.Document.UndoStack.SizeLimit = 0;
            secondScrollOutput.TextArea.TextView.ScrollOffsetChanged += Second_Output_TextView_ScrollOffsetChanged;

            Loaded += (o, e) => CommandInput.Focus();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override TextBoxWithHistory CommandInput
        {
            get
            {
                return _txtCommandInput;
            }
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

        private void MainOutput_TextView_ScrollOffsetChanged(object sender, EventArgs e)
        {
            var textView = sender as TextView;

            if (textView.DocumentHeight - (mainScrollOutput.ViewportHeight + textView.VerticalOffset) < 0.01)
                _isMainOutputEndToScroll = true;
            else
                _isMainOutputEndToScroll = false;
        }

        private void TextArea_GotFocus(object sender, RoutedEventArgs e)
        {
            CommandInput.Focus();
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            ((Caret)sender).Hide();
        }

        private void Second_Output_TextView_ScrollOffsetChanged(object sender, EventArgs e)
        {
            var textView = sender as TextView;
            if (scrollGridRow.Height.Value != 0 
                && textView.DocumentHeight - (textView.VerticalOffset + secondScrollOutput.ViewportHeight + splitterGridRow.Height.Value) < 0.01)
            {
                if (_isMainOutputEndToScroll)
                    _needMainOutputScrollToEnd = true;

                if (!gridSplitter.IsDragging)
                {
                    scrollGridRow.Height = new GridLength(0);
                    splitterGridRow.Height = new GridLength(0);
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.PreviewKeyDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown([NotNull] KeyEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            _mainWindow.CheckGlobalHotkeys(e);

            if (e.Handled)
                return;

            var hotkeyCommand = new HotkeyCommand()
            {
                Key = e.Key == Key.System ? e.SystemKey : e.Key,
                ModifierKeys = Keyboard.Modifiers,
                Handled = false,
            };

            _rootModel.PushCommandToConveyor(hotkeyCommand);

            if (hotkeyCommand.Handled)
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Up && Keyboard.Modifiers == 0 && CommandInput.IsFocused)
            {
                CommandInput.ShowPreviousCommand();
                e.Handled = true;
            }

            if (e.Key == Key.Down && Keyboard.Modifiers == 0 && CommandInput.IsFocused)
            {
                CommandInput.ShowNextCommand();
                e.Handled = true;
            }

            if (e.Key == Key.Enter && CommandInput.IsFocused)
            {
                CommandInput.SendCurrentCommand();
                e.Handled = true;
            }

            if (e.Key == Key.PageUp)
            {
                if (scrollGridRow.Height.Value == 0)
                {
                    bool scrollToEnd = mainScrollOutput.ExtentHeight - (mainScrollOutput.ViewportHeight + mainScrollOutput.VerticalOffset) < 0.01;
                    scrollGridRow.Height = new GridLength(Math.Max(mainScrollOutput.ViewportHeight * 0.6, 1));
                    splitterGridRow.Height = new GridLength(5);

                    if (scrollToEnd)
                        mainScrollOutput.ScrollToEnd();
                }
                else
                {
                    secondScrollOutput.PageUp();
                }

                e.Handled = true;
            }

            if (e.Key == Key.PageDown)
            {
                if (scrollGridRow.Height.Value == 0)
                {
                    mainScrollOutput.PageDown();
                }
                else
                {
                    secondScrollOutput.PageDown();
                }
                e.Handled = true;
            }

            if (e.Key == Key.End && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (scrollGridRow.Height.Value == 0)
                {
                    mainScrollOutput.ScrollToEnd();
                }
                else
                {
                    mainScrollOutput.ScrollToLine(mainScrollOutput.LineCount);
                }

                e.Handled = true;
            }

            //Очищение коммандной строки клавишей escape
            if (e.Key == Key.Escape && Keyboard.Modifiers == 0 && CommandInput.IsFocused)
            {
                CommandInput.Clear();
                e.Handled = true;
            }

            if (!e.Handled)
            {
                base.OnPreviewKeyDown(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages">The messages.</param>
        public override void AddMessages(IList<TextMessage> messages)
        {
            Assert.ArgumentNotNull(messages, "messages");

            foreach (var message in messages)
            {
                mainScrollOutput.AppendText("\r\n");
                mainScrollOutput.AppendText(GetANSIColoredText(message));
            }

            if (mainScrollOutput.ExtentHeight == 0 || _isMainOutputEndToScroll || _needMainOutputScrollToEnd)
            {
                _needMainOutputScrollToEnd = false;
                mainScrollOutput.ScrollToLine(mainScrollOutput.LineCount);
            }
            else
            {
                mainScrollOutput.TextArea.TextView.Redraw();
            }

            if (scrollGridRow.Height.Value == 0)
            {
                secondScrollOutput.ScrollToLine(mainScrollOutput.Document.LineCount - 2);
            }
            else
            {
                secondScrollOutput.TextArea.TextView.Redraw();
            }

            if (mainScrollOutput.LineCount > SettingsHolder.Instance.Settings.ScrollBuffer)
            {
                int offset = mainScrollOutput.Document.GetOffset(
                    Math.Max(mainScrollOutput.LineCount - SettingsHolder.Instance.Settings.ScrollBuffer, 1), 0);

                mainScrollOutput.Document.Remove(0, offset);
            }
        }

        /// <summary>
        /// Actually now we have converted text twice.. At first from ansi text to blocks and then vice versa..
        /// But that allow use 1 TextDeserializer class
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string GetANSIColoredText(TextMessage message)
        {
            var sb = new StringBuilder();

            foreach (var block in message.MessageBlocks)
            {
                if (!String.IsNullOrEmpty(block.Text))
                {
                    sb.Append(ConvertTextColorToANSIString(block.Foreground, block.Background));
                    sb.Append(block.Text);
                }
            }

            return sb.ToString();
        }

        private string ConvertTextColorToANSIString(TextColor foreground, TextColor background)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\x1B[");
            
            switch(foreground)
            {
                case TextColor.Black:
                    sb.Append("30");
                    break;
                case TextColor.BrightBlack:
                    sb.Append("1;30");
                    break;
                case TextColor.Blue:
                    sb.Append("34");
                    break;
                case TextColor.BrightBlue:
                    sb.Append("1;34");
                    break;
                case TextColor.Cyan:
                    sb.Append("36");
                    break;
                case TextColor.BrightCyan:
                    sb.Append("1;36");
                    break;
                case TextColor.Green:
                    sb.Append("32");
                    break;
                case TextColor.BrightGreen:
                    sb.Append("1;32");
                    break;
                case TextColor.Magenta:
                    sb.Append("35");
                    break;
                case TextColor.BrightMagenta:
                    sb.Append("1;35");
                    break;
                case TextColor.Red:
                    sb.Append("31");
                    break;
                case TextColor.BrightRed:
                    sb.Append("1;31");
                    break;
                case TextColor.White:
                    sb.Append("37");
                    break;
                case TextColor.BrightWhite:
                    sb.Append("1;37");
                    break;
                case TextColor.Yellow:
                    sb.Append("33");
                    break;
                case TextColor.BrightYellow:
                    sb.Append("1;33");
                    break;
                case TextColor.RepeatCommandTextColor:
                    sb.Append("4m");
                    return sb.ToString();
                case TextColor.None:
                default:
                    sb.Append("0");
                    break;
            }

            switch (background)
            {
                case TextColor.Black:
                    sb.Append(";0;40");
                    break;
                case TextColor.BrightBlack:
                    sb.Append(";1;40");
                    break;
                case TextColor.Blue:
                    sb.Append(";0;44");
                    break;
                case TextColor.BrightBlue:
                    sb.Append(";1;44");
                    break;
                case TextColor.Cyan:
                    sb.Append(";0;46");
                    break;
                case TextColor.BrightCyan:
                    sb.Append(";1;46");
                    break;
                case TextColor.Green:
                    sb.Append(";0;42");
                    break;
                case TextColor.BrightGreen:
                    sb.Append(";1;42");
                    break;
                case TextColor.Magenta:
                    sb.Append(";0;45");
                    break;
                case TextColor.BrightMagenta:
                    sb.Append(";1;45");
                    break;
                case TextColor.Red:
                    sb.Append(";0;41");
                    break;
                case TextColor.BrightRed:
                    sb.Append(";1;41");
                    break;
                case TextColor.White:
                    sb.Append(";0;47");
                    break;
                case TextColor.BrightWhite:
                    sb.Append(";1;47");
                    break;
                case TextColor.Yellow:
                    sb.Append(";0;43");
                    break;
                case TextColor.BrightYellow:
                    sb.Append(";1;43");
                    break;
                case TextColor.None:
                case TextColor.RepeatCommandTextColor:
                default:
                    break;
            }

            sb.Append('m');
            return sb.ToString();
        }
    }
}
