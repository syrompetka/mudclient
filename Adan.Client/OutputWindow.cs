using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adan.Client.Common.Conveyor;
using Adan.Client.Common.Networking;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using System.Windows.Threading;
using Adan.Client.Common.Messages;
using Adan.Client.Controls;
using Adan.Client.Common.Model;
using Adan.Client.Common.Settings;
using System.Windows.Controls;
using System.Windows;
using AvalonDock;
using System.IO;
using System.Threading.Tasks;
using Adan.Client.Properties;
using System.Globalization;
using System.Diagnostics;
using System.Timers;
using System.Windows.Forms.Integration;

namespace Adan.Client
{
    /// <summary>
    /// 
    /// </summary>
    public class OutputWindow : IDisposable
    {
        private Queue<TextMessage> _messageQueue = new Queue<TextMessage>();
        private object _messageQueueLockObject = new object();
        private RootModel _rootModel;
#if NATIVE
        private MainOutputWindowNative _window;
#else
        private MainOutputWindowExx _window;
#endif
        private object _loggingLockObject = new object();
        private StreamWriter _streamWriter;
        private bool _isLogging;

        
        /// <summary>
        /// 
        /// </summary>
        public OutputWindow(MainWindow mainWindow, string name)
        {
            Name = name;

            //var conveyor = new MessageConveyor(new MccpClientEx());
            var conveyor = new MessageConveyor(new MccpClient());

            RootModel = new RootModel(conveyor, SettingsHolder.Instance.GetProfile(name));
            conveyor.RootModel = RootModel;

            conveyor.MessageReceived += HandleMessage;

#if NATIVE
            _window = new MainOutputWindowNative(mainWindow, _rootModel);
#else
            _window = new MainOutputWindowExx(mainWindow, RootModel);
#endif
            VisibleControl = _window;
            
            _window.txtCommandInput.RootModel = RootModel;
            _window.txtCommandInput.LoadHistory(RootModel.Profile);
            _window.txtCommandInput.GotFocus += txtCommandInput_GotFocus;
            _window.txtCommandInput.GotKeyboardFocus += txtCommandInput_GotFocus;

            IsLogging = false;
        }

        private void txtCommandInput_GotFocus(object sender, RoutedEventArgs e)
        {
            PluginHost.Instance.OutputWindowChanged(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public DockableContent DockContent
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
        public bool IsLogging
        {
            get
            {
                return _isLogging;
            }
            set
            {
                _isLogging = value;
                _rootModel.IsLogging = value;
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
            _window.txtCommandInput.SaveCurrentHistory(RootModel.Profile);
            SettingsHolder.Instance.SetProfile(RootModel.Profile, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logName"></param>
        public void StartLogging(string logName)
        {
            try
            {
                var fullLogPath = Path.Combine(GetLogsFolder(), logName + ".log");
                var directoryPath = Path.GetDirectoryName(fullLogPath);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                var fileStream = new FileStream(fullLogPath, FileMode.Append, FileAccess.Write);

                _streamWriter = new StreamWriter(fileStream, Encoding.Unicode) { AutoFlush = true };

                RootModel.PushMessageToConveyor(new InfoMessage(string.Format(Resources.LoggingStarted, fullLogPath)));

                IsLogging = true;
            }
            catch (IOException ex)
            {
                RootModel.PushMessageToConveyor(new ErrorMessage(ex.Message));
                IsLogging = false;
                _streamWriter = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopLogging()
        {
            if (IsLogging)
            {

                if (_streamWriter != null)
                {
                    lock (_loggingLockObject)
                    {
                        _streamWriter.Close();
                        _streamWriter = null;
                    }
                }

                IsLogging = false;
                RootModel.PushMessageToConveyor(new InfoMessage(Resources.LoggingStopped));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
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
                if (_streamWriter != null)
                {
                    lock (_loggingLockObject)
                    {
                        _streamWriter.Close();
                        _streamWriter = null;
                    }
                }
            }

            GC.SuppressFinalize(this);
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

                if (IsLogging)
                {
                    Task.Factory.StartNew(() =>
                    {
                        lock (_loggingLockObject)
                        {
                            if (IsLogging)
                            {
                                if (_streamWriter != null)
                                {
                                    _streamWriter.WriteLine(message.InnerText);
                                }
                            }
                        }
                    });
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

        [NotNull]
        private string GetLogsFolder()
        {
            var path = Path.Combine(SettingsHolder.Instance.Folder, "Logs");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}