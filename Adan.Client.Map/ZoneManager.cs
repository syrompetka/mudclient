// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoneManager.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ZoneManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Xml;
    using System.Xml.Serialization;
    using Adan.Client.Common;
    using Adan.Client.Common.Messages;
    using Common.Model;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Dialogs;
    using Model;
    using Properties;
    using ViewModel;
    using OperationMode = SharpAESCrypt.OperationMode;

    /// <summary>
    /// Class to manage zone loading/saving etc.
    /// </summary>
    public class ZoneManager
    {
        private readonly XmlSerializer _zoneSerializer = new XmlSerializer(typeof(Zone));
        private readonly XmlSerializer _zoneVisitsSerializer = new XmlSerializer(typeof(List<int>));

        private readonly object _syncRoot = new object();
        private readonly ZoneViewModel _emptyZone;
        private readonly MapControl _mapControl;
        private readonly Window _mainWindow;
        private readonly RootModel _rootModel;
        private readonly RouteManager _routeManger;
        private readonly ConcurrentDictionary<int, ZoneViewModel> _loadedZones = new ConcurrentDictionary<int, ZoneViewModel>();

        private ZoneViewModel _currentZone;
        private XmlSerializer _additionalRoomParametersSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneManager"/> class.
        /// </summary>
        /// <param name="mapControl">The map control.</param>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="rootModel">The root model.</param>
        /// <param name="routeManger">The route manger.</param>
        public ZoneManager([NotNull] MapControl mapControl, [NotNull] Window mainWindow, [NotNull] RootModel rootModel, [NotNull] RouteManager routeManger)
        {
            Assert.ArgumentNotNull(mapControl, "mapControl");
            Assert.ArgumentNotNull(mainWindow, "mainWindow");
            Assert.ArgumentNotNull(rootModel, "rootModel");
            Assert.ArgumentNotNull(routeManger, "routeManger");

            _mapControl = mapControl;
            _mainWindow = mainWindow;
            _rootModel = rootModel;
            _routeManger = routeManger;
            _emptyZone = new ZoneViewModel(new Zone { Id = -1000 }, Enumerable.Empty<AdditionalRoomParameters>(), _rootModel) { ZoomLevel = Settings.Default.MapZoomLevel };
            _currentZone = _emptyZone;
            _mapControl.RoadMapShowRequired += ShowRoadMap;
            _mapControl.RoomEditDialogRequired += ShowRoomEditDialog;
            _mapControl.NavigateToRoomRequired += NavigateToRoom;
            _mapControl.RoutesDialogShowRequired += (o, e) => _routeManger.ShowRoutesDialog();
        }

        [NotNull]
        private XmlSerializer AdditionalRoomParametersSerializer
        {
            get
            {
                if (_additionalRoomParametersSerializer == null)
                {
                    _additionalRoomParametersSerializer = new XmlSerializer(typeof(List<AdditionalRoomParameters>), _rootModel.CustomSerializationTypes.ToArray());
                }

                return _additionalRoomParametersSerializer;
            }
        }

        /// <summary>
        /// Updates the current room.
        /// </summary>
        /// <param name="roomId">The room id.</param>
        /// <param name="zoneId">The zone id.</param>
        public void UpdateCurrentRoom(int roomId, int zoneId)
        {
            if (zoneId != _currentZone.Id)
            {
                ZoneViewModel zone;
                if (_loadedZones.TryGetValue(zoneId, out zone))
                {
                    zone.ZoomLevel = _currentZone.ZoomLevel;
                    _currentZone = zone;
                    var currentRoom = zone.AllRooms.FirstOrDefault(r => r.RoomId == roomId);
                    _mapControl.UpdateCurrentZone(zone, currentRoom);

                    Task.Factory.StartNew(() => 
                        {
                            try
                            {
                                UnloadUnusedZones();
                            }
                            catch (Exception ex)
                            {
                                _rootModel.PushMessageToConveyor(new ErrorMessage(ex.ToString()));
                            }
                        });
                }
                else
                {
                    _emptyZone.ZoomLevel = _currentZone.ZoomLevel;
                    _currentZone = _emptyZone;
                    _mapControl.UpdateCurrentZone(_emptyZone, null);

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            FetchZone(zoneId, roomId);
                        }
                        catch (Exception ex)
                        {
                            _rootModel.PushMessageToConveyor(new ErrorMessage(ex.ToString()));
                        }
                    });
                }
            }
            else
            {
                var currentRoom = _currentZone.AllRooms.FirstOrDefault(r => r.RoomId == roomId);
                if (_currentZone.CurrentRoom == null || currentRoom.RoomId != _currentZone.CurrentRoom.RoomId)
                {
                    foreach (var action in currentRoom.AdditionalRoomParameters.ActionsToExecuteOnRoomEntry)
                    {
                        action.Execute(_rootModel, ActionExecutionContext.Empty);
                    }

                    _mapControl.UpdateCurrentRoom(currentRoom);
                }
            }
        }

        [NotNull]
        private static string GetZonesFolder()
        {
            return Path.Combine(ProfileHolder.Instance.Folder, "Maps", "MapGenerator", "MapResults");
        }

        [NotNull]
        private static string GetZoneVisitsFolder()
        {
            return Path.Combine(ProfileHolder.Instance.Folder, "Maps", "ZoneVisits");
        }

        private void FetchZone(int zoneId, int roomId)
        {
            var zone = LoadZone(zoneId);
            if (zone != null)
            {
                _loadedZones.TryAdd(zoneId, zone);
                var room = zone.AllRooms.FirstOrDefault(r => r.RoomId == roomId);
                zone.ZoomLevel = _currentZone.ZoomLevel;
                Settings.Default.MapZoomLevel = zone.ZoomLevel;
                Settings.Default.Save();
                _currentZone = zone;
                _mapControl.UpdateCurrentZone(zone, room);
                UnloadUnusedZones();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "It's ok here.")]
        [CanBeNull]
        private ZoneViewModel LoadZone(int zoneId)
        {
            Zone loadedZone;
            var zoneVisits = new List<AdditionalRoomParameters>();

            lock (_syncRoot)
            {
                try
                {
                    ZoneViewModel result;
                    if (_loadedZones.TryGetValue(zoneId, out result))
                    {
                        return result;
                    }

                    loadedZone = LoadZoneFromFile(zoneId.ToString(CultureInfo.InvariantCulture) + ".xml");
                    if (loadedZone == null)
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }

                try
                {
                    var zoneVisitsFileName = Path.Combine(GetZoneVisitsFolder(), zoneId.ToString(CultureInfo.InvariantCulture) + ".xml");
                    if (File.Exists(zoneVisitsFileName))
                    {
                        using (var inStream = File.OpenRead(zoneVisitsFileName))
                        {
                            zoneVisits = (List<AdditionalRoomParameters>)AdditionalRoomParametersSerializer.Deserialize(inStream);
                        }
                    }

#if DEBUG
                    var zoneMapFileName = Path.Combine(GetZoneVisitsFolder(), zoneId.ToString(CultureInfo.InvariantCulture) + ".map");
                    if (File.Exists(zoneMapFileName))
                    {
                        using (var reader = new StreamReader(zoneMapFileName))
                        {
                            var line = reader.ReadLine();
                            while (line != null)
                            {
                                var lineContent = line.Split(' ');
                                if (lineContent.Length == 4)
                                {
                                    var roomId = int.Parse(lineContent[0], CultureInfo.InvariantCulture);
                                    var x = int.Parse(lineContent[1], CultureInfo.InvariantCulture);
                                    var y = int.Parse(lineContent[2], CultureInfo.InvariantCulture);
                                    var z = int.Parse(lineContent[3], CultureInfo.InvariantCulture);
                                    var room = loadedZone.Rooms.Single(r => r.Id == roomId);
                                    room.XLocation = x;
                                    room.YLocation = y;
                                    room.ZLocation = z;
                                }

                                line = reader.ReadLine();
                            }
                        }
                    }
#endif
                    return new ZoneViewModel(loadedZone, zoneVisits, _rootModel);
                }
                catch
                {
                    // legacy format support - remove in next version.
                    var zoneVisitsFileName = Path.Combine(GetZoneVisitsFolder(), zoneId.ToString(CultureInfo.InvariantCulture) + ".xml");
                    if (File.Exists(zoneVisitsFileName))
                    {
                        using (var inStream = File.OpenRead(zoneVisitsFileName))
                        {
                            var visits = (List<int>)_zoneVisitsSerializer.Deserialize(inStream);
                            foreach (var visit in visits)
                            {
                                zoneVisits.Add(new AdditionalRoomParameters { RoomId = visit, HasBeenVisited = true });
                            }
                        }
                    }
                }
            }

            return new ZoneViewModel(loadedZone, zoneVisits, _rootModel);
        }

        /// <summary>
        /// Loads the zone from file.
        /// </summary>
        /// <param name="zoneFileName">Name of the zone file.</param>
        /// <returns>A loaded zone or <c>null</c> if specified file does not exists.</returns>
        [CanBeNull]
        private Zone LoadZoneFromFile([NotNull] string zoneFileName)
        {
            Assert.ArgumentNotNullOrWhiteSpace(zoneFileName, "zoneFileName");

            if (File.Exists(Path.Combine(GetZonesFolder(), zoneFileName)))
            {
                using (var inStream = File.OpenRead(Path.Combine(GetZonesFolder(), zoneFileName)))
                {
                    var aesStream = new SharpAESCrypt.SharpAESCrypt("A5Ub5T7j5cYg40v", inStream, OperationMode.Decrypt);
                    using (var streamReader = new StreamReader(aesStream, Encoding.GetEncoding(20866)))
                    {
                        return (Zone)_zoneSerializer.Deserialize(streamReader);
                    }
                }
            }

            return null;
        }

        private void UnloadUnusedZones()
        {
            if (!Directory.Exists(GetZoneVisitsFolder()))
            {
                Directory.CreateDirectory(GetZoneVisitsFolder());
            }

            lock (_syncRoot)
            {
                foreach (var zoneViewModel in _loadedZones.Values.ToList())
                {
                    if (zoneViewModel == _currentZone)
                    {
                        continue;
                    }

                    ZoneViewModel tempVal;
                    _loadedZones.TryRemove(zoneViewModel.Id, out tempVal);

                    SaveAdditionalRoomParameters(zoneViewModel);

#if DEBUG
                    var zoneMapFileName = Path.Combine(GetZoneVisitsFolder(), zoneViewModel.Id.ToString(CultureInfo.InvariantCulture) + ".map");
                    using (var outStream = File.Open(zoneMapFileName, FileMode.Create, FileAccess.Write))
                    using (var streamWriter = new StreamWriter(outStream))
                    {
                        foreach (var room in zoneViewModel.AllRooms)
                        {
                            streamWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}", room.RoomId, room.XLocation, room.YLocation, room.ZLocation));
                        }
                    }
#endif
                }
            }
        }

        private void SaveAdditionalRoomParameters([NotNull] ZoneViewModel zoneViewModel)
        {
            Assert.ArgumentNotNull(zoneViewModel, "zoneViewModel");

            var zoneVisitsFileName = Path.Combine(GetZoneVisitsFolder(), zoneViewModel.Id.ToString(CultureInfo.InvariantCulture) + ".xml");
            using (var outStream = File.Open(zoneVisitsFileName, FileMode.Create, FileAccess.Write))
            using (var streamWriter = new XmlTextWriter(outStream, Encoding.UTF8))
            {
                streamWriter.Formatting = Formatting.Indented;
                AdditionalRoomParametersSerializer.Serialize(streamWriter, zoneViewModel.AllRooms.Select(r => r.AdditionalRoomParameters).ToList());
            }
        }

        private void ShowRoadMap([NotNull] object sender, [NotNull] EventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var zone = LoadZoneFromFile("roads.xml");
            if (zone == null)
            {
                return;
            }

#if DEBUG
            var zoneMapFileName = Path.Combine(GetZoneVisitsFolder(), "roads.map");
            if (File.Exists(zoneMapFileName))
            {
                using (var reader = new StreamReader(zoneMapFileName))
                {
                    var line = reader.ReadLine();
                    while (line != null)
                    {
                        var lineContent = line.Split(' ');
                        if (lineContent.Length == 4)
                        {
                            var roomId = int.Parse(lineContent[0], CultureInfo.InvariantCulture);
                            var x = int.Parse(lineContent[1], CultureInfo.InvariantCulture);
                            var y = int.Parse(lineContent[2], CultureInfo.InvariantCulture);
                            var z = int.Parse(lineContent[3], CultureInfo.InvariantCulture);
                            var room = zone.Rooms.Single(r => r.Id == roomId);
                            room.XLocation = x;
                            room.YLocation = y;
                            room.ZLocation = z;
                        }

                        line = reader.ReadLine();
                    }
                }
            }
#endif
            var roadMapDialog = new RoadMapDialog { Owner = _mainWindow };
            var zoneViewModel = new ZoneViewModel(zone, new List<AdditionalRoomParameters>(), _rootModel) { CurrentLevel = -201, ZoomLevel = 0.2f };
            foreach (var room in zoneViewModel.AllRooms)
            {
                room.AdditionalRoomParameters.HasBeenVisited = true;
            }

            RoomViewModel currentRoom = null;
            if (_currentZone.CurrentRoom != null)
            {
                var currentRoomId = _currentZone.CurrentRoom.RoomId;
                currentRoom = zoneViewModel.CurrentLevelRooms.FirstOrDefault(r => r.RoomId == currentRoomId);
            }

            roadMapDialog.MapControl.UpdateCurrentZone(zoneViewModel, currentRoom);
            roadMapDialog.ShowDialog();
#if DEBUG
            using (var outStream = File.Open(zoneMapFileName, FileMode.Create, FileAccess.Write))
            using (var streamWriter = new StreamWriter(outStream))
            {
                foreach (var room in zone.Rooms)
                {
                    streamWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}", room.Id, room.XLocation, room.YLocation, room.ZLocation));
                }
            }
#endif
        }

        private void ShowRoomEditDialog([NotNull] RoomViewModel room)
        {
            Assert.ArgumentNotNull(room, "room");

            var editDialog = new RoomEditDialog { DataContext = room.Clone(), Owner = _mainWindow };
            var result = editDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                room.Update((RoomViewModel)editDialog.DataContext);
                SaveAdditionalRoomParameters(room.Zone);
            }
        }

        private void NavigateToRoom([NotNull]RoomViewModel roomToNavigateTo)
        {
            Assert.ArgumentNotNull(roomToNavigateTo, "roomToNavigateTo");

            if (_currentZone.CurrentRoom != null)
            {
                _routeManger.NavigateToRoom(roomToNavigateTo);
            }
        }
    }
}
