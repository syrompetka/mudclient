// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionViewModelBase.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ActionsViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.ViewModel
{
    using System.Collections.Generic;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model;

    using Plugins;

    /// <summary>
    /// Delegate declaration for <see cref="ActionViewModelBase.ActionTypeChanged"/> event.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="newActionDescription">The new action description.</param>
    public delegate void ActionTypeChangedEventHandler([NotNull]ActionViewModelBase action, ActionDescription newActionDescription);

    /// <summary>
    /// Base class for action view models.
    /// </summary>
    public abstract class ActionViewModelBase : ViewModelBase
    {
        private readonly ActionDescription _actionDescriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionViewModelBase"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <param name="allActionDescriptions">All action descriptions.</param>
        protected ActionViewModelBase([NotNull] ActionBase action, [NotNull] ActionDescription actionDescriptor, [NotNull] IEnumerable<ActionDescription> allActionDescriptions)
        {
            Assert.ArgumentNotNull(action, "action");
            Assert.ArgumentNotNull(actionDescriptor, "actionDescriptor");
            Assert.ArgumentNotNull(allActionDescriptions, "allActionDescriptions");

            _actionDescriptor = actionDescriptor;

            Action = action;
            AllActionDescriptions = allActionDescriptions;
        }

        /// <summary>
        /// Occurs when type of this action is changed.
        /// </summary>
        public event ActionTypeChangedEventHandler ActionTypeChanged;

        /// <summary>
        /// Gets or sets the action description.
        /// </summary>
        /// <value>
        /// The action descriptor.
        /// </value>
        [NotNull]
        public ActionDescription ActionDescriptor
        {
            get
            {
                return _actionDescriptor;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                if (ActionTypeChanged != null)
                {
                    ActionTypeChanged(this, value);
                }
            }
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        [NotNull]
        public ActionBase Action
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all action descriptions available.
        /// </summary>
        [NotNull]
        public IEnumerable<ActionDescription> AllActionDescriptions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the action description.
        /// </summary>
        [NotNull]
        public abstract string ActionDescription
        {
            get;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        [NotNull]
        public abstract ActionViewModelBase Clone();
    }
}
