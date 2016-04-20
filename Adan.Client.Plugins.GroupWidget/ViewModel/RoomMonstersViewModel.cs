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
        private bool _moreItemsAvailable;

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
            AffectsPanelWidth = _displayedAffectCount * 23;
            Width = AffectsPanelWidth + 22 + 140 + 60 + 20 + 20 + 5 + 5;
            if (!DisplayNumber)
            {
                Width -= 22;
            }


            DisplayedItemLimit = Settings.Default.IsMonsterLimitOn ? Settings.Default.MonsterLimit : 9999;
            if (DisplayedItemLimit < 1)
            {
                DisplayedItemLimit = 1;
            }
        }

        /// <summary>
        /// Gets the Affects panel width.
        /// </summary>
        public double AffectsPanelWidth
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the width of control displaying monsters.
        /// </summary>
        public double Width
        {
            get; private set;
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

        public bool MoreItemsAvailable
        {
            get { return _moreItemsAvailable; }
            set
            {
                if (_moreItemsAvailable != value)
                {
                    _moreItemsAvailable = value;
                    OnPropertyChanged("MoreItemsAvailable");
                }
            }
        }

        public int DisplayedItemLimit { get; set; }



        /// <summary>
        /// Updates the model.
        /// </summary>
        /// <param name="monsters">The monsters.</param>
        public void UpdateModel([NotNull] IEnumerable<MonsterStatus> monsters)
        {
            Assert.ArgumentNotNull(monsters, "monsters");

            int position = 0;
            bool moreItemsAvailable = false;
            foreach (var monster in monsters)
            {
                if (position >= DisplayedItemLimit)
                {
                    moreItemsAvailable = true;
                    break;
                }

                if (position < Monsters.Count && Monsters[position].Name == monster.Name)
                {
                    Monsters[position].UpdateFromModel(monster, position + 1);
                    if (SelectedMonster != null && SelectedMonster == Monsters[position])
                    {
                        RootModel.SelectedRoomMonster = monster;
                    }
                }
                else
                {
                    var affectsList = _displayedAffectPriorities.Select(af => Constants.AllAffects.First(a => a.Name == af));
                    Monsters.Insert(position, new MonsterViewModel(monster, affectsList, position + 1, AffectsPanelWidth) { DisplayNumber = DisplayNumber });
                }

                position++;
            }

            var count = Monsters.Count;
            for (int i = position; i < count; i++)
            {
                if (SelectedMonster != null && SelectedMonster == Monsters[position])
                {
                    SelectedMonster = null;
                }

                Monsters.RemoveAt(position);
            }

            MoreItemsAvailable = moreItemsAvailable;
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
            AffectsPanelWidth = 23 * _displayedAffectCount;
            Width = AffectsPanelWidth + 22 + 140 + 60 + 20 + 20 + 5 + 5;
            if (!DisplayNumber)
            {
                Width -= 22;
            }

            DisplayedItemLimit = Settings.Default.IsMonsterLimitOn ? Settings.Default.MonsterLimit : 9999;
            if (DisplayedItemLimit < 1)
            {
                DisplayedItemLimit = 1;
            }
            MoreItemsAvailable = false;
            OnPropertyChanged("Width");
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

