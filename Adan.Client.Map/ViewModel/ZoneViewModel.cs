// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoneViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ZoneViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Common.Model;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model;

    /// <summary>
    /// View model for single zone.
    /// </summary>
    public class ZoneViewModel : ViewModelBase
    {
        private readonly Zone _zone;
        private readonly IEnumerable<AdditionalRoomParameters> _additionalRoomParameters;
        private readonly RootModel _rootModel;
        private double _zoomLevel;
        private int _currentLevel;
        private RoomViewModel _currentRoom;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneViewModel"/> class.
        /// </summary>
        /// <param name="zone">The zone to display.</param>
        /// <param name="additionalRoomParameters">The additional room parameterses.</param>
        /// <param name="rootModel">The root model.</param>
        public ZoneViewModel([NotNull] Zone zone, [NotNull] IEnumerable<AdditionalRoomParameters> additionalRoomParameters, [NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(zone, "zone");
            Assert.ArgumentNotNull(additionalRoomParameters, "additionalRoomParameters");
            Assert.ArgumentNotNull(rootModel, "rootModel");

            AllRooms = new List<RoomViewModel>();
            _zone = zone;
            _additionalRoomParameters = additionalRoomParameters;
            _rootModel = rootModel;
            _zoomLevel = 1;
            CurrentLevelRooms = new ObservableCollection<RoomViewModel>();
            BuildRoomsViewModel();
            BuildCurrentLevelRooms();
        }

        /// <summary>
        /// Gets the name of the zone.
        /// </summary>
        [NotNull]
        public string Name
        {
            get
            {
                return _zone.Name;
            }
        }

        /// <summary>
        /// Gets the zone identifier.
        /// </summary>
        public int Id
        {
            get
            {
                return _zone.Id;
            }
        }

        /// <summary>
        /// Gets or sets the zoom level.
        /// </summary>
        /// <value>
        /// The zoom level.
        /// </value>
        public double ZoomLevel
        {
            get
            {
                return _zoomLevel;
            }

            set
            {
                if (value == _zoomLevel)
                {
                    return;
                }

                _zoomLevel = value;
                OnPropertyChanged("ZoomLevel");
            }
        }

        /// <summary>
        /// Gets or sets the current level.
        /// </summary>
        /// <value>
        /// The current level.
        /// </value>
        public int CurrentLevel
        {
            get
            {
                return _currentLevel;
            }

            set
            {
                if (value == _currentLevel)
                {
                    return;
                }

                _currentLevel = value;
                BuildCurrentLevelRooms();
                OnPropertyChanged("CurrentLevel");
            }
        }

        /// <summary>
        /// Gets or sets the current room.
        /// </summary>
        /// <value>
        /// The current room.
        /// </value>
        [CanBeNull]
        public RoomViewModel CurrentRoom
        {
            get
            {
                return _currentRoom;
            }

            set
            {
                if (_currentRoom == value)
                {
                    return;
                }

                if (_currentRoom != null)
                {
                    _currentRoom.IsCurrent = false;
                }

                _currentRoom = value;
                if (_currentRoom != null)
                {
                    CurrentLevel = _currentRoom.ZLocation;
                    _currentRoom.IsCurrent = true;
                }

                OnPropertyChanged("CurrentRoom");
            }
        }

        /// <summary>
        /// Gets or sets the current room id.
        /// </summary>
        public int? CurrentRoomId
        {
            get
            {
                return CurrentRoom != null ? CurrentRoom.RoomId : (int?)null;
            }

            set
            {
                if (value == null)
                {
                    CurrentRoom = null;
                    return;
                }

                CurrentRoom = AllRooms.Single(r => r.RoomId == value.Value);
            }
        }

        /// <summary>
        /// Gets the current level rooms.
        /// </summary>
        [NotNull]
        public ObservableCollection<RoomViewModel> CurrentLevelRooms
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all rooms of this zone.
        /// </summary>
        [NotNull]
        public List<RoomViewModel> AllRooms
        {
            get;
            private set;
        }

        private void BuildCurrentLevelRooms()
        {
            CurrentLevelRooms.Clear();

            foreach (var roomViewModel in AllRooms.Where(r => r.ZLocation == CurrentLevel))
            {
                CurrentLevelRooms.Add(roomViewModel);
            }
        }

        private void BuildRoomsViewModel()
        {
            var roomExitByIdLookup = new Dictionary<int, IList<RoomExitViewModel>>();
            var roomByIdLookup = new Dictionary<int, RoomViewModel>();
            var additionalRoomParametersLookup = _additionalRoomParameters.ToDictionary(r => r.RoomId);
            foreach (var room in _zone.Rooms)
            {
                var exists = new List<RoomExitViewModel>();
                AdditionalRoomParameters additionalRoomParameters;
                if (additionalRoomParametersLookup.ContainsKey(room.Id))
                {
                    additionalRoomParameters = additionalRoomParametersLookup[room.Id];
                }
                else
                {
                    additionalRoomParameters = new AdditionalRoomParameters { RoomId = room.Id };
                }

                var roomViewModel = new RoomViewModel(room, additionalRoomParameters, exists, this, _rootModel.AllActionDescriptions);
                roomByIdLookup.Add(room.Id, roomViewModel);
                roomExitByIdLookup.Add(room.Id, exists);
                AllRooms.Add(roomViewModel);
            }

            foreach (var room in _zone.Rooms)
            {
                var exists = roomExitByIdLookup[room.Id];
                foreach (var roomExit in room.Exits)
                {
                    RoomExitViewModel roomExitViewModel;
                    if (roomByIdLookup.ContainsKey(roomExit.RoomId))
                    {
                        roomExitViewModel = new RoomExitViewModel(roomExit.Direction, roomByIdLookup[roomExit.RoomId]);
                    }
                    else
                    {
                        roomExitViewModel = new RoomExitViewModel(roomExit.Direction, null);
                    }

                    exists.Add(roomExitViewModel);
                }
            }
        }
    }
}
