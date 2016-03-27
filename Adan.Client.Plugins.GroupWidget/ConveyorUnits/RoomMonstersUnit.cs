// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomMonstersUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the RoomMonstersUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;
    using Adan.Client.Plugins.GroupWidget.Messages;
    using Adan.Client.Common.Model;
    using System.Text.RegularExpressions;

    /// <summary>
    /// <see cref="ConveyorUnit"/> implementation to handle <see cref="RoomMonstersMessage"/> messages.
    /// </summary>
    public class RoomMonstersUnit : ConveyorUnit
    {
        private readonly MonstersWidgetControl _monstersWidgetControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomMonstersUnit"/> class.
        /// </summary>
        /// <param name="monstersWidgetControl">The monsters widget control.</param>
        public RoomMonstersUnit([NotNull] MonstersWidgetControl monstersWidgetControl)
            : base()
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="rootModel"></param>
        /// <param name="isImport"></param>
        public override void HandleCommand([NotNull]Command command, [NotNull]RootModel rootModel, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");
            Assert.ArgumentNotNull(rootModel, "rootModel");

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
