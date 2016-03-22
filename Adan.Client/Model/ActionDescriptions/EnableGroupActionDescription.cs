// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnableGroupActionDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the EnableGroupActionDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.ActionDescriptions
{
    using System.Collections.Generic;

    using Actions;

    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel.Actions;

    /// <summary>
    /// A <see cref="ActionDescription"/> implementation for <see cref="EnableGroupAction"/>.
    /// </summary>
    public class EnableGroupActionDescription : ActionDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnableGroupActionDescription"/> class.
        /// </summary>
        /// <param name="allDescriptions">All descriptions.</param>
        public EnableGroupActionDescription([NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base("Enable group", allDescriptions)
        {
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");
        }

        /// <summary>
        /// Creates the <see cref="ActionBase"/> derived class instace.
        /// </summary>
        /// <returns>
        /// The instance of <see cref="ActionBase"/> derived class.
        /// </returns>
        public override ActionBase CreateAction()
        {
            return new EnableGroupAction();
        }

        /// <summary>
        /// Creates the action view model by specified <see cref="ActionBase"/> instance.
        /// </summary>
        /// <param name="action">The <see cref="ActionBase"/> instance to create view model for.</param>
        /// <returns>
        /// Created action view model or <c>null</c> if specified action is not supported by this description.
        /// </returns>
        public override ActionViewModelBase CreateActionViewModel(ActionBase action)
        {
            Assert.ArgumentNotNull(action, "action");

            var enableGroupAction = action as EnableGroupAction;
            if (enableGroupAction != null)
            {
                return new EnableGroupActionViewModel(enableGroupAction, this, AllDescriptions);
            }

            return null;
        }
    }
}
