// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ConnectionUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Globalization;

    using Commands;

    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Properties;

    using ViewModel;

    /// <summary>
    /// Conveyor unit responsible for connection handling.
    /// </summary>
    public class ConnectionUnit : ConveyorUnit
    {
        private readonly MessageConveyor _messageConveyor;
        private readonly ConnectionStatusModel _connectionStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        public ConnectionUnit([NotNull] MessageConveyor messageConveyor)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
            _messageConveyor = messageConveyor;
            _connectionStatus = new ConnectionStatusModel();
        }

        #region Overrides of ConveyorUnit

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return new[] { BuiltInCommandTypes.TextCommand, BuiltInCommandTypes.ConnectionCommands };
            }
        }

        /// <summary>
        /// Gets a set of command types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledCommandTypes
        {
            get
            {
                return new[] { BuiltInMessageTypes.ConnectionMessages, BuiltInMessageTypes.TextMessage };
            }
        }

        /// <summary>
        /// Handles the command.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        public override void HandleCommand(Command command)
        {
            Assert.ArgumentNotNull(command, "command");

            var connectCommand = command as ConnectCommand;
            if (connectCommand != null)
            {
                connectCommand.Handled = true;
                if (_connectionStatus.Connected || _connectionStatus.ConnectionInProgress)
                {
                    PushMessageToConveyor(new ErrorMessage(Resources.AlreadyConnected));
                }
                else
                {
                    PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.TryingToConnect, connectCommand.Host, connectCommand.Port)));
                    _connectionStatus.ConnectionInProgress = true;
                    _messageConveyor.Connect(connectCommand.Host, connectCommand.Port);
                }

                return;
            }

            var disconnectCommand = command as DisconnectCommand;
            if (disconnectCommand != null)
            {
                disconnectCommand.Handled = true;
                if (!(_connectionStatus.Connected || _connectionStatus.ConnectionInProgress))
                {
                    PushMessageToConveyor(new ErrorMessage(Resources.NotConnected));
                }
                else
                {
                    _messageConveyor.Disconnect();
                }

                return;
            }

            if (!_connectionStatus.Connected)
            {
                command.Handled = true;
                PushMessageToConveyor(new ErrorMessage(Resources.NotConnectedPleaseConnectFirst));
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            var connectedMessage = message as ConnectedMessage;
            if (connectedMessage != null)
            {
                PushMessageToConveyor(new InfoMessage(Resources.ConnectionEstablished));
                _connectionStatus.Connected = true;
                _connectionStatus.ConnectionInProgress = false;
                return;
            }

            var disconnectedMessage = message as DisconnectedMessage;
            if (disconnectedMessage != null)
            {
                if (_connectionStatus.Connected)
                {
                    _connectionStatus.Connected = false;
                    _connectionStatus.ConnectionInProgress = false;
                    PushMessageToConveyor(new InfoMessage(Resources.ConnectionLost));
                    PushMessageToConveyor(
                        new InfoMessage(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.ConnectionStatistic,
                                disconnectedMessage.TotalBytesReceived,
                                disconnectedMessage.BytesDecompressed,
                                100.0f * (disconnectedMessage.TotalBytesReceived / (float)disconnectedMessage.BytesDecompressed))));
                }

                return;
            }

            var networkErrorMessage = message as NetworkErrorMessage;
            if (networkErrorMessage != null)
            {
                PushMessageToConveyor(new ErrorMessage(networkErrorMessage.SocketException.Message));
                _connectionStatus.Connected = false;
                _connectionStatus.ConnectionInProgress = false;

                //Автореконнект
                if (SettingsHolder.Instance.AutoReconnect)
                    _messageConveyor.PushCommand(new ConnectCommand(_messageConveyor.LastConnectionHost, _messageConveyor.LastConnectPort));
            }
        }

        #endregion
    }
}
