// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnableGroupActionViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the EnableGroupActionViewModel type.
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
    public class EnableGroupActionViewModel : ActionViewModelBase
    {
        private readonly EnableGroupAction _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableGroupActionViewModel"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <param name="allDescriptions">All descriptions.</param>
        public EnableGroupActionViewModel([NotNull] EnableGroupAction action, [NotNull] ActionDescription actionDescriptor, [NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base(action, actionDescriptor, allDescriptions)
        {
            Assert.ArgumentNotNull(action, "action");
            Assert.ArgumentNotNull(actionDescriptor, "actionDescriptor");
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");

            _action = action;
            //AllGroups = availableGroups.Where(g => !g.IsBuildIn);
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
        /// Gets or sets the group name to enable.
        /// </summary>
        /// <value>
        /// The group name to enable.
        /// </value>
        [NotNull]
        public string GroupNameToEnable
        {
            get
            {
                return _action.GroupNameToEnable;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _action.GroupNameToEnable = value;
                OnPropertyChanged("GroupNameToEnable");
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
                return "#EnableGroup " + GroupNameToEnable;
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionViewModelBase Clone()
        {
            return new EnableGroupActionViewModel(new EnableGroupAction(), ActionDescriptor, AllActionDescriptions) { GroupNameToEnable = GroupNameToEnable };
        }
    }
}
