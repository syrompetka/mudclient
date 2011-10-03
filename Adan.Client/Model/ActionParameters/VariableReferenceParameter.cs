// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VariableReferenceParameter.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the VariableReferenceParameter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.ActionParameters
{
    using System;
    using System.Xml.Serialization;

    using Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Parameter value of which equals to some variable.
    /// </summary>
    [Serializable]
    public class VariableReferenceParameter : ActionParameterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableReferenceParameter"/> class.
        /// </summary>
        public VariableReferenceParameter()
        {
            VariableName = string.Empty;
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

        #region Overrides of ActionParameterBase

        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// Parameter value.
        /// </returns>
        public override string GetParameterValue(RootModel rootModel, ActionExecutionContext context)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");
            Assert.ArgumentNotNull(context, "context");

            return rootModel.GetVariableValue(VariableName);
        }

        #endregion
    }
}
