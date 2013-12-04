// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TriggerUnit type.
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
    /// A <see cref="ConveyorUnit"/> that processes triggers.
    /// </summary>
    public class TriggerUnit : ConveyorUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerUnit"/> class.
        /// </summary>
        public TriggerUnit()
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
            foreach (var trigger in rootModel.EnabledTriggersOrderedByPriority)
            {
                if (message.SkipTriggers)
                {
                    break;
                }

                trigger.HandleMessage(message, rootModel);
            }
        }

        #endregion
    }
}
