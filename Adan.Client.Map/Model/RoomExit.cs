// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomExit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the RoomExit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Model
{
    using System;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// A single exit from some room.
    /// </summary>
    [Serializable]
    public class RoomExit
    {
        /// <summary>
        /// Gets or sets the direction of this exit.
        /// </summary>
        /// <value>
        /// The direction of this exit.
        /// </value>
        [XmlAttribute]
        public ExitDirection Direction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the identifier of a room this exit leads to.
        /// </summary>
        /// <value>
        /// The identifier of a room this exit leads to.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public int RoomId
        {
            get;
            set;
        }
    }
}