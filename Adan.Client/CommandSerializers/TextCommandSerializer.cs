namespace Adan.Client.CommandSerializers
{
    #region Namespace Imports

    using System.Text;

    using Common.Commands;
    using Common.CommandSerializers;
    using Common.Conveyor;
    using CSLib.Net.Diagnostics;

    #endregion

    /// <summary>
    /// <see cref="CommandSerializer"/> implementation responcible for serializing text commands.
    /// </summary>
    public class TextCommandSerializer : CommandSerializer
    {
        #region Constants and Fields

        private readonly byte[] _buffer = new byte[32767];
        private int _currentBufferPosition = 0;
        private readonly Encoding _encoding = Encoding.GetEncoding(1251);

        #endregion

        #region Constructors and Destructors
        public TextCommandSerializer(MessageConveyor conveyor) : base(conveyor)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes the and sends specified command (if possible).
        /// </summary>
        /// <param name="command">The command.</param>
        public override void SerializeAndSendCommand(Command command)
        {
            Assert.ArgumentNotNull(command, "command");

            var textCommand = command as TextCommand;
            if (textCommand != null)
            {
                textCommand.Handled = true;
                _currentBufferPosition += _encoding.GetBytes(textCommand.CommandText, 0, textCommand.CommandText.Length, _buffer, _currentBufferPosition);
                _buffer[_currentBufferPosition] = 0x0A;
                _currentBufferPosition++;
                return;
            }

            var flushCommand = command as FlushOutputQueueCommand;
            if ((flushCommand != null && _currentBufferPosition > 0) || (_currentBufferPosition > 200))
            {
                SendRawDataToServer(0, _currentBufferPosition, _buffer);
                _currentBufferPosition = 0;
            }
        }
        #endregion

    }
}