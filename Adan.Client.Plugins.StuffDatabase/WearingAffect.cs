// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WearingAffect.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Description of affect that is applied to player character when some object is weared.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase
{
    using System;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// Description of affect that is applied to player character when some object is weared.
    /// </summary>
    [Serializable]
    public class WearingAffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WearingAffect"/> class.
        /// </summary>
        public WearingAffect()
        {
            AffectName = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name of the affect.
        /// </summary>
        /// <value>
        /// The name of the affect.
        /// </value>
        [NotNull]
        [XmlAttribute(AttributeName = "Name")]
        public string AffectName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the level of the affect.
        /// </summary>
        /// <value>
        /// The level of the affect.
        /// </value>
        [XmlAttribute]
        public int Level
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the timeout between affect superpositions.
        /// </summary>
        /// <value>
        /// The reset timeout in game ticks.
        /// </value>
        [XmlAttribute]
        public int ResetTimeout
        {
            get;
            set;
        }
    }
}