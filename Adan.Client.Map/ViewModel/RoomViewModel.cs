// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the RoomViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.ViewModel
{
    using System.Collections.Generic;

    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model;

    /// <summary>
    /// View model for room.
    /// </summary>
    public class RoomViewModel : ViewModelBase
    {
        private readonly IEnumerable<ActionDescription> _allActionDescriptions;
        private bool _isCurrent;
        private bool _isConnectedToCurrent;
        private bool _isSelected;
        private ActionsViewModel _actionsViewModel;
        private bool _isStartOrEndOfRoute;
        private bool _isPartOfRoute;
        private bool _isPartOfCurrentRoute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomViewModel"/> class.
        /// </summary>
        /// <param name="room">The room this view model is associated to.</param>
        /// <param name="additionalRoomParameters">The additional room parameters.</param>
        /// <param name="exists">The exists of this room.</param>
        /// <param name="zone">The zone this room belongs to.</param>
        /// <param name="allActionDescriptions">All action descriptions.</param>
        public RoomViewModel([NotNull] Room room, [NotNull] AdditionalRoomParameters additionalRoomParameters, [NotNull] IEnumerable<RoomExitViewModel> exists, [NotNull] ZoneViewModel zone, [NotNull]IEnumerable<ActionDescription> allActionDescriptions)
        {
            Assert.ArgumentNotNull(room, "room");
            Assert.ArgumentNotNull(additionalRoomParameters, "additionalRoomParameters");
            Assert.ArgumentNotNull(exists, "exists");
            Assert.ArgumentNotNull(zone, "zone");
            Assert.ArgumentNotNull(allActionDescriptions, "allActionDescriptions");

            Room = room;
            _allActionDescriptions = allActionDescriptions;
            AdditionalRoomParameters = additionalRoomParameters;
            Exits = exists;
            Zone = zone;
        }

        /// <summary>
        /// Gets the room.
        /// </summary>
        [NotNull]
        public Room Room { get; }

        /// <summary>
        /// Gets the room identifier.
        /// </summary>
        public int RoomId => Room.Id;

        /// <summary>
        /// Gets the name of this room.
        /// </summary>
        [NotNull]
        public string Name => Room.Name;

        /// <summary>
        /// Gets the description of this room.
        /// </summary>
        [NotNull]
        public string Description => Room.Description.Replace("\n", string.Empty);

        /// <summary>
        /// Gets or sets the comments for this room.
        /// </summary>
        /// <value>
        /// The comments for this room.
        /// </value>
        [NotNull]
        public string Comments
        {
            get
            {
                return AdditionalRoomParameters.Comments;
            }

            set
            {
                Assert.ArgumentNotNullOrWhiteSpace(value, "value");

                AdditionalRoomParameters.Comments = value;
                AdditionalRoomParameters.HasChanges = true;
                OnPropertyChanged("Comments");
            }
        }

        /// <summary>
        /// Gets or sets the alias for this room.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        [NotNull]
        public string Alias
        {
            get
            {
                return AdditionalRoomParameters.RoomAlias;
            }

            set
            {
                Assert.ArgumentNotNullOrWhiteSpace(value, "value");
                AdditionalRoomParameters.RoomAlias = value;
                AdditionalRoomParameters.HasChanges = true;
                OnPropertyChanged("Alias");
            }
        }

        /// <summary>
        /// Gets or sets the Z location ot the room.
        /// </summary>
        /// <value>
        /// The Z location.
        /// </value>
        public int ZLocation
        {
            get
            {
                return Room.ZLocation;
            }

            set
            {
                AdditionalRoomParameters.HasChanges = true;
                Room.ZLocation = value;
                OnPropertyChanged("ZLocation");
            }
        }

        /// <summary>
        /// Gets or sets the Y location of the room.
        /// </summary>
        public int YLocation
        {
            get
            {
                return Room.YLocation;
            }

            set
            {
                AdditionalRoomParameters.HasChanges = true;
                Room.YLocation = value;
                OnPropertyChanged("YLocation");
            }
        }

        /// <summary>
        /// Gets or sets the X location of the room.
        /// </summary>
        public int XLocation
        {
            get
            {
                return Room.XLocation;
            }

            set
            {
                AdditionalRoomParameters.HasChanges = true;
                Room.XLocation = value;
                OnPropertyChanged("XLocation");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this room is current players room.
        /// </summary>
        /// <value>
        /// <c>true</c> if this room is current players room; otherwise, <c>false</c>.
        /// </value>
        public bool IsCurrent
        {
            get
            {
                return _isCurrent;
            }

            set
            {
                _isCurrent = value;
                foreach (var exit in Exits)
                {
                    if (exit.Room != null)
                    {
                        exit.Room.IsConnectedToCurrent = value;
                    }
                }

                if (value)
                {
                    if (!AdditionalRoomParameters.HasBeenVisited)
                    {
                        AdditionalRoomParameters.HasChanges = true;
                    }

                    AdditionalRoomParameters.HasBeenVisited = true;
                }

                OnPropertyChanged("IsCurrent");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this room is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this room is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this room is connected to current players room.
        /// </summary>
        /// <value>
        /// <c>true</c> if this room is connected to current players room; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnectedToCurrent
        {
            get
            {
                return _isConnectedToCurrent;
            }

            set
            {
                _isConnectedToCurrent = value;
                OnPropertyChanged("IsConnectedToCurrent");
            }
        }

        /// <summary>
        /// Gets a value indicating whether this room has been visited by player previously.
        /// </summary>
        /// <value>
        /// <c>true</c> if this room has been visited; otherwise, <c>false</c>.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "For debugging only.")]
        public bool HasBeenVisited
        {
            get
            {
#if DEBUG
                return true;
#else
                return AdditionalRoomParameters.HasBeenVisited;
#endif
            }
        }

        /// <summary>
        /// Gets the additional room parameters.
        /// </summary>
        [NotNull]
        public AdditionalRoomParameters AdditionalRoomParameters
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the exits of this room.
        /// </summary>
        [NotNull]
        public IEnumerable<RoomExitViewModel> Exits { get; }

        /// <summary>
        /// Gets the zone this room belongs to.
        /// </summary>
        [NotNull]
        public ZoneViewModel Zone { get; }

        /// <summary>
        /// Gets the actions view model.
        /// </summary>
        [NotNull]
        public ActionsViewModel ActionsViewModel => _actionsViewModel ??
                                                    (_actionsViewModel =
                                                        new ActionsViewModel(AdditionalRoomParameters.ActionsToExecuteOnRoomEntry,
                                                            _allActionDescriptions, true));

        /// <summary>
        /// Gets or sets the color to use to display this room on the map.
        /// </summary>
        /// <value>
        /// The color of this room.
        /// </value>
        public RoomColor Color
        {
            get
            {
                return AdditionalRoomParameters.Color;
            }

            set
            {
                AdditionalRoomParameters.HasChanges = true;
                AdditionalRoomParameters.Color = value;
                OnPropertyChanged("Color");
            }
        }

        /// <summary>
        /// Gets or sets the icon to display over this room.
        /// </summary>
        /// <value>
        /// The icon of this room.
        /// </value>
        public RoomIcon Icon
        {
            get
            {
                return AdditionalRoomParameters.Icon;
            }

            set
            {
                AdditionalRoomParameters.HasChanges = true;
                AdditionalRoomParameters.Icon = value;
                OnPropertyChanged("Icon");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether room is start or end of some route.
        /// </summary>
        /// <value>
        /// <c>true</c> if this room is start or end of some route; otherwise, <c>false</c>.
        /// </value>
        public bool IsStartOrEndOfRoute
        {
            get
            {
                return _isStartOrEndOfRoute;
            }

            set
            {
                if (value != _isStartOrEndOfRoute)
                {
                    _isStartOrEndOfRoute = value;
                    OnPropertyChanged("IsStartOrEndOfRoute");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this room is part of some route.
        /// </summary>
        /// <value>
        /// <c>true</c> if this room is part of some route; otherwise, <c>false</c>.
        /// </value>
        public bool IsPartOfRoute
        {
            get
            {
                return _isPartOfRoute;
            }

            set
            {
                if (value != _isPartOfRoute)
                {
                    _isPartOfRoute = value;
                    OnPropertyChanged("IsPartOfRoute");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this room is part of recorded route.
        /// </summary>
        /// <value>
        /// <c>true</c> if this room is part of recorded route; otherwise, <c>false</c>.
        /// </value>
        public bool IsPartOfRecordedRoute
        {
            get
            {
                return _isPartOfCurrentRoute;
            }

            set
            {
                if (value != _isPartOfCurrentRoute)
                {
                    _isPartOfCurrentRoute = value;
                    OnPropertyChanged("IsPartOfRecordedRoute");
                }
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        [NotNull]
        public RoomViewModel Clone()
        {
            var additionalRoomParameters = AdditionalRoomParameters.Clone();
            return new RoomViewModel(Room, additionalRoomParameters, Exits, Zone, _allActionDescriptions)
                       {
                           _actionsViewModel = ActionsViewModel.Clone(additionalRoomParameters.ActionsToExecuteOnRoomEntry)
                       };
        }

        /// <summary>
        /// Updates the specified room view model.
        /// </summary>
        /// <param name="roomViewModel">The room view model.</param>
        public void Update([NotNull]RoomViewModel roomViewModel)
        {
            Assert.ArgumentNotNull(roomViewModel, "roomViewModel");

            _actionsViewModel = null;
            AdditionalRoomParameters = roomViewModel.AdditionalRoomParameters;
            AdditionalRoomParameters.HasChanges = true;
            OnPropertyChanged("Icon");
            OnPropertyChanged("Color");
            OnPropertyChanged("Alias");
            OnPropertyChanged("Comments");
        }
    }
}