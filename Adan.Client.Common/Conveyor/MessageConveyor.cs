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
    using System.Linq;

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
        private readonly MccpClient _mccpClient;
        private readonly IList<MessageDeserializer> _messageDeserializers = new List<MessageDeserializer>();
        private readonly IDictionary<int, IList<ConveyorUnit>> _conveyorUnitsByMessageType = new Dictionary<int, IList<ConveyorUnit>>();
        private readonly IDictionary<int, IList<ConveyorUnit>> _conveyorUnitsByCommandType = new Dictionary<int, IList<ConveyorUnit>>();
        private readonly byte[] _buffer = new byte[32767];

        private int _currentMessageType = BuiltInMessageTypes.TextMessage;

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

        /// <summary>
        /// Last connection host
        /// </summary>
        public string LastConnectionHost
        {
            get;
            private set;
        }

        /// <summary>
        /// Last connection port
        /// </summary>
        public int LastConnectPort
        {
            get;
            private set;
        }

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

            foreach (var handledMessageType in conveyorUnit.HandledMessageTypes)
            {
                if (!_conveyorUnitsByMessageType.ContainsKey(handledMessageType))
                {
                    _conveyorUnitsByMessageType[handledMessageType] = new List<ConveyorUnit>();
                }

                _conveyorUnitsByMessageType[handledMessageType].Add(conveyorUnit);
            }

            foreach (var handledCommandType in conveyorUnit.HandledCommandTypes)
            {
                if (!_conveyorUnitsByCommandType.ContainsKey(handledCommandType))
                {
                    _conveyorUnitsByCommandType[handledCommandType] = new List<ConveyorUnit>();
                }

                _conveyorUnitsByCommandType[handledCommandType].Add(conveyorUnit);
            }
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

            LastConnectionHost = host;
            LastConnectPort = port;
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

            if (_conveyorUnitsByCommandType.ContainsKey(command.CommandType))
            {
                foreach (var conveyorUnit in _conveyorUnitsByCommandType[command.CommandType])
                {
                    conveyorUnit.HandleCommand(command);
                    if (command.Handled)
                    {
                        break;
                    }
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

            if (_conveyorUnitsByMessageType.ContainsKey(message.MessageType))
            {
                foreach (var conveyorUnit in _conveyorUnitsByMessageType[message.MessageType])
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
            var allUnits = new List<ConveyorUnit>();
            foreach (var unitList in _conveyorUnitsByMessageType.Values)
            {
                allUnits.AddRange(unitList);
            }

            foreach (var unitList in _conveyorUnitsByCommandType.Values)
            {
                allUnits.AddRange(unitList);
            }

            foreach (var conveyorUnit in allUnits.Distinct())
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

            try
            {
                int offset = e.Offset;
                int bytesRecieved = e.BytesReceived;
                byte[] data = e.GetData();

                int actualBytesReceived = 0;
                for (int i = 0; i < bytesRecieved; i++)
                {
                    // removing double IAC and processing IAC GA
                    if (i < bytesRecieved - 1 && data[offset + i] == TelnetConstants.InterpretAsCommandCode && data[offset + i + 1] == TelnetConstants.InterpretAsCommandCode)
                    {
                        _buffer[actualBytesReceived] = TelnetConstants.InterpretAsCommandCode;
                        i++;
                        actualBytesReceived++;
                        continue;
                    }

                    if (i < bytesRecieved - 1 && data[offset + i] == TelnetConstants.InterpretAsCommandCode && data[offset + i + 1] == TelnetConstants.GoAheadCode)
                    {
                        // new line
                        _buffer[actualBytesReceived] = 0xA;
                        i++;
                        actualBytesReceived++;
                        continue;
                    }

                    // handling echo mode on/off
                    if (i < bytesRecieved - 2 && data[offset + i] == TelnetConstants.InterpretAsCommandCode && data[offset + i + 1] == TelnetConstants.WillCode && data[offset + i + 2] == TelnetConstants.EchoCode)
                    {
                        PushMessage(new ChangeEchoModeMessage(false));
                        i += 2;
                        continue;
                    }

                    if (i < bytesRecieved - 2 && data[offset + i] == TelnetConstants.InterpretAsCommandCode && data[offset + i + 1] == TelnetConstants.WillNotCode && data[offset + i + 2] == TelnetConstants.EchoCode)
                    {
                        PushMessage(new ChangeEchoModeMessage(true));
                        i += 2;
                        continue;
                    }

                    // handling custom message header
                    if (i < bytesRecieved - 3 && data[offset + i] == TelnetConstants.InterpretAsCommandCode && data[offset + i + 1] == TelnetConstants.SubNegotiationStartCode && data[offset + i + 2] == TelnetConstants.CustomProtocolCode)
                    {
                        var messageType = data[offset + i + 3];
                        FlushBufferToSerializer(actualBytesReceived, true);
                        actualBytesReceived = 0;
                        _currentMessageType = messageType;
                        i += 3;
                        continue;
                    }

                    // handling custom message footer
                    if (i < bytesRecieved - 1 && data[offset + i] == TelnetConstants.InterpretAsCommandCode && data[offset + i + 1] == TelnetConstants.SubNegotiationEndCode)
                    {
                        FlushBufferToSerializer(actualBytesReceived, true);

                        _currentMessageType = BuiltInMessageTypes.TextMessage;
                        actualBytesReceived = 0;
                        i++;
                        continue;
                    }

                    _buffer[actualBytesReceived] = data[offset + i];
                    actualBytesReceived++;
                }

                FlushBufferToSerializer(actualBytesReceived, false);
            }
            catch (Exception ex)
            {
                PushMessage(new ErrorMessage(ex.ToString()));
            }
        }

        private void FlushBufferToSerializer(int actualBytesReceived, bool isComplete)
        {
            if (actualBytesReceived <= 0)
            {
                return;
            }

            var deserializer = _messageDeserializers.FirstOrDefault(d => d.DeserializedMessageType == _currentMessageType);
            if (deserializer == null)
            {
                // falling back to text if there is no
                deserializer = _messageDeserializers.First(d => d.DeserializedMessageType == BuiltInMessageTypes.TextMessage);
            }

            deserializer.DeserializeDataFromServer(0, actualBytesReceived, _buffer, isComplete);
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

            PushMessage(new DisconnectedMessage(_mccpClient.TotalBytesReceived, _mccpClient.BytesDecompressed));
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