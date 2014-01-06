// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuiltInCommandTypes.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the BuiltInCommandTypes type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Commands
{
    /// <summary>
    /// A class to store built in command types.
    /// </summary>
    public static class BuiltInCommandTypes
    {
        /// <summary>
        /// Type of any "text" command (i.e. command typed by end-user).
        /// </summary>
        public const int TextCommand = 1;

        /// <summary>
        /// Type of hotkey command.
        /// </summary>
        public const int HotkeyCommand = 2;

        /// <summary>
        /// Type of command to connect/disconnect.
        /// </summary>
        public const int ConnectionCommands = 3;

        /// <summary>
        /// 
        /// </summary>
        public const int ShowMainOutputCommand = 4;

        /// <summary>
        /// 
        /// </summary>
        public const int SendToWindow = 5;
    }
}
