﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentRoomMessageDeserializer.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the CurrentRoomMessageDeserializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    using Common.Conveyor;
    using Common.MessageDeserializers;
    using Common.Messages;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// <see cref="MessageDeserializer"/> implementation to handle "current room" messages.
    /// </summary>
    public class CurrentRoomMessageDeserializer : MessageDeserializer
    {
        private readonly ZoneManager _zoneManager;

        private readonly StringBuilder _builder = new StringBuilder();
        private readonly Encoding _encoding = Encoding.GetEncoding(1251);
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(CurrentRoomMessage));

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentRoomMessageDeserializer"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        /// <param name="zoneManager">The zone manager.</param>
        public CurrentRoomMessageDeserializer([NotNull] MessageConveyor messageConveyor, [NotNull] ZoneManager zoneManager)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
            Assert.ArgumentNotNull(zoneManager, "zoneManager");
            _zoneManager = zoneManager;
        }

        /// <summary>
        /// Gets the type of deserialized message.
        /// </summary>
        /// <value>
        /// The type of deserialized message.
        /// </value>
        public override int DeserializedMessageType
        {
            get
            {
                return Constants.CurrentRoomMessageType;
            }
        }

        /// <summary>
        /// Processes the data from server.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="bytesReceived">The bytes received.</param>
        /// <param name="data">The get data.</param>
        /// <param name="isComplete">Indicates whether message should be completed or wait for next data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "It's ok here.")]
        public override void DeserializeDataFromServer(int offset, int bytesReceived, byte[] data, bool isComplete)
        {
            Assert.ArgumentNotNull(data, "data");

            try
            {
                var messageXml = _encoding.GetString(data, offset, bytesReceived);
                _builder.Append(messageXml);
                if (isComplete)
                {
                    using (var stringReader = new StringReader(_builder.ToString()))
                    {
                        var message = (CurrentRoomMessage)_serializer.Deserialize(stringReader);
                        _zoneManager.UpdateCurrentRoom(message.RoomId, message.ZoneId);
                    }

                    _builder.Clear();
                }
            }
            catch (Exception ex)
            {
                _builder.Clear();
                PushMessageToConveyor(new ErrorMessage(ex.ToString()));
            }
        }
    }
}