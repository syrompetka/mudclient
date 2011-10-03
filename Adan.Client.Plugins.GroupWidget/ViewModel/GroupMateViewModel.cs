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
    using System.Collections.ObjectModel;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupMateViewModel"/> class.
        /// </summary>
        /// <param name="groupMate">The group mate.</param>
        /// <param name="affectsToDisplay">The affects to display.</param>
        public GroupMateViewModel([NotNull]CharacterStatus groupMate, [NotNull] IEnumerable<AffectDescription> affectsToDisplay)
        {
            Assert.ArgumentNotNull(groupMate, "groupMate");
            Assert.ArgumentNotNull(affectsToDisplay, "affectsToDisplay");

            GroupMate = groupMate;
            _hitsColor = GetColor(groupMate.HitsPercent);
            _movesColor = GetColor(groupMate.MovesPercent);
            Affects = new ObservableCollection<AffectViewModel>();
            foreach (var affectDescription in affectsToDisplay)
            {
                Affects.Add(new AffectViewModel(affectDescription));
            }
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
        public ObservableCollection<AffectViewModel> Affects
        {
            get;
            private set;
        }

        /// <summary>
        /// Updates this view model from model.
        /// </summary>
        /// <param name="characterStatus">The character status model.</param>
        public virtual void UpdateFromModel([NotNull] CharacterStatus characterStatus)
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
