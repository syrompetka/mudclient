﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomMonstersViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the GroupStatusViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Threading;

    using Common.Model;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Properties;

    /// <summary>
    /// A view model for monsters status widget.
    /// </summary>
    public class RoomMonstersViewModel : ViewModelBase
    {
        private readonly DispatcherTimer _tickingTimer;

        private IList<string> _displayedAffectPriorities = new List<string>(Settings.Default.MonsterAffects);
        private int _displayedAffectCount = Settings.Default.MonsterDisplayAffectsCount;
        private bool _displayNumber = Settings.Default.MonsterDisplayNumber;
        private MonsterViewModel _selectedMonster;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomMonstersViewModel"/> class.
        /// </summary>
        public RoomMonstersViewModel()
        {
            Monsters = new ObservableCollection<MonsterViewModel>();

            _tickingTimer = new DispatcherTimer(DispatcherPriority.Background);
            _tickingTimer.Interval = TimeSpan.FromSeconds(1);
            _tickingTimer.Tick += (o, e) => UpdateTimings();
            _tickingTimer.Start();
            MinimumWidth = (29 * _displayedAffectPriorities.Count) + 26 + 26 + 31 + 60 + 155;
        }

        /// <summary>
        /// Gets the minimum width.
        /// </summary>
        public double MinimumWidth
        {
            get;
            private set;
        }

        public RootModel RootModel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the selected monster.
        /// </summary>
        /// <value>
        /// The selected monster.
        /// </value>
        [CanBeNull]
        public MonsterViewModel SelectedMonster
        {
            get
            {
                return _selectedMonster;
            }

            set
            {
                _selectedMonster = value;
                if (RootModel != null)
                    RootModel.SelectedRoomMonster = value != null ? value.MonsterStatus : null;
                OnPropertyChanged("SelectedMonster");
            }
        }

        /// <summary>
        /// Gets the group mates of current player.
        /// </summary>
        [NotNull]
        public ObservableCollection<MonsterViewModel> Monsters
        {
            get;
            private set;
        }

        public int DisplayedAffectCount
        {
            get { return _displayedAffectCount; }
            set
            {
                _displayedAffectCount = value;
                OnPropertyChanged("DisplayedAffectCount");
            }
        }

        public bool DisplayNumber
        {
            get { return _displayNumber; }
            set
            {
                _displayNumber = value;
                OnPropertyChanged("DisplayNumber");
            }
        }

        /// <summary>
        /// Updates the model.
        /// </summary>
        /// <param name="monsters">The monsters.</param>
        public void UpdateModel([NotNull] IEnumerable<MonsterStatus> monsters)
        {
            Assert.ArgumentNotNull(monsters, "monsters");

            int position = 0;
            foreach (var monster in monsters)
            {
                if (position < Monsters.Count && Monsters[position].Name == monster.Name)
                {
                    Monsters[position].UpdateFromModel(monster, position + 1);
                }
                else
                {
                    var affectsList = _displayedAffectPriorities.Select(af => Constants.AllAffects.First(a => a.Name == af));
                    Monsters.Insert(position, new MonsterViewModel(monster, affectsList, position + 1) {DisplayNumber = DisplayNumber});
                }

                position++;
            }

            var count = Monsters.Count;
            for (int i = position; i < count; i++)
            {
                Monsters.RemoveAt(position);
            }
        }

        /// <summary>
        /// Reloads the displayed affects.
        /// </summary>
        public void ReloadDisplayedAffects()
        {
            Monsters.Clear();
            _displayedAffectPriorities = new List<string>(Settings.Default.MonsterAffects);
            DisplayNumber = Settings.Default.GroupWidgetDisplayNumber;
            DisplayedAffectCount = Settings.Default.MonsterDisplayAffectsCount;
            MinimumWidth = (29 * DisplayedAffectCount) + 26 + 26 + 31 + 60 + 155;
            OnPropertyChanged("MinimumWidth");
        }

        private void UpdateTimings()
        {
            foreach (var monster in Monsters)
            {
                monster.UpdateTimings();
            }
        }
    }
}

