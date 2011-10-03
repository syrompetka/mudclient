// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enums.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Possible condition types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.Actions
{
    using System;

    /// <summary>
    /// Possible condition types.
    /// </summary>
    [Serializable]
    public enum ActionCondition
    {
        /// <summary>
        /// Two values equal to each other.
        /// </summary>
        Equals,

        /// <summary>
        /// Two values do not equal to each other.
        /// </summary>
        NotEquals,

        /// <summary>
        /// First value is greater than second one.
        /// </summary>
        Greater,

        /// <summary>
        /// First value is greater or equals to second one.
        /// </summary>
        GreaterOrEquals,

        /// <summary>
        /// First value is less than second one.
        /// </summary>
        Less,

        /// <summary>
        /// First value is less or equal to second one.
        /// </summary>
        LessOrEquals,

        /// <summary>
        /// First value is empty or not defined.
        /// </summary>
        IsEmpty,

        /// <summary>
        /// First value is not empty or not defined.
        /// </summary>
        IsNotEmpty,
    }
}
