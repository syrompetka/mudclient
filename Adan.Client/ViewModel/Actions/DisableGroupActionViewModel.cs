// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisableGroupActionViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the DisableGroupActionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel.Actions
{
    using System.Collections.Generic;
    using System.Linq;

    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model.Actions;

    /// <summary>
    /// View model for enable group action.
    /// </summary>
    public class DisableGroupActionViewModel : ActionViewModelBase
    {
        private readonly DisableGroupAction _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableGroupActionViewModel"/> class.
        /// </summary>
        /// <param name="availableGroups">The available groups.</param>
        /// <param name="action">The action.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <param name="allDescriptions">All descriptions.</param>
        public DisableGroupActionViewModel([NotNull] IEnumerable<Group> availableGroups, [NotNull] DisableGroupAction action, [NotNull] ActionDescription actionDescriptor, [NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base(action, actionDescriptor, allDescriptions)
        {
            Assert.ArgumentNotNull(availableGroups, "availableGroups");
            Assert.ArgumentNotNull(action, "action");
            Assert.ArgumentNotNull(actionDescriptor, "actionDescriptor");
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");

            _action = action;
            AllGroups = availableGroups.Where(g => !g.IsBuildIn);
        }

        /// <summary>
        /// Gets all groups.
        /// </summary>
        [NotNull]
        public IEnumerable<Group> AllGroups
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the group name to disable.
        /// </summary>
        /// <value>
        /// The group name to disable.
        /// </value>
        [NotNull]
        public string GroupNameToDisable
        {
            get
            {
                return _action.GroupNameToDisable;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _action.GroupNameToDisable = value;
                OnPropertyChanged("GroupNameToDisable");
                OnPropertyChanged("ActionDescription");
            }
        }

        /// <summary>
        /// Gets the action description.
        /// </summary>
        public override string ActionDescription
        {
            get
            {
                return "#DisableGroup " + GroupNameToDisable;
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionViewModelBase Clone()
        {
            return new DisableGroupActionViewModel(AllGroups, new DisableGroupAction(), ActionDescriptor, AllActionDescriptions) { GroupNameToDisable = GroupNameToDisable };
        }
    }
}
