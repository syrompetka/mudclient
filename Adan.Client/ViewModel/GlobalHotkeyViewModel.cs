using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adan.Client.Common.ViewModel;
using Adan.Client.Common.Plugins;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using Adan.Client.Common.Model;
using System.Windows.Input;

namespace Adan.Client.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class GlobalHotkeyViewModel : ViewModelBase
    {
        private readonly IEnumerable<ActionDescription> _actionDescriptions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotkey"></param>
        /// <param name="actionDescriptions"></param>
        public GlobalHotkeyViewModel([NotNull] Hotkey hotkey, [NotNull] IEnumerable<ActionDescription> actionDescriptions)
        {
            Assert.ArgumentNotNull(hotkey, "hotkey");
            Assert.ArgumentNotNull(actionDescriptions, "actionDescriptions");

            _actionDescriptions = actionDescriptions;
            Hotkey = hotkey;
            ActionsViewModel = new ActionsViewModel(hotkey.Actions, _actionDescriptions);
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

                StringBuilder sb = new StringBuilder();

                if ((ModifierKeys & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    sb.Append("Control+");
                }

                if ((ModifierKeys & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    sb.Append("Shift+");
                }

                if ((ModifierKeys & ModifierKeys.Alt) == ModifierKeys.Alt)
                {
                    sb.Append("Alt+");
                }

                if ((ModifierKeys & ModifierKeys.Windows) == ModifierKeys.Windows)
                {
                    sb.Append("Windows+");
                }

                sb.Append(Key.ToString());
                return sb.ToString();
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this instance.
        /// </returns>
        [NotNull]
        public GlobalHotkeyViewModel Clone()
        {
            var clonedHotkey = new Hotkey() { Key = Key, ModifierKeys = ModifierKeys };
            return new GlobalHotkeyViewModel(clonedHotkey, _actionDescriptions)
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
