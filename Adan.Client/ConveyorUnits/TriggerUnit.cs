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
        private readonly RootModel _rootModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        /// <param name="rootModel">The RootModel.</param>
        public TriggerUnit([NotNull] MessageConveyor messageConveyor, [NotNull] RootModel rootModel)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
            Assert.ArgumentNotNull(rootModel, "rootModel");

            _rootModel = rootModel;
        }

        #region Overrides of ConveyorUnit

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            foreach (var trigger in _rootModel.EnabledTriggersOrderedByPriority)
            {
                if (message.SkipTriggers)
                {
                    break;
                }

                trigger.HandleMessage(message, _rootModel);
            }
        }

        #endregion
    }
}
