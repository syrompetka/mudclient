// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomVisual.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the RoomVisual type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Render
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model;

    using ViewModel;

    /// <summary>
    /// Class responsible for rendering single roome.
    /// </summary>
    public class RoomVisual : Visual
    {
        private readonly ZoneVisual _zoneVisual;
        private readonly IList<Visual> _childVisuals = new List<Visual>();
        private readonly GeometryDrawing _iconDrawing;
        private readonly GeometryDrawing _roomDrawing;
        private readonly GeometryDrawing _northExitDrawing;
        private readonly GeometryDrawing _southExitDrawing;
        private readonly GeometryDrawing _westExitDrawing;
        private readonly GeometryDrawing _eastExitDrawing;
        private readonly GeometryDrawing _upExitDrawing;
        private readonly GeometryDrawing _downExitDrawing;
        private readonly DrawingVisual _rootDrawing;
        private readonly TranslateTransform _transform;
        private RoomViewModel _roomToVisualize;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomVisual"/> class.
        /// </summary>
        /// <param name="zoneVisual">The zone visual.</param>
        public RoomVisual([NotNull] ZoneVisual zoneVisual)
        {
            _zoneVisual = zoneVisual;
            Assert.ArgumentNotNull(zoneVisual, "zoneVisual");

            _rootDrawing = new DrawingVisual();
            _childVisuals.Add(_rootDrawing);

            var dc = _rootDrawing.RenderOpen();

            _roomDrawing = new GeometryDrawing(zoneVisual.RenderConstants.GetRoomColorBrush(RoomColor.Default), _zoneVisual.RenderConstants.DefaultRoomPen, _zoneVisual.RenderConstants.RoomGeometry);
            dc.DrawDrawing(_roomDrawing);

            _northExitDrawing = new GeometryDrawing(_zoneVisual.RenderConstants.DefaultExitBrush, null, _zoneVisual.RenderConstants.NorthExitGeometry.Clone())
                                    {
                                        Geometry = { Transform = new ScaleTransform(1, 1, 1, 5) }
                                    };
            dc.DrawDrawing(_northExitDrawing);

            _southExitDrawing = new GeometryDrawing(_zoneVisual.RenderConstants.DefaultExitBrush, null, _zoneVisual.RenderConstants.SouthExitGeometry.Clone())
                                    {
                                        Geometry = { Transform = new ScaleTransform(1, 1, 1, 25) }
                                    };
            dc.DrawDrawing(_southExitDrawing);

            _westExitDrawing = new GeometryDrawing(_zoneVisual.RenderConstants.DefaultExitBrush, null, _zoneVisual.RenderConstants.WestExitGeometry.Clone())
            {
                Geometry = { Transform = new ScaleTransform(1, 1, 5, 1) }
            };
            dc.DrawDrawing(_westExitDrawing);

            _eastExitDrawing = new GeometryDrawing(_zoneVisual.RenderConstants.DefaultExitBrush, null, _zoneVisual.RenderConstants.EastExitGeometry.Clone())
            {
                Geometry = { Transform = new ScaleTransform(1, 1, 25, 1) }
            };
            dc.DrawDrawing(_eastExitDrawing);

            _upExitDrawing = new GeometryDrawing(_zoneVisual.RenderConstants.DefaultExitBrush, null, _zoneVisual.RenderConstants.UpExitGeometry);
            dc.DrawDrawing(_upExitDrawing);

            _downExitDrawing = new GeometryDrawing(_zoneVisual.RenderConstants.DefaultExitBrush, null, _zoneVisual.RenderConstants.DownExitGeometry);
            dc.DrawDrawing(_downExitDrawing);

            _iconDrawing = new GeometryDrawing(zoneVisual.RenderConstants.GetRoomIconBrush(RoomIcon.None), new Pen(Brushes.Transparent, 0), new RectangleGeometry(new Rect(5, 5, 20, 20)));
            dc.DrawDrawing(_iconDrawing);
            dc.Close();
            _transform = new TranslateTransform(-10000, -10000);
            _rootDrawing.Transform = _transform;
        }

        /// <summary>
        /// Gets or sets the room to visualize.
        /// </summary>
        /// <value>
        /// The room to visualize.
        /// </value>
        [CanBeNull]
        public RoomViewModel RoomToVisualize
        {
            get
            {
                return _roomToVisualize;
            }

            set
            {
                if (_roomToVisualize == value)
                {
                    return;
                }

                if (_roomToVisualize != null)
                {
                    _roomToVisualize.PropertyChanged -= OnRoomChanged;
                }

                _roomToVisualize = value;
                if (_roomToVisualize != null)
                {
                    _roomToVisualize.PropertyChanged += OnRoomChanged;
                }

                OnRoomChanged(this, new PropertyChangedEventArgs(string.Empty));
            }
        }

        /// <summary>
        /// Gets the number of child elements for the <see cref="T:System.Windows.Media.Visual"/>.
        /// </summary>
        /// <returns>The number of child elements.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return _childVisuals.Count;
            }
        }

        /// <summary>
        /// Returns the specified <see cref="T:System.Windows.Media.Visual"/> in the parent <see cref="T:System.Windows.Media.VisualCollection"/>.
        /// </summary>
        /// <param name="index">The index of the visual object in the <see cref="T:System.Windows.Media.VisualCollection"/>.</param>
        /// <returns>
        /// The child in the <see cref="T:System.Windows.Media.VisualCollection"/> at the specified <paramref name="index"/> value.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return _childVisuals[index];
        }

        private void OnRoomChanged([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (RoomToVisualize == null)
            {
                _transform.X = -10000;
                _transform.Y = -10000;
                ((ScaleTransform)_northExitDrawing.Geometry.Transform).ScaleY = 1;
                ((ScaleTransform)_northExitDrawing.Geometry.Transform).ScaleX = 1;
                ((ScaleTransform)_southExitDrawing.Geometry.Transform).ScaleY = 1;
                ((ScaleTransform)_southExitDrawing.Geometry.Transform).ScaleX = 1;
                ((ScaleTransform)_eastExitDrawing.Geometry.Transform).ScaleY = 1;
                ((ScaleTransform)_eastExitDrawing.Geometry.Transform).ScaleX = 1;
                ((ScaleTransform)_westExitDrawing.Geometry.Transform).ScaleY = 1;
                ((ScaleTransform)_westExitDrawing.Geometry.Transform).ScaleX = 1;
                return;
            }

            if (!RoomToVisualize.HasBeenVisited && !RoomToVisualize.IsConnectedToCurrent && !RoomToVisualize.Exits.Any(ex => ex.Room != null && ex.Room.HasBeenVisited))
            {
                _transform.X = -10000;
                _transform.Y = -10000;
                return;
            }

            _roomDrawing.Pen = RoomToVisualize.IsCurrent
                                   ? _zoneVisual.RenderConstants.CurrentRoomPen
                                   : _zoneVisual.RenderConstants.DefaultRoomPen;

            if (RoomToVisualize.IsSelected)
            {
                _roomDrawing.Pen = _zoneVisual.RenderConstants.SelectedRoomPen;
            }

            _roomDrawing.Brush = !RoomToVisualize.HasBeenVisited ? _zoneVisual.RenderConstants.NonVisitedRoomBrush : _zoneVisual.RenderConstants.GetRoomColorBrush(_roomToVisualize.Color);

            foreach (var exitDrawing in GetAllExits())
            {
                exitDrawing.Brush = _zoneVisual.RenderConstants.InvisibleExitBrush;
            }

            var exitsToRender = RoomToVisualize.Exits;
            if (!RoomToVisualize.HasBeenVisited)
            {
                exitsToRender = RoomToVisualize.Exits.Where(x => x.Room != null && (x.Room.IsCurrent || x.Room.HasBeenVisited));
            }

            _iconDrawing.Brush = _zoneVisual.RenderConstants.GetRoomIconBrush(_roomToVisualize.Icon);

            if (RoomToVisualize.HasBeenVisited)
            {
                if (RoomToVisualize.IsPartOfRoute)
                {
                    _roomDrawing.Brush = _zoneVisual.RenderConstants.GetRoomColorBrush(RoomColor.Green);
                }

                if (RoomToVisualize.IsPartOfRecordedRoute)
                {
                    _roomDrawing.Brush = _zoneVisual.RenderConstants.GetRoomColorBrush(RoomColor.Yellow);
                }

                if (RoomToVisualize.IsStartOrEndOfRoute)
                {
                    _iconDrawing.Brush = _zoneVisual.RenderConstants.GetRoomIconBrush(RoomIcon.Route);
                }
            } 
            
            foreach (var roomExit in exitsToRender)
            {
                var drawing = GetGeometryByExitDirection(roomExit.Direction);
                StretchExitIfNeeded(roomExit, drawing);
                if (RoomToVisualize.IsCurrent || (roomExit.Room != null && roomExit.Room.IsCurrent))
                {
                    drawing.Brush = _zoneVisual.RenderConstants.CurrentRoomExitBrush;
                }
                else
                {
                    drawing.Brush = _zoneVisual.RenderConstants.DefaultExitBrush;
                }
            }

            _transform.X = RoomToVisualize.XLocation * 30;
            _transform.Y = RoomToVisualize.YLocation * 30;
        }

        private void StretchExitIfNeeded([NotNull]RoomExitViewModel roomExit, [NotNull]GeometryDrawing exitDrawing)
        {
            Assert.ArgumentNotNull(roomExit, "roomExit");
            Assert.ArgumentNotNull(exitDrawing, "exitDrawing");

            if (RoomToVisualize == null)
            {
                return;
            }

            if (roomExit.Room == null)
            {
                return;
            }

            if (roomExit.Room.ZLocation != RoomToVisualize.ZLocation)
            {
                return;
            }

            // pretty ugly write-only copy-paste
            if (roomExit.Direction == ExitDirection.North)
            {
                if (roomExit.Room.YLocation < RoomToVisualize.YLocation - 1 && roomExit.Room.XLocation == RoomToVisualize.XLocation)
                {
                    bool roomFound = false;
                    foreach (var zoneRoom in RoomToVisualize.Zone.AllRooms)
                    {
                        if (zoneRoom == RoomToVisualize || zoneRoom == roomExit.Room)
                        {
                            continue;
                        }

                        if (zoneRoom.ZLocation == RoomToVisualize.ZLocation && zoneRoom.XLocation == RoomToVisualize.XLocation && zoneRoom.YLocation <= RoomToVisualize.YLocation && zoneRoom.YLocation >= roomExit.Room.YLocation)
                        {
                            roomFound = true;
                            break;
                        }
                    }

                    if (!roomFound)
                    {
                        var targetLength = (RoomToVisualize.YLocation - roomExit.Room.YLocation - 1) / 2.0f;
                        targetLength = (targetLength * 30.0f) / 5.0f;
                        targetLength += 1.0f;
                        ((ScaleTransform)exitDrawing.Geometry.Transform).ScaleY = targetLength;
                    }
                }
            }
            else if (roomExit.Direction == ExitDirection.South)
            {
                if (roomExit.Room.YLocation > RoomToVisualize.YLocation + 1 && roomExit.Room.XLocation == RoomToVisualize.XLocation)
                {
                    bool roomFound = false;
                    foreach (var zoneRoom in RoomToVisualize.Zone.AllRooms)
                    {
                        if (zoneRoom == RoomToVisualize || zoneRoom == roomExit.Room)
                        {
                            continue;
                        }

                        if (zoneRoom.ZLocation == RoomToVisualize.ZLocation && zoneRoom.XLocation == RoomToVisualize.XLocation && zoneRoom.YLocation <= roomExit.Room.YLocation && zoneRoom.YLocation >= RoomToVisualize.YLocation)
                        {
                            roomFound = true;
                            break;
                        }
                    }

                    if (!roomFound)
                    {
                        var targetLength = (roomExit.Room.YLocation - RoomToVisualize.YLocation - 1) / 2.0f;
                        targetLength = (targetLength * 30.0f) / 5.0f;
                        targetLength += 1.0f;
                        ((ScaleTransform)exitDrawing.Geometry.Transform).ScaleY = targetLength;
                    }
                }
            }
            else if (roomExit.Direction == ExitDirection.East)
            {
                if (roomExit.Room.XLocation > RoomToVisualize.XLocation + 1 && roomExit.Room.YLocation == RoomToVisualize.YLocation)
                {
                    bool roomFound = false;
                    foreach (var zoneRoom in RoomToVisualize.Zone.AllRooms)
                    {
                        if (zoneRoom == RoomToVisualize || zoneRoom == roomExit.Room)
                        {
                            continue;
                        }

                        if (zoneRoom.ZLocation == RoomToVisualize.ZLocation && zoneRoom.YLocation == RoomToVisualize.YLocation && zoneRoom.XLocation <= roomExit.Room.XLocation && zoneRoom.XLocation >= RoomToVisualize.XLocation)
                        {
                            roomFound = true;
                            break;
                        }
                    }

                    if (!roomFound)
                    {
                        var targetLength = (roomExit.Room.XLocation - RoomToVisualize.XLocation - 1) / 2.0f;
                        targetLength = (targetLength * 30.0f) / 5.0f;
                        targetLength += 1.0f;
                        ((ScaleTransform)exitDrawing.Geometry.Transform).ScaleX = targetLength;
                    }
                }
            }
            else if (roomExit.Direction == ExitDirection.West)
            {
                if (roomExit.Room.XLocation < RoomToVisualize.XLocation - 1 && roomExit.Room.YLocation == RoomToVisualize.YLocation)
                {
                    bool roomFound = false;
                    foreach (var zoneRoom in RoomToVisualize.Zone.AllRooms)
                    {
                        if (zoneRoom == RoomToVisualize || zoneRoom == roomExit.Room)
                        {
                            continue;
                        }

                        if (zoneRoom.ZLocation == RoomToVisualize.ZLocation && zoneRoom.YLocation == RoomToVisualize.YLocation && zoneRoom.XLocation <= RoomToVisualize.XLocation && zoneRoom.XLocation >= roomExit.Room.XLocation)
                        {
                            roomFound = true;
                            break;
                        }
                    }

                    if (!roomFound)
                    {
                        var targetLength = (RoomToVisualize.XLocation - roomExit.Room.XLocation - 1) / 2.0f;
                        targetLength = (targetLength * 30.0f) / 5.0f;
                        targetLength += 1.0f;
                        ((ScaleTransform)exitDrawing.Geometry.Transform).ScaleX = targetLength;
                    }
                }
            }
        }

        [NotNull]
        private GeometryDrawing GetGeometryByExitDirection(ExitDirection direction)
        {
            switch (direction)
            {
                case ExitDirection.Up:
                    return _upExitDrawing;
                case ExitDirection.Down:
                    return _downExitDrawing;
                case ExitDirection.North:
                    return _northExitDrawing;
                case ExitDirection.South:
                    return _southExitDrawing;
                case ExitDirection.West:
                    return _westExitDrawing;
                case ExitDirection.East:
                    return _eastExitDrawing;
                default:
                    return _upExitDrawing;
            }
        }

        [NotNull]
        private IEnumerable<GeometryDrawing> GetAllExits()
        {
            yield return _upExitDrawing;
            yield return _downExitDrawing;
            yield return _southExitDrawing;
            yield return _northExitDrawing;
            yield return _westExitDrawing;
            yield return _eastExitDrawing;
        }
    }
}
