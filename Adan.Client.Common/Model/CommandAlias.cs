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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Serialization;
    using CSLib.Net.Annotations;

    /// <summary>
    /// A "shortcut" to a command or a set of commands. Whenever the user types an alias command, it will be replaces by actions of the alias.
    /// </summary>
    [Serializable]
    public class CommandAlias : IUndo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAlias"/> class.
        /// </summary>
        public CommandAlias()
        {
            Command = string.Empty;
            Actions = new List<ActionBase>();

            Group = null;
            Operation= UndoOperation.None;
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Command
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the actions to be performed when user uses this alias.
        /// </summary>
        [NotNull]
        public List<ActionBase> Actions
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public UndoOperation Operation
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public Group Group
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string UndoInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#Алиас {").Append(Command).Append("} ");
            switch(Operation)
            {
                case UndoOperation.Add:
                    sb.Append("восстановлен");
                    break;
                case UndoOperation.Remove:
                    sb.Append("удален");
                    break;
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Undo(RootModel rootModel)
        {
            if (Group != null && Operation != UndoOperation.None)
            {
                switch(Operation)
                {
                    case UndoOperation.Add:
                        Group.Aliases.Add(this);
                        break;
                    case UndoOperation.Remove:
                        Group.Aliases.Remove(this);
                        break;
                }
            }
        }
    }
}
