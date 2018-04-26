namespace Adan.Client.Map
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Timers;
    using System.Windows;
    using System.Xml;
    using System.Xml.Serialization;
    using Common.Commands;
    using Common.Model;
    using Common.Settings;
    using Common.Utils;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Dialogs;
    using Model;
    using Properties;
    using SharpAESCrypt;
    using ViewModel;

    /// <summary>
    /// Class to manage zone loading/saving etc.
    /// </summary>
    public class ZoneManager : IDisposable
    {
        private readonly XmlSerializer _zoneSerializer = new XmlSerializer(typeof(Zone));

        private readonly object _syncRoot = new object();
        private readonly ZoneViewModel _emptyZone;

        [CanBeNull]
        private readonly MapControl _mapControl;

        [CanBeNull]
        private readonly Window _mainWindow;
        private readonly RouteManager _routeManger;
        private readonly ConcurrentDictionary<int, ZoneViewModel> _loadedZones = new ConcurrentDictionary<int, ZoneViewModel>();
        private readonly Timer _timer;

        private readonly Dictionary<string, ZoneHolder> _zoneHolders = new Dictionary<string, ZoneHolder>();
        private XmlSerializer _additionalRoomParametersSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneManager"/> class.
        /// </summary>
        /// <param name="mapControl">The map control.</param>
        /// <param name="MainWindowEx">The main window.</param>
        /// <param name="routeManger">The route manger.</param>
        public ZoneManager([CanBeNull] MapControl mapControl, [CanBeNull] Window MainWindowEx, [NotNull] RouteManager routeManger)
        {
            Assert.ArgumentNotNull(routeManger, "routeManger");

            _mapControl = mapControl;
            _mainWindow = MainWindowEx;
            _routeManger = routeManger;
            _emptyZone = new ZoneViewModel(new Zone { Id = -1000 }, Enumerable.Empty<AdditionalRoomParameters>()) { ZoomLevel = Settings.Default.MapZoomLevel };
            _timer = new Timer(5000) { AutoReset = false };
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
            if (_mapControl != null)
            {
                _mapControl.RoadMapShowRequired += ShowRoadMap;
                _mapControl.RoomEditDialogRequired += ShowRoomEditDialog;
                _mapControl.NavigateToRoomRequired += NavigateToRoom;
                _mapControl.RoutesDialogShowRequired += (o, e) => _routeManger.ShowRoutesDialog();
            }

            MapDownloader.UpgradeComplete += MapDownloader_UpgradeComplete;
        }

        private void MapDownloader_UpgradeComplete(object sender, EventArgs e)
        {
            if (_mapControl?.ViewModel != null)
            {
                if (_zoneHolders.ContainsKey(_mapControl.ViewModelUid))
                {
                    UpdateControl(_zoneHolders[_mapControl.ViewModelUid]);
                }
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UnloadUnusedZones();
            _timer.Start();
        }

        private void UnloadUnusedZones()
        {
            if (!Directory.Exists(GetZoneVisitsFolder()))
            {
                try
                {
                    Directory.CreateDirectory(GetZoneVisitsFolder());
                }
                catch (Exception)
                {
                    return;
                }
            }

            lock (_syncRoot)
            {
                foreach (var zoneViewModel in _loadedZones.Values.ToList())
                {
                    if (_mapControl == null || zoneViewModel.Id == _mapControl.ViewModel.Id)
                        continue;

                    ZoneViewModel tempVal;

                    SaveAdditionalRoomParameters(zoneViewModel);
#if DEBUG
                    SaveZoneDebugInfo(zoneViewModel);
#endif

                    _loadedZones.TryRemove(zoneViewModel.Id, out tempVal);
                }
            }
        }

#if DEBUG
        private void SaveZoneDebugInfo(ZoneViewModel zoneViewModel)
        {
            var zoneMapFileName = Path.Combine(GetZoneVisitsFolder(), zoneViewModel.Id.ToString(CultureInfo.InvariantCulture) + ".map");
            if (!File.Exists(zoneMapFileName) || zoneViewModel.AllRooms.Any(r => r.AdditionalRoomParameters.HasChanges))
            {
                using (var outStream = File.Open(zoneMapFileName, FileMode.Create, FileAccess.Write))
                using (var streamWriter = new StreamWriter(outStream))
                {
                    foreach (var room in zoneViewModel.AllRooms)
                    {
                        streamWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}", room.RoomId, room.XLocation, room.YLocation, room.ZLocation));
                    }
                }
            }
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();

            foreach (var zoneViewModel in _loadedZones.Values.ToArray())
            {
                SaveAdditionalRoomParameters(zoneViewModel);
#if DEBUG
                SaveZoneDebugInfo(zoneViewModel);
#endif
            }

            Settings.Default.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public void OutputWindowCreated(RootModel rootModel)
        {
            _zoneHolders.Add(rootModel.Uid, new ZoneHolder(this, rootModel));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public void OutputWindowChanged(RootModel rootModel)
        {
            var uid = rootModel.Uid;

            if (_mapControl == null)
            {
                return;
            }

            if (_mapControl.ViewModel == null)
            {
                _mapControl.ViewModelUid = uid;
                var zoneViewModel = GetZone(_zoneHolders[uid].ZoneId);

                _mapControl.UpdateCurrentZone(zoneViewModel ?? _emptyZone, null);

                return;
            }

            if (_zoneHolders.ContainsKey(uid) && _mapControl.ViewModelUid != uid)
            {
                _mapControl.ViewModelUid = uid;

                var zoneHolder = _zoneHolders[uid];

                if (_mapControl.ViewModel.Id != zoneHolder.ZoneId)
                {
                    var zoneViewModel = GetZone(_zoneHolders[uid].ZoneId);
                    if (zoneViewModel != null)
                    {
                        var room = zoneViewModel.AllRooms.FirstOrDefault(r => r.RoomId == zoneHolder.RoomId);
                        _mapControl.UpdateCurrentZone(zoneViewModel, room);
                    }
                    else
                        _mapControl.UpdateCurrentZone(_emptyZone, null);
                }
                else
                {
                    _mapControl.UpdateCurrentRoom(zoneHolder.RoomId);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        public void OutputWindowClosed(string uid)
        {
            if (_zoneHolders.ContainsKey(uid))
                _zoneHolders.Remove(uid);
        }

        [NotNull]
        private XmlSerializer AdditionalRoomParametersSerializer
        {
            get
            {
                if (_additionalRoomParametersSerializer == null)
                {
                    _additionalRoomParametersSerializer = new XmlSerializer(typeof(List<AdditionalRoomParameters>), SettingsHolder.Instance.AllSerializationTypes.ToArray());
                }

                return _additionalRoomParametersSerializer;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        public ZoneViewModel GetZone(int zoneId)
        {
            ZoneViewModel result;

            lock (_syncRoot)
            {
                if (!_loadedZones.TryGetValue(zoneId, out result))
                {
                    result = LoadZone(zoneId) ?? _emptyZone;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoneHolder"></param>
        public void UpdateControl(ZoneHolder zoneHolder)
        {
            if (_mapControl?.ViewModel != null && _mapControl.ViewModelUid == zoneHolder.Uid)
            {
                if (_mapControl.ViewModel.Id != zoneHolder.ZoneId)
                {
                    var zoneViewModel = GetZone(zoneHolder.ZoneId);
                    var room = zoneViewModel.AllRooms.FirstOrDefault(r => r.RoomId == zoneHolder.RoomId);

                    _mapControl.UpdateCurrentZone(zoneViewModel, room);
                }
                else
                {
                    _mapControl.UpdateCurrentRoom(zoneHolder.RoomId);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoneHolder"></param>
        public void ExecuteRoomAction(ZoneHolder zoneHolder)
        {
            ZoneViewModel zone = GetZone(zoneHolder.ZoneId);
            var room = zone?.AllRooms.FirstOrDefault(r => r.RoomId == zoneHolder.RoomId);
            if (room != null && room.AdditionalRoomParameters.ActionsToExecuteOnRoomEntry.Any())
            {
                foreach (var action in room.AdditionalRoomParameters.ActionsToExecuteOnRoomEntry)
                {
                    action.Execute(zoneHolder.RootModel, ActionExecutionContext.Empty);
                }

                zoneHolder.RootModel.PushCommandToConveyor(FlushOutputQueueCommand.Instance);
            }
        }

        [NotNull]
        private static string GetZonesFolder()
        {
            return Path.Combine(SettingsHolder.Instance.Folder, "Maps", "MapGenerator", "MapResults");
        }

        [NotNull]
        private static string GetZoneVisitsFolder()
        {
            return Path.Combine(SettingsHolder.Instance.Folder, "Maps", "ZoneVisits");
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "It's ok here.")]
        [CanBeNull]
        private ZoneViewModel LoadZone(int zoneId)
        {
            ZoneViewModel zoneViewModel = null;
            var zoneVisits = new List<AdditionalRoomParameters>();

            lock (_syncRoot)
            {
                Zone loadedZone;
                try
                {
                    ZoneViewModel result;
                    if (_loadedZones.TryGetValue(zoneId, out result))
                    {
                        result.BuildCurrentLevelRooms();
                        return result;
                    }

                    if (MapDownloader.IsUpgrading)
                        return null;

                    loadedZone = LoadZoneFromFile(zoneId.ToString(CultureInfo.InvariantCulture) + ".xml");

                    if (loadedZone == null)
                        return null;
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
                                    //TODO: Почему Single() ?
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
                }
                catch
                {
                }
                zoneViewModel = new ZoneViewModel(loadedZone, zoneVisits);
                _loadedZones.TryAdd(zoneId, zoneViewModel);
            }


            return zoneViewModel;
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
                    var aesStream = new SharpAESCrypt("A5Ub5T7j5cYg40v", inStream, OperationMode.Decrypt);
                    using (var streamReader = new StreamReader(aesStream, Encoding.GetEncoding(20866)))
                    {
                        return (Zone)_zoneSerializer.Deserialize(streamReader);
                    }
                }
            }

            return null;
        }

        private void SaveAdditionalRoomParameters([NotNull] ZoneViewModel zoneViewModel)
        {
            Assert.ArgumentNotNull(zoneViewModel, "zoneViewModel");
            if (!zoneViewModel.AllRooms.Any(r => r.AdditionalRoomParameters.HasChanges))
            {
                return;
            }

            var zoneVisitsFileName = Path.Combine(GetZoneVisitsFolder(), zoneViewModel.Id.ToString(CultureInfo.InvariantCulture) + ".xml");
            using (var outStream = File.Open(zoneVisitsFileName, FileMode.Create, FileAccess.Write))
            using (var streamWriter = new XmlTextWriter(outStream, Encoding.UTF8))
            {
                streamWriter.Formatting = Formatting.Indented;

                var tmp = zoneViewModel.AllRooms.Select(r => r.AdditionalRoomParameters).ToList();
                try
                {
                    AdditionalRoomParametersSerializer.Serialize(streamWriter, tmp);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK);
                    ErrorLogger.Instance.Write($"Error save additional room parameters: {ex.Message}\r\n{ex.StackTrace}");
                }
            }
        }

        private void ShowRoadMap([NotNull] object sender, [NotNull] EventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (_mapControl == null)
            {
                return;
            }

            var zone = LoadZoneFromFile("roads.xml");

            if (zone == null)
                return;

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
            var zoneViewModel = new ZoneViewModel(zone, new List<AdditionalRoomParameters>()) { CurrentLevel = -201, ZoomLevel = 0.2f };

            foreach (var room in zoneViewModel.AllRooms)
            {
                room.AdditionalRoomParameters.HasBeenVisited = true;
            }

            RoomViewModel currentRoom = null;
            if (_mapControl.ViewModel.CurrentRoom != null)
            {
                var currentRoomId = _mapControl.ViewModel.CurrentRoom.RoomId;
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

            _routeManger.NavigateToRoom(roomToNavigateTo);
        }
    }
}
