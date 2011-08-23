// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstantStringParameter.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ConstantStringParameter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.ActionParameters
{
    using System.Runtime.Serialization;

    using Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A constant string parameter.
    /// </summary>
    [DataContract]
    public class ConstantStringParameter : ActionParameterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantStringParameter"/> class.
        /// </summary>
        public ConstantStringParameter()
        {
            ConstantString = string.Empty;
        }

        /// <summary>
        /// Gets or sets the constant string.
        /// </summary>
        /// <value>
        /// The constant string.
        /// </value>
        [NotNull]
        [DataMember]
        public string ConstantString
        {
            get;
            set;
        }

        #region Overrides of ActionParameterBase

        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <param name="rootModel">The root Model.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// Parameter value.
        /// </returns>
        public override string GetParameterValue([NotNull]RootModel rootModel, ActionExecutionContext context)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");
            Assert.ArgumentNotNull(context, "context");

            return ConstantString;
        }

        #endregion
    }
}
