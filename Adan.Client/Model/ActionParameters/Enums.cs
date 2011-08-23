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
    using System.Runtime.Serialization;

    /// <summary>
    /// Possible types of math operations.
    /// </summary>
    [DataContract]
    public enum MathOperation
    {
        /// <summary>
        /// Addition operation.
        /// </summary>
        [EnumMember]
        Plus,

        /// <summary>
        /// Substraction operation.
        /// </summary>
        [EnumMember]
        Minus,

        /// <summary>
        /// Division operation.
        /// </summary>
        [EnumMember]
        Division,

        /// <summary>
        /// Multiplication operation.
        /// </summary>
        [EnumMember]
        Multiplication,
    }
}
