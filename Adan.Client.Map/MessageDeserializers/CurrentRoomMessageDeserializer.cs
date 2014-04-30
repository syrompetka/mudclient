// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentRoomMessageDeserializer.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the CurrentRoomMessageDeserializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.MessageDeserializers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Adan.Client.Common.Utils;
    using Common.Conveyor;
    using Common.MessageDeserializers;
    using Common.Messages;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Adan.Client.Map.Messages;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Threading;

    /// <summary>
    /// <see cref="MessageDeserializer"/> implementation to handle "current room" messages.
    /// </summary>
    public class CurrentRoomMessageDeserializer : MessageDeserializer
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly Encoding _encoding = Encoding.GetEncoding(1251);
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(CurrentRoomMessage));

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

            var messageXml = _encoding.GetString(data, offset, bytesReceived);
            _builder.Append(messageXml);

            if (isComplete)
            {
                string str = _builder.ToString();
                _builder.Clear();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        using (var stringReader = new StringReader(str))
                        {
                            var message = (CurrentRoomMessage)_serializer.Deserialize(stringReader);
                            PushMessageToConveyor(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Instance.Write(string.Format("Error deserialize room message: {0}\r\nException: {1}\r\n{2}", str, ex.Message, ex.StackTrace));
                        PushMessageToConveyor(new ErrorMessage(ex.Message));
                    }
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MessageDeserializer NewInstance()
        {
            return new CurrentRoomMessageDeserializer();
        }
    }
}
