// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandAlias.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the CommandAlias type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// A "shortcut" to a command or a set of commands. Whenever the user types an alias command, it will be replaces by actions of the alias.
    /// </summary>
    [DataContract]
    public class CommandAlias
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAlias"/> class.
        /// </summary>
        public CommandAlias()
        {
            Command = string.Empty;
            Actions = new List<ActionBase>();
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        [NotNull]
        [DataMember]
        public string Command
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the actions to be performed when user uses this alias.
        /// </summary>
        [NotNull]
        [DataMember]
        public List<ActionBase> Actions
        {
            get;
            set;
        }
    }
}
