using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adan.Client.Common.Commands;

namespace Adan.Client.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class SendToWindowCommand : Command
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputWindowName"></param>
        /// <param name="command"></param>
        /// <param name="toAll"></param>
        public SendToWindowCommand(string outputWindowName, Command command, bool toAll)
        {
            OutputWindowName = outputWindowName;
            Command = command;
            ToAll = toAll;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string OutputWindowName
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Command Command
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ToAll
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public override int CommandType
        {
            get
            {
                return BuiltInCommandTypes.SendToWindow;
            }
        }
    }
}
