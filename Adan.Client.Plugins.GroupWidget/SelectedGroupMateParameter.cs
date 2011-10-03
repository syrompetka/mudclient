// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectedGroupMateParameter.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the SelectedGroupMateParameter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget
{
    using System;

    using Common.Model;

    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A parameter that return a name of selected group mate.
    /// </summary>
    [Serializable]
    public class SelectedGroupMateParameter : ActionParameterBase
    {
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

            return rootModel.SelectedGroupMate != null ? rootModel.SelectedGroupMate.Name : string.Empty;
        }
    }
}
