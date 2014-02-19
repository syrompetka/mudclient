// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionBase.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ActionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// Base class for all actions issued by triggers, aliases etc.
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(ActionWithParameters))]
    public abstract class ActionBase
    {
        /// <summary>
        /// Gets a value indicating whether this action can be executed in global scope or not.
        /// </summary>
        public abstract bool IsGlobal
        {
            get;
        }

        /// <summary>
        /// Executes this action.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public abstract void Execute([NotNull]RootModel model, [NotNull]ActionExecutionContext context);
    }
}
