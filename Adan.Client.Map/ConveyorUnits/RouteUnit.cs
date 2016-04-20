namespace Adan.Client.Map.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Linq;
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

        private readonly char[] _splitChars = { ' ' };

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteUnit"/> class.
        /// </summary>
        public RouteUnit([NotNull] RouteManager routeManager, MessageConveyor conveyor)
            : base(conveyor)
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
        
        public override void HandleCommand(Command command, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");

            var textCommand = command as TextCommand;
            if (textCommand == null)
            {
                return;
            }
            
            var splittedCommands = textCommand.CommandText.Split(_splitChars, System.StringSplitOptions.RemoveEmptyEntries);

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
            }
        }
    }
}
