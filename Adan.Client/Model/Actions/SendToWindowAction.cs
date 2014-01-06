using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adan.Client.Common.Model;

namespace Adan.Client.Model.Actions
{
    /// <summary>
    /// 
    /// </summary>
    public class SendToWindowAction : ActionBase
    {
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
        /// <param name="model"></param>
        /// <param name="context"></param>
        public override void Execute(RootModel model, ActionExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
