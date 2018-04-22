// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the LoggingUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Adan.Client.Common.Settings;

namespace Adan.Client.ConveyorUnits
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;
    using Properties;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementation  repsponsible for logging.
    /// </summary>
    public sealed class LoggingUnit : ConveyorUnit
    {
        private readonly object _syncRoot = new object();
        private readonly ConcurrentQueue<TextMessage> _messageQueue = new ConcurrentQueue<TextMessage>();

        private bool _isLogging;
        private bool _disposed;
        private FileStream _fileStream;
        private StreamWriter _streamWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        public LoggingUnit([NotNull] MessageConveyor messageConveyor) : base(messageConveyor)
        {
            Task.Factory.StartNew(MessageWriterFunction);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="LoggingUnit"/> class. 
        /// <see cref="LoggingUnit"/> is reclaimed by garbage collection.
        /// </summary>
        ~LoggingUnit()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes => new[] { BuiltInMessageTypes.LoggingMessage, BuiltInMessageTypes.TextMessage, BuiltInMessageTypes.ConnectionMessages};

        /// <summary>
        /// Gets a set of command types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledCommandTypes
        {
            get
            {
                return Enumerable.Empty<int>();
            }
        }

        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            if (_isLogging)
            {
                var outputMessage = message as OutputToMainWindowMessage;
                if (outputMessage != null)
                {
                    _messageQueue.Enqueue(outputMessage);
                }
            }

            var startLogMessage = message as StartLoggingMessage;
            if (startLogMessage != null)
            {
                if (string.IsNullOrEmpty(startLogMessage.LogName))
                {
                    startLogMessage.LogName = DateTime.Today.ToString("yyyy-MM-dd") + "-" + Conveyor.RootModel.Profile.Name;
                    PushMessageToConveyor(new ErrorMessage(Resources.LogNameCanNotBeEmpty));
                }
                else
                {
                    StartLogging(startLogMessage.LogName);
                }

                startLogMessage.Handled = true;
            }

            var stopLogMessage = message as StopLoggingMessage;
            if (stopLogMessage != null)
            {
                StopLogging();
                stopLogMessage.Handled = true;
            }

            var connectedMessage = message as ConnectedMessage;
            if (connectedMessage != null)
            {
                StopLogging();
            }

            var disconnectedMessage = message as DisconnectedMessage;
            if (disconnectedMessage != null)
            {
                StopLogging();
            }
        }


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                lock (_syncRoot)
                {
                    if (_streamWriter != null)
                    {
                        _streamWriter.Dispose();
                    }

                    _disposed = true;
                }
            }
        }

        [NotNull]
        private static string GetLogsFolder()
        {
            return Path.Combine(SettingsHolder.Instance.Folder, "Logs");
        }

        private void StartLogging([NotNull] string logName)
        {
            Assert.ArgumentNotNullOrEmpty(logName, "logName");

            lock (_syncRoot)
            {
                if (!Directory.Exists(GetLogsFolder()))
                {
                    Directory.CreateDirectory(GetLogsFolder());
                }

                if (_isLogging)
                {
                    StopLogging();
                }

                try
                {
                    var fullLogPath = Path.Combine(GetLogsFolder(), logName + ".log");
                    _fileStream = File.Open(fullLogPath, FileMode.Append, FileAccess.Write, FileShare.Read);
                    _streamWriter = new StreamWriter(_fileStream, Encoding.Unicode);
                    _isLogging = true;
                    PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.InvariantCulture, Resources.LoggingStarted, fullLogPath)));
                }
                catch (IOException ex)
                {
                    PushMessageToConveyor(new ErrorMessage(ex.Message));
                    _isLogging = true;
                    _fileStream = null;
                }
            }
        }

        private void StopLogging()
        {
            lock (_syncRoot)
            {
                if (!_isLogging)
                {
                    return;
                }

                TextMessage message;
                while (_messageQueue.TryDequeue(out message))
                {
                    _streamWriter.WriteLine(message.InnerText);
                }

                _streamWriter.Dispose();
                _streamWriter = null;
                _fileStream = null;

                _isLogging = false;
                PushMessageToConveyor(new InfoMessage(Resources.LoggingStopped));
            }
        }

        private void MessageWriterFunction()
        {
            while (true)
            {
                if (_disposed)
                {
                    return;
                }

                lock (_syncRoot)
                {
                    if (_isLogging)
                    {
                        TextMessage message;
                        while (_messageQueue.TryDequeue(out message))
                        {
                            _streamWriter.WriteLine(message.InnerText);
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}
