// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelnetConstants.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TelnetConstants type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Networking
{
    /// <summary>
    /// Class to store telnet constants.
    /// </summary>
    public static class TelnetConstants
    {
        /// <summary>
        /// Telnet "go ahead".
        /// </summary>
        public const byte GoAheadCode = 0xF9;

        /// <summary>
        /// Telnet "IAC" code.
        /// </summary>
        public const byte InterpretAsCommandCode = 0xFF;

        /// <summary>
        /// Telnet "WILL" code.
        /// </summary>
        public const byte WillCode = 0xFB;

        /// <summary>
        /// Telnet "WONT" code.
        /// </summary>
        public const byte WillNotCode = 0xFC;

        /// <summary>
        /// Telnet "DO" code.
        /// </summary>
        public const byte DoCode = 0xFD;

        /// <summary>
        /// Telnet "DONOT" code
        /// </summary>
        public const byte DoNotCode = 0xFE;

        /// <summary>
        /// Telnet "COMPRESS" code.
        /// </summary>
        public const byte CompressCode = 0x55;

        /// <summary>
        /// Telnet "CUSTOM_PROTOCOL" code.
        /// </summary>
        public const byte CustomProtocolCode = 0x57;

        /// <summary>
        /// Telnet "SB" code.
        /// </summary>
        public const byte SubNegotiationStartCode = 0xFA;

        /// <summary>
        /// Telnet "SE" code.
        /// </summary>
        public const byte SubNegotiationEndCode = 0xF0;

        /// <summary>
        /// Telnet "ECHO" code.
        /// </summary>
        public const byte EchoCode = 0x01;
    }
}
