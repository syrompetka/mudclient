// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtocolVersionMessageDeserializer.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ProtocolVersionMessageDeserializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.MessageDeserializers
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

    using Messages;

    /// <summary>
    /// <see cref="MessageDeserializer"/> implementation that handles "Protocol version" messages.
    /// </summary>
    public class ProtocolVersionMessageDeserializer : MessageDeserializer
    {
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(ProtocolVersionMessage));
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly Encoding _encoding = Encoding.GetEncoding(1251);

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
                return BuiltInMessageTypes.ProtocolVersionMessage;
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

            var messageXml = _encoding.GetString(data, offset, bytesReceived);
            _builder.Append(messageXml);
            if (isComplete)
            {
                using (var stringReader = new StringReader(_builder.ToString()))
                {
                    var message = (ProtocolVersionMessage)_serializer.Deserialize(stringReader);
                    PushMessageToConveyor(message);
                }

                _builder.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MessageDeserializer NewInstance()
        {
            return new ProtocolVersionMessageDeserializer();
        }
    }
}
