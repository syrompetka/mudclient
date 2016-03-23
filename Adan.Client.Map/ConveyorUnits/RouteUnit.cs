// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the RouteUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Adan.Client.Common.Model;
    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Properties;

    /// <summary>
    /// <see cref="ConveyorUnit"/> implementation to handle route command.
    /// </summary>
    public class RouteUnit : ConveyorUnit
    {
        private readonly RouteManager _routeManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteUnit"/> class.
        /// </summary>
        /// <param name="routeManager">The route manager.</param>
        public RouteUnit([NotNull] RouteManager routeManager)
            : base()
        {
            Assert.ArgumentNotNull(routeManager, "routeManager");

            _routeManager = routeManager;
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
        public override void HandleCommand(Command command, RootModel rootModel, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");

            var textCommand = command as TextCommand;
            if (textCommand == null)
            {
                return;
            }

            var commandText = Regex.Replace(textCommand.CommandText.Trim(), @"\s+", " ");
            var splittedCommands = commandText.Split(' ');

            if (splittedCommands.Length < 2)
            {
                return;
            }

            if (string.Equals(splittedCommands[0], Resources.RouteCommandGoto))
            {
                _routeManager.NavigateToRoom(string.Join(" ", splittedCommands.Skip(1)));
                command.Handled = true;
                return;
            }

            if (!string.Equals(splittedCommands[0], Resources.RouteCommandPrefix))
            {
                return;
            }

            if (string.Equals(splittedCommands[1], Resources.RouteCommandHelp))
            {
                _routeManager.PrintHelp();
                command.Handled = true;
                return;
            }

            if (string.Equals(splittedCommands[1], Resources.RouteCommandStartRecording) && _routeManager.CanCreateNewRoute)
            {
                if (splittedCommands.Length > 2)
                {
                    _routeManager.StartNewRouteRecording(string.Join(" ", splittedCommands.Skip(2)));
                }
                else
                {
                    _routeManager.StartNewRouteRecording();
                }

                command.Handled = true;
                return;
            }

            if (string.Equals(splittedCommands[1], Resources.RouteCommandStopRecording) && _routeManager.CanStopCurrentRouteRecording)
            {
                if (splittedCommands.Length > 2)
                {
                    _routeManager.StopRouteRecording(string.Join(" ", splittedCommands.Skip(2)));
                }
                else
                {
                    _routeManager.StopRouteRecording();
                }

                command.Handled = true;
                return;
            }

            if (string.Equals(splittedCommands[1], Resources.RouteCommandCancelRecording) && _routeManager.CanCancelCurrentRouteRecording)
            {
                _routeManager.CancelRouteRecording();

                command.Handled = true;
                return;
            }

            if (string.Equals(splittedCommands[1], Resources.RouteCommandGoto) && _routeManager.CanStartRoute)
            {
                if (splittedCommands.Length > 2)
                {
                    _routeManager.GotoDestination(string.Join(" ", splittedCommands.Skip(2)));
                }
                else
                {
                    _routeManager.GotoDestination();
                }

                command.Handled = true;
                return;
            }

            if (string.Equals(splittedCommands[1], Resources.RouteCommandStop) && _routeManager.CanStopCurrentRoute)
            {
                _routeManager.StopRoutingToDestination();

                command.Handled = true;
                return;
            }
        }
    }
}
