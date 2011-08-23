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
    using System.Runtime.Serialization;

    /// <summary>
    /// Possible condition types.
    /// </summary>
    [DataContract]
    public enum ActionCondition
    {
        /// <summary>
        /// Two values equal to each other.
        /// </summary>
        [EnumMember]
        Equals,

        /// <summary>
        /// Two values do not equal to each other.
        /// </summary>
        [EnumMember]
        NotEquals,

        /// <summary>
        /// First value is greater than second one.
        /// </summary>
        [EnumMember]
        Greater,

        /// <summary>
        /// First value is greater or equals to second one.
        /// </summary>
        [EnumMember]
        GreaterOrEquals,

        /// <summary>
        /// First value is less than second one.
        /// </summary>
        [EnumMember]
        Less,

        /// <summary>
        /// First value is less or equal to second one.
        /// </summary>
        [EnumMember]
        LessOrEquals,

        /// <summary>
        /// First value is empty or not defined.
        /// </summary>
        [EnumMember]
        IsEmpty,

        /// <summary>
        /// First value is not empty or not defined.
        /// </summary>
        [EnumMember]
        IsNotEmpty,
    }
}
