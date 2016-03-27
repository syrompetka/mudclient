// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Zone.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Zone type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Model
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// Represents a single zone.
    /// </summary>
    [Serializable]
    public class Zone
    {
        private readonly List<Room> _rooms = new List<Room>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Zone"/> class.
        /// </summary>
        public Zone()
        {
            Name = string.Empty;
        }

        /// <summary>
        /// Gets or sets the unique identifier of this zone.
        /// </summary>
        /// <value>
        /// The unique identifier of this zone.
        /// </value>
        [XmlAttribute]
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of this zone.
        /// </summary>
        /// <value>
        /// The name of this zone.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection of rooms this zone contains.
        /// </summary>
        [NotNull]
        public List<Room> Rooms
        {
            get
            {
                return _rooms;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="Zone"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Zone"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="Zone"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals([CanBeNull] Zone other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.Id == Id;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals([CanBeNull]object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(Zone))
            {
                return false;
            }

            return Equals((Zone)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
