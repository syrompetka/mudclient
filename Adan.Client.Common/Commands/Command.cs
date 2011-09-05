// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Command.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Command type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Commands
{
    /// <summary>
    /// Base class for all commands sent from client to server.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Gets or sets a value indicating whether this command was handled and needs not further processing.
        /// </summary>
        /// <value>
        /// <c>true</c> if this command is handled; otherwise, <c>false</c>.
        /// </value>
        public bool Handled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the type of this command.
        /// </summary>
        /// <value>
        /// The type of this command.
        /// </value>
        public abstract int CommandType
        {
            get;
        }
    }
}
