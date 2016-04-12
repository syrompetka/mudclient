namespace Adan.Client.Map
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Xml;
    using System.Xml.Serialization;
    using Common.Commands;
    using Common.Dialogs;
    using Common.Messages;
    using Common.Model;
    using Common.Settings;
    using Common.Themes;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Dialogs;
    using Model;
    using Properties;
    using ViewModel;

    /// <summary>
    /// Class to manage routes and navigation.
    /// </summary>
    public sealed class RouteManager
    {
        private readonly XmlSerializer _routesSerializer = new XmlSerializer(typeof(List<Route>));
        private readonly Window _mainWindow;
        private readonly HashSet<int> _routeRoomIdentifiers = new HashSet<int>();
        private readonly HashSet<int> _routeEndRoomIdentifiers = new HashSet<int>();
        private readonly Queue<Tuple<RoomViewModel, ZoneViewModel>> _pendingUpdates = new Queue<Tuple<RoomViewModel, ZoneViewModel>>();

        private RootModel _rootModel;
        private bool _isUpdateInProgress;
        private int _groupMembersCountOnRouteStart;
        private Route _currentlyRecordedRoute;
        private RoomViewModel _currentRoom;
        private ZoneViewModel _currentZone;
        private string _currentRouteTarget = string.Empty;
        private IList<Route> _allRoutes = new List<Route>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteManager"/> class.
        /// </summary>
        /// <param name="MainWindowEx">The main window.</param>
        public RouteManager(Window MainWindowEx)
        {
            _rootModel = null;
            _mainWindow = MainWindowEx;

            SelectedRouteDestination = string.Empty;
        }

        /// <summary>
        /// Gets or sets the selected route destination.
        /// </summary>
        /// <value>
        /// The selected route destination.
        /// </value>
        [NotNull]
        public string SelectedRouteDestination
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the selected route.
        /// </summary>
        /// <value>
        /// The selected route.
        /// </value>
        [CanBeNull]
        public Route SelectedRoute
        {
            get;
            set;
        }

        /// <summary>
        /// Gets all routes.
        /// </summary>
        [NotNull]
        public IEnumerable<Route> AllRoutes
        {
            get
            {
                return _allRoutes;
            }
        }

        /// <summary>
        /// Gets the available destinations.
        /// </summary>
        [NotNull]
        public IEnumerable<string> AvailableDestinations
        {
            get
            {
                var res = new HashSet<string>();
                if (_currentRoom == null)
                {
                    return res;
                }

                var routesContainigCurrentRoom = _allRoutes.Where(r => r.RoomIdentifiersSet.Contains(_currentRoom.RoomId));
                foreach (var route in routesContainigCurrentRoom)
                {
                    res.UnionWith(route.RoutePointsAvailableFromStart.Keys);
                    res.UnionWith(route.RoutePointsAvailableFromEnd.Keys);
                }

                return res;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this route manager can create new route.
        /// </summary>
        public bool CanCreateNewRoute
        {
            get
            {
                return _currentRoom != null && string.IsNullOrEmpty(_currentRouteTarget) && _currentlyRecordedRoute == null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this route manager can cancel current route recording.
        /// </summary>
        public bool CanCancelCurrentRouteRecording
        {
            get
            {
                return _currentlyRecordedRoute != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this route manager stop current route recording.
        /// </summary>
        public bool CanStopCurrentRouteRecording
        {
            get
            {
                return _currentlyRecordedRoute != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this route manager can start route.
        /// </summary>
        public bool CanStartRoute
        {
            get
            {
                return string.IsNullOrEmpty(_currentRouteTarget) && _currentlyRecordedRoute == null && AvailableDestinations.Any();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this route manager can stop current route.
        /// </summary>
        public bool CanStopCurrentRoute
        {
            get
            {
                return !string.IsNullOrEmpty(_currentRouteTarget);
            }
        }

        /// <summary>
        /// Loads the routes.
        /// </summary>
        public void LoadRoutes()
        {
            var routesFileName = Path.Combine(GetMapsFolder(), "Routes.xml");

            if (File.Exists(routesFileName))
            {
                using (var inStream = File.OpenRead(routesFileName))
                {
                    _allRoutes = (List<Route>)_routesSerializer.Deserialize(inStream);
                }
            }

            RebuildRouteIndexes();
        }

        /// <summary>
        /// Updates the current room.
        /// </summary>
        /// <param name="newCurrentRoom">The new current room.</param>
        /// <param name="newCurrentZone">The new current zone.</param>
        public void UpdateCurrentRoom([CanBeNull] RoomViewModel newCurrentRoom, [NotNull] ZoneViewModel newCurrentZone)
        {
            Assert.ArgumentNotNull(newCurrentZone, "newCurrentZone");
            if (_rootModel == null)
            {
                return;
            }

            if (_isUpdateInProgress || _pendingUpdates.Count > 0)
            {
                _pendingUpdates.Enqueue(new Tuple<RoomViewModel, ZoneViewModel>(newCurrentRoom, newCurrentZone));
                return;
            }

            _isUpdateInProgress = true;

            UpdateCurrentRoomInternal(newCurrentZone, newCurrentRoom);

            while (_pendingUpdates.Count > 0)
            {
                var update = _pendingUpdates.Dequeue();
                UpdateCurrentRoomInternal(update.Item2, update.Item1);
            }

            _isUpdateInProgress = false;
        }

        /// <summary>
        /// Starts the new route recording.
        /// </summary>
        /// <returns><c>true</c> if route recording was started; otherwise - <c>false</c>.</returns>
        public bool StartNewRouteRecording()
        {
            string startName = string.Empty;
            if (_rootModel == null)
            {
                return false;
            } 
            
            if (_currentRoom == null)
            {
                return false;
            }

            var existingRoute = _allRoutes.FirstOrDefault(r => r.StartRoomId == _currentRoom.RoomId);
            if (existingRoute != null)
            {
                startName = existingRoute.StartName;
            }
            else
            {
                existingRoute = _allRoutes.FirstOrDefault(r => r.EndRoomId == _currentRoom.RoomId);
                if (existingRoute != null)
                {
                    startName = existingRoute.EndName;
                }
            }

            if (string.IsNullOrEmpty(startName))
            {
                SelectedRouteDestination = string.Empty;
                var roomStartDialog = new RoutePointNameEnterDialog { Owner = _mainWindow, DataContext = this };
                var result = roomStartDialog.ShowDialog();
                if (!(result.HasValue && result.Value))
                {
                    return false;
                }

                startName = SelectedRouteDestination;
            }

            return StartNewRouteRecording(startName);
        }

        /// <summary>
        /// Starts the new route recording.
        /// </summary>
        /// <param name="startName">The start name.</param>
        /// <returns><c>true</c> if route recording was started; otherwise - <c>false</c>.</returns>
        public bool StartNewRouteRecording([NotNull]string startName)
        {
            Assert.ArgumentNotNull(startName, "startName");

            if (_rootModel == null)
            {
                return false;
            } 
            
            if (_currentRoom == null)
            {
                return false;
            }

            var existingRoute = _allRoutes.FirstOrDefault(r => r.StartRoomId == _currentRoom.RoomId);
            if (existingRoute != null)
            {
                startName = existingRoute.StartName;
            }

            existingRoute = _allRoutes.FirstOrDefault(r => r.EndRoomId == _currentRoom.RoomId);
            if (existingRoute != null)
            {
                startName = existingRoute.EndName;
            }

            if (_allRoutes.Any(r => r.EndRoomId != _currentRoom.RoomId && string.Equals(r.EndName, startName, StringComparison.CurrentCultureIgnoreCase))
                || _allRoutes.Any(r => r.StartRoomId != _currentRoom.RoomId && string.Equals(r.StartName, startName, StringComparison.CurrentCultureIgnoreCase)))
            {
                _rootModel.PushMessageToConveyor(new ErrorMessage(string.Format(CultureInfo.InvariantCulture, Resources.RoutePointNameNotUnique, startName)));
                return false;
            }

            if (string.IsNullOrEmpty(startName))
            {
                _rootModel.PushMessageToConveyor(new ErrorMessage(Resources.RouteStartNameCanNotBeEmpty));
                return false;
            }

            var route = new Route { StartName = startName };
            route.RouteRoomIdentifiers.Add(_currentRoom.RoomId);
            _rootModel.PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.InvariantCulture, Resources.RouteRecordingStarted, startName), TextColor.BrightYellow));
            _currentlyRecordedRoute = route;
            _currentRoom.IsStartOrEndOfRoute = true;
            _currentRoom.IsPartOfRecordedRoute = true;
            return true;
        }

        /// <summary>
        /// Stops the route recording.
        /// </summary>
        /// <returns><c>true</c> if route recording was stopped; otherwise - <c>false</c>.</returns>
        public bool StopRouteRecording()
        {
            string endName = string.Empty;
            if (_rootModel == null)
            {
                return false; 
            }

            if (_currentRoom == null)
            {
                return false;
            }

            var existingRoute = _allRoutes.FirstOrDefault(r => r.StartRoomId == _currentRoom.RoomId);
            if (existingRoute != null)
            {
                endName = existingRoute.StartName;
            }
            else
            {
                existingRoute = _allRoutes.FirstOrDefault(r => r.EndRoomId == _currentRoom.RoomId);
                if (existingRoute != null)
                {
                    endName = existingRoute.EndName;
                }
            }

            if (string.IsNullOrEmpty(endName))
            {
                SelectedRouteDestination = string.Empty;
                var routeEnddialog = new RoutePointNameEnterDialog { Owner = _mainWindow, DataContext = this };
                var result = routeEnddialog.ShowDialog();
                if (!(result.HasValue && result.Value))
                {
                    return false;
                }

                endName = SelectedRouteDestination;
            }

            return StopRouteRecording(endName);
        }

        /// <summary>
        /// Stops route recording.
        /// </summary>
        /// <param name="endName">The end name.</param>
        /// <returns>
        ///   <c>true</c> if route recording was stopped; otherwise - <c>false</c>.
        /// </returns>
        public bool StopRouteRecording([NotNull]string endName)
        {
            Assert.ArgumentNotNull(endName, "endName");

            if (_rootModel == null)
            {
                return false;
            } 
            
            if (_currentlyRecordedRoute == null)
            {
                return false;
            }

            if (_currentRoom == null)
            {
                return false;
            }

            var existingRoute = _allRoutes.FirstOrDefault(r => r.StartRoomId == _currentRoom.RoomId);
            if (existingRoute != null)
            {
                endName = existingRoute.StartName;
            }

            existingRoute = _allRoutes.FirstOrDefault(r => r.EndRoomId == _currentRoom.RoomId);
            if (existingRoute != null)
            {
                endName = existingRoute.EndName;
            }

            if (_allRoutes.Any(r => r.EndRoomId != _currentRoom.RoomId && string.Equals(r.EndName, endName, StringComparison.CurrentCultureIgnoreCase))
                || _allRoutes.Any(r => r.StartRoomId != _currentRoom.RoomId && string.Equals(r.StartName, endName, StringComparison.CurrentCultureIgnoreCase)))
            {
                _rootModel.PushMessageToConveyor(new ErrorMessage(string.Format(CultureInfo.InvariantCulture, Resources.RoutePointNameNotUnique, endName)));
                return false;
            }

            if (string.IsNullOrEmpty(endName))
            {
                _rootModel.PushMessageToConveyor(new ErrorMessage(Resources.RouteStartNameCanNotBeEmpty));
                return false;
            }

            _currentlyRecordedRoute.EndName = endName;
            _rootModel.PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.InvariantCulture, Resources.RouteRecordingStopped, _currentlyRecordedRoute.StartName, _currentlyRecordedRoute.EndName), TextColor.BrightYellow));
            _currentRoom.IsStartOrEndOfRoute = true;
            _currentRoom.IsPartOfRecordedRoute = true;
            _allRoutes.Add(_currentlyRecordedRoute);
            _currentlyRecordedRoute = null;
            RebuildRouteIndexes();
            UpdateCurrentZoneRooms();
            SaveRoutes();
            return true;
        }

        /// <summary>
        /// Cancels route recording.
        /// </summary>
        public void CancelRouteRecording()
        {
            if (_rootModel == null)
            {
                return;
            } 
            
            if (_currentlyRecordedRoute == null)
            {
                return;
            }

            _currentlyRecordedRoute = null;
            _rootModel.PushMessageToConveyor(new InfoMessage(Resources.RouteRecordingCanceled, TextColor.BrightYellow));
            UpdateCurrentZoneRooms();
        }

        /// <summary>
        /// Deletes the route.
        /// </summary>
        /// <returns><c>true</c> if route was deleted; otherwise - <c>false</c></returns>
        public bool DeleteRoute()
        {
            if (_rootModel == null)
            {
                return false;
            }

            var deleteDialog = new RouteDeleteDialog { Owner = _mainWindow, DataContext = this };
            var res = deleteDialog.ShowDialog();
            if (res.HasValue && res.Value && SelectedRoute != null)
            {
                _allRoutes.Remove(SelectedRoute);
                RebuildRouteIndexes();
                UpdateCurrentZoneRooms();
                SaveRoutes();
                _rootModel.PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.InvariantCulture, Resources.RouteDeletedMessage, SelectedRoute.StartName, SelectedRoute.EndName), TextColor.BrightYellow));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Navigates to specified room.
        /// </summary>
        /// <param name="roomAliasOrName">Name of the room alias or.</param>
        public void NavigateToRoom([NotNull] string roomAliasOrName)
        {
            Assert.ArgumentNotNullOrWhiteSpace(roomAliasOrName, "roomAliasOrName");

            if (_rootModel == null)
            {
                return;
            }

            if (_currentRoom == null || _currentZone == null)
            {
                return;
            }

            var room = _currentZone.AllRooms.FirstOrDefault(r => string.Equals(r.Alias, roomAliasOrName, StringComparison.CurrentCultureIgnoreCase))
                       ??
                       _currentZone.AllRooms.FirstOrDefault(r => string.Equals(r.Name, roomAliasOrName, StringComparison.CurrentCultureIgnoreCase));

            if (room == null)
            {
                _rootModel.PushMessageToConveyor(new ErrorMessage(Resources.RouteSpecifiedRoomDoesNotExists));
                return;
            }

            NavigateToRoom(room);
        }

        /// <summary>
        /// Navigates to specified room.
        /// </summary>
        /// <param name="roomToNavigateTo">The room to navigate to.</param>
        public void NavigateToRoom([NotNull] RoomViewModel roomToNavigateTo)
        {
            Assert.ArgumentNotNull(roomToNavigateTo, "roomToNavigateTo");
            
            if (_rootModel == null)
            {
                return;
            }

            if (_currentRoom == null || _currentZone == null)
            {
                return;
            }

            var currentRoom = _currentRoom;
            foreach (var room in FindRouteToRoom(currentRoom, roomToNavigateTo))
            {
                var closureRoom = room;
                var exit = currentRoom.Exits.FirstOrDefault(ex => ex.Room == closureRoom);
                if (exit == null)
                {
                    _rootModel.PushMessageToConveyor(new ErrorMessage(Resources.RoomNavigationError));
                    return;
                }

                GotoDirection(exit.Direction);

                currentRoom = room;
            }
        }

        /// <summary>
        /// Shows the routes dialog.
        /// </summary>
        public void ShowRoutesDialog()
        {
            var dialog = new RoutesDialog { Owner = _mainWindow, DataContext = this };
            dialog.ShowDialog();
        }

        /// <summary>
        /// Starts routing to some destination.
        /// </summary>
        /// <returns><c>true</c> if routing was started; otherwise - <c>false</c>.</returns>
        public bool GotoDestination()
        {
            if (_rootModel == null)
            {
                return false;
            }

            if (_currentRoom == null)
            {
                return false;
            }

            if (!_allRoutes.Any(r => r.RoomIdentifiersSet.Contains(_currentRoom.RoomId)))
            {
                _rootModel.PushMessageToConveyor(new ErrorMessage(Resources.RoutesAreNotAvailable));
                return false;
            }

            var dialog = new RouteDestinationSelectDialog { Owner = _mainWindow, DataContext = this };
            var res = dialog.ShowDialog();
            if (res.HasValue && res.Value && !string.IsNullOrEmpty(SelectedRouteDestination))
            {
                return GotoDestination(SelectedRouteDestination);
            }

            return false;
        }

        /// <summary>
        /// Starts routing to some destination.
        /// </summary>
        /// <param name="selectedRouteDestination">The selected route destination.</param>
        /// <returns>
        ///   <c>true</c> if routing was started; otherwise - <c>false</c>.
        /// </returns>
        public bool GotoDestination([NotNull] string selectedRouteDestination)
        {
            Assert.ArgumentNotNull(selectedRouteDestination, "selectedRouteDestination");

            if (_rootModel == null)
            {
                return false;
            }

            if (_currentRoom == null)
            {
                return false;
            }

            var routesContainingCurrentRoom = _allRoutes.Where(r => r.RoomIdentifiersSet.Contains(_currentRoom.RoomId));
            if (!routesContainingCurrentRoom.Any())
            {
                _rootModel.PushMessageToConveyor(new ErrorMessage(Resources.RoutesAreNotAvailable));
                return false;
            }

            if (!(routesContainingCurrentRoom.Any(r => r.RoutePointsAvailableFromStart.ContainsKey(selectedRouteDestination))
                || routesContainingCurrentRoom.Any(r => r.RoutePointsAvailableFromEnd.ContainsKey(selectedRouteDestination))))
            {
                _rootModel.PushMessageToConveyor(new ErrorMessage(string.Format(CultureInfo.InvariantCulture, Resources.RouteTargetIsNotAvailable, selectedRouteDestination)));
                return false;
            }

            _currentRouteTarget = selectedRouteDestination;
            _rootModel.PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.InvariantCulture, Resources.RouteStarted, selectedRouteDestination), TextColor.BrightYellow));
            _groupMembersCountOnRouteStart = _rootModel.GroupStatus.Count(g => g.InSameRoom);
            UpdateCurrentRoom(_currentRoom, _currentZone);
            return true;
        }

        /// <summary>
        /// Stops the routing to destination.
        /// </summary>
        public void StopRoutingToDestination()
        {
            if (_rootModel == null)
            {
                return;
            }

            if(string.IsNullOrEmpty(_currentRouteTarget))
            {
                return;
            }

            _rootModel.PushMessageToConveyor(new InfoMessage(Resources.RouteStopped, TextColor.BrightYellow));
            _currentRouteTarget = string.Empty;
        }

        /// <summary>
        /// Prints the help.
        /// </summary>
        public void PrintHelp()
        {
            if(_rootModel==null)
            {
                return;
            }

            _rootModel.PushMessageToConveyor(new InfoMessage(Resources.RouteHelpGoto, TextColor.BrightYellow));
            _rootModel.PushMessageToConveyor(new InfoMessage(Resources.RouteHelpStartRecording, TextColor.BrightYellow));
            _rootModel.PushMessageToConveyor(new InfoMessage(Resources.RouteHelpStopRecording, TextColor.BrightYellow));
            _rootModel.PushMessageToConveyor(new InfoMessage(Resources.RouteHelpCancelRecording, TextColor.BrightYellow));
            _rootModel.PushMessageToConveyor(new InfoMessage(Resources.RouteHelpRoute, TextColor.BrightYellow));
            _rootModel.PushMessageToConveyor(new InfoMessage(Resources.RouteHelpStopRoute, TextColor.BrightYellow));
            _rootModel.PushMessageToConveyor(new InfoMessage(Resources.RouteHelpHelp, TextColor.BrightYellow));
        }

        /// <summary>
        /// Handles the change of main output window.
        /// </summary>
        /// <param name="rootModel">The root model of new output window.</param>
        public void OutputWindowChanged(RootModel rootModel)
        {
            CancelRouteRecording();
            StopRoutingToDestination();
            _rootModel = rootModel;
        }

        [NotNull]
        private static string GetMapsFolder()
        {
            return Path.Combine(SettingsHolder.Instance.Folder, "Maps");
        }

        [NotNull]
        private static IEnumerable<RoomViewModel> FindRouteToRoom([NotNull]RoomViewModel currentRoom, [NotNull] RoomViewModel roomToNavigateTo)
        {
            Assert.ArgumentNotNull(currentRoom, "currentRoom");
            Assert.ArgumentNotNull(roomToNavigateTo, "roomToNavigateTo");

            if (currentRoom == roomToNavigateTo)
            {
                return Enumerable.Empty<RoomViewModel>();
            }

            var visitedRooms = new HashSet<RoomViewModel>();
            var pathQueue = new Queue<RoutePathElement>();
            var currentElement = new RoutePathElement(currentRoom);
            while (true)
            {
                if (currentElement.Room == roomToNavigateTo)
                {
                    break;
                }

                foreach (var exit in currentElement.Room.Exits)
                {
                    if (exit.Room == null)
                    {
                        continue;
                    }

                    if (!visitedRooms.Contains(exit.Room))
                    {
                        pathQueue.Enqueue(new RoutePathElement(exit.Room, currentElement));
                        visitedRooms.Add(exit.Room);
                    }
                }

                if (pathQueue.Count == 0)
                {
                    return Enumerable.Empty<RoomViewModel>();
                }

                currentElement = pathQueue.Dequeue();
            }

            var result = new List<RoomViewModel>();
            while (currentElement.Previous != null)
            {
                result.Add(currentElement.Room);
                currentElement = currentElement.Previous;
            }

            result.Reverse();
            return result;
        }

        private void GotoDirection(ExitDirection exitDirection)
        {
            switch (exitDirection)
            {
                case ExitDirection.North:
                    _rootModel.PushCommandToConveyor(new TextCommand("north"));
                    break;
                case ExitDirection.South:
                    _rootModel.PushCommandToConveyor(new TextCommand("south"));
                    break;
                case ExitDirection.East:
                    _rootModel.PushCommandToConveyor(new TextCommand("east"));
                    break;
                case ExitDirection.West:
                    _rootModel.PushCommandToConveyor(new TextCommand("west"));
                    break;
                case ExitDirection.Up:
                    _rootModel.PushCommandToConveyor(new TextCommand("up"));
                    break;
                case ExitDirection.Down:
                    _rootModel.PushCommandToConveyor(new TextCommand("down"));
                    break;
                default:
                    _rootModel.PushMessageToConveyor(new ErrorMessage(Resources.RoomNavigationError));
                    break;
            }

            _rootModel.PushCommandToConveyor(FlushOutputQueueCommand.Instance);
        }

        private void RebuildRouteIndexes()
        {
            _routeEndRoomIdentifiers.Clear();
            _routeRoomIdentifiers.Clear();
            foreach (var route in _allRoutes)
            {
                _routeEndRoomIdentifiers.Add(route.EndRoomId);
                _routeEndRoomIdentifiers.Add(route.StartRoomId);
                _routeRoomIdentifiers.UnionWith(route.RouteRoomIdentifiers);
                route.RoutePointsAvailableFromStart.Clear();
                route.RoutePointsAvailableFromEnd.Clear();
                route.RoomIdentifiersSet.Clear();
                route.RoomIdentifiersSet.UnionWith(route.RouteRoomIdentifiers);
                var visitedRoutePoints = new HashSet<string> { route.StartName };
                var visitedRoutes = new HashSet<Route> { route };
                route.RoutePointsAvailableFromStart[route.StartName] = 0;
                TraverseRoute(route.StartName, visitedRoutePoints, visitedRoutes, route.RoutePointsAvailableFromStart, 1);

                visitedRoutePoints = new HashSet<string> { route.EndName };
                visitedRoutes = new HashSet<Route> { route };
                route.RoutePointsAvailableFromEnd[route.EndName] = 0;
                TraverseRoute(route.EndName, visitedRoutePoints, visitedRoutes, route.RoutePointsAvailableFromEnd, 1);
            }
        }

        private void TraverseRoute([NotNull] string routePointName, [NotNull] ISet<string> visitedRoutePoints, [NotNull] ISet<Route> visitedRoutes, [NotNull] IDictionary<string, int> routePointsAvailabilityList, int currentDepth)
        {
            Assert.ArgumentNotNullOrWhiteSpace(routePointName, "routePointName");
            Assert.ArgumentNotNull(visitedRoutePoints, "visitedRoutePoints");
            Assert.ArgumentNotNull(visitedRoutes, "visitedRoutes");
            Assert.ArgumentNotNull(routePointsAvailabilityList, "routePointsAvailabilityList");

            foreach (var nextRoute in _allRoutes.Except(visitedRoutes))
            {
                if (nextRoute.StartName == routePointName && !visitedRoutePoints.Contains(nextRoute.EndName))
                {
                    visitedRoutes.Add(nextRoute);
                    visitedRoutePoints.Add(nextRoute.EndName);
                    routePointsAvailabilityList[nextRoute.EndName] = currentDepth;
                    TraverseRoute(nextRoute.EndName, visitedRoutePoints, visitedRoutes, routePointsAvailabilityList, currentDepth + 1);
                }

                if (nextRoute.EndName == routePointName && !visitedRoutePoints.Contains(nextRoute.StartName))
                {
                    visitedRoutes.Add(nextRoute);
                    visitedRoutePoints.Add(nextRoute.StartName);
                    routePointsAvailabilityList[nextRoute.StartName] = currentDepth;
                    TraverseRoute(nextRoute.StartName, visitedRoutePoints, visitedRoutes, routePointsAvailabilityList, currentDepth + 1);
                }
            }
        }

        private void UpdateCurrentZoneRooms()
        {
            if (_currentZone == null)
            {
                return;
            }

            foreach (var room in _currentZone.AllRooms)
            {
                room.IsStartOrEndOfRoute = _routeEndRoomIdentifiers.Contains(room.RoomId);
                room.IsPartOfRoute = _routeRoomIdentifiers.Contains(room.RoomId);
                room.IsPartOfRecordedRoute = _currentlyRecordedRoute != null ? _currentlyRecordedRoute.RouteRoomIdentifiers.Contains(room.RoomId) : false;
            }
        }

        private void SaveRoutes()
        {
            if (!Directory.Exists(GetMapsFolder()))
            {
                Directory.CreateDirectory(GetMapsFolder());
            }

            var routesFileName = Path.Combine(GetMapsFolder(), "Routes.xml");
            using (var outStream = File.Open(routesFileName, FileMode.Create, FileAccess.Write))
            using (var streamWriter = new XmlTextWriter(outStream, Encoding.UTF8))
            {
                streamWriter.Formatting = Formatting.Indented;
                _routesSerializer.Serialize(streamWriter, _allRoutes);
            }
        }

        private void UpdateCurrentRoomInternal([NotNull] ZoneViewModel newCurrentZone, [CanBeNull] RoomViewModel newCurrentRoom)
        {
            Assert.ArgumentNotNull(newCurrentZone, "newCurrentZone");

            if (_currentlyRecordedRoute != null && newCurrentRoom != null)
            {
                var lastRoom = _currentlyRecordedRoute.RouteRoomIdentifiers.LastOrDefault();
                if (lastRoom != newCurrentRoom.RoomId)
                {
                    _currentlyRecordedRoute.RouteRoomIdentifiers.Add(newCurrentRoom.RoomId);
                    if (_routeEndRoomIdentifiers.Contains(newCurrentRoom.RoomId) && newCurrentRoom.RoomId != _currentlyRecordedRoute.StartRoomId)
                    {
                        var existingRoute = _allRoutes.FirstOrDefault(r => r.StartRoomId == _currentlyRecordedRoute.StartRoomId && r.EndRoomId == newCurrentRoom.RoomId);
                        existingRoute = existingRoute ?? _allRoutes.FirstOrDefault(r => r.EndRoomId == _currentlyRecordedRoute.StartRoomId && r.StartRoomId == newCurrentRoom.RoomId);
                        _currentRoom = newCurrentRoom;
                        _currentZone = newCurrentZone;

                        if (existingRoute != null)
                        {
                            var yesNoDialog = new YesNoDialog
                            {
                                Owner = _mainWindow,
                                Title = Resources.RouteAlreadyExistsTitle,
                                TextToDisplay = string.Format(CultureInfo.InvariantCulture, Resources.RouteAlreadyExistsQuestion, existingRoute.StartName, existingRoute.EndName)
                            };

                            var res = yesNoDialog.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                StopRouteRecording();
                                StartNewRouteRecording();
                                _allRoutes.Remove(existingRoute);
                                RebuildRouteIndexes();
                            }
                            else
                            {
                                _currentlyRecordedRoute = null;
                                StartNewRouteRecording();
                            }
                        }
                        else
                        {
                            StopRouteRecording();
                            StartNewRouteRecording();
                        }

                        UpdateCurrentZoneRooms();
                    }

                    newCurrentRoom.IsPartOfRecordedRoute = true;
                }
            }

            if (!string.IsNullOrEmpty(_currentRouteTarget) && newCurrentRoom != null)
            {
                var routesContainigCurrentRoom = _allRoutes.Where(r => r.RoomIdentifiersSet.Contains(newCurrentRoom.RoomId));
                Route minRoute = null;
                int minDestinationLength = int.MaxValue;
                bool gotoStart = false;
                bool targetAchieved = false;

                foreach (var route in routesContainigCurrentRoom)
                {
                    if (route.StartRoomId == newCurrentRoom.RoomId && string.Equals(_currentRouteTarget, route.StartName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        targetAchieved = true;
                        break;
                    }

                    if (route.EndRoomId == newCurrentRoom.RoomId && string.Equals(_currentRouteTarget, route.EndName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        targetAchieved = true;
                        break;
                    }

                    if (route.RoutePointsAvailableFromStart.ContainsKey(_currentRouteTarget))
                    {
                        if (route.StartRoomId != newCurrentRoom.RoomId
                            && route.RoutePointsAvailableFromStart[_currentRouteTarget] < minDestinationLength)
                        {
                            gotoStart = true;
                            minDestinationLength = route.RoutePointsAvailableFromStart[_currentRouteTarget];
                            minRoute = route;
                        }
                    }

                    if (route.RoutePointsAvailableFromEnd.ContainsKey(_currentRouteTarget))
                    {
                        if (route.EndRoomId != newCurrentRoom.RoomId && route.RoutePointsAvailableFromEnd[_currentRouteTarget] < minDestinationLength)
                        {
                            gotoStart = false;
                            minDestinationLength = route.RoutePointsAvailableFromEnd[_currentRouteTarget];
                            minRoute = route;
                        }
                    }
                }

                if (targetAchieved)
                {
                    _rootModel.PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.InvariantCulture, Resources.RouteTargetAchieved, _currentRouteTarget), TextColor.BrightYellow));
                    StopRoutingToDestination();
                }
                else if (minRoute == null)
                {
                    _rootModel.PushMessageToConveyor(new ErrorMessage(string.Format(CultureInfo.InvariantCulture, Resources.RouteTargetIsNotAvailable, _currentRouteTarget)));
                    StopRoutingToDestination();
                }
                else if (_rootModel.GroupStatus.Count(gr => gr.InSameRoom) != _groupMembersCountOnRouteStart || _rootModel.GroupStatus.Any(gr => gr.InSameRoom && gr.MovesPercent < 2))
                {
                    _rootModel.PushMessageToConveyor(new ErrorMessage(Resources.RouteGroupMateLostOrTired));
                    StopRoutingToDestination();
                }
                else
                {
                    var currentRoomIndex = minRoute.RouteRoomIdentifiers.IndexOf(newCurrentRoom.RoomId);
                    int nextRoomId = gotoStart
                                         ? minRoute.RouteRoomIdentifiers[currentRoomIndex - 1]
                                         : minRoute.RouteRoomIdentifiers[currentRoomIndex + 1];
                    var exit = newCurrentRoom.Room.Exits.FirstOrDefault(ex => ex.RoomId == nextRoomId);
                    if (exit == null)
                    {
                        _rootModel.PushMessageToConveyor(new ErrorMessage(string.Format(CultureInfo.InvariantCulture, Resources.RouteTargetIsNotAvailable, _currentRouteTarget)));
                        StopRoutingToDestination();
                    }
                    else
                    {
                        GotoDirection(exit.Direction);
                    }
                }
            }

            var prevZone = _currentZone;
            _currentRoom = newCurrentRoom;
            _currentZone = newCurrentZone;
            if (prevZone == null || prevZone.Id != _currentZone.Id)
            {
                UpdateCurrentZoneRooms();
            }
        }
    }
}
