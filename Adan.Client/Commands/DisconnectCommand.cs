// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisconnectCommand.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the DisconnectCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Commands
{
    using Common.Commands;

    /// <summary>
    /// Command to disconnect from server.
    /// </summary>
    public class DisconnectCommand : Command
    {
        /// <summary>
        /// Gets the type of this command.
        /// </summary>
        /// <value>
        /// The type of this command.
        /// </value>
        public override int CommandType
        {
            get
            {
                return BuiltInCommandTypes.ConnectionCommands;
            }
        }
    }
}
