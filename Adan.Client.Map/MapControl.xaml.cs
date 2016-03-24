// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapControl.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for MapControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Threading;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using ViewModel;

    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl
    {
        private readonly RoomContextMenu _contextMenu = new RoomContextMenu();
        private readonly ToolTip _toolTip = new ToolTip();
        private readonly RoomDetailsControl _roomDetailsControl = new RoomDetailsControl();

        private ZoneViewModel _zoneViewModel;
        private bool _dragging;
        private Point _draggingStartPosition;
        private double _draggingStartY;
        private double _draggingStartX;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapControl"/> class.
        /// </summary>
        public MapControl()
        {
            InitializeComponent();
            _toolTip.Content = _roomDetailsControl;
            _toolTip.PlacementTarget = this;
            _toolTip.Placement = PlacementMode.RelativePoint;
            _toolTip.Opacity = 0.9;
            _contextMenu.PlacementTarget = this;
            _contextMenu.Placement = PlacementMode.RelativePoint;
            _contextMenu.RoomEditDialogRequired += OnRoomEditDialogRequired;
            _contextMenu.NavigateToRoomRequired += OnNavigateToRoomRequired;
#if DEBUG
            debugModeButtons.Visibility = System.Windows.Visibility.Visible;
            roomIdLabel.Visibility = System.Windows.Visibility.Visible;
#endif
        }

        /// <summary>
        /// Occurs when user wants to see routes dialog.
        /// </summary>
        public event EventHandler RoutesDialogShowRequired;

        /// <summary>
        /// Occurs when user wants to see road map.
        /// </summary>
        public event EventHandler RoadMapShowRequired;

        /// <summary>
        /// Occurs when room edit dialog is required.
        /// </summary>
        public event Action<RoomViewModel> RoomEditDialogRequired;

        /// <summary>
        /// Occurs when user wants to navigate to certain room.
        /// </summary>
        public event Action<RoomViewModel> NavigateToRoomRequired;

        /// <summary>
        /// Gets or sets a value indicating whether to display current room arrow or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if current room arrow should be displayed; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayCurrentRoomArrow
        {
            get
            {
                return zoneVisual.DisplayCurrentRoomArrow;
            }

            set
            {
                zoneVisual.DisplayCurrentRoomArrow = value;
            }
        }        

        /// <summary>
        /// 
        /// </summary>
        public string ViewModelUid { get; set; }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        [CanBeNull]
        public ZoneViewModel ViewModel
        {
            get
            {
                return _zoneViewModel;
            }

            set
            {
                _zoneViewModel = value;
                DataContext = value;
                zoneVisual.ViewModel = value;
            }
        }

        /// <summary>
        /// Gets or sets the route manager.
        /// </summary>
        /// <value>
        /// The route manager.
        /// </value>
        [NotNull]
        public RouteManager RouteManager
        {
            get;
            set;
        }

        /// <summary>
        /// Updates the current zone.
        /// </summary>
        /// <param name="newZone">The new zone.</param>
        /// <param name="currentRoom">The current room.</param>
        public void UpdateCurrentZone([NotNull] ZoneViewModel newZone, [CanBeNull]RoomViewModel currentRoom)
        {
            Assert.ArgumentNotNull(newZone, "newZone");

            var actionToExecute = (Action)(() =>
            {
                ViewModel = newZone;
                NavigateToCurrentRoom(currentRoom);
                ViewModel.CurrentRoom = currentRoom;

                if (RouteManager != null)
                    RouteManager.UpdateCurrentRoom(currentRoom, newZone);
            });

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, actionToExecute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newZone"></param>
        /// <param name="currentRoom"></param>
        public void UpdateCurrentZone([NotNull] ZoneViewModel newZone, int currentRoom)
        {
            Assert.ArgumentNotNull(newZone, "newZone");

            var actionToExecute = (Action)(() =>
            {
                ViewModel = newZone;
                var room = ViewModel.AllRooms.FirstOrDefault(r => r.RoomId == currentRoom);
                NavigateToCurrentRoom(room);
                ViewModel.CurrentRoom = room;

                if (RouteManager != null)
                    RouteManager.UpdateCurrentRoom(room, newZone);
            });

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, actionToExecute);
        }

        /// <summary>
        /// Updates the current room.
        /// </summary>
        /// <param name="currentRoom">The current room.</param>
        public void UpdateCurrentRoom([CanBeNull] RoomViewModel currentRoom)
        {
            var actionToExecute = (Action)(() =>
            {
                if (ViewModel != null)
                {
                    NavigateToCurrentRoom(currentRoom);
                    ViewModel.CurrentRoom = currentRoom;
                    if (RouteManager != null)
                        RouteManager.UpdateCurrentRoom(currentRoom, ViewModel);
                }
            });

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, actionToExecute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentRoom"></param>
        public void UpdateCurrentRoom(int currentRoom)
        {
            var actionToExecute = (Action)(() =>
            {
                if (ViewModel != null)
                {
                    var room = ViewModel.AllRooms.FirstOrDefault(r => r.RoomId == currentRoom);
                    NavigateToCurrentRoom(room);
                    ViewModel.CurrentRoom = room;
                    if (RouteManager != null)
                        RouteManager.UpdateCurrentRoom(room, ViewModel);
                }
            });

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, actionToExecute);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.PreviewMouseMove"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewMouseMove([NotNull] MouseEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            if (_contextMenu.IsOpen)
            {
                return;
            }

            Point pt = e.GetPosition(zoneVisual);
            var room = GetRoom(pt);
            if (room != null)
            {
                Point point;
                if (zoneVisual.RenderTransform.TryTransform(new Point((room.XLocation * 30) + 30, (room.YLocation * 30) + 30), out point))
                {
                    _roomDetailsControl.DataContext = room;
                    _toolTip.IsOpen = true;
                    _toolTip.HorizontalOffset = point.X;
                    _toolTip.VerticalOffset = point.Y;
                }
            }
            else
            {
                _toolTip.IsOpen = false;
            }

            base.OnPreviewMouseMove(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.MouseRightButtonDown"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was pressed.</param>
        protected override void OnMouseRightButtonDown([NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            Point pt = e.GetPosition(zoneVisual);

            _toolTip.IsOpen = false;
            var room = GetRoom(pt);
            if (room != null)
            {
                Point point;
                if (zoneVisual.RenderTransform.TryTransform(new Point((room.XLocation * 30) + 30, (room.YLocation * 30) + 30), out point))
                {
                    _contextMenu.DataContext = room;
                    _contextMenu.IsOpen = true;
                    _contextMenu.HorizontalOffset = point.X;
                    _contextMenu.VerticalOffset = point.Y;
                }
            }
            else
            {
                _contextMenu.IsOpen = false;
            }

            base.OnMouseRightButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.MouseLeave"/> attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave([NotNull] MouseEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            _toolTip.IsOpen = false;
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown([NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            base.OnMouseLeftButtonDown(e);
            if (_contextMenu.IsOpen)
            {
                _contextMenu.IsOpen = false;
            }
#if DEBUG
            Point pt = e.GetPosition(zoneVisual);

            if (ViewModel == null)
            {
                return;
            }

            foreach (var room in ViewModel.CurrentLevelRooms)
            {
                if (room.XLocation * 30 < pt.X && (room.XLocation * 30) + 30 > pt.X && room.YLocation * 30 < pt.Y && (room.YLocation * 30) + 30 > pt.Y)
                {
                    room.IsSelected = !room.IsSelected;
                    zoneVisual.InvalidateVisual();
                    break;
                }
            }
#endif
            _dragging = true;
            _draggingStartX = translateTransform.X;
            _draggingStartY = translateTransform.Y;
            _draggingStartPosition = e.GetPosition(this);

            CaptureMouse();
            e.Handled = true;
        }

        /// <summary>
        /// Raises the <see cref="Control.MouseDoubleClick"/> routed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick([NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            base.OnMouseDoubleClick(e);
            _contextMenu.IsOpen = false;
            Point pt = e.GetPosition(zoneVisual);

            var room = GetRoom(pt);
            if (room != null)
            {
                OnRoomEditDialogRequired(room);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.MouseMove"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove([NotNull] MouseEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            base.OnMouseMove(e);
            if (_contextMenu.IsOpen)
            {
                _dragging = false;
                return;
            }

            if (_dragging)
            {
                var currentMousePosition = e.GetPosition(this);
                var zoomLevel = ViewModel == null ? 1.0f : 1.0f / ViewModel.ZoomLevel;
                translateTransform.X = _draggingStartX + (zoomLevel * (currentMousePosition.X - _draggingStartPosition.X));
                translateTransform.Y = _draggingStartY + (zoomLevel * (currentMousePosition.Y - _draggingStartPosition.Y));
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp([NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            base.OnMouseLeftButtonUp(e);

            if (_dragging)
            {
                _dragging = false;
                ReleaseMouseCapture();
            }
        }

        private void NavigateToCurrentRoom([CanBeNull]RoomViewModel currentRoom)
        {
            if (ViewModel == null)
            {
                return;
            }

            if (currentRoom == null)
            {
                currentRoom = ViewModel.AllRooms.FirstOrDefault(r => ViewModel != null && r.ZLocation == ViewModel.CurrentLevel);
            }

            if (currentRoom != null)
            {
                translateTransform.X = (ActualWidth / 2) - (currentRoom.XLocation * 30) - 15;
                translateTransform.Y = (ActualHeight / 2) - (currentRoom.YLocation * 30) - 15;
                scaleTransform.CenterX = ActualWidth / 2;
                scaleTransform.CenterY = ActualHeight / 2;
            }
        }

        private void DecreaseCurrentLevel([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (ViewModel == null)
            {
                return;
            }

            ViewModel.CurrentLevel--;
        }

        private void IncreaseCurrentLevel([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (ViewModel == null)
            {
                return;
            }

            ViewModel.CurrentLevel++;
        }

        private void CenterToCurrentRoom([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (ViewModel == null)
            {
                return;
            }

            if (ViewModel.CurrentRoom == null)
            {
                return;
            }

            ViewModel.CurrentLevel = ViewModel.CurrentRoom.ZLocation;
            translateTransform.X = (ActualWidth / 2) - (ViewModel.CurrentRoom.XLocation * 30) - 15;
            translateTransform.Y = (ActualHeight / 2) - (ViewModel.CurrentRoom.YLocation * 30) - 15;
        }

        private void MoveSelectedRoomsLeft([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
#if DEBUG
            if (ViewModel == null)
            {
                return;
            }

            foreach (var roomViewModel in ViewModel.AllRooms)
            {
                if (roomViewModel.IsSelected)
                {
                    roomViewModel.XLocation--;
                }
            }

            zoneVisual.InvalidateVisual();
#endif
        }

        private void MoveSelectedRoomsRight([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
#if DEBUG
            if (ViewModel == null)
            {
                return;
            }

            foreach (var roomViewModel in ViewModel.AllRooms)
            {
                if (roomViewModel.IsSelected)
                {
                    roomViewModel.XLocation++;
                }
            }

            zoneVisual.InvalidateVisual();
#endif
        }

        private void MoveSelectedRoomsNorth([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
#if DEBUG
            if (ViewModel == null)
            {
                return;
            }

            foreach (var roomViewModel in ViewModel.AllRooms)
            {
                if (roomViewModel.IsSelected)
                {
                    roomViewModel.YLocation--;
                }
            }

            zoneVisual.InvalidateVisual();
#endif
        }

        private void MoveSelectedRoomsSouth([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
#if DEBUG
            if (ViewModel == null)
            {
                return;
            }

            foreach (var roomViewModel in ViewModel.AllRooms)
            {
                if (roomViewModel.IsSelected)
                {
                    roomViewModel.YLocation++;
                }
            }

            zoneVisual.InvalidateVisual();
#endif
        }

        private void MoveSelectedRoomsUp([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
#if DEBUG
            if (ViewModel == null)
            {
                return;
            }

            foreach (var roomViewModel in ViewModel.AllRooms)
            {
                if (roomViewModel.IsSelected)
                {
                    roomViewModel.ZLocation++;
                }
            }

            ViewModel.CurrentLevel++;
            ViewModel.CurrentLevel--;
            zoneVisual.InvalidateVisual();
#endif
        }

        private void MoveSelectedRoomsDown([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
#if DEBUG
            if (ViewModel == null)
            {
                return;
            }

            foreach (var roomViewModel in ViewModel.AllRooms)
            {
                if (roomViewModel.IsSelected)
                {
                    roomViewModel.ZLocation--;
                }
            }

            ViewModel.CurrentLevel++;
            ViewModel.CurrentLevel--;
            zoneVisual.InvalidateVisual();
#endif
        }

        private void ClearSelection([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
#if DEBUG
            if (ViewModel == null)
            {
                return;
            }

            foreach (var roomViewModel in ViewModel.AllRooms)
            {
                roomViewModel.IsSelected = false;
            }

            zoneVisual.InvalidateVisual();
#endif
        }

        private void ShowRoadMap([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (RoadMapShowRequired != null)
            {
                RoadMapShowRequired(this, EventArgs.Empty);
            }
        }

        private void OpenRoutesDialog([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (RoutesDialogShowRequired != null)
            {
                RoutesDialogShowRequired(this, EventArgs.Empty);
            }
        }

        [CanBeNull]
        private RoomViewModel GetRoom(Point pt)
        {
            if (ViewModel == null)
            {
                return null;
            }

            foreach (var room in ViewModel.CurrentLevelRooms)
            {
                if (room.XLocation * 30 < pt.X && (room.XLocation * 30) + 30 > pt.X && room.YLocation * 30 < pt.Y && (room.YLocation * 30) + 30 > pt.Y)
                {
                    return room;
                }
            }

            return null;
        }

        private void OnRoomEditDialogRequired([NotNull]RoomViewModel room)
        {
            Assert.ArgumentNotNull(room, "room");

            if (RoomEditDialogRequired != null)
            {
                RoomEditDialogRequired(room);
            }
        }

        private void OnNavigateToRoomRequired([NotNull]RoomViewModel roomViewModel)
        {
            Assert.ArgumentNotNull(roomViewModel, "roomViewModel");

            Action<RoomViewModel> handler = NavigateToRoomRequired;
            if (handler != null)
            {
                handler(roomViewModel);
            }
        }
    }
}
