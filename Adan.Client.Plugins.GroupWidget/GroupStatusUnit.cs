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

    /// <summary>
    /// <see cref="ConveyorUnit"/> implementation to handle <see cref="GroupStatusMessage"/> messages.
    /// </summary>
    public class GroupStatusUnit : ConveyorUnit
    {
        private readonly GroupWidgetControl _groupWidgetControl;
        private readonly GroupStatusViewModel _groupStatusViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupStatusUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        /// <param name="groupWidgetControl">The group widget control.</param>
        /// <param name="groupStatusViewModel">The group status view model.</param>
        public GroupStatusUnit([NotNull] MessageConveyor messageConveyor, [NotNull] GroupWidgetControl groupWidgetControl, [NotNull] GroupStatusViewModel groupStatusViewModel)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
            Assert.ArgumentNotNull(groupWidgetControl, "groupWidgetControl");
            Assert.ArgumentNotNull(groupStatusViewModel, "groupStatusViewModel");

            _groupWidgetControl = groupWidgetControl;
            _groupStatusViewModel = groupStatusViewModel;
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
                return Enumerable.Repeat(BuiltInCommandTypes.HotkeyCommand, 1);
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

            if (hotKeyCommand.Key == Key.Tab && hotKeyCommand.ModifierKeys == ModifierKeys.None)
            {
                hotKeyCommand.Handled = true;
                if (_groupStatusViewModel.SelectedGroupMate == null || _groupStatusViewModel.GroupMates.IndexOf(_groupStatusViewModel.SelectedGroupMate) == _groupStatusViewModel.GroupMates.Count - 1)
                {
                    _groupStatusViewModel.SelectedGroupMate = _groupStatusViewModel.GroupMates.FirstOrDefault();
                    return;
                }

                var index = _groupStatusViewModel.GroupMates.IndexOf(_groupStatusViewModel.SelectedGroupMate);
                _groupStatusViewModel.SelectedGroupMate = _groupStatusViewModel.GroupMates[index + 1];
                return;
            }

            if (hotKeyCommand.Key == Key.Tab && hotKeyCommand.ModifierKeys == ModifierKeys.Shift)
            {
                hotKeyCommand.Handled = true;
                if (_groupStatusViewModel.SelectedGroupMate == null || _groupStatusViewModel.GroupMates.IndexOf(_groupStatusViewModel.SelectedGroupMate) == 0)
                {
                    _groupStatusViewModel.SelectedGroupMate = _groupStatusViewModel.GroupMates.LastOrDefault();
                    return;
                }

                var index = _groupStatusViewModel.GroupMates.IndexOf(_groupStatusViewModel.SelectedGroupMate);
                _groupStatusViewModel.SelectedGroupMate = _groupStatusViewModel.GroupMates[index - 1];
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

            var groupStatusMessage = message as GroupStatusMessage;
            if (groupStatusMessage != null)
            {
                _groupWidgetControl.UpdateModel(groupStatusMessage);
            }
        }
    }
}
