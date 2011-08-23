// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageConveyor.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the MessageConveyor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Conveyor
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;

    using Commands;
    using CommandSerializers;

    using ConveyorUnits;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using MessageDeserializers;
    using Messages;

    using Networking;

    #endregion

    /// <summary>
    /// Conveyor that passess messages throught handlers.
    /// </summary>
    public sealed class MessageConveyor : IDisposable
    {
        #region Constants and Fields

        private readonly IList<CommandSerializer> _commandSerializers = new List<CommandSerializer>();
        private readonly IList<ConveyorUnit> _conveyorUnits = new List<ConveyorUnit>();
        private readonly MccpClient _mccpClient;
        private readonly IList<MessageDeserializer> _messageDeserializers = new List<MessageDeserializer>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageConveyor"/> class.
        /// </summary>
        /// <param name="mccpClient">The MCCP client.</param>
        public MessageConveyor([NotNull] MccpClient mccpClient)
        {
            Assert.ArgumentNotNull(mccpClient, "mccpClient");

            _mccpClient = mccpClient;
            _mccpClient.DataReceived += HandleDataReceived;
            _mccpClient.NetworkError += HandleNetworkError;
            _mccpClient.Connected += HandleConnected;
            _mccpClient.Disconnected += HandleDisconnected;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when message if revieved from server.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the command serializer.
        /// </summary>
        /// <param name="commandSerializer">The command serializer to add.</param>
        public void AddCommandSerializer([NotNull] CommandSerializer commandSerializer)
        {
            Assert.ArgumentNotNull(commandSerializer, "commandSerializer");

            _commandSerializers.Add(commandSerializer);
        }

        /// <summary>
        /// Adds the conveyor unit.
        /// </summary>
        /// <param name="conveyorUnit">The conveyor unit.</param>
        public void AddConveyorUnit([NotNull] ConveyorUnit conveyorUnit)
        {
            Assert.ArgumentNotNull(conveyorUnit, "conveyorUnit");

            _conveyorUnits.Add(conveyorUnit);
        }

        /// <summary>
        /// Adds the message deserializer.
        /// </summary>
        /// <param name="messageDeserializer">The message deserializer to add.</param>
        public void AddMessageDeserializer([NotNull] MessageDeserializer messageDeserializer)
        {
            Assert.ArgumentNotNull(messageDeserializer, "messageDeserializer");

            _messageDeserializers.Add(messageDeserializer);
        }

        /// <summary>
        /// Connects to the specified host.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        public void Connect([NotNull] string host, int port)
        {
            Assert.ArgumentNotNullOrWhiteSpace(host, "host");

            _mccpClient.Connect(host, port);
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            _mccpClient.Disconnect();
        }

        /// <summary>
        /// Pushes the command.
        /// </summary>
        /// <param name="command">The text command to send.</param>
        public void PushCommand([NotNull] Command command)
        {
            Assert.ArgumentNotNull(command, "command");

            foreach (var conveyorUnit in _conveyorUnits)
            {
                conveyorUnit.HandleCommand(command);
                if (command.Handled)
                {
                    break;
                }
            }

            if (!command.Handled)
            {
                foreach (var commandSerializer in _commandSerializers)
                {
                    commandSerializer.SerializeAndSendCommand(command);
                    if (command.Handled)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Pushes the message into conveyor queue.
        /// </summary>
        /// <param name="message">The message to push.</param>
        public void PushMessage([NotNull] Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            if (!message.SkipProcessing)
            {
                foreach (var conveyorUnit in _conveyorUnits)
                {
                    conveyorUnit.HandleMessage(message);
                    if (message.Handled)
                    {
                        break;
                    }
                }
            }

            if (message.Handled)
            {
                return;
            }

            if (MessageReceived != null)
            {
                MessageReceived(this, new MessageReceivedEventArgs(message));
            }
        }

        /// <summary>
        /// Sends the raw data to server.
        /// </summary>
        /// <param name="offset">The offset if <paramref name="data"/> array to start.</param>
        /// <param name="bytesToSend">The bytes to send.</param>
        /// <param name="data">The array of bytes to send.</param>
        public void SendRawDataToServer(int offset, int bytesToSend, [NotNull] byte[] data)
        {
            Assert.ArgumentNotNull(data, "data");

            _mccpClient.Send(data, offset, bytesToSend);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var conveyorUnit in _conveyorUnits)
            {
                conveyorUnit.Dispose();
            }
        }
        #endregion

        #region Methods

        private void HandleDataReceived([NotNull] object sender, [NotNull] DataReceivedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            int offset = e.Offset;
            int bytesRecieved = e.BytesReceived;

            foreach (var messageDeserializer in _messageDeserializers)
            {
                var processedBytes = messageDeserializer.DeserializeDataFromServer(
                    offset, bytesRecieved, e.GetData(), e.IsGagReceived);
                offset += processedBytes;
                bytesRecieved -= processedBytes;
                if (bytesRecieved == 0)
                {
                    break;
                }
            }
        }

        private void HandleConnected([NotNull] object sender, [NotNull] EventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            PushMessage(new ConnectedMessage());
        }

        private void HandleDisconnected([NotNull] object sender, [NotNull] EventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            PushMessage(new DisconnectedMessage());
        }

        private void HandleNetworkError([NotNull] object sender, [NotNull] NetworkErrorEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            PushMessage(new NetworkErrorMessage(e.SocketException));
        }

        #endregion
    }
}