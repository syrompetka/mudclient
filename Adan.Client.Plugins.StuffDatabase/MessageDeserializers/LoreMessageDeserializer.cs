// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoreMessageDeserializer.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the LoreMessageDeserializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase.MessageDeserializers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using System.Diagnostics;

    using Common.Conveyor;
    using Common.MessageDeserializers;
    using Common.Messages;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Adan.Client.Plugins.StuffDatabase.Messages;
    using Adan.Client.Common.Utils;

    /// <summary>
    /// <see cref="MessageDeserializer"/> implementation to handle lore messages.
    /// </summary>
    public class LoreMessageDeserializer : MessageDeserializer
    {
        private readonly StringBuilder _builder = new StringBuilder();

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

            var messageXml = Encoding.GetEncoding(1251).GetString(data, offset, bytesReceived);
            _builder.Append(messageXml);
            if (isComplete)
            {
                string str = _builder.ToString();
                _builder.Clear();

                try
                {
                    using (var stringReader = new StringReader(str))
                    {
                        var serializer = new XmlSerializer(typeof(LoreMessage));
                        var message = (LoreMessage)serializer.Deserialize(stringReader);
                        PushMessageToConveyor(message);
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.Instance.Write(string.Format("Error deserialize lore message: {0}\r\n{1}\r\n{2}", str, ex.Message, ex.StackTrace));
                    PushMessageToConveyor(new ErrorMessage(ex.Message));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MessageDeserializer NewInstance()
        {
            return new LoreMessageDeserializer();
        }
    }
}
