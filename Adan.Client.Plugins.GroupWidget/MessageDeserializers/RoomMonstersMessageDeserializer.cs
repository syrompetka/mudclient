namespace Adan.Client.Plugins.GroupWidget.MessageDeserializers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Common.Conveyor;
    using Common.MessageDeserializers;
    using Common.Messages;
    using CSLib.Net.Diagnostics;
    using Common.Utils;
    using Messages;

    /// <summary>
    /// <see cref="MessageDeserializer"/> to deserializer <see cref="RoomMonstersMessage"/> messages.
    /// </summary>
    public class RoomMonstersMessageDeserializer : MessageDeserializer
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly Encoding _encoding = Encoding.GetEncoding(1251);
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(RoomMonstersMessage));

        public RoomMonstersMessageDeserializer(MessageConveyor conveyor) : base(conveyor)
        {
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
                string str = _builder.ToString();
                _builder.Clear();

                try
                {
                    using (var stringReader = new StringReader(str))
                    {
                        var message = (RoomMonstersMessage)_serializer.Deserialize(stringReader);
                        PushMessageToConveyor(message);
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.Instance.Write(string.Format("Error deserialize mosters message: {0}\r\n{1}\r\n{2}", str, ex.Message, ex.StackTrace));
                    _builder.Clear();
                    PushMessageToConveyor(new ErrorMessage(ex.Message));
                }
            }
        }
    }
}
