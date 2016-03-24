// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotkeyViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the HotkeyViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel
{
    using System.Collections.Generic;
    using System.Windows.Input;

    using Actions;

    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// View model for hot key editor.
    /// </summary>
    public class HotkeyViewModel : ViewModelBase
    {
        private readonly IEnumerable<ActionDescription> _actionDescriptions;
        private GroupViewModel _hotkeyGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyViewModel"/> class.
        /// </summary>
        /// <param name="allGroups">All groups.</param>
        /// <param name="hotkeyGroup">The hot key group.</param>
        /// <param name="hotkey">The hotkey.</param>
        /// <param name="actionDescriptions">The action descriptions.</param>
        public HotkeyViewModel([NotNull] IEnumerable<GroupViewModel> allGroups, [NotNull] GroupViewModel hotkeyGroup, [NotNull] Hotkey hotkey, [NotNull] IEnumerable<ActionDescription> actionDescriptions)
        {
            Assert.ArgumentNotNull(allGroups, "allGroups");
            Assert.ArgumentNotNull(hotkeyGroup, "hotkeyGroup");
            Assert.ArgumentNotNull(hotkey, "hotkey");
            Assert.ArgumentNotNull(actionDescriptions, "actionDescriptions");

            AllGroups = allGroups;
            _hotkeyGroup = hotkeyGroup;
            _actionDescriptions = actionDescriptions;
            Hotkey = hotkey;
            ActionsViewModel = new ActionsViewModel(hotkey.Actions, _actionDescriptions);
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
        /// Gets the hotkey.
        /// </summary>
        public Hotkey Hotkey
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the hot key group.
        /// </summary>
        /// <value>
        /// The hot key group.
        /// </value>
        [NotNull]
        public GroupViewModel HotkeyGroup
        {
            get
            {
                return _hotkeyGroup;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _hotkeyGroup = value;
                OnPropertyChanged("hotkeyGroup");
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

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key that should be pressed.
        /// </value>
        public Key Key
        {
            get
            {
                return Hotkey.Key;
            }

            set
            {
                Hotkey.Key = value;
            }
        }

        /// <summary>
        /// Gets or sets the modifier keys.
        /// </summary>
        /// <value>
        /// The modifier keys.
        /// </value>
        public ModifierKeys ModifierKeys
        {
            get
            {
                return Hotkey.ModifierKeys;
            }

            set
            {
                Hotkey.ModifierKeys = value;
            }
        }

        /// <summary>
        /// Gets the hotkey description.
        /// </summary>
        [NotNull]
        public string HotkeyDescription
        {
            get
            {
                if (Key == Key.None)
                {
                    return string.Empty;
                }

                var res = string.Empty;
                if ((ModifierKeys & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    res += "Control+";
                }

                if ((ModifierKeys & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    res += "Shift+";
                }

                if ((ModifierKeys & ModifierKeys.Alt) == ModifierKeys.Alt)
                {
                    res += "Alt+";
                }

                if ((ModifierKeys & ModifierKeys.Windows) == ModifierKeys.Windows)
                {
                    res += "Windows+";
                }

                res += Key;
                return res;
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this instance.
        /// </returns>
        [NotNull]
        public HotkeyViewModel Clone()
        {
            var clonedHotkey = new Hotkey();
            return new HotkeyViewModel(AllGroups, HotkeyGroup, clonedHotkey, _actionDescriptions)
                       {
                           Key = Key,
                           ModifierKeys = ModifierKeys,
                           ActionsViewModel = ActionsViewModel.Clone(clonedHotkey.Actions)
                       };
        }

        /// <summary>
        /// Sets the hotkey.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="modifiers">The modifiers.</param>
        public void SetHotkey(Key key, ModifierKeys modifiers)
        {
            Key = key;
            ModifierKeys = modifiers;
            OnPropertyChanged("HotkeyDescription");
        }
    }
}
