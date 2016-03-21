// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Group.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Group type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;
    using System.Collections.Concurrent;

    /// <summary>
    /// A group of triggers, aliases etc.
    /// </summary>
    [Serializable]
    public class Group
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        public Group()
        {
            Triggers = new List<TriggerBase>();
            Aliases = new List<CommandAlias>();
            Hotkeys = new List<Hotkey>();
            Highlights = new List<Highlight>();
            Substitutions = new List<Substitution>();
        }

        /// <summary>
        /// Gets or sets the name of this group.
        /// </summary>
        /// <value>
        /// The name of this group.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this group is enabled or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if this group is enabled; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool IsEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is build in or not.
        /// If a group is built in then it can not be deleted and disabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is build in; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool IsBuildIn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the triggers that belong to this group.
        /// </summary>
        /// <value>
        /// The triggers.
        /// </value>
        [NotNull]
        public List<TriggerBase> Triggers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets aliases that belong to this group.
        /// </summary>
        /// <value>
        /// The aliases.
        /// </value>
        [NotNull]
        public List<CommandAlias> Aliases
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the hotkeys of this group.
        /// </summary>
        /// <value>
        /// The hotkeys.
        /// </value>
        [NotNull]
        public List<Hotkey> Hotkeys
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the highlights.
        /// </summary>
        /// <value>
        /// The highlights.
        /// </value>
        [NotNull]
        public List<Highlight> Highlights
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the substitutions.
        /// </summary>
        /// <value>
        /// The substitutions.
        /// </value>
        [NotNull]
        public List<Substitution> Substitutions
        {
            get;
            set;
        }
    }
}
