using Adan.Client.Common.Commands;
using Adan.Client.Common.Controls;
using Adan.Client.Common.Messages;
using Adan.Client.Common.Model;
using Adan.Client.Common.Settings;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Adan.Client.Controls
{
    /// <summary>
    /// Логика взаимодействия для MainOutputWindowNative.xaml
    /// </summary>
    public partial class MainOutputWindowNative : MainOutputWindowBase, IDisposable
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

            _mainTextBoxNative.TextViewNative.GotFocus += TextViewNative_GotFocus;
            MouseWheelRedirector.Attach(_mainTextBoxNative.TextViewNative);
            _mainTextBoxNative.TextViewNative.MouseWheel += MainTextViewNative_MouseWheel;

            _secondTextBoxNative.TextViewNative.GotFocus += TextViewNative_GotFocus;
            MouseWheelRedirector.Attach(_secondTextBoxNative.TextViewNative);
            _secondTextBoxNative.TextViewNative.MouseWheel += SecondTextViewNative_MouseWheel;

            this.GotFocus += MainOutputWindowNative_GotFocus;
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

        private void MainOutputWindowNative_GotFocus(object sender, RoutedEventArgs e)
        {
            _txtCommandInput.Focus();
        }

        private void MainTextViewNative_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
                _mainTextBoxNative.MouseWheelUp();
            else if(e.Delta < 0)
                _mainTextBoxNative.MouseWheelDown();
        }

        private void SecondTextViewNative_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
                _mainTextBoxNative.MouseWheelUp();
            else if (e.Delta < 0)
                _mainTextBoxNative.MouseWheelDown();
        }
        
        private void TextViewNative_GotFocus(object sender, System.EventArgs e)
        {
            CommandInput.Focus();
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

            if (e.Key == Key.PageUp)
            {
                _mainTextBoxNative.PageUp();
                e.Handled = true;
            }

            if (e.Key == Key.PageDown)
            {
                _mainTextBoxNative.PageDown();
                e.Handled = true;
            }

            if (e.Key == Key.End && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _mainTextBoxNative.ScrollToEnd();

                e.Handled = true;
            }

            if (e.Key == Key.Enter && CommandInput.IsFocused)
            {
                CommandInput.SendCurrentCommand();
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

            _mainTextBoxNative.AddMessage(messages);

            if (_mainTextBoxNative.ExtentHeight > SettingsHolder.Instance.Settings.CommandsHistorySize)
            {
                if (_mainTextBoxNative.ExtentHeight > 10)
                    _mainTextBoxNative.RemoveMessage(0, 10);
                else
                    _mainTextBoxNative.RemoveMessage(0, 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            try
            {
                MouseWheelRedirector.Detach(this._mainTextBoxNative.TextViewNative);
                MouseWheelRedirector.Detach(this._secondTextBoxNative.TextViewNative);
            }
            catch { }
        }
    }
}
