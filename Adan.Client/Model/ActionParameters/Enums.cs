// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enums.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Enums type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.ActionParameters
{
    using System;

    /// <summary>
    /// Possible types of math operations.
    /// </summary>
    [Serializable]
    public enum MathOperation
    {
        /// <summary>
        /// Addition operation.
        /// </summary>
        Plus,

        /// <summary>
        /// Substraction operation.
        /// </summary>
        Minus,

        /// <summary>
        /// Division operation.
        /// </summary>
        Division,

        /// <summary>
        /// Multiplication operation.
        /// </summary>
        Multiplication,
    }
}
