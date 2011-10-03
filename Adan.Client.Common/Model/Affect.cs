// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Affect.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   A class to describe single affect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// A class to describe single affect.
    /// </summary>
    [Serializable]
    public class Affect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Affect"/> class.
        /// </summary>
        public Affect()
        {
            Name = string.Empty;
            Duration = -1.0f;
        }

        /// <summary>
        /// Gets or sets the affects name.
        /// </summary>
        /// <value>
        /// The affects name.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the affects duration.
        /// </summary>
        /// <value>
        /// The affects duration in seconds or -1 if affects duration is infinity.
        /// </value>
        [XmlAttribute]
        [DefaultValue(-1.0f)]
        public float Duration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of rounds left for this affect to disappear.
        /// </summary>
        [XmlAttribute]
        public int Rounds
        {
            get;
            set;
        }
    }
}