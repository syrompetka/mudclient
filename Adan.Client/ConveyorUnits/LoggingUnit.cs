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
    using System.Linq;
    using Common.Model;
    using Common.ConveyorUnits;
    using Common.Messages;
    using CSLib.Net.Diagnostics;
    using Messages;
    using Properties;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementation repsponsible for logging.
    /// </summary>
    public class LoggingUnit : ConveyorUnit
    {
        private readonly MainWindow _mainWindow;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainWindow"></param>
        public LoggingUnit(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
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
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="rootModel"></param>
        public override void HandleMessage(Message message, RootModel rootModel)
        {
            Assert.ArgumentNotNull(message, "message");
            Assert.ArgumentNotNull(rootModel, "rootModel");

            var startLogMessage = message as StartLoggingMessage;
            if (startLogMessage != null)
            {
                if (string.IsNullOrEmpty(startLogMessage.LogName))
                {
                    PushMessageToConveyor(new ErrorMessage(Resources.LogNameCanNotBeEmpty), rootModel);
                }
                else
                {
                    //TODO:_mainWindow.StartLogging(startLogMessage.LogName, rootModel);
                }

                return;
            }

            var stopLogMessage = message as StopLoggingMessage;
            if (stopLogMessage != null)
            {
                //TODO:_mainWindow.StopLogging(rootModel);

                return;
            }

            var connectedMessage = message as ConnectedMessage;
            if (connectedMessage != null)
            {
                //TODO:_mainWindow.StopLogging(rootModel);

                return;
            }

            var disconnectedMessage = message as DisconnectedMessage;
            if (disconnectedMessage != null)
            {
                //TODO:_mainWindow.StopLogging(rootModel);
            }
        }
    }
}
