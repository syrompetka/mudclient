using Adan.Client.Common.Commands;

namespace Adan.Client.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class ShowMainOutputCommand : Command
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputWindowName"></param>
        public ShowMainOutputCommand(string outputWindowName)
        {
            OutputWindowName = outputWindowName;
        }

        /// <summary>
        /// 
        /// </summary>
        public string OutputWindowName
        {
            get;
            set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override int CommandType
        {
            get 
            {
                return BuiltInCommandTypes.ShowMainOutputCommand;
            }        
        }
    }
}
