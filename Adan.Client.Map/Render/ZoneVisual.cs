// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoneVisual.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ZoneVisual type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Render
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;

    /// <summary>
    /// Class responsible for rendering entire zone.
    /// </summary>
    public class ZoneVisual : Control
    {
        private readonly LinkedList<RoomVisual> _visualsPool = new LinkedList<RoomVisual>();
        private readonly List<Visual> _visuals = new List<Visual>();
        private readonly DrawingVisual _currentRoomHighlight;
        private readonly TranslateTransform _currentRoomTranslateTransform = new TranslateTransform(-1000, -1000);
        private ZoneViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneVisual"/> class.
        /// </summary>
        public ZoneVisual()
        {
            DisplayCurrentRoomArrow = false;
            RenderConstants = new RenderConstants(this);
            _currentRoomHighlight = new DrawingVisual();
            DrawingContext drawingContext = _currentRoomHighlight.RenderOpen();

            var geometry = Geometry.Parse("M12.62173,0 C11.150318,2.3096478 10.200088,4.6685028 9.6989231,7.0282497 L9.6329937,7.3540044 L38.759003,7.3540044 L38.759003,15.354004 L9.6925592,15.354004 L9.7251463,15.516076 C10.281377,18.168938 11.322644,20.744579 12.737,23.167997 L0,11.123159 z").Clone();
            var currentRoomArrowBrush = new LinearGradientBrush { StartPoint = new Point(0.507, 0.303), EndPoint = new Point(0.498, 0.705) };
            currentRoomArrowBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0x50, 0x11, 0x11), 1));
            currentRoomArrowBrush.GradientStops.Add(new GradientStop(Colors.Red, 0.279));
            currentRoomArrowBrush.GradientStops.Add(new GradientStop(Colors.Red, 0.358));
            currentRoomArrowBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0xBA, 0x15, 0x15), 0.566));
            currentRoomArrowBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0xA1, 0x14, 0x14), 0));

            drawingContext.DrawGeometry(currentRoomArrowBrush, new Pen(Brushes.Transparent, 0), geometry);
            drawingContext.Close();

            var currentRoomTransform = new TransformGroup();
            currentRoomTransform.Children.Add(new ScaleTransform(6, 6, 20, 20));
            currentRoomTransform.Children.Add(new RotateTransform(-47, 20, 20));
            currentRoomTransform.Children.Add(_currentRoomTranslateTransform);
            _currentRoomHighlight.Transform = currentRoomTransform;

            _visuals.Add(_currentRoomHighlight);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display current room arrow or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if current room arrow should be displayed; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayCurrentRoomArrow
        {
            get;
            set;
        }

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
                return _viewModel;
            }

            set
            {
                if (value == _viewModel)
                {
                    return;
                }

                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= HandleViewModelPropertyChange;
                }

                if (value != null)
                {
                    value.PropertyChanged += HandleViewModelPropertyChange;
                }

                _viewModel = value;
                RebuildAllRooms();
                MoveCurrentRoomArrow();
            }
        }

        /// <summary>
        /// Gets the render constants.
        /// </summary>
        public RenderConstants RenderConstants
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of child elements for the <see cref="T:System.Windows.Media.Visual"/>.
        /// </summary>
        /// <returns>The number of child elements.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return _visuals.Count;
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
            return _visuals[index];
        }

        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>
        /// The size of the control, up to the maximum specified by <paramref name="constraint"/>.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            return constraint;
        }

        /// <summary>
        /// Called to arrange and size the content of a <see cref="T:System.Windows.Controls.Control"/> object.
        /// </summary>
        /// <param name="arrangeBounds">The computed size that is used to arrange the content.</param>
        /// <returns>
        /// The size of the control.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
           return arrangeBounds;
        }

        private void RebuildAllRooms()
        {
            _currentRoomTranslateTransform.X = -10000;
            _currentRoomTranslateTransform.Y = -10000;
            foreach (var roomVisual in _visuals.OfType<RoomVisual>())
            {
                roomVisual.RoomToVisualize = null;
                _visualsPool.AddLast(roomVisual);
            }

            _visuals.Clear();
            _visuals.Add(_currentRoomHighlight);

            if (ViewModel == null)
            {
                return;
            }

            foreach (var roomViewModel in ViewModel.CurrentLevelRooms)
            {
                var visual = GetRoomVisualFromPool();
                visual.RoomToVisualize = roomViewModel;
                _visuals.Insert(0, visual);
            }

            InvalidateVisual();
        }

        [NotNull]
        private RoomVisual GetRoomVisualFromPool()
        {
            if (_visualsPool.Any())
            {
                var result = _visualsPool.Last.Value;
                _visualsPool.RemoveLast();
                return result;
            }

            return new RoomVisual(this);
        }

        private void HandleViewModelPropertyChange([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (e.PropertyName == "CurrentLevel")
            {
                RebuildAllRooms();
            }

            MoveCurrentRoomArrow();
        }

        private void MoveCurrentRoomArrow()
        {
            if (!DisplayCurrentRoomArrow)
            {
                return;
            }

            if (ViewModel == null)
            {
                return;
            }

            if (ViewModel.CurrentRoom == null)
            {
                return;
            }

            _currentRoomTranslateTransform.X = (ViewModel.CurrentRoom.XLocation * 30) + 130;
            _currentRoomTranslateTransform.Y = (ViewModel.CurrentRoom.YLocation * 30) - 75;
        }
    }
}
