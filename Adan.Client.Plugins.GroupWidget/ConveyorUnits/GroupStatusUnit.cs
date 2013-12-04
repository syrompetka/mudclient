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

    /// <summary>
    /// <see cref="ConveyorUnit"/> implementation to handle <see cref="GroupStatusMessage"/> messages.
    /// </summary>
    public class GroupStatusUnit : ConveyorUnit
    {
        private readonly GroupWidgetControl _groupWidget;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupWidget"></param>
        public GroupStatusUnit([NotNull] GroupWidgetControl groupWidget)
            : base()
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
                return Enumerable.Repeat(BuiltInCommandTypes.HotkeyCommand, 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="rootModel"></param>
        /// <param name="isImport"></param>
        public override void HandleCommand([NotNull] Command command, [NotNull] RootModel rootModel, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");
            Assert.ArgumentNotNull(rootModel, "rootModel");

            var hotKeyCommand = command as HotkeyCommand;
            if (hotKeyCommand == null)
            {
                return;
            }

            if (hotKeyCommand.Key == Key.Tab && hotKeyCommand.ModifierKeys == ModifierKeys.None)
            {
                hotKeyCommand.Handled = true;
                _groupWidget.NextGroupMate();
                return;
            }

            if (hotKeyCommand.Key == Key.Tab && hotKeyCommand.ModifierKeys == ModifierKeys.Shift)
            {
                hotKeyCommand.Handled = true;
                _groupWidget.PreviousGroupMate();
                return;
            }
        }
    }
}