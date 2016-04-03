namespace Adan.Client.Common.Commands
{
    public sealed class FlushOutputQueueCommand : Command
    {
        private static readonly FlushOutputQueueCommand _instance = new FlushOutputQueueCommand();
        private FlushOutputQueueCommand()
        {
        }

        public override int CommandType
        {
            get
            {
                return BuiltInCommandTypes.FlushOutputQueue;
            }
        }

        public static FlushOutputQueueCommand Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
