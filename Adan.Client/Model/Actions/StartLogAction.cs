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
    using System.Runtime.Serialization;

    using ActionParameters;

    using Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Actions that starts logging to file.
    /// </summary>
    [DataContract]
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
        /// Gets or sets the log name parameter.
        /// </summary>
        /// <value>
        /// The log name parameter.
        /// </value>
        [NotNull]
        [DataMember]
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

            model.PushMessageToConveyor(new Messages.StartLoggingMessage(LogNameParameter.GetParameterValue(model, context)));
        }

        #endregion
    }
}
