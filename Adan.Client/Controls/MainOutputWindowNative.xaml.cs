using Adan.Client.Common.Commands;
using Adan.Client.Common.Controls;
using Adan.Client.Common.Messages;
using Adan.Client.Common.Model;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Adan.Client.Controls
{
    /// <summary>
    /// Логика взаимодействия для MainOutputWindowNative.xaml
    /// </summary>
    public partial class MainOutputWindowNative : UserControl
    {
        private MainWindow _mainWindow;
        private RootModel _rootModel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MainWindowEx"></param>
        /// <param name="rootModel"></param>
        public MainOutputWindowNative(MainWindow MainWindowEx, RootModel rootModel)
        {
            InitializeComponent();
            _mainWindow = MainWindowEx;
            _rootModel = rootModel;

            _textBoxNative.TextViewNative.GotFocus += TextViewNative_GotFocus;
            MouseWheelRedirector.Attach(_textBoxNative.TextViewNative);
            _textBoxNative.TextViewNative.MouseWheel += TextViewNative_MouseWheel;
            Loaded += (o, e) => _txtCommandInput.Focus();
        }

        private void TextViewNative_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
                _textBoxNative.MouseWheelUp();
            else if(e.Delta < 0)
                _textBoxNative.MouseWheelDown();
        }
        
        private void TextViewNative_GotFocus(object sender, System.EventArgs e)
        {
            _txtCommandInput.Focus();
        }

        /// <summary>
        /// 
        /// </summary>
        public RootModel RootModel
        {
            get
            {
                return _rootModel;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.PreviewKeyDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown([NotNull] KeyEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

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

            if (e.Key == Key.Up && Keyboard.Modifiers == 0 && _txtCommandInput.IsFocused)
            {
                _txtCommandInput.ShowPreviousCommand();
                e.Handled = true;
            }

            if (e.Key == Key.Down && Keyboard.Modifiers == 0 && _txtCommandInput.IsFocused)
            {
                _txtCommandInput.ShowNextCommand();
                e.Handled = true;
            }

            if (e.Key == Key.PageUp)
            {
                //if (scrollGridRow.Height.Value == 0)
                //{
                //    bool scrollToEnd = mainScrollOutput.ExtentHeight - (mainScrollOutput.ViewportHeight + mainScrollOutput.VerticalOffset) < 0.01;
                //    scrollGridRow.Height = new GridLength(Math.Max(mainScrollOutput.ViewportHeight * 0.6, 1));
                //    splitterGridRow.Height = new GridLength(5);

                //    if (scrollToEnd)
                //        mainScrollOutput.ScrollToEnd();
                //}
                //else
                //{
                //    secondScrollOutput.PageUp();
                //}

                e.Handled = true;
            }

            if (e.Key == Key.PageDown)
            {
            //    if (scrollGridRow.Height.Value == 0)
            //    {
            //        mainScrollOutput.PageDown();
            //    }
            //    else
            //    {
            //        secondScrollOutput.PageDown();
            //    }

                e.Handled = true;
            }

            if (e.Key == Key.End && Keyboard.Modifiers == ModifierKeys.Control)
            {
                //if (scrollGridRow.Height.Value == 0)
                //{
                //    mainScrollOutput.ScrollToEnd();
                //}
                //else
                //{
                //    mainScrollOutput.ScrollToLine(mainScrollOutput.LineCount);
                //}

                e.Handled = true;
            }

            if (e.Key == Key.Enter && _txtCommandInput.IsFocused)
            {
                _txtCommandInput.SendCurrentCommand();
                e.Handled = true;
            }

            //Очищение коммандной строки клавишей escape
            if (e.Key == Key.Escape && Keyboard.Modifiers == 0 && _txtCommandInput.IsFocused)
            {
                _txtCommandInput.Clear();
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
        public void AddMessages([NotNull] IList<TextMessage> messages)
        {
            Assert.ArgumentNotNull(messages, "messages");

            _textBoxNative.AddMessage(messages);
        }

        private void _txtCommandInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
