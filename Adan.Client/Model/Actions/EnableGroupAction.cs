// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnableGroupAction.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the EnableGroupAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.Actions
{
    using System.Runtime.Serialization;
    
    using Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Action that enables certain group.
    /// </summary>
    [DataContract]
    public class EnableGroupAction : ActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnableGroupAction"/> class.
        /// </summary>
        public EnableGroupAction()
        {
            GroupNameToEnable = string.Empty;
        }

        /// <summary>
        /// Gets or sets the group to enable.
        /// </summary>
        /// <value>
        /// The group to enable.
        /// </value>
        [NotNull]
        [DataMember]
        public string GroupNameToEnable
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

            if (!string.IsNullOrEmpty(GroupNameToEnable))
            {
                model.EnableGroup(GroupNameToEnable);
            }
        }
    }
}
