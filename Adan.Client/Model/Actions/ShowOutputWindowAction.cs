using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adan.Client.Commands;
using Adan.Client.Common.Commands;
using Adan.Client.Common.Model;

namespace Adan.Client.Model.Actions
{
    /// <summary>
    /// 
    /// </summary>
    public class ShowOutputWindowAction : ActionBase
    {
        /// <summary>
        /// 
        /// </summary>
        public ShowOutputWindowAction()
        {
            OutputWindowName = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsGlobal
        {
            get
            {
                return false;
            }
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
        /// <param name="model"></param>
        /// <param name="context"></param>
        public override void Execute(RootModel model, ActionExecutionContext context)
        {
            model.PushCommandToConveyor(new ShowMainOutputCommand(OutputWindowName));
        }
    }
}
