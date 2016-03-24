// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomExitViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the RoomExitViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.ViewModel
{
    using CSLib.Net.Annotations;

    using Model;

    /// <summary>
    /// View model for single room exit.
    /// </summary>
    public class RoomExitViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomExitViewModel"/> class.
        /// </summary>
        /// <param name="direction">The direction of an exit.</param>
        /// <param name="room">The room this exit leads to.</param>
        public RoomExitViewModel(ExitDirection direction, [CanBeNull] RoomViewModel room)
        {
            Direction = direction;
            Room = room;
        }

        /// <summary>
        /// Gets the direction of this exit.
        /// </summary>
        public ExitDirection Direction
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the room this exit leads to.
        /// </summary>
        [CanBeNull]
        public RoomViewModel Room
        {
            get;
            private set;
        }
    }
}
