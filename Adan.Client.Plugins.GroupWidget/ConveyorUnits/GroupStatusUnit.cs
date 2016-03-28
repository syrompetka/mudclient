// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupStatusUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the GroupStatusUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using GroupWidget.Messages;

    /// <summary>
    /// <see cref="ConveyorUnit"/> implementation to handle <see cref="GroupStatusMessage"/> messages.
    /// </summary>
    public class GroupStatusUnit : ConveyorUnit
    {
        private readonly GroupWidgetControl _groupWidget;

        public GroupStatusUnit([NotNull] GroupWidgetControl groupWidget, MessageConveyor conveyor)
            : base(conveyor)
        {
            Assert.ArgumentNotNull(groupWidget, "groupWidget");

            _groupWidget = groupWidget;
        }

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return Enumerable.Empty<int>();
            }
        }

        /// <summary>
        /// Gets a set of command types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledCommandTypes
        {
            get
            {
                return Enumerable.Repeat(BuiltInCommandTypes.TextCommand, 1);
            }
        }

        public override void HandleCommand([NotNull] Command command, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");

            var textCommand = command as TextCommand;
            if (textCommand == null)
                return;

            if (textCommand.CommandText.StartsWith("#nextgroupmate", System.StringComparison.InvariantCulture))
            {
                textCommand.Handled = true;
                _groupWidget.NextGroupMate();
            }
            else if (textCommand.CommandText.StartsWith("#previousgroupmate", System.StringComparison.InvariantCulture))
            {
                textCommand.Handled = true;
                _groupWidget.PreviousGroupMate();
            }
        }
    }
}