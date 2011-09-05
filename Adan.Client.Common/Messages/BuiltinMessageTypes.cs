// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuiltInMessageTypes.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the BuiltInMessageTypes type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Messages
{
    /// <summary>
    /// A class to store built in message types.
    /// </summary>
    public static class BuiltInMessageTypes
    {
        /// <summary>
        /// Type of connect/disconnect messages.
        /// </summary>
        public const int ConnectionMessages = 1;
        
        /// <summary>
        /// Type of misc system messages (error message, for example) 
        /// </summary>
        public const int SystemMessage = 2;

        /// <summary>
        /// Type of any text message (i.e. message that comes from server)
        /// </summary>
        public const int TextMessage = 3;

        /// <summary>
        /// Type of message to start/stop logging.
        /// </summary>
        public const int LoggingMessage = 4;

        /// <summary>
        /// Type of message to turn echoing on/off.
        /// </summary>
        public const int EchoModeMessage = 5;
    }
}
