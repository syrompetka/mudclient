// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutePathElement.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   A part of path between two rooms.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map
{
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;

    /// <summary>
    /// A part of path between two rooms.
    /// </summary>
    public class RoutePathElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutePathElement"/> class.
        /// </summary>
        /// <param name="room">The room of this element.</param>
        public RoutePathElement([NotNull]RoomViewModel room)
        {
            Assert.ArgumentNotNull(room, "room");

            Room = room;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutePathElement"/> class.
        /// </summary>
        /// <param name="room">The room of this element.</param>
        /// <param name="previous">The previous <see cref="RoutePathElement"/>.</param>
        public RoutePathElement([NotNull]RoomViewModel room, [NotNull] RoutePathElement previous)
        {
            Assert.ArgumentNotNull(room, "room");
            Assert.ArgumentNotNull(previous, "previous");
            Room = room;
            Previous = previous;
        }

        /// <summary>
        /// Gets the previous <see cref="RoutePathElement"/>.
        /// </summary>
        [CanBeNull]
        public RoutePathElement Previous
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the room of this element.
        /// </summary>
        [NotNull]
        public RoomViewModel Room
        {
            get;
            private set;
        }
    }
}