namespace Adan.Client.Common.MessageDeserializers
{
    using Conveyor;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Messages;

    /// <summary>
    /// Base class for all message processors.
    /// </summary>
    public abstract class MessageDeserializer
    {
        #region Constants and Fields

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type of deserialized message.
        /// </summary>
        /// <value>
        /// The type of deserialized message.
        /// </value>
        public abstract int DeserializedMessageType
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public MessageConveyor Conveyor
        {
            get;
            set;
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
        public abstract void DeserializeDataFromServer(int offset, int bytesReceived, [NotNull] byte[] data, bool isComplete);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract MessageDeserializer NewInstance();

        #endregion

        #region Methods

        /// <summary>
        /// Pushes the message to conveyor.
        /// </summary>
        /// <param name="message">The message to push to conveyor.</param>
        protected void PushMessageToConveyor([NotNull] Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            Conveyor.PushMessage(message);
        }

        #endregion
    }
}