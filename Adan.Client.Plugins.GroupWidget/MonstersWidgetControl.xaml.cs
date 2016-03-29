// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonstersWidgetControl.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for GroupWidgetControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget
{
    using System;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;
    using Common.Model;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for GroupWidgetControl.xaml
    /// </summary>
    public partial class MonstersWidgetControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonstersWidgetControl"/> class.
        /// </summary>
        public MonstersWidgetControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public string ViewModelUid
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public void NextMonster()
        {
            Action executeToAct = () =>
            {
                if (this.DataContext != null)
                {
                    RoomMonstersViewModel _roomMonstersViewModel = (RoomMonstersViewModel)this.DataContext;
                    if (_roomMonstersViewModel.SelectedMonster == null || _roomMonstersViewModel.Monsters.IndexOf(_roomMonstersViewModel.SelectedMonster) == _roomMonstersViewModel.Monsters.Count - 1)
                    {
                        _roomMonstersViewModel.SelectedMonster = _roomMonstersViewModel.Monsters.FirstOrDefault();
                        return;
                    }

                    var index = _roomMonstersViewModel.Monsters.IndexOf(_roomMonstersViewModel.SelectedMonster);
                    _roomMonstersViewModel.SelectedMonster = _roomMonstersViewModel.Monsters[index + 1];
                }
            };

            Application.Current.Dispatcher.BeginInvoke(executeToAct, DispatcherPriority.Background);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PreviousMonster()
        {
            Action executeToAct = () =>
            {
                if (this.DataContext != null)
                {
                    RoomMonstersViewModel _roomMonstersViewModel = (RoomMonstersViewModel)this.DataContext;
                    if (_roomMonstersViewModel.SelectedMonster == null || _roomMonstersViewModel.Monsters.IndexOf(_roomMonstersViewModel.SelectedMonster) == 0)
                    {
                        _roomMonstersViewModel.SelectedMonster = _roomMonstersViewModel.Monsters.LastOrDefault();
                        return;
                    }

                    var index = _roomMonstersViewModel.Monsters.IndexOf(_roomMonstersViewModel.SelectedMonster);
                    _roomMonstersViewModel.SelectedMonster = _roomMonstersViewModel.Monsters[index - 1];
                }
            };

            Application.Current.Dispatcher.BeginInvoke(executeToAct, DispatcherPriority.Background);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="characters"></param>
        public void UpdateModel([NotNull] List<MonsterStatus> characters)
        {
            Assert.ArgumentNotNull(characters, "roomMonstersMessage");

            Action actToExecute = () =>
            {
                RoomMonstersViewModel viewModel = DataContext as RoomMonstersViewModel;
                viewModel.UpdateModel(characters);
            };

            Application.Current.Dispatcher.BeginInvoke(actToExecute, DispatcherPriority.Background);
        }

        private void CancelFocusingListBoxItem([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            ((ListBoxItem)sender).IsSelected = true;
            e.Handled = true;
        }
    }
}
