// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExitDirection.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ExitDirection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Model
{
    using System;

    /// <summary>
    /// Possible exit directions
    /// </summary>
    [Serializable]
    public enum ExitDirection
    {
        /// <summary>
        /// North direction.
        /// </summary>
        North,

        /// <summary>
        /// South direction.
        /// </summary>
        South,

        /// <summary>
        /// West direction.
        /// </summary>
        West,

        /// <summary>
        /// East direction.
        /// </summary>
        East,

        /// <summary>
        /// Up direction. 
        /// </summary>
        Up,

        /// <summary>
        /// Down direction.
        /// </summary>
        Down,
    }
}