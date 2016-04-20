// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderConstants.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the RenderConstants type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Render
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model;

    /// <summary>
    /// Class to hold misc constants for rendering.
    /// </summary>
    public sealed class RenderConstants
    {
        private readonly ZoneVisual _zoneVisual;
        private readonly IDictionary<RoomColor, Brush> _fetchedRoomColors = new Dictionary<RoomColor, Brush>();
        private readonly IDictionary<RoomIcon, Brush> _fetchedRoomIcons = new Dictionary<RoomIcon, Brush>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderConstants"/> class.
        /// </summary>
        /// <param name="zoneVisual">The zone visual.</param>
        public RenderConstants([NotNull]ZoneVisual zoneVisual)
        {
            _zoneVisual = zoneVisual;
            Assert.ArgumentNotNull(zoneVisual, "zoneVisual");

            RoomGeometry = new RectangleGeometry(new Rect(5, 5, 20, 20), 2, 2);
            RoomGeometry.Freeze();

            NorthExitGeometry = new RectangleGeometry(new Rect(14, 0, 2, 5));
            NorthExitGeometry.Freeze();

            SouthExitGeometry = new RectangleGeometry(new Rect(14, 25, 2, 5));
            SouthExitGeometry.Freeze();

            WestExitGeometry = new RectangleGeometry(new Rect(0, 14, 5, 2));
            WestExitGeometry.Freeze();

            EastExitGeometry = new RectangleGeometry(new Rect(25, 14, 5, 2));
            EastExitGeometry.Freeze();

            UpExitGeometry = Geometry.Parse("M5.1790161,5.1693864 L1,5.208189 L1,7.2087359 L5.1790161,7.1703711 z M5.1790161,2.1503892 L1,2.1891921 L1,4.2297473 L5.1790161,4.1913824 z M5.6591058,6.686151E-05 C5.8254476,-0.0017583817 5.9984264,0.033741869 6.1790161,0.11204322 L6.1790161,8.4298897 C5.7820988,8.6196117 5.4618106,8.5806875 5.1790161,8.4298897 L5.1790161,8.1483746 L1,8.1871777 L1,8.4372969 C0.71720558,8.5864563 0.39691716,8.6249571 0,8.4372969 L0,0.20984003 C0.18058951,0.13238949 0.35356849,0.097275011 0.51991057,0.099080339 C0.68625271,0.10088577 0.84595799,0.13961101 1,0.20984003 L1,1.2107502 L5.1790161,1.1723855 L5.1790161,0.11204322 C5.3330584,0.041042738 5.4927635,0.0018920451 5.6591058,6.686151E-05 z");
            UpExitGeometry.Freeze();

            DownExitGeometry = Geometry.Parse("M0.99999994,4.9353776 L0.99999994,7.2073736 L5.1790161,7.2457385 L5.1790161,4.9741802 z M0.99999994,1.4983659 L0.99999994,3.9573739 L5.1790161,3.9957387 L5.1790161,1.5371687 z M0.51991075,7.9870224E-05 C0.68625271,0.002260685 0.84595793,0.049042225 0.99999994,0.13388109 L0.99999994,0.52036214 L5.1790161,0.55872703 L5.1790161,0.27224088 C5.4871001,0.10589814 5.8178372,0.088793516 6.1790161,0.27224088 L6.1790161,10.015921 C5.7820988,10.238165 5.4618106,10.192569 5.1790161,10.015921 L5.1790161,8.2241802 L0.99999994,8.1853771 L0.99999994,10.072922 C0.71720552,10.253111 0.3969171,10.299622 0,10.072922 L0,0.13388109 C0.18058944,0.040318251 0.35356843,-0.0021011829 0.51991075,7.9870224E-05 z");
            DownExitGeometry = DownExitGeometry.Clone();
            DownExitGeometry.Transform = new TranslateTransform(0, 19);
            DownExitGeometry.Freeze();

            NonVisitedRoomBrush = new SolidColorBrush(Color.FromRgb(0x5E, 0x5E, 0x5E));
            NonVisitedRoomBrush.Freeze();

            CurrentRoomPen = new Pen(Brushes.White, 2);
            CurrentRoomPen.Freeze();

            DefaultRoomPen = new Pen(Brushes.White, 0);
            DefaultRoomPen.Freeze();

            SelectedRoomPen = new Pen(Brushes.Red, 2);
            SelectedRoomPen.Freeze();

            DefaultExitBrush = new SolidColorBrush(Color.FromRgb(0x55, 0x69, 0x74));
            DefaultExitBrush.Freeze();

            CurrentRoomExitBrush = new SolidColorBrush(Colors.White);
            CurrentRoomExitBrush.Freeze();

            InvisibleExitBrush = new SolidColorBrush(Colors.Transparent);
            InvisibleExitBrush.Freeze();
        }

        /// <summary>
        /// Gets the geometry to use to render room.
        /// </summary>
        [NotNull]
        public RectangleGeometry RoomGeometry
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the geometry to use to render north exit.
        /// </summary>
        [NotNull]
        public RectangleGeometry NorthExitGeometry
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the geometry to use to render south exit.
        /// </summary>
        [NotNull]
        public RectangleGeometry SouthExitGeometry
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the geometry to use to render east exit.
        /// </summary>
        [NotNull]
        public RectangleGeometry EastExitGeometry
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the geometry to use to render west exit.
        /// </summary>
        [NotNull]
        public RectangleGeometry WestExitGeometry
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the geometry to use to render up exit.
        /// </summary>
        [NotNull]
        public Geometry UpExitGeometry
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the geometry to use to render down exit.
        /// </summary>
        [NotNull]
        public Geometry DownExitGeometry
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the pen to use to render room.
        /// </summary>
        [NotNull]
        public Pen DefaultRoomPen
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the pen to use to render selected room.
        /// </summary>
        [NotNull]
        public Pen SelectedRoomPen
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the pen to use to render current room.
        /// </summary>
        [NotNull]
        public Pen CurrentRoomPen
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the brush to use to render room that has not been visited yet.
        /// </summary>
        [NotNull]
        public Brush NonVisitedRoomBrush
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the brush to use to render exit.
        /// </summary>
        [NotNull]
        public Brush DefaultExitBrush
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the invisible exit brush.
        /// </summary>
        [NotNull]
        public Brush InvisibleExitBrush
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the brush to use to render exit of current room.
        /// </summary>
        [NotNull]
        public Brush CurrentRoomExitBrush
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the room color brush.
        /// </summary>
        /// <param name="roomColor">Color of the room.</param>
        /// <returns>A brush to use to display a room.</returns>
        [NotNull]
        public Brush GetRoomColorBrush(RoomColor roomColor)
        {
            if (_fetchedRoomColors.ContainsKey(roomColor))
            {
                return _fetchedRoomColors[roomColor];
            }

            Brush result;

            switch (roomColor)
            {
                case RoomColor.Default:
                    result = (Brush)_zoneVisual.FindResource("Brush_Room_Default");
                    break;
                case RoomColor.Purple:
                    result = (Brush)_zoneVisual.FindResource("Brush_Room_Purple");
                    break;
                case RoomColor.Red:
                    result = (Brush)_zoneVisual.FindResource("Brush_Room_Red");
                    break;
                case RoomColor.Green:
                    result = (Brush)_zoneVisual.FindResource("Brush_Room_Green");
                    break;
                case RoomColor.Yellow:
                    result = (Brush)_zoneVisual.FindResource("Brush_Room_Yellow");
                    break;
                case RoomColor.Brown:
                    result = (Brush)_zoneVisual.FindResource("Brush_Room_Brown");
                    break;
                default:
                    result = (Brush)_zoneVisual.FindResource("Brush_Room_Default");
                    break;
            }

            _fetchedRoomColors[roomColor] = result;
            return result;
        }

        /// <summary>
        /// Gets the room icon brush.
        /// </summary>
        /// <param name="roomIcon">The room icon.</param>
        /// <returns>A brush to use to display a room icon.</returns>
        [NotNull]
        public Brush GetRoomIconBrush(RoomIcon roomIcon)
        {
            if (_fetchedRoomIcons.ContainsKey(roomIcon))
            {
                return _fetchedRoomIcons[roomIcon];
            }

            Brush result;

            switch (roomIcon)
            {
                case RoomIcon.None:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_None");
                    break;
                case RoomIcon.FoodShop:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_FoodShop");
                    break;
                case RoomIcon.WeaponShop:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_WeaponShop");
                    break;
                case RoomIcon.MagicShop:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_MagicShop");
                    break;
                case RoomIcon.LeatherShop:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_LeatherShop");
                    break;
                case RoomIcon.Bank:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Bank");
                    break;
                case RoomIcon.Route:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Route");
                    break;
                case RoomIcon.Quester:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Quester");
                    break;
                case RoomIcon.Archer:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Archer");
                    break;
                case RoomIcon.Barbarian:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Barbarian");
                    break;
                case RoomIcon.Cleric:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Cleric");
                    break;
                case RoomIcon.DarkKnight:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_DarkKnight");
                    break;
                case RoomIcon.Druid:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Druid");
                    break;
                case RoomIcon.Mage:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Mage");
                    break;
                case RoomIcon.Paladin:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Paladin");
                    break;
                case RoomIcon.Pathfinder:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_PathFinder");
                    break;
                case RoomIcon.Thief:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Thief");
                    break;
                case RoomIcon.Warrior:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Warrior");
                    break;
                case RoomIcon.Question:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Question");
                    break;
                case RoomIcon.Horse:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Horse");
                    break;
                case RoomIcon.Doska:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Doska");
                    break;
                case RoomIcon.Sklad:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Sklad");
                    break;
                case RoomIcon.Post:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_Post");
                    break;
                case RoomIcon.MiscShop:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_MiscShop");
                    break;
                default:
                    result = (Brush)_zoneVisual.FindResource("Brush_MapIcon_None");
                    break;
            }

            _fetchedRoomIcons[roomIcon] = result;
            return result;
        }
    }
}
