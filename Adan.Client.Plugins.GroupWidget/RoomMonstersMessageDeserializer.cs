// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomMonstersMessageDeserializer.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the RoomMonstersMessageDeserializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget
{
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    using Common.Conveyor;
    using Common.MessageDeserializers;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// <see cref="MessageDeserializer"/> to deserializer <see cref="RoomMonstersMessage"/> messages.
    /// </summary>
    public class RoomMonstersMessageDeserializer : MessageDeserializer
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly Encoding _encoding = Encoding.GetEncoding(1251);
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(RoomMonstersMessage));

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomMonstersMessageDeserializer"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        public RoomMonstersMessageDeserializer([NotNull] MessageConveyor messageConveyor)
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
                return Constants.RoomMonstersMessage;
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
                    var message = (RoomMonstersMessage)_serializer.Deserialize(stringReader);
                    PushMessageToConveyor(message);
                }

                _builder.Clear();
            }
        }
    }
}
