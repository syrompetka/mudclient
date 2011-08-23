// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartLoggingMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the StartLoggingMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Messages
{
    using Common.Messages;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Message to start logging.
    /// </summary>
    public class StartLoggingMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartLoggingMessage"/> class.
        /// </summary>
        /// <param name="logName">Name of the log.</param>
        public StartLoggingMessage([NotNull] string logName)
        {
            Assert.ArgumentNotNull(logName, "logName");

            LogName = logName;
        }

        /// <summary>
        /// Gets or sets the name of the log.
        /// </summary>
        /// <value>
        /// The name of the log.
        /// </value>
        [NotNull]
        public string LogName
        {
            get;
            set;
        }
    }
}
