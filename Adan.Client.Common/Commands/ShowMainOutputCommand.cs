using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adan.Client.Common.Commands
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
