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
        /// 
        /// </summary>
        /// <param name="zoneManager"></param>
        public CurrentRoomMessageDeserializer([NotNull] ZoneManager zoneManager)
        {
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
                            PushMessageToConveyor(message);                            
                        }

                    _builder.Clear();
                }
            }
            catch (Exception ex)
            {
                var deseirilizer = MessageConveyor.MessageDeserializers.FirstOrDefault(x => x.DeserializedMessageType == BuiltInMessageTypes.TextMessage);
                string str = FakeXmlParser.Parse(_builder.ToString().Replace("**OVERFLOW**", ""));
                byte[] buf = _encoding.GetBytes(str);
                deseirilizer.DeserializeDataFromServer(0, buf.Length, buf, true);
                PushMessageToConveyor(new OutputToMainWindowMessage(str));
                _builder.Clear();
                //PushMessageToConveyor(new ErrorMessage(ex.ToString()));
                PushMessageToConveyor(new ErrorMessage(ex.Message));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MessageDeserializer NewInstance()
        {
            return new CurrentRoomMessageDeserializer(_zoneManager);
        }
    }
}
