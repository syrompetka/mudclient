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

    /// <summary>
    /// <see cref="ConveyorUnit"/> implementation to handle <see cref="RoomMonstersMessage"/> messages.
    /// </summary>
    public class RoomMonstersUnit : ConveyorUnit
    {
        private readonly MonstersWidgetControl _monstersWidgetControl;
        private readonly RoomMonstersViewModel _roomMonstersViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomMonstersUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        /// <param name="monstersWidgetControl">The monsters widget control.</param>
        /// <param name="roomMonstersViewModel">The room monsters view model.</param>
        public RoomMonstersUnit([NotNull] MessageConveyor messageConveyor, [NotNull] MonstersWidgetControl monstersWidgetControl, [NotNull] RoomMonstersViewModel roomMonstersViewModel)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
            Assert.ArgumentNotNull(monstersWidgetControl, "monstersWidgetControl");
            Assert.ArgumentNotNull(roomMonstersViewModel, "roomMonstersViewModel");

            _monstersWidgetControl = monstersWidgetControl;
            _roomMonstersViewModel = roomMonstersViewModel;
        }

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return Enumerable.Repeat(Constants.RoomMonstersMessage, 1);
            }
        }

        /// <summary>
        /// Gets a set of command types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledCommandTypes
        {
            get
            {
                //Временное включение буквы ё
                //return Enumerable.Repeat(BuiltInCommandTypes.HotkeyCommand, 1);
                return Enumerable.Empty<int>();
            }
        }

        /// <summary>
        /// Handles the command.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        public override void HandleCommand(Command command)
        {
            Assert.ArgumentNotNull(command, "command");

            var hotKeyCommand = command as HotkeyCommand;
            if (hotKeyCommand == null)
            {
                return;
            }

            if (hotKeyCommand.Key == Key.OemTilde && hotKeyCommand.ModifierKeys == ModifierKeys.None)
            {
                hotKeyCommand.Handled = true;
                if (_roomMonstersViewModel.SelectedMonster == null || _roomMonstersViewModel.Monsters.IndexOf(_roomMonstersViewModel.SelectedMonster) == _roomMonstersViewModel.Monsters.Count - 1)
                {
                    _roomMonstersViewModel.SelectedMonster = _roomMonstersViewModel.Monsters.FirstOrDefault();
                    return;
                }

                var index = _roomMonstersViewModel.Monsters.IndexOf(_roomMonstersViewModel.SelectedMonster);
                _roomMonstersViewModel.SelectedMonster = _roomMonstersViewModel.Monsters[index + 1];
                return;
            }

            if (hotKeyCommand.Key == Key.OemTilde && hotKeyCommand.ModifierKeys == ModifierKeys.Shift)
            {
                hotKeyCommand.Handled = true;
                if (_roomMonstersViewModel.SelectedMonster == null || _roomMonstersViewModel.Monsters.IndexOf(_roomMonstersViewModel.SelectedMonster) == 0)
                {
                    _roomMonstersViewModel.SelectedMonster = _roomMonstersViewModel.Monsters.LastOrDefault();
                    return;
                }

                var index = _roomMonstersViewModel.Monsters.IndexOf(_roomMonstersViewModel.SelectedMonster);
                _roomMonstersViewModel.SelectedMonster = _roomMonstersViewModel.Monsters[index - 1];
                return;
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            var roomMonstersMessage = message as RoomMonstersMessage;
            if (roomMonstersMessage != null)
            {
                _monstersWidgetControl.UpdateModel(roomMonstersMessage);
            }
        }
    }
}
