// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Route.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Route type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// Class to store single route.
    /// </summary>
    [Serializable]
    public class Route
    {
        private readonly List<int> _routeRoomIdentifiers = new List<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Route"/> class.
        /// </summary>
        public Route()
        {
            StartName = string.Empty;
            EndName = string.Empty;
            RoomIdentifiersSet = new HashSet<int>();
            RoutePointsAvailableFromStart = new Dictionary<string, int>();
            RoutePointsAvailableFromEnd = new Dictionary<string, int>();
        }

        /// <summary>
        /// Gets or sets the start name.
        /// </summary>
        /// <value>
        /// The start name.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string StartName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the end name.
        /// </summary>
        /// <value>
        /// The end name.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string EndName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the route room identifiers.
        /// </summary>
        [NotNull]
        public List<int> RouteRoomIdentifiers
        {
            get
            {
                return _routeRoomIdentifiers;
            }
        }

        /// <summary>
        /// Gets the start room id.
        /// </summary>
        [XmlIgnore]
        public int StartRoomId
        {
            get
            {
                return RouteRoomIdentifiers.First();
            }
        }

        /// <summary>
        /// Gets the end room id.
        /// </summary>
        [XmlIgnore]
        public int EndRoomId
        {
            get
            {
                return RouteRoomIdentifiers.Last();
            }
        }

        /// <summary>
        /// Gets the route points available from start.
        /// </summary>
        [NotNull]
        [XmlIgnore]
        public IDictionary<string, int> RoutePointsAvailableFromStart
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the route points available from end.
        /// </summary>
        [NotNull]
        [XmlIgnore]
        public IDictionary<string, int> RoutePointsAvailableFromEnd
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the room identifiers set.
        /// </summary>
        [NotNull]
        [XmlIgnore]
        public HashSet<int> RoomIdentifiersSet
        {
            get;
            private set;
        }
    }
}
