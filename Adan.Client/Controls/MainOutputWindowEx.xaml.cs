using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Adan.Client.Common.Commands;
using Adan.Client.Common.Controls.AvalonEdit;
using Adan.Client.Common.Messages;
using Adan.Client.Common.Model;
using Adan.Client.Common.Settings;
using Adan.Client.Common.Themes;
using Adan.Client.Common.Utils;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace Adan.Client.Controls
{
    /// <summary>
    /// Логика взаимодействия для MainOutputWindowEx.xaml
    /// </summary>
    public partial class MainOutputWindowEx : UserControl
    {
        private static TextEditorOptions _options;

        private MainWindow _mainWindow;
        private RootModel _rootModel;
        private ColorsQueue<TextColorHolder> _colors;

        static MainOutputWindowEx()
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
        public MainOutputWindowEx(MainWindow mainWindow, RootModel rootModel)
        {
            InitializeComponent();

            mainScrollOutput.Options = _options;
            secondScrollOutput.Options = _options;

            _colors = new ColorsQueue<TextColorHolder>();
            _mainWindow = mainWindow;
            _rootModel = rootModel;

            var documentColorizier = new DocumentColorizer() { ColorsQueue = _colors };
            var mainSelectionColorizer = new SelectionColorizer(
               ThemeManager.Instance.ActiveTheme.GetSelectionBrushByTextColor(false),
              ThemeManager.Instance.ActiveTheme.GetSelectionBrushByTextColor(true),
                 mainScrollOutput.TextArea);

            var secondSelectionColorizer = new SelectionColorizer(
                ThemeManager.Instance.ActiveTheme.GetSelectionBrushByTextColor(false),
                ThemeManager.Instance.ActiveTheme.GetSelectionBrushByTextColor(true), secondScrollOutput.TextArea);

            mainScrollOutput.TextArea.TextView.LineTransformers.Clear();
            mainScrollOutput.TextArea.TextView.LineTransformers.Add(documentColorizier);
            mainScrollOutput.TextArea.TextView.LineTransformers.Add(mainSelectionColorizer);
            mainScrollOutput.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            mainScrollOutput.TextArea.Caret.Hide();
            mainScrollOutput.TextArea.GotFocus += TextArea_GotFocus;
            mainScrollOutput.TextArea.PreviewMouseLeftButtonUp += TextArea_PreviewMouseLeftButtonUp;

            secondScrollOutput.TextArea.Document = mainScrollOutput.TextArea.Document;
            secondScrollOutput.TextArea.TextView.LineTransformers.Clear();
            secondScrollOutput.TextArea.TextView.LineTransformers.Add(documentColorizier);
            secondScrollOutput.TextArea.TextView.LineTransformers.Add(secondSelectionColorizer);
            secondScrollOutput.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            secondScrollOutput.TextArea.Caret.Hide();
            secondScrollOutput.TextArea.GotFocus += TextArea_GotFocus;
            secondScrollOutput.TextArea.PreviewMouseLeftButtonUp += TextArea_PreviewMouseLeftButtonUp;
            secondScrollOutput.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;

            Loaded += (o, e) => txtCommandInput.Focus();
        }

        private void TextArea_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var textArea = sender as TextArea;
            if (!textArea.Selection.IsEmpty)
            {
                Clipboard.SetText(textArea.Selection.GetText());
                textArea.ClearSelection();
            }
        }

        private void TextArea_GotFocus(object sender, RoutedEventArgs e)
        {
            txtCommandInput.Focus();
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            ((Caret)sender).Hide();
        }

        private void TextView_ScrollOffsetChanged(object sender, EventArgs e)
        {
            var textView = sender as TextView;
            if (scrollGridRow.Height.Value != 0 && textView.DocumentHeight - (textView.VerticalOffset + secondScrollOutput.ViewportHeight + splitterGridRow.Height.Value) < 0.01)
            {
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

            if (e.Key == Key.Up && Keyboard.Modifiers == 0 && txtCommandInput.IsFocused)
            {
                txtCommandInput.ShowPreviousCommand();
                e.Handled = true;
            }

            if (e.Key == Key.Down && Keyboard.Modifiers == 0 && txtCommandInput.IsFocused)
            {
                txtCommandInput.ShowNextCommand();
                e.Handled = true;
            }

            if (e.Key == Key.Enter && txtCommandInput.IsFocused)
            {
                txtCommandInput.SendCurrentCommand();
                e.Handled = true;
            }

            if (e.Key == Key.PageUp)
            {
                if (scrollGridRow.Height.Value == 0)
                {
                    bool scrollToEnd = mainScrollOutput.ExtentHeight - (mainScrollOutput.ViewportHeight + mainScrollOutput.VerticalOffset) < 0.01;
                    scrollGridRow.Height = new GridLength(Math.Max(mainScrollOutput.ViewportHeight * 0.4, 1));
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
                secondScrollOutput.PageDown();
                e.Handled = true;
            }

            if (e.Key == Key.End)
            {
                secondScrollOutput.ScrollToEnd();
                e.Handled = true;
            }

            //Очищение коммандной строки клавишей escape
            if (e.Key == Key.Escape && Keyboard.Modifiers == 0 && txtCommandInput.IsFocused)
            {
                txtCommandInput.Clear();
                e.Handled = true;
            }

            if (!e.Handled)
            {
                base.OnPreviewKeyDown(e);
            }
        }

        /// <summary>
        /// Очень странная реализация цветного текста О_о
        /// </summary>
        /// <param name="messages">The messages.</param>
        public void AddMessages([NotNull] IEnumerable<TextMessage> messages)
        {
            Assert.ArgumentNotNull(messages, "messages");

            //foreach (var message in messages)
            //{
            //    //Увеличиваем рэнж цветов на 2 для перевода строки
            //    if (_colors.Count > 0)
            //    {
            //        var range = _colors.GetRange(_colors.Count - 1);
            //        range.ChangeRange(range.Low, range.High + 2);
            //    }
            //    else
            //    {
            //        _colors.Enqueue(
            //            new Range(0, 1),
            //            new TextColorHolder(TextColor.None, TextColor.None));
            //    }

            //    mainScrollOutput.AppendText("\r\n");

            //    foreach (var block in message.MessageBlocks)
            //    {
            //        if (block.Text.Length > 0)
            //        {
            //            if (_colors.Count > 0)
            //            {
            //                //Если цвет фона и бэкграунда такой же, то просто увеличиваем рэнж.
            //                //Если нет, создаем новый рэнж с нужным цветом.
            //                var value = _colors.GetValue(_colors.Count - 1);
            //                if (value.Background == block.Background && value.Foreground == block.Foreground)
            //                {
            //                    var range = _colors.GetRange(_colors.Count - 1);
            //                    range.ChangeRange(range.Low, mainScrollOutput.Document.TextLength + block.Text.Length - 1);
            //                }
            //                else
            //                {
            //                    _colors.Enqueue(
            //                            new Range(_colors.GetRange(_colors.Count - 1).High + 1, _colors.GetRange(_colors.Count - 1).High + block.Text.Length),
            //                            new TextColorHolder(block.Foreground, block.Background));
            //                }
            //            }
            //            else
            //            {
            //                _colors.Enqueue(
            //                        new Range(mainScrollOutput.Document.TextLength, mainScrollOutput.Document.TextLength + block.Text.Length - 1),
            //                        new TextColorHolder(block.Foreground, block.Background));
            //            }

            //            mainScrollOutput.AppendText(block.Text);
            //        }
            //    }
            //}

            //if (mainScrollOutput.ExtentHeight == 0 || mainScrollOutput.ExtentHeight - (mainScrollOutput.VerticalOffset + mainScrollOutput.ViewportHeight) < 0.01)
            //{
            //    mainScrollOutput.ScrollToEnd();
            //}

            //if (scrollGridRow.Height.Value == 0)
            //{
            //    secondScrollOutput.ScrollToLine(mainScrollOutput.Document.LineCount - 2);
            //}

            //if (mainScrollOutput.LineCount > SettingsHolder.Instance.Settings.ScrollBuffer)
            //{
            //    int offset = mainScrollOutput.Document.GetOffset(
            //        Math.Max(SettingsHolder.Instance.Settings.ScrollBuffer - 5, 1), 0);

            //    //Удаляем лишние цветовые рэнжи
            //    var range = _colors.Dequeue(_colors.GetIndexForElement(offset));

            //    if (range.Range.Low < offset)
            //        range.Range.ChangeRange(offset, range.Range.High);

            //    //Изменяем все рэнжи на необходимое смещение
            //    for (int i = 0; i < _colors.Count; ++i)
            //        _colors.GetRange(i).ChangeRange(_colors.GetRange(i).Low - offset, _colors.GetRange(i).High - offset);

            //    //Удаляем сам текст
            //    mainScrollOutput.Document.Remove(0, offset);
            //}
        }
    }
}
