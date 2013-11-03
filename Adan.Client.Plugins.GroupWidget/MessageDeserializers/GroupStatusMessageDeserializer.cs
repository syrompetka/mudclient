﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupStatusMessageDeserializer.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the GroupStatusMessageDeserializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget.MessageDeserializers
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
    using Adan.Client.Plugins.GroupWidget.Messages;

    /// <summary>
    /// <see cref="MessageDeserializer"/> to deserializer <see cref="GroupStatusMessage"/> messages.
    /// </summary>
    public class GroupStatusMessageDeserializer : MessageDeserializer
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly Encoding _encoding = Encoding.GetEncoding(1251);
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(GroupStatusMessage));

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupStatusMessageDeserializer"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        public GroupStatusMessageDeserializer([NotNull] MessageConveyor messageConveyor)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
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
                return Constants.GroupStatusMessageType;
            }
        }

        /// <summary>
        /// Processes the data from server.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="bytesReceived">The bytes received.</param>
        /// <param name="data">The get data.</param>
        /// <param name="isComplete">Indicates whether message should be completed or wait for next data.</param>
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
                        var message = (GroupStatusMessage)_serializer.Deserialize(stringReader);
                        PushMessageToConveyor(message);
                    }

                    _builder.Clear();
                }
            }
            catch (Exception ex)
            {
                _builder.Clear();
                //PushMessageToConveyor(new ErrorMessage(ex.ToString()));
                PushMessageToConveyor(new ErrorMessage(ex.Message));
            }
        }
    }
}
