// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupMateViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the GroupMateViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget.ViewModel
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Data;
    using Common.Model;
    using Common.Themes;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// View model for single group mate.
    /// </summary>
    public class GroupMateViewModel : ViewModelBase
    {
        private readonly List<AffectViewModel> _notProcessedAffects = new List<AffectViewModel>();

        private bool _isDeleting;
        private TextColor _movesColor;
        private TextColor _hitsColor;
        private int _number;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupMateViewModel"/> class.
        /// </summary>
        public GroupMateViewModel([NotNull]CharacterStatus groupMate, [NotNull] IEnumerable<AffectDescription> affectsToDisplay, int number, double affectsPanelWidth)
        {
            Assert.ArgumentNotNull(groupMate, "groupMate");
            Assert.ArgumentNotNull(affectsToDisplay, "affectsToDisplay");

            GroupMate = groupMate;
            Number = number;
            _hitsColor = GetColor(groupMate.HitsPercent);
            _movesColor = GetColor(groupMate.MovesPercent);
            Affects = new List<AffectViewModel>(affectsToDisplay.Count());

            int priority = 0;
            foreach (var affectDescription in affectsToDisplay)
            {
                Affects.Add(new AffectViewModel(affectDescription, priority));
                priority++;
            }

            var affectsSortedAndFiltered = (ICollectionViewLiveShaping)CollectionViewSource.GetDefaultView(Affects);
            affectsSortedAndFiltered.IsLiveSorting = true;
            ((ICollectionView)affectsSortedAndFiltered).SortDescriptions.Add(new SortDescription() { Direction = ListSortDirection.Ascending, PropertyName = "Priority" });
            AffectsSortedAndFiltered = (ICollectionView)affectsSortedAndFiltered;
            AffectsPanelWidth = affectsPanelWidth;
        }

        /// <summary>
        /// Gets the group mate.
        /// </summary>
        [NotNull]
        public CharacterStatus GroupMate
        {
            get;
            private set;
        }

        public double AffectsPanelWidth
        {
            get;
            private set;
        }

        public int Number
        {
            get
            {
                return _number;
            }
            private set
            {
                if (_number != value)
                {
                    _number = value;
                    OnPropertyChanged("Number");
                }
            }
        }

        public bool DisplayNumber { get; set; }

        /// <summary>
        /// Gets the name of group mate.
        /// </summary>
        [NotNull]
        public string Name
        {
            get
            {
                return GroupMate.Name;
            }
        }

        public TextColor TextColor
        {
            get { return TextColor.White; }
        }

        /// <summary>
        /// Gets the color to use to display hits.
        /// </summary>
        /// <value>
        /// <see cref="TextColor"/> to use to display hits.
        /// </value>
        public TextColor HitsColor
        {
            get
            {
                return _hitsColor;
            }

            private set
            {
                if (value != _hitsColor)
                {
                    _hitsColor = value;
                    OnPropertyChanged("HitsColor");
                }
            }
        }

        /// <summary>
        /// Gets the color to use to display moves.
        /// </summary>
        /// <value>
        /// <see cref="TextColor"/> to use to display moves.
        /// </value>
        public TextColor MovesColor
        {
            get
            {
                return _movesColor;
            }

            private set
            {
                if (value != _movesColor)
                {
                    _movesColor = value;
                    OnPropertyChanged("MovesColor");
                }
            }
        }

        /// <summary>
        /// Gets the group mate hits percent.
        /// </summary>
        public float HitsPercent
        {
            get
            {
                return GroupMate.HitsPercent;
            }

            private set
            {
                if (value != GroupMate.HitsPercent)
                {
                    GroupMate.HitsPercent = value;
                    OnPropertyChanged("HitsPercent");
                }
            }
        }

        public string MemTime
        {
            get
            {
                if (!MemTimeVisibleSetting)
                {
                    return string.Empty;
                }

                if (GroupMate.MemTime < 0)
                {
                    return "-----";
                }

                if (GroupMate.MemTime == 0)
                {
                    return string.Empty;
                }

                var memTimeMinutes = GroupMate.MemTime / 60;
                var memTimeSeconds = GroupMate.MemTime % 60;
                return string.Format("{0:00}:{1:00}", memTimeMinutes, memTimeSeconds);
            }
        }

        public float WaitTimeHeight
        {
            get
            {
                if (GroupMate.WaitState > 80)
                {
                    return 20;
                }

                if (GroupMate.WaitState <= 0)
                {
                    return 0;
                }

                return GroupMate.WaitState * 20.0f / 80.0f;
            }
        }

        /// <summary>
        /// Gets the group mate moves percent.
        /// </summary>
        public float MovesPercent
        {
            get
            {
                return GroupMate.MovesPercent;
            }

            private set
            {
                if (value != GroupMate.MovesPercent)
                {
                    GroupMate.MovesPercent = value;
                    OnPropertyChanged("MovesPercent");
                }
            }
        }

        /// <summary>
        /// Gets the grou mate position.
        /// </summary>
        public Position Position
        {
            get
            {
                return GroupMate.Position;
            }

            private set
            {
                if (value != GroupMate.Position)
                {
                    GroupMate.Position = value;
                    OnPropertyChanged("Position");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this charrackter is attacked by somebody else.
        /// </summary>
        /// <value>
        /// <c>true</c> if this charackter is attacked; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttacked
        {
            get
            {
                return GroupMate.IsAttacked;
            }

            private set
            {
                if (value != GroupMate.IsAttacked)
                {
                    GroupMate.IsAttacked = value;
                    OnPropertyChanged("IsAttacked");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this character is in same room with current player.
        /// </summary>
        /// <value>
        /// <c>true</c> if this character is in same room; otherwise, <c>false</c>.
        /// </value>
        public bool InSameRoom
        {
            get
            {
                return GroupMate.InSameRoom;
            }

            set
            {
                if (value != GroupMate.InSameRoom)
                {
                    GroupMate.InSameRoom = value;
                    OnPropertyChanged("InSameRoom");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this group mate is being deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if this group mate is being deleted.; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleting
        {
            get
            {
                return _isDeleting;
            }

            set
            {
                _isDeleting = value;
                OnPropertyChanged("IsDeleting");
            }
        }

        /// <summary>
        /// Gets the affects of this character.
        /// </summary>
        [NotNull]
        public IList<AffectViewModel> Affects
        {
            get;
            private set;
        }

        [NotNull]
        public ICollectionView AffectsSortedAndFiltered { get; private set; }

        public bool MemTimeVisibleSetting { get; set; }

        /// <summary>
        /// Updates this view model from model.
        /// </summary>
        public virtual void UpdateFromModel([NotNull] CharacterStatus characterStatus, int position)
        {
            Assert.ArgumentNotNull(characterStatus, "characterStatus");

            if (characterStatus.Name != Name)
            {
                return;
            }

            HitsPercent = characterStatus.HitsPercent;

            MovesPercent = characterStatus.MovesPercent;
            
            Position = characterStatus.Position;
            IsAttacked = characterStatus.IsAttacked;
            InSameRoom = characterStatus.InSameRoom;
            Number = position;
            HitsColor = GetColor(HitsPercent);
            MovesColor = GetColor(MovesPercent);
            _notProcessedAffects.Clear();
            _notProcessedAffects.AddRange(Affects);

            foreach (var affect in characterStatus.Affects)
            {
                foreach (var affectViewModel in Affects)
                {
                    if (affectViewModel.AffectDescription.AffectNames.Contains(affect.Name))
                    {
                        affectViewModel.UpdateFromModel(affect);
                        _notProcessedAffects.Remove(affectViewModel);
                        break;
                    }
                }
            }

            foreach (var notProcessedAffect in _notProcessedAffects)
            {
                notProcessedAffect.OnAffectRemoved();
            }

            var oldMemTime = GroupMate.MemTime;
            var oldWaitTime = GroupMate.WaitState;
            GroupMate = characterStatus;
            if (MemTimeVisibleSetting && oldMemTime != GroupMate.MemTime)
            {
                OnPropertyChanged("MemTime");
            }

            if (oldWaitTime != GroupMate.WaitState)
            {
                OnPropertyChanged("WaitTimeHeight");
            }
        }

        /// <summary>
        /// Updates the timings.
        /// </summary>
        public void UpdateTimings()
        {
            foreach (var affectViewModel in Affects)
            {
                affectViewModel.UpdateTimings();
            }
        }

        private static TextColor GetColor(float hitsPercent)
        {
            if (hitsPercent > 80)
            {
                return TextColor.Green;
            }

            if (hitsPercent > 20)
            {
                return TextColor.Yellow;
            }

            return hitsPercent > 10 ? TextColor.Red : TextColor.BrightRed;
        }
    }
}
