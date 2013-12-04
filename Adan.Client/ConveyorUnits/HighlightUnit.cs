// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the HighlightUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Linq;

    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;
    using Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementation that highlights text.
    /// </summary>
    public class HighlightUnit : ConveyorUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightUnit"/> class.
        /// </summary>
        public HighlightUnit()
            : base()
        {
        }

        #region Overrides of ConveyorUnit

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return new[] { BuiltInMessageTypes.TextMessage };
            }
        }

        /// <summary>
        /// Gets a set of command types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledCommandTypes
        {
            get
            {
                return Enumerable.Empty<int>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="rootModel"></param>
        public override void HandleMessage(Message message, RootModel rootModel)
        {
            Assert.ArgumentNotNull(message, "message");

            var textMessage = message as TextMessage;
            if (textMessage == null)
            {
                return;
            }

            foreach (var group in rootModel.Groups.Where(g => g.IsEnabled))
            {
                foreach (var highlight in group.Highlights)
                {
                    highlight.ProcessMessage(textMessage, rootModel);
                }
            }
        }

        #endregion
    }
}
