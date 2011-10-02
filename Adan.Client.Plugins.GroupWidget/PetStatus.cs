// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PetStatus.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the PetStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget
{
    using System;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// A class to describe status of single pet in a group.
    /// </summary>
    [Serializable]
    public class PetStatus : CharacterStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PetStatus"/> class.
        /// </summary>
        public PetStatus()
        {
            Owner = string.Empty;
        }

        /// <summary>
        /// Gets or sets the owner name of this pet.
        /// </summary>
        /// <value>
        /// The owner.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Owner
        {
            get;
            set;
        }
    }
}