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
    public class SendTextOneParameterAction : ActionWithOneParameterOrDefault
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="SendTextAction"/> class.
        /// </summary>
        public SendTextOneParameterAction()
        {
            CommandText = string.Empty;
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
        /// Gets or sets the command text.
        /// </summary>
        /// <value>
        /// The command text.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string CommandText
        {
            get;
            set;
        }

        /// <summary>
        /// Executes this action.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public override void Execute([NotNull]RootModel model, [NotNull]ActionExecutionContext context)
        {
            Assert.ArgumentNotNull(model, "model");
            Assert.ArgumentNotNull(context, "context");

            model.PushCommandToConveyor(new TextCommand(PostProcessString(CommandText + GetParametersString(model, context), model, context)));
        }
    }
}
