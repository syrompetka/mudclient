// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextMessageDeserializer.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TextMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.MessageDeserializers
{
    #region Namespace Imports

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Common.Conveyor;
    using Common.MessageDeserializers;
    using Common.Messages;
    using Common.Themes;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;

    #endregion

    /// <summary>
    /// Plain text message processor.
    /// </summary>
    public class TextMessageDeserializerEx : MessageDeserializer
    {
        #region Constants and Fields
        private readonly char[] _charBuffer = new char[32767];

        private readonly Encoding _encoding = Encoding.GetEncoding(1251);
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        #endregion

        #region Properties

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
                return BuiltInMessageTypes.TextMessage;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Processes the data from server.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="bytesReceived">The bytes received.</param>
        /// <param name="data">The get data.</param>
        /// <param name="isComplete">Indicates whether message should be completed or wait for next data.</param>
        public override void DeserializeDataFromServer(int offset, int bytesReceived, [NotNull] byte[] data, bool isComplete)
        {
            Assert.ArgumentNotNull(data, "data");

#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            int currentDataBufferPosition = 0;

            var incomingCharsCount = _encoding.GetChars(data, offset, bytesReceived, _charBuffer, 0);

            while (currentDataBufferPosition < incomingCharsCount)
            {
                char currentChar = _charBuffer[currentDataBufferPosition];

                if (currentChar == '\n')
                {
                    FlushCurrentText();
                }
                else if (currentChar != '\r')
                {
                    _stringBuilder.Append(currentChar);
                }

                currentDataBufferPosition++;
            }

#if DEBUG
            sw.Stop();
            //PushMessageToConveyor(new InfoMessage(string.Format("TextMessageDeserializer = {0} ms", sw.ElapsedMilliseconds)));
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MessageDeserializer NewInstance()
        {
            return new TextMessageDeserializerEx();
        }

        #endregion

        #region Methods

        private void FlushCurrentText()
        {
            if (_stringBuilder.Length > 0)
            {
                PushMessageToConveyor(new OutputToMainWindowMessage(_stringBuilder.ToString(), true));
                _stringBuilder.Clear();
            }
        }

        #endregion
    }
}