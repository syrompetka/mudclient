// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Variable.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Variable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// A user defined variable.
    /// </summary>
    [Serializable]
    public class Variable
    {
        /// <summary>
        /// Gets or sets the name of this variable.
        /// </summary>
        /// <value>
        /// The name of this variable.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of this variable.
        /// </summary>
        /// <value>
        /// The value of this variable.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Value
        {
            get;
            set;
        }
    }
}
