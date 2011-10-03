// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Position.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Position type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    /// <summary>
    /// Possible positions of character.
    /// </summary>
    public enum Position
    {
        /// <summary>
        /// Character is dying.
        /// </summary>
        Dying,

        /// <summary>
        /// Character is sleeping.
        /// </summary>
        Sleeping,

        /// <summary>
        /// Character is resting.
        /// </summary>
        Resting,

        /// <summary>
        /// Character is sitting (bashed).
        /// </summary>
        Sitting,

        /// <summary>
        /// Character is fighting. 
        /// </summary>
        Fighting,
        
        /// <summary>
        /// Character is standing.
        /// </summary>
        Standing,

        /// <summary>
        /// Character is riding a horse.
        /// </summary>
        Riding
    }
}
