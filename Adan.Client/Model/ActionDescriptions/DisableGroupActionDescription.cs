// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisableGroupActionDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the DisableGroupActionDescription type.
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
    /// A <see cref="ActionDescription"/> implementation for <see cref="DisableGroupAction"/>.
    /// </summary>
    public class DisableGroupActionDescription : ActionDescription
    {
        //private readonly IEnumerable<Group> _allGroups;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableGroupActionDescription"/> class.
        /// </summary>
        /// <param name="allDescriptions">All descriptions.</param>
        public DisableGroupActionDescription([NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base("Disable group", allDescriptions)
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
            return new DisableGroupAction();
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

            var disableGroupAction = action as DisableGroupAction;
            if (disableGroupAction != null)
            {
                return new DisableGroupActionViewModel(disableGroupAction, this, AllDescriptions);
            }

            return null;
        }
    }
}
