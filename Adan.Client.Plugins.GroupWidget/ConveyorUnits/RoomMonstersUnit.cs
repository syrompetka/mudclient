namespace Adan.Client.Plugins.GroupWidget.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;

    /// <summary>
    /// <see cref="ConveyorUnit"/> implementation to handle <see cref="RoomMonstersMessage"/> messages.
    /// </summary>
    public class RoomMonstersUnit : ConveyorUnit
    {
        private readonly MonstersWidgetControl _monstersWidgetControl;

        public RoomMonstersUnit([NotNull] MonstersWidgetControl monstersWidgetControl, MessageConveyor conveyor)
            : base(conveyor)
        {
            Assert.ArgumentNotNull(monstersWidgetControl, "monstersWidgetControl");

            _monstersWidgetControl = monstersWidgetControl;
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
        
        public override void HandleCommand([NotNull]Command command, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");

            var textCommand = command as TextCommand;
            if (textCommand == null)
                return;

           if (textCommand.CommandText.StartsWith("#nextmonster", System.StringComparison.InvariantCulture))
           {
                textCommand.Handled = true;

                _monstersWidgetControl.NextMonster();
            }
            else if (textCommand.CommandText.StartsWith("#previousmonster", System.StringComparison.InvariantCulture))
            {
                textCommand.Handled = true;

                _monstersWidgetControl.PreviousMonster();
            }
        }
    }
}
