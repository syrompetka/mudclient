// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendTextAction.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the SendTextAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.Actions
{
    using System.Runtime.Serialization;

    using Common.Commands;
    using Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Action that sends text to server.
    /// </summary>
    [DataContract]
    public class SendTextAction : ActionWithParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendTextAction"/> class.
        /// </summary>
        public SendTextAction()
        {
            CommandText = string.Empty;
        }

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        /// <value>
        /// The command text.
        /// </value>
        [NotNull]
        [DataMember]

        public string CommandText
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

            model.PushCommandToConveyor(new TextCommand(PostProcessString(CommandText + GetParametersString(model, context), model, context)));
        }

        #endregion
    }
}
