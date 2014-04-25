// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerOrCommandParameter.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TriggerOrCommandParameter type.
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
    /// Parameter that references trigger or command parameter.
    /// </summary>
    [Serializable]
    public class TriggerOrCommandParameter : ActionParameterBase
    {
        /// <summary>
        /// Gets or sets the parameter number.
        /// </summary>
        /// <value>
        /// The parameter number.
        /// </value>
        [XmlAttribute]
        public int ParameterNumber
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

            if (ParameterNumber >= 0 && ParameterNumber < 10 && ParameterNumber < context.Parameters.Count)
            {
                return context.Parameters[ParameterNumber];
            }

            return string.Empty;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetParameterValue()
        {
            return "%" + ParameterNumber;
        }
    }
}
