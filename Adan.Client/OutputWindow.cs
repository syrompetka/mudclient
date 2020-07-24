using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Adan.Client.Common.Conveyor;
using Adan.Client.Common.Messages;
using Adan.Client.Common.Model;
using Adan.Client.Common.Settings;
using Adan.Client.Controls;
using Adan.Client.Messages;
using Adan.Client.Model;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using Xceed.Wpf.AvalonDock.Layout;

namespace Adan.Client
{
    public class OutputWindow : IDisposable
    {
        private readonly MainWindow _mainWindow;
        private readonly Queue<TextMessage> _messageQueue = new Queue<TextMessage>();
        private readonly object _messageQueueLockObject = new object();
        private RootModel _rootModel;
        private readonly MainOutputWindow _window;

        public OutputWindow(MainWindow mainWindow, string name, IList<RootModel> allRootModels)
        {
            _mainWindow = mainWindow;
            Name = name;


            var conveyor = ConveyorFactory.CreateNew(name, allRootModels);
            RootModel = conveyor.RootModel;
            conveyor.MessageReceived += HandleMessage;

            _window = new MainOutputWindow(mainWindow, _rootModel);
            VisibleControl = _window;
        }

        /// <summary>
        /// 
        /// </summary>
        public LayoutContent DockContent
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Uid
        {
            get
            {
                return _rootModel.Uid;
            }
            set
            {
                _rootModel.Uid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Control VisibleControl
        {
            get;
            private set;
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
            private set
            {
                _rootModel = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Focus()
        {
            _window.txtCommandInput.Focus();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            _window.SaveCurrentHistory(RootModel.Profile);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                RootModel.Dispose();
            }
        }

        private void HandleMessage([NotNull] object sender, [NotNull] MessageReceivedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var message = e.Message as OutputToMainWindowMessage;

            if (message != null)
            {
                lock (_messageQueueLockObject)
                {
                    _messageQueue.Enqueue(message);
                }

                if (Application.Current != null)
                {
                    if (_window?.IsVisible==true)
                        Application.Current.Dispatcher.BeginInvoke((Action)ProcessMessageQueue, DispatcherPriority.Normal);
                    else
                        Application.Current.Dispatcher.BeginInvoke((Action)ProcessMessageQueue, DispatcherPriority.Background);
                }
                return;
            }

            var fullScreenModeMessage = e.Message as ToggleFullScreenModeMessage;
            if (fullScreenModeMessage != null)
            {
                _mainWindow.ToggleFullScreenMode();
                return;
            }

            var showOutputWindowMessage = e.Message as ShowOutputWindowMessage;
            if (showOutputWindowMessage != null)
            {
                _mainWindow.ShowOutputWindow(showOutputWindowMessage.WindowName);
            }

            var showStatusBarMessage = e.Message as ShowStatusBarMessage;
            if (showStatusBarMessage != null)
            {
                // for some reason, if called from a trigger, we'll have an exception thrown
                // due to not being on the UI thread
                var context = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();
                var token = System.Threading.Tasks.Task.Factory.CancellationToken;
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    _window.DisplayStatusBar(showStatusBarMessage.State, true);
                }, token, System.Threading.Tasks.TaskCreationOptions.None, context);
            }

            var setStatusMessage = e.Message as SetStatusMessage;
            if (setStatusMessage != null)
            {
                // for some reason, if called from a trigger, we'll have an exception thrown
                // due to not being on the UI thread
                var context = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();
                var token = System.Threading.Tasks.Task.Factory.CancellationToken;
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    _window.SetStatusBar(setStatusMessage.Id, setStatusMessage.Msg, setStatusMessage.Color);
                }, token, System.Threading.Tasks.TaskCreationOptions.None, context);
            }
        }

        private void ProcessMessageQueue()
        {
            IList<TextMessage> messages;
            lock (_messageQueueLockObject)
            {
                messages = _messageQueue.ToList();
                _messageQueue.Clear();
            }

            if (messages.Count > 0)
            {
                _window.AddMessages(messages);
            }
        }
    }
}