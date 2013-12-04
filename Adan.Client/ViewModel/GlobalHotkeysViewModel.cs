using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Adan.Client.Common.ViewModel;
using Adan.Client.Common.Model;
using CSLib.Net.Annotations;
using Adan.Client.Common.Utils;
using CSLib.Net.Diagnostics;
using System.Windows;
using Adan.Client.Dialogs;
using Adan.Client.Common.Plugins;
using Adan.Client.Model.Actions;

namespace Adan.Client.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class GlobalHotkeysViewModel : ViewModelBase
    {
        private IList<Hotkey> _hotkeys;
        private ObservableCollection<GlobalHotkeyViewModel> _hotkeyViewModels;
        private GlobalHotkeyViewModel _selectedHotkey;
        private IEnumerable<ActionDescription> _actionDescriptions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotkeys"></param>
        /// <param name="actionDescriptions"></param>
        public GlobalHotkeysViewModel([NotNull] IList<Hotkey> hotkeys, [NotNull] IEnumerable<ActionDescription> actionDescriptions)
        {
            Assert.ArgumentNotNull(hotkeys, "hotkeys");
            Assert.ArgumentNotNull(actionDescriptions, "actionDescriptions");

            _hotkeys = hotkeys;
            _actionDescriptions = actionDescriptions;

            _hotkeyViewModels = new ObservableCollection<GlobalHotkeyViewModel>();

            foreach (var hotkey in _hotkeys)
            {
                _hotkeyViewModels.Add(new GlobalHotkeyViewModel(hotkey, _actionDescriptions));
            }

            AddHotkeyCommand = new DelegateCommand(AddHotkeyCommandExecute, true);
            EditHotkeyCommand = new DelegateCommand(EditHotkeyCommandExecute, false);
            DeleteHotkeyCommand = new DelegateCommand(DeleteHotkeyCommandExecute, false);
        }

        /// <summary>
        /// Gets the add hot key command.
        /// </summary>
        [NotNull]
        public DelegateCommand AddHotkeyCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public DelegateCommand EditHotkeyCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the delete hot key command.
        /// </summary>
        [NotNull]
        public DelegateCommand DeleteHotkeyCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public ObservableCollection<GlobalHotkeyViewModel> Hotkeys
        {
            get
            {
                return _hotkeyViewModels;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [CanBeNull]
        public GlobalHotkeyViewModel SelectedHotkey
        {
            get
            {
                return _selectedHotkey;
            }
            set
            {
                _selectedHotkey = value;
                EditHotkeyCommand.CanBeExecuted = value != null;
                DeleteHotkeyCommand.CanBeExecuted = value != null;
                OnPropertyChanged("SelectedHotkey");
            }
        }

        private void AddHotkeyCommandExecute([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            var oldHotkey = new Hotkey();
            oldHotkey.Actions.Add(new SendTextAction());
            var hotKeyToAdd = new GlobalHotkeyViewModel(oldHotkey, _actionDescriptions);
            var hotKeyEditDialog = new GlobalHotkeyEditDialog { DataContext = hotKeyToAdd, Owner = (Window)obj };

            var dialogResult = hotKeyEditDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                var hotkeyViewModel = Hotkeys.FirstOrDefault(hotk => hotk.Key == hotKeyToAdd.Key && hotk.ModifierKeys == hotKeyToAdd.ModifierKeys);
                if (hotkeyViewModel != null)
                    Hotkeys.Remove(hotkeyViewModel);

                var hotkey = _hotkeys.FirstOrDefault(hotk => hotk.Key == hotKeyToAdd.Key && hotk.ModifierKeys == hotKeyToAdd.ModifierKeys);
                if (hotkey != null)
                    _hotkeys.Remove(hotkey);

                Hotkeys.Add(hotKeyToAdd);
                _hotkeys.Add(hotKeyToAdd.Hotkey);
            }
        }

        private void DeleteHotkeyCommandExecute([CanBeNull] object obj)
        {
            if (SelectedHotkey == null)
                return;

            var hotkey = _hotkeys.FirstOrDefault(hotk => hotk.Key == SelectedHotkey.Key && hotk.ModifierKeys == SelectedHotkey.ModifierKeys);
            if (hotkey != null)
            {
                _hotkeys.Remove(hotkey);
            }

            var hotkeyViewModel = Hotkeys.FirstOrDefault(hotk => hotk.Key == SelectedHotkey.Key && hotk.ModifierKeys == SelectedHotkey.ModifierKeys);
            if (hotkeyViewModel != null)
            {
                Hotkeys.Remove(hotkeyViewModel);
            }
        }

        private void EditHotkeyCommandExecute([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            if (SelectedHotkey == null)
                return;

            var hotkeyViewModel = SelectedHotkey.Clone();

            var hotKeyEditDialog = new GlobalHotkeyEditDialog { DataContext = hotkeyViewModel, Owner = (Window)obj };
            var dialogResult = hotKeyEditDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                var hotkeyViewModelToRemove = Hotkeys.FirstOrDefault(hotk => hotk.Key == hotkeyViewModel.Key && hotk.ModifierKeys == hotkeyViewModel.ModifierKeys);
                if (hotkeyViewModelToRemove != null)
                    Hotkeys.Remove(hotkeyViewModelToRemove);

                var hotkey = _hotkeys.FirstOrDefault(hotk => hotk.Key == hotkeyViewModel.Key && hotk.ModifierKeys == hotkeyViewModel.ModifierKeys);
                if (hotkey != null)
                {
                    _hotkeys.Remove(hotkey);
                }

                Hotkeys.Add(hotkeyViewModel);
                _hotkeys.Add(hotkeyViewModel.Hotkey);

                SelectedHotkey = hotkeyViewModel;
            }
        }
    }
}