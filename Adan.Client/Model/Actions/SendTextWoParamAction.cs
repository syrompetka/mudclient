using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Adan.Client.Common.Commands;
using Adan.Client.Common.Model;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.Model.Actions
{
    /// <summary>
    /// Action that sends text to server without checking variables etc.
    /// </summary>
    public class SendTextWoParamAction : SendTextAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendTextWoParamAction"/> class.
        /// </summary>
        public SendTextWoParamAction()
            : base() { }

        /// <summary>
        /// Executes this action.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public override void Execute([NotNull]RootModel model, [NotNull]ActionExecutionContext context)
        {
            Assert.ArgumentNotNull(model, "model");
            Assert.ArgumentNotNull(context, "context");

            StringBuilder sb = new StringBuilder(CommandText);

            foreach(KeyValuePair<int, string> kvp in context.Parameters)
                sb.Append(kvp.Value);

            //model.PushCommandToConveyor(new TextCommand(ReplaceVariables(sb.ToString(), model)));
            model.PushCommandToConveyor(new TextCommand(sb.ToString()));
        }
    }
}
