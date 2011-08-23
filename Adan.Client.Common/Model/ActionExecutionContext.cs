// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionExecutionContext.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ActionExecutionContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System.Collections.Generic;

    using CSLib.Net.Annotations;

    using Messages;

    /// <summary>
    /// Context of action execution.
    /// </summary>
    public class ActionExecutionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionExecutionContext"/> class.
        /// </summary>
        public ActionExecutionContext()
        {
            Parameters = new Dictionary<int, string>();
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        [NotNull]
        public IDictionary<int, string> Parameters
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the current message.
        /// </summary>
        /// <value>
        /// The current message.
        /// </value>
        [CanBeNull]
        public Message CurrentMessage
        {
            get;
            set;
        }
    }
}
