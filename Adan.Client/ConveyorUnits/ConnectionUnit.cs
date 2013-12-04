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
    using Adan.Client.Common.Model;
    using Adan.Client.Common.Settings;
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
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="rootModel"></param>
        /// <param name="isImport"></param>
        public override void HandleCommand(Command command, RootModel rootModel, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");

            var connectCommand = command as ConnectCommand;
            if (connectCommand != null)
            {
                connectCommand.Handled = true;
                if (rootModel.Connected || rootModel.ConnectionInProgress)
                {
                    PushMessageToConveyor(new ErrorMessage(Resources.AlreadyConnected), rootModel);
                }
                else
                {
                    PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.TryingToConnect, connectCommand.Host, connectCommand.Port)), rootModel);
                    rootModel.ConnectionInProgress = true;
                    rootModel.MessageConveyor.Connect(connectCommand.Host, connectCommand.Port);
                }

                return;
            }

            var disconnectCommand = command as DisconnectCommand;
            if (disconnectCommand != null)
            {
                disconnectCommand.Handled = true;
                if (!(rootModel.Connected || rootModel.ConnectionInProgress))
                {
                    PushMessageToConveyor(new ErrorMessage(Resources.NotConnected), rootModel);
                }
                else
                {
                    rootModel.MessageConveyor.Disconnect();
                }

                return;
            }

            if (!rootModel.Connected)
            {
                command.Handled = true;
                PushMessageToConveyor(new ErrorMessage(Resources.NotConnectedPleaseConnectFirst), rootModel);
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

            var connectedMessage = message as ConnectedMessage;
            if (connectedMessage != null)
            {
                PushMessageToConveyor(new InfoMessage(Resources.ConnectionEstablished), rootModel);
                rootModel.Connected = true;
                rootModel.ConnectionInProgress = false;
                return;
            }

            var disconnectedMessage = message as DisconnectedMessage;
            if (disconnectedMessage != null)
            {
                if (rootModel.Connected)
                {
                    rootModel.Connected = false;
                    rootModel.ConnectionInProgress = false;
                    PushMessageToConveyor(new InfoMessage(Resources.ConnectionLost), rootModel);
                    PushMessageToConveyor(
                        new InfoMessage(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.ConnectionStatistic,
                                disconnectedMessage.TotalBytesReceived,
                                disconnectedMessage.BytesDecompressed,
                                100.0f * (disconnectedMessage.TotalBytesReceived / (float)disconnectedMessage.BytesDecompressed))), rootModel);
                }

                return;
            }

            var networkErrorMessage = message as NetworkErrorMessage;
            if (networkErrorMessage != null)
            {
                PushMessageToConveyor(new ErrorMessage(networkErrorMessage.SocketException.Message), rootModel);
                rootModel.Connected = false;
                rootModel.ConnectionInProgress = false;

                //Автореконнект
                if (SettingsHolder.Instance.Settings.AutoReconnect)
                    rootModel.PushCommandToConveyor(new ConnectCommand(rootModel.MessageConveyor.LastConnectHost, rootModel.MessageConveyor.LastConnectPort));
            }
        }

        #endregion
    }
}
