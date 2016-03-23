// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ingredient.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Details of some ingridient.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase
{
    using System;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// Details of some ingredient.
    /// </summary>
    [Serializable]
    public class Ingredient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ingredient"/> class.
        /// </summary>
        public Ingredient()
        {
            Color = string.Empty;
        }

        /// <summary>
        /// Gets or sets the color of an ingredient.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the power of an ingredient.
        /// </summary>
        /// <value>
        /// The power.
        /// </value>
        [XmlAttribute]
        public int Power
        {
            get;
            set;
        }
    }
}