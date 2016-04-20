using Adan.Client.Common.Messages;

namespace Adan.Client.Messages
{
    public sealed class ToggleFullScreenModeMessage : Message
    {
        public override int MessageType
        {
            get { return BuiltInMessageTypes.ToggleFullScreenModeMessage; }
        }
    }
}
