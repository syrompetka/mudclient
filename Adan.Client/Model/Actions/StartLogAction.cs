// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartLogAction.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the StartLogAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.Actions
{
    using System;
    using ActionParameters;
    using Adan.Client.Messages;
    using Adan.Client.Common.Model;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using System.Text;

    /// <summary>
    /// Actions that starts logging to file.
    /// </summary>
    [Serializable]
    public class StartLogAction : ActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartLogAction"/> class.
        /// </summary>
        public StartLogAction()
        {
            LogNameParameter = new TriggerOrCommandParameter();
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
        /// Gets or sets the log name parameter.
        /// </summary>
        /// <value>
        /// The log name parameter.
        /// </value>
        [NotNull]
        public ActionParameterBase LogNameParameter
        {
            get;
            set;
        }

        #region Overrides of ActionBase

        /// <summary>
        /// Executes this action.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public override void Execute(RootModel model, ActionExecutionContext context)
        {
            Assert.ArgumentNotNull(model, "model");
            Assert.ArgumentNotNull(context, "context");

            model.PushMessageToConveyor(new StartLoggingMessage(LogNameParameter.GetParameterValue(model, context)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#startlog {").Append(LogNameParameter.GetParameterValue()).Append("}");

            return sb.ToString();
        }

        #endregion
    }
}
