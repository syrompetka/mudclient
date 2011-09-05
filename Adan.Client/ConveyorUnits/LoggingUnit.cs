// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the LoggingUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

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
        private bool _isLogging;
        private FileStream _fileStream;
        private StreamWriter _streamWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        public LoggingUnit([NotNull] MessageConveyor messageConveyor)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
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
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return new[] { BuiltInMessageTypes.LoggingMessage, BuiltInMessageTypes.TextMessage, BuiltInMessageTypes.ConnectionMessages };
            }
        }

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

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            if (_isLogging)
            {
                var outputMessage = message as OutputToMainWindowMessage;
                if (outputMessage != null)
                {
                    _streamWriter.WriteLine(outputMessage.InnerText);
                }
            }

            var startLogMessage = message as StartLoggingMessage;
            if (startLogMessage != null)
            {
                if (string.IsNullOrEmpty(startLogMessage.LogName))
                {
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
                if (_streamWriter != null)
                {
                    _streamWriter.Dispose();
                }
            }
        }

        private void StartLogging([NotNull] string logName)
        {
            Assert.ArgumentNotNullOrEmpty(logName, "logName");

            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }

            if (_isLogging)
            {
                StopLogging();
            }

            try
            {
                _fileStream = File.Open(@"Logs\" + logName + ".log", FileMode.Append, FileAccess.Write, FileShare.Read);
                _streamWriter = new StreamWriter(_fileStream, Encoding.Unicode);
                _isLogging = true;
                PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.InvariantCulture, Resources.LoggingStarted, @"Logs\" + logName + ".log")));
            }
            catch (IOException ex)
            {
                PushMessageToConveyor(new ErrorMessage(ex.Message));
                _isLogging = true;
                _fileStream = null;
            }
        }

        private void StopLogging()
        {
            if (!_isLogging)
            {
                return;
            }

            _streamWriter.Dispose();
            _streamWriter = null;
            _fileStream = null;

            _isLogging = false;
            PushMessageToConveyor(new InfoMessage(Resources.LoggingStopped));
        }
    }
}
