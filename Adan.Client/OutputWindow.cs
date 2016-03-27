using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Adan.Client.Common.Conveyor;
using Adan.Client.Common.Messages;
using Adan.Client.Common.Model;
using Adan.Client.Common.Networking;
using Adan.Client.Common.Settings;
using Adan.Client.Controls;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using Xceed.Wpf.AvalonDock.Layout;

namespace Adan.Client
{
    public class OutputWindow : IDisposable
    {
        private readonly Queue<TextMessage> _messageQueue = new Queue<TextMessage>();
        private readonly object _messageQueueLockObject = new object();
        private RootModel _rootModel;
        private readonly MainOutputWindow _window;

        /// <summary>
        /// 
        /// </summary>
        public OutputWindow(MainWindow mainWindow, string name)
        {
            Name = name;

            var conveyor = new MessageConveyor(new MccpClient());

            RootModel = new RootModel(conveyor, SettingsHolder.Instance.GetProfile(name));
            conveyor.RootModel = RootModel;
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
                RootModel.MessageConveyor.Dispose();
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
                    if (_window.IsVisible)
                        Application.Current.Dispatcher.BeginInvoke((Action)ProcessMessageQueue, DispatcherPriority.Normal);
                    else
                        Application.Current.Dispatcher.BeginInvoke((Action)ProcessMessageQueue, DispatcherPriority.Background);
                }
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