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
    using System.Windows.Threading;
    using System.Xml;
    using System.Xml.Serialization;
    using Adan.Client.Common;
    using Adan.Client.Common.Messages;
    using Adan.Client.Common.Settings;
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
    public class ZoneManager : IDisposable
    {
        private readonly XmlSerializer _zoneSerializer = new XmlSerializer(typeof(Zone));
        private readonly XmlSerializer _zoneVisitsSerializer = new XmlSerializer(typeof(List<int>));

        private readonly object _syncRoot = new object();
        private readonly ZoneViewModel _emptyZone;
        private readonly MapControl _mapControl;
        private readonly Window _mainWindow;
        private readonly RouteManager _routeManger;
        private readonly ConcurrentDictionary<int, ZoneViewModel> _loadedZones = new ConcurrentDictionary<int, ZoneViewModel>();
        private readonly System.Timers.Timer _timer;

        private Dictionary<string, ZoneHolder> _zoneHolders = new Dictionary<string, ZoneHolder>();
        private XmlSerializer _additionalRoomParametersSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneManager"/> class.
        /// </summary>
        /// <param name="mapControl">The map control.</param>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="routeManger">The route manger.</param>
        public ZoneManager([NotNull] MapControl mapControl, [NotNull] Window mainWindow, [NotNull] RouteManager routeManger)
        {
            Assert.ArgumentNotNull(mapControl, "mapControl");
            Assert.ArgumentNotNull(mainWindow, "mainWindow");
            Assert.ArgumentNotNull(routeManger, "routeManger");

            _mapControl = mapControl;
            _mainWindow = mainWindow;
            _routeManger = routeManger;
            _emptyZone = new ZoneViewModel(new Zone { Id = -1000 }, Enumerable.Empty<AdditionalRoomParameters>()) { ZoomLevel = Settings.Default.MapZoomLevel };
            _timer = new System.Timers.Timer(5000) { AutoReset = false };
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

            _mapControl.RoadMapShowRequired += ShowRoadMap;
            _mapControl.RoomEditDialogRequired += ShowRoomEditDialog;
            _mapControl.NavigateToRoomRequired += NavigateToRoom;
            //_mapControl.RoutesDialogShowRequired += (o, e) => _routeManger.ShowRoutesDialog();
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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
                    if (zoneViewModel.Id == _mapControl.ViewModel.Id)
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

            using (var outStream = File.Open(zoneMapFileName, FileMode.Create, FileAccess.Write))
            using (var streamWriter = new StreamWriter(outStream))
            {
                foreach (var room in zoneViewModel.AllRooms)
                {
                    streamWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}", room.RoomId, room.XLocation, room.YLocation, room.ZLocation));
                }
            }
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _timer.Dispose();

            foreach (var zoneViewModel in _loadedZones.Values.ToArray())
            {
                SaveAdditionalRoomParameters(zoneViewModel);
#if DEBUG
                SaveZoneDebugInfo(zoneViewModel);
#endif
            }
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

#if DEBUG
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
#endif
            var uid = rootModel.Uid;

            if (_mapControl.ViewModel == null)
            {
                _mapControl.ViewModelUid = uid;

#if DEBUG
                sw.Start();
#endif
                var zoneViewModel = GetZone(_zoneHolders[uid].ZoneId);
#if DEBUG
                sw.Stop();
                rootModel.PushMessageToConveyor(new InfoMessage(string.Format("Load zone: {0} ms", sw.ElapsedMilliseconds)));
#endif

                if (zoneViewModel != null)
                    _mapControl.UpdateCurrentZone(zoneViewModel, null);
                else
                    _mapControl.UpdateCurrentZone(_emptyZone, null);

                return;
            }

            if (_zoneHolders.ContainsKey(uid) && _mapControl.ViewModelUid != uid)
            {
                _mapControl.ViewModelUid = uid;

                var zoneHolder = _zoneHolders[uid];

                if (_mapControl.ViewModel.Id != zoneHolder.ZoneId)
                {
#if DEBUG
                    sw.Start();
#endif
                    var zoneViewModel = GetZone(_zoneHolders[uid].ZoneId);
#if DEBUG
                    sw.Stop();
                    rootModel.PushMessageToConveyor(new InfoMessage(string.Format("Load zone: {0} ms", sw.ElapsedMilliseconds)));
#endif
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
                    _additionalRoomParametersSerializer = new XmlSerializer(typeof(List<AdditionalRoomParameters>), RootModel.CustomSerializationTypes.ToArray());
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
                    result = LoadZone(zoneId);

                    if (result == null)
                        result = _emptyZone;
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
            ExecuteRoomAction(zoneHolder);

            if (_mapControl.ViewModel != null && _mapControl.ViewModelUid == zoneHolder.Uid)
            {
                if (_mapControl.ViewModel.Id != zoneHolder.ZoneId)
                {
#if DEBUG
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
#endif
                    var zoneViewModel = GetZone(zoneHolder.ZoneId);
#if DEBUG
                    sw.Stop();
                    zoneHolder.RootModel.PushMessageToConveyor(new InfoMessage(string.Format("Load zone: {0} ms", sw.ElapsedMilliseconds)));
#endif
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
            if (_mapControl.ViewModel != null)
            {
                var room = _mapControl.ViewModel.AllRooms.FirstOrDefault(r => r.RoomId == zoneHolder.RoomId);
                if (room != null)
                {
                    foreach (var action in room.AdditionalRoomParameters.ActionsToExecuteOnRoomEntry)
                    {
                        action.Execute(zoneHolder.RootModel, ActionExecutionContext.Empty);
                    }
                }
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "It's ok here.")]
        [CanBeNull]
        private ZoneViewModel LoadZone(int zoneId)
        {
            Zone loadedZone;
            ZoneViewModel zoneViewModel = null;
            var zoneVisits = new List<AdditionalRoomParameters>();

            lock (_syncRoot)
            {
                try
                {
                    ZoneViewModel result;
                    if (_loadedZones.TryGetValue(zoneId, out result))
                    {
                        result.BuildCurrentLevelRooms();
                        return result;
                    }

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
                    //TODO: Разобраться что это такое
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
                    var aesStream = new SharpAESCrypt.SharpAESCrypt("A5Ub5T7j5cYg40v", inStream, OperationMode.Decrypt);
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

#if DEBUG
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif

            var zone = LoadZoneFromFile("roads.xml");

            if (zone == null)
                return;

#if DEBUG
            long loadZoneTime = sw.ElapsedMilliseconds;
            sw.Stop();

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

            sw.Start();
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
#if DEBUG
            long initDialogTime = sw.ElapsedMilliseconds;
            sw.Stop();
            if (_zoneHolders.Count > 0)
            {
                ZoneHolder zoneHolder = _zoneHolders.Values.FirstOrDefault();
                zoneHolder.RootModel.PushMessageToConveyor(new InfoMessage(string.Format("LoadZoneTime = {0}, InitDialogTime = {1}", loadZoneTime, initDialogTime)));
            }
#endif
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

            if (_mapControl.ViewModel.CurrentRoom != null)
            {
                _routeManger.NavigateToRoom(roomToNavigateTo);
            }
        }
    }
}
