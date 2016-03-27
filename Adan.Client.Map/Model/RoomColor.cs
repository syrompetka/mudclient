// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomColor.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the RoomColor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Model
{
    using System;

    /// <summary>
    /// Possible colors that can be used to display room on the map.
    /// </summary>
    [Serializable]
    public enum RoomColor
    {
        /// <summary>
        /// Default room color.
        /// </summary>
        Default,

        /// <summary>
        /// Red room color.
        /// </summary>
        Red,

        /// <summary>
        /// Yellow room color.
        /// </summary>
        Yellow,

        /// <summary>
        /// Purple room color.
        /// </summary>
        Purple,

        /// <summary>
        /// Brown room color.
        /// </summary>
        Brown,

        /// <summary>
        /// Green room color.
        /// </summary>
        Green
    }
}
