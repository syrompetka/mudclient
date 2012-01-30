// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoreMessageDeserializer.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the LoreMessageDeserializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase
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
    /// <see cref="MessageDeserializer"/> implementation to handle lore messages.
    /// </summary>
    public class LoreMessageDeserializer : MessageDeserializer
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly Encoding _encoding = Encoding.GetEncoding(1251);
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(LoreMessage));

        /// <summary>
        /// Initializes a new instance of the <see cref="LoreMessageDeserializer"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        public LoreMessageDeserializer([NotNull] MessageConveyor messageConveyor)
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
                return Constants.LoreMessageType;
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
                        var message = (LoreMessage)_serializer.Deserialize(stringReader);
                        PushMessageToConveyor(message);
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
