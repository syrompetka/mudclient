// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ActionDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Plugins
{
    using System.Collections.Generic;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model;

    using ViewModel;

    /// <summary>
    /// A description of action type.
    /// </summary>
    public abstract class ActionDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionDescription"/> class.
        /// </summary>
        /// <param name="displayName">The display name of the action type.</param>
        /// <param name="allDescriptions">All descriptions.</param>
        protected ActionDescription([NotNull] string displayName, [NotNull] IEnumerable<ActionDescription> allDescriptions)
        {
            Assert.ArgumentNotNullOrWhiteSpace(displayName, "displayName");
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");

            DisplayName = displayName;
            AllDescriptions = allDescriptions;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        [NotNull]
        public string DisplayName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all action descriptions available.
        /// </summary>
        [NotNull]
        public IEnumerable<ActionDescription> AllDescriptions
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates the <see cref="ActionBase"/> derived class instace.
        /// </summary>
        /// <returns>
        /// The instance of <see cref="ActionBase"/> derived class.
        /// </returns>
        [NotNull]
        public abstract ActionBase CreateAction();

        /// <summary>
        /// Creates the action view model by specified <see cref="ActionBase"/> instance.
        /// </summary>
        /// <param name="action">The <see cref="ActionBase"/> instance to create view model for.</param>
        /// <returns>Created action view model or <c>null</c> if specified action is not supported by this description.</returns>
        [CanBeNull]
        public abstract ActionViewModelBase CreateActionViewModel([NotNull] ActionBase action);
    }
}
