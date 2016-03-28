namespace Adan.Client.Messages
{
    using Common.Messages;

    public sealed class ShowOutputWindowMessage : Message
    {
        public ShowOutputWindowMessage(string windowName)
        {
            WindowName = windowName;
        }

        public override int MessageType
        {
            get { return BuiltInMessageTypes.ToggleFullScreenModeMessage; }
        }

        public string WindowName { get; private set; }
    }
}
