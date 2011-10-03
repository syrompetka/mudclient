// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetVariableValueAction.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the SetVariableValueAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.Actions
{
    using System;
    using System.Xml.Serialization;

    using ActionParameters;

    using Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Action that set value of some variable.
    /// </summary>
    [Serializable]
    public class SetVariableValueAction : ActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetVariableValueAction"/> class.
        /// </summary>
        public SetVariableValueAction()
        {
            VariableName = string.Empty;
            ValueToSet = new TriggerOrCommandParameter();
        }

        /// <summary>
        /// Gets or sets the name of the variable.
        /// </summary>
        /// <value>
        /// The name of the variable.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string VariableName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value to set.
        /// </summary>
        /// <value>
        /// The value to set.
        /// </value>
        [NotNull]
        public ActionParameterBase ValueToSet
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

            model.SetVariableValue(VariableName, ValueToSet.GetParameterValue(model, context));
        }

        #endregion
    }
}
