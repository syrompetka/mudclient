// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the GroupViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel
{
    #region Namespace Imports

    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    #endregion

    /// <summary>
    /// Describes a group of triggers, aliases etc.
    /// </summary>
    public class GroupViewModel : ViewModelBase
    {
        private readonly IEnumerable<ActionDescription> _actionDescriptions;

        #region Constants and Fields

        private readonly ObservableCollection<AliasViewModel> _aliases = new ObservableCollection<AliasViewModel>();
        private readonly ObservableCollection<TriggerViewModel> _triggers = new ObservableCollection<TriggerViewModel>();
        private readonly ObservableCollection<HotkeyViewModel> _hotkeys = new ObservableCollection<HotkeyViewModel>();
        private readonly ObservableCollection<HighlightViewModel> _highlights = new ObservableCollection<HighlightViewModel>();
        private readonly ObservableCollection<SubstitutionViewModel> _substitutions = new ObservableCollection<SubstitutionViewModel>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupViewModel"/> class.
        /// </summary>
        /// <param name="allGroups">All groups.</param>
        /// <param name="group">The group.</param>
        /// <param name="actionDescriptions">The action descriptions.</param>
        public GroupViewModel([NotNull]IEnumerable<GroupViewModel> allGroups, [NotNull] Group group, [NotNull] IEnumerable<ActionDescription> actionDescriptions)
        {
            _actionDescriptions = actionDescriptions;
            Assert.ArgumentNotNull(allGroups, "allGroups");
            Assert.ArgumentNotNull(group, "group");
            Assert.ArgumentNotNull(actionDescriptions, "actionDescriptions");

            Group = group;
            Triggers = new ReadOnlyObservableCollection<TriggerViewModel>(_triggers);
            foreach (var trigger in group.Triggers)
            {
                _triggers.Add(new TriggerViewModel(allGroups, this, (TextTrigger)trigger, _actionDescriptions));
            }

            Aliases = new ReadOnlyObservableCollection<AliasViewModel>(_aliases);
            foreach (var alias in group.Aliases)
            {
                _aliases.Add(new AliasViewModel(allGroups, this, alias, _actionDescriptions));
            }

            Hotkeys = new ReadOnlyObservableCollection<HotkeyViewModel>(_hotkeys);
            foreach (var hotkey in group.Hotkeys)
            {
                _hotkeys.Add(new HotkeyViewModel(allGroups, this, hotkey, _actionDescriptions));
            }

            Highlights = new ReadOnlyObservableCollection<HighlightViewModel>(_highlights);
            foreach (var highlight in group.Highlights)
            {
                _highlights.Add(new HighlightViewModel(allGroups, this, highlight));
            }

            Substitutions = new ReadOnlyObservableCollection<SubstitutionViewModel>(_substitutions);
            foreach (var substitution in group.Substitutions)
            {
                _substitutions.Add(new SubstitutionViewModel(allGroups, this, substitution));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the group model associated with this view model.
        /// </summary>
        [NotNull]
        public Group Group
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the aliases that belong to this group.
        /// </summary>
        [NotNull]
        public ReadOnlyObservableCollection<AliasViewModel> Aliases
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this group is buildin or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if this group is buildin; otherwise, <c>false</c>.
        /// </value>
        public bool IsBuildIn
        {
            get
            {
                return Group.IsBuildIn;
            }

            set
            {
                Group.IsBuildIn = value;
                OnPropertyChanged("IsBuildIn");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this group is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this group is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get
            {
                return Group.IsEnabled;
            }

            set
            {
                Group.IsEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        /// <summary>
        /// Gets or sets the name of this group.
        /// </summary>
        /// <value>
        /// The name of this group.
        /// </value>
        [NotNull]
        public string Name
        {
            get
            {
                return Group.Name;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                Group.Name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets a collection of triggers that belong to this group.
        /// </summary>
        [NotNull]
        public ReadOnlyObservableCollection<TriggerViewModel> Triggers
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the hot keys.
        /// </summary>
        [NotNull]
        public ReadOnlyObservableCollection<HotkeyViewModel> Hotkeys
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the highlights.
        /// </summary>
        [NotNull]
        public ReadOnlyObservableCollection<HighlightViewModel> Highlights
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the substitutions.
        /// </summary>
        [NotNull]
        public ReadOnlyObservableCollection<SubstitutionViewModel> Substitutions
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the alias.
        /// </summary>
        /// <param name="aliasToAdd">The alias to add.</param>
        public void AddAlias([NotNull] AliasViewModel aliasToAdd)
        {
            Assert.ArgumentNotNull(aliasToAdd, "aliasToAdd");

            _aliases.Add(aliasToAdd);
            Group.Aliases.Add(aliasToAdd.CommandAlias);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="aliasToAdd"></param>
        public void InsertAlias(int index, [NotNull] AliasViewModel aliasToAdd)
        {
            Assert.ArgumentNotNull(aliasToAdd, "aliasToAdd");
            CommandAlias alias;
            Group.Aliases.TryTake(out alias);
            Group.Aliases.Add(aliasToAdd.CommandAlias);
            //_aliases.Add(aliasToAdd);
            //Group.Aliases.Insert(index, aliasToAdd.CommandAlias);
            _aliases.Insert(index, aliasToAdd);
        }

        /// <summary>
        /// Removes the alias.
        /// </summary>
        /// <param name="aliasToRemove">The alias to remove.</param>
        public void RemoveAlias([NotNull] AliasViewModel aliasToRemove)
        {
            Assert.ArgumentNotNull(aliasToRemove, "aliasToRemove");

            CommandAlias alias;
            Group.Aliases.TryTake(out alias);
            _aliases.Remove(aliasToRemove);
        }

        /// <summary>
        /// Adds the trigger.
        /// </summary>
        /// <param name="triggerToAdd">The trigger to add.</param>
        public void AddTrigger([NotNull] TriggerViewModel triggerToAdd)
        {
            Assert.ArgumentNotNull(triggerToAdd, "triggerToAdd");

            Group.Triggers.Add(triggerToAdd.Trigger);
            _triggers.Add(triggerToAdd);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="triggerToAdd"></param>
        public void InsertTrigger(int index, [NotNull] TriggerViewModel triggerToAdd)
        {
            Assert.ArgumentNotNull(triggerToAdd, "triggerToAdd");

            TriggerBase trigger;
            Group.Triggers.TryTake(out trigger);
            Group.Triggers.Add(triggerToAdd.Trigger);

            //Group.Triggers.Insert(index, triggerToAdd.Trigger);
            _triggers.Insert(index, triggerToAdd);
        }

        /// <summary>
        /// Removes the trigger.
        /// </summary>
        /// <param name="triggerToRemove">The trigger to remove.</param>
        public void RemoveTrigger([NotNull] TriggerViewModel triggerToRemove)
        {
            Assert.ArgumentNotNull(triggerToRemove, "triggerToRemove");

            TriggerBase trigger;
            Group.Triggers.TryTake(out trigger);
            //Group.Triggers.Remove(triggerToRemove.Trigger);
            _triggers.Remove(triggerToRemove);
        }

        /// <summary>
        /// Adds the hot key.
        /// </summary>
        /// <param name="hotkeyToAdd">The hot key to add.</param>
        public void AddHotkey([NotNull] HotkeyViewModel hotkeyToAdd)
        {
            Assert.ArgumentNotNull(hotkeyToAdd, "hotkeyToAdd");
            Group.Hotkeys.Add(hotkeyToAdd.Hotkey);
            _hotkeys.Add(hotkeyToAdd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="hotkeyToAdd"></param>
        public void InsertHotkey(int index, [NotNull] HotkeyViewModel hotkeyToAdd)
        {
            Assert.ArgumentNotNull(hotkeyToAdd, "hotkeyToAdd");

            Hotkey hotkey;
            Group.Hotkeys.TryTake(out hotkey);
            Group.Hotkeys.Add(hotkeyToAdd.Hotkey);
            //Group.Hotkeys.Insert(index, hotkeyToAdd.Hotkey);
            _hotkeys.Insert(index, hotkeyToAdd);
        }

        /// <summary>
        /// Removes the hot key.
        /// </summary>
        /// <param name="hotkeyToRemove">The hot key to remove.</param>
        public void RemoveHotkey([NotNull] HotkeyViewModel hotkeyToRemove)
        {
            Assert.ArgumentNotNull(hotkeyToRemove, "hotkeyToRemove");
            Hotkey hotkey;
            Group.Hotkeys.TryTake(out hotkey);
            //Group.Hotkeys.Remove(hotkeyToRemove.Hotkey);
            _hotkeys.Remove(hotkeyToRemove);
        }

        /// <summary>
        /// Adds the highlight.
        /// </summary>
        /// <param name="highlightToAdd">The highlight to add.</param>
        public void AddHighlight([NotNull] HighlightViewModel highlightToAdd)
        {
            Assert.ArgumentNotNull(highlightToAdd, "highlightToAdd");
            Group.Highlights.Add(highlightToAdd.Highlight);
            _highlights.Add(highlightToAdd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="highlightToAdd"></param>
        public void InsertHighlight(int index, [NotNull] HighlightViewModel highlightToAdd)
        {
            Assert.ArgumentNotNull(highlightToAdd, "highlightToAdd");
            Highlight highlight;
            Group.Highlights.TryTake(out highlight);
            Group.Highlights.Add(highlightToAdd.Highlight);

            //Group.Highlights.Insert(index, highlightToAdd.Highlight);
            _highlights.Insert(index, highlightToAdd);
        }

        /// <summary>
        /// Removes the highlight.
        /// </summary>
        /// <param name="highlightToRemove">The highlight to remove.</param>
        public void RemoveHighlight([NotNull] HighlightViewModel highlightToRemove)
        {
            Assert.ArgumentNotNull(highlightToRemove, "highlightToRemove");
            Highlight highlight;
            Group.Highlights.TryTake(out highlight);
            //Group.Highlights.Remove(highlightToRemove.Highlight);
            _highlights.Remove(highlightToRemove);
        }

        /// <summary>
        /// Adds the substitution.
        /// </summary>
        /// <param name="substitutionToAdd">The substitution to add.</param>
        public void AddSubstitution([NotNull] SubstitutionViewModel substitutionToAdd)
        {
            Assert.ArgumentNotNull(substitutionToAdd, "substitutionToAdd");
            Group.Substitutions.Add(substitutionToAdd.Substitution);
            _substitutions.Add(substitutionToAdd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="substitutionToAdd"></param>
        public void InsertSubstitution(int index, [NotNull] SubstitutionViewModel substitutionToAdd)
        {
            Assert.ArgumentNotNull(substitutionToAdd, "substitutionToAdd");

            Substitution sub;
            Group.Substitutions.TryTake(out sub);
            Group.Substitutions.Add(substitutionToAdd.Substitution);
            //Group.Substitutions.Insert(index, substitutionToAdd.Substitution);
            _substitutions.Insert(index, substitutionToAdd);
        }

        /// <summary>
        /// Removes the substitution.
        /// </summary>
        /// <param name="substitutionToRemove">The substitution to remove.</param>
        public void RemoveSubstitution([NotNull] SubstitutionViewModel substitutionToRemove)
        {
            Assert.ArgumentNotNull(substitutionToRemove, "substitutionToRemove");

            Substitution sub;
            Group.Substitutions.TryTake(out sub);
            //Group.Substitutions.Remove(substitutionToRemove.Substitution);
            _substitutions.Remove(substitutionToRemove);
        }

        #endregion
    }
}