// --------------------------------------------------------------------------------------------------------------------
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
        private readonly DispatcherTimer _tickingTimer = new DispatcherTimer();

        private IList<string> _displayedAffectNames = new List<string>(Settings.Default.MonsterAffects);
        private MonsterViewModel _selectedMonster;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomMonstersViewModel"/> class.
        /// </summary>
        public RoomMonstersViewModel()
        {
            Monsters = new ObservableCollection<MonsterViewModel>();
            _tickingTimer.Interval = TimeSpan.FromSeconds(1);
            _tickingTimer.Tick += (o, e) => UpdateTimings();
            _tickingTimer.Start();
            MinimumWidth = (29 * _displayedAffectNames.Count) + 26 + 26 + 31 + 60 + 155;
        }

        /// <summary>
        /// Gets the minimum width.
        /// </summary>
        public double MinimumWidth
        {
            get;
            private set;
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
                //_rootModel.SelectedRoomMonster = value != null ? value.MonsterStatus : null;
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
                    Monsters[position].UpdateFromModel(monster);
                }
                else
                {
                    var affectsList = _displayedAffectNames.Select(af => Constants.AllAffects.First(a => a.Name == af));
                    //_rootModel.RoomMonstersStatus.Insert(position, monster);
                    Monsters.Insert(position, new MonsterViewModel(monster, affectsList));
                }

                position++;
            }

            var count = Monsters.Count;
            for (int i = position; i < count; i++)
            {
                Monsters.RemoveAt(position);
               // _rootModel.RoomMonstersStatus.RemoveAt(position);
            }
        }

        /// <summary>
        /// Reloads the displayed affects.
        /// </summary>
        public void ReloadDisplayedAffects()
        {
            Monsters.Clear();
            _displayedAffectNames = new List<string>(Settings.Default.MonsterAffects);
            MinimumWidth = (29 * _displayedAffectNames.Count) + 26 + 26 + 31 + 60 + 155;
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

