// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TriggerViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel
{
    #region Namespace Imports

    using System.Collections.Generic;

    using Actions;

    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    #endregion

    /// <summary>
    /// View model for trigger editor.
    /// </summary>
    public class TriggerViewModel : ViewModelBase
    {
        #region Constants and Fields

        private readonly IEnumerable<ActionDescription> _actionDescriptions;
        private GroupViewModel _triggersGroup;
        
        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerViewModel"/> class.
        /// </summary>
        /// <param name="allGroups">All groups.</param>
        /// <param name="triggersGroup">The triggers group.</param>
        /// <param name="trigger">The trigger.</param>
        /// <param name="actionDescriptions">The action descriptions.</param>
        public TriggerViewModel([NotNull] IEnumerable<GroupViewModel> allGroups, [NotNull]GroupViewModel triggersGroup, [NotNull] TextTrigger trigger, [NotNull] IEnumerable<ActionDescription> actionDescriptions)
        {
            Assert.ArgumentNotNull(allGroups, "allGroups");
            Assert.ArgumentNotNull(triggersGroup, "triggersGroup");
            Assert.ArgumentNotNull(trigger, "trigger");
            Assert.ArgumentNotNull(actionDescriptions, "actionDescriptions");

            AllGroups = allGroups;
            _triggersGroup = triggersGroup;
            _actionDescriptions = actionDescriptions;
            Trigger = trigger;
            ActionsViewModel = new ActionsViewModel(trigger.Actions, _actionDescriptions);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the trigger.
        /// </summary>
        [NotNull]
        public TextTrigger Trigger
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all groups.
        /// </summary>
        [NotNull]
        public IEnumerable<GroupViewModel> AllGroups
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public int Priority
        {
            get
            {
                return Trigger.Priority;
            }

            set
            {
                Trigger.Priority = value;
                OnPropertyChanged("Priority");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether message should not be processed against triggers or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if message should not be processed against triggers; otherwise, <c>false</c>.
        /// </value>
        public bool StopProcessingTriggersForMessage
        {
            get
            {
                return Trigger.StopProcessingTriggersAfterThis;
            }

            set
            {
                Trigger.StopProcessingTriggersAfterThis = value;
                OnPropertyChanged("StopProcessingTriggersForMessage");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to hide message that caused this trigger or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if to hide original message; otherwise, <c>false</c>.
        /// </value>
        public bool DoNotDisplayOriginalMessage
        {
            get
            {
                return Trigger.DoNotDisplayOriginalMessage;
            }

            set
            {
                Trigger.DoNotDisplayOriginalMessage = value;
                OnPropertyChanged("DoNotDisplayOriginalMessage");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsRegExp
        {
            get
            {
                return Trigger.IsRegExp;
            }
            set
            {
                Trigger.IsRegExp = value;
                OnPropertyChanged("IsRegExp");
            }
        }

        /// <summary>
        /// Gets or sets the triggers group.
        /// </summary>
        /// <value>
        /// The triggers group.
        /// </value>
        [NotNull]
        public GroupViewModel TriggersGroup
        {
            get
            {
                return _triggersGroup;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _triggersGroup = value;
                OnPropertyChanged("TriggersGroup");
            }
        }

        /// <summary>
        /// Gets or sets the matching pattern.
        /// </summary>
        /// <value>
        /// The matching pattern.
        /// </value>
        [NotNull]
        public string MatchingPattern
        {
            get
            {
                return Trigger.MatchingPattern;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                Trigger.MatchingPattern = value;
                OnPropertyChanged("MatchingPattern");
            }
        }

        /// <summary>
        /// Gets the trigger description.
        /// </summary>
        [NotNull]
        public string TriggerDescription
        {
            get
            {
                return MatchingPattern;
            }
        }

        /// <summary>
        /// Gets the action view model.
        /// </summary>
        [NotNull]
        public ActionsViewModel ActionsViewModel
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        [NotNull]
        public TriggerViewModel Clone()
        {
            var textTrigger = new TextTrigger();
            return new TriggerViewModel(AllGroups, TriggersGroup, textTrigger, _actionDescriptions)
            {
                IsRegExp = IsRegExp,
                Priority = Priority,
                DoNotDisplayOriginalMessage = DoNotDisplayOriginalMessage,
                MatchingPattern = MatchingPattern,
                StopProcessingTriggersForMessage = StopProcessingTriggersForMessage,
                TriggersGroup = TriggersGroup,
                ActionsViewModel = ActionsViewModel.Clone(textTrigger.Actions)
            };
        }
    }
}