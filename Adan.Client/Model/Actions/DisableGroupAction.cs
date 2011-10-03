// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisableGroupAction.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the DisableGroupAction type.
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
    /// Action that disables a group.
    /// </summary>
    [Serializable]
    public class DisableGroupAction : ActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisableGroupAction"/> class.
        /// </summary>
        public DisableGroupAction()
        {
            GroupNameToDisable = string.Empty;
        }

        /// <summary>
        /// Gets or sets the group to disable.
        /// </summary>
        /// <value>
        /// The group to disable.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string GroupNameToDisable
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

            if (!string.IsNullOrEmpty(GroupNameToDisable))
            {
                model.DisableGroup(GroupNameToDisable);
            }
        }
    }
}
