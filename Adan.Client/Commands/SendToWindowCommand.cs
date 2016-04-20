using System.Collections.Generic;
using Adan.Client.Common.Commands;
using Adan.Client.Common.Model;

namespace Adan.Client.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class SendToWindowCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendToWindowCommand"/> class.
        /// </summary>
        /// <param name="outputWindowName">Name of the output window.</param>
        /// <param name="actionsToExecute">The actions to execute.</param>
        /// <param name="toAll">if set to <c>true</c> [to all].</param>
        /// <param name="context"></param>
        public SendToWindowCommand(string outputWindowName, IEnumerable<ActionBase> actionsToExecute, bool toAll, ActionExecutionContext context)
        {
            OutputWindowName = outputWindowName;
            ActionsToExecute = actionsToExecute;
            ToAll = toAll;
            ActionExecutionContext = context;
        }

        /// <summary>
        /// Gets the name of the output window.
        /// </summary>
        public string OutputWindowName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the actions to execute in specified window or windows.
        /// </summary>
        public IEnumerable<ActionBase> ActionsToExecute
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
        /// Gets the execution context that initially caused this command.
        /// </summary>
        public ActionExecutionContext ActionExecutionContext
        {
            get; private set;
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
