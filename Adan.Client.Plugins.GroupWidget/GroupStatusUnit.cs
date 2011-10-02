// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupStatusUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the GroupStatusUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Threading;

    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// <see cref="ConveyorUnit"/> implementation to handle <see cref="GroupStatusMessage"/> messages.
    /// </summary>
    public class GroupStatusUnit : ConveyorUnit
    {
        private readonly GroupWidgetControl _groupWidgetControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupStatusUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        /// <param name="groupWidgetControl">The group widget control.</param>
        public GroupStatusUnit([NotNull] MessageConveyor messageConveyor, [NotNull] GroupWidgetControl groupWidgetControl)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
            Assert.ArgumentNotNull(groupWidgetControl, "groupWidgetControl");

            _groupWidgetControl = groupWidgetControl;
        }

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return Enumerable.Repeat(Constants.GroupStatusMessageType, 1);
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
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            var groupStatusMessage = message as GroupStatusMessage;
            if (groupStatusMessage != null)
            {
                _groupWidgetControl.UpdateModel(groupStatusMessage);
            }
        }
    }
}
