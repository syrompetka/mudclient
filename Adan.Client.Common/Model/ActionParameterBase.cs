// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionParameterBase.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ActionParameterBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System;

    using CSLib.Net.Annotations;

    /// <summary>
    /// Base class for all action parameters.
    /// </summary>
    [Serializable]
    public abstract class ActionParameterBase
    {
        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// Parameter value.
        /// </returns>
        [NotNull]
        public abstract string GetParameterValue([NotNull]RootModel rootModel, [NotNull] ActionExecutionContext context);
    }
}
