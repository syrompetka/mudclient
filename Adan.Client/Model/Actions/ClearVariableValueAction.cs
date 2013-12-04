// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearVariableValueAction.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ClearVariableValueAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.Actions
{
    using System;
    using System.Xml.Serialization;

    using Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Action that clears value of some variable.
    /// </summary>
    [Serializable]
    public class ClearVariableValueAction : ActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClearVariableValueAction"/> class.
        /// </summary>
        public ClearVariableValueAction()
        {
            VariableName = string.Empty;
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
        /// Gets or sets Silent mode
        /// </summary>
        [NotNull]
        [XmlAttribute]
        public bool SilentSet
        {
            get;
            set;
        }

        /// <summary>
        /// Executes this action.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public override void Execute(RootModel model, ActionExecutionContext context)
        {
            Assert.ArgumentNotNull(model, "model");
            Assert.ArgumentNotNull(context, "context");
            if (!string.IsNullOrEmpty(VariableName))
            {
                model.ClearVariableValue(VariableName, SilentSet);
            }
        }
    }
}
