using System;
using System.Collections.Generic;
using System.Linq;
using Adan.Client.Commands;
using Adan.Client.Common.Commands;
using Adan.Client.Common.ConveyorUnits;
using Adan.Client.Common.Model;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.ConveyorUnits
{
    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementation to toggle full screen mode of the main window.
    /// </summary>
    public sealed class ToggleFullScreenModeUnit : ConveyorUnit
    {
        private readonly MainWindow _mainWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleFullScreenModeUnit"/> class.
        /// </summary>
        /// <param name="MainWindowEx">The main window.</param>
        public ToggleFullScreenModeUnit([NotNull] MainWindow MainWindowEx)
        {
            Assert.ArgumentNotNull(MainWindowEx, "MainWindowEx");
            _mainWindow = MainWindowEx;
        }

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get { return Enumerable.Empty<int>(); }
        }

        /// <summary>
        /// Gets a set of command types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledCommandTypes
        {
            get { return new[] { BuiltInCommandTypes.ToggleFullScreenMode, BuiltInCommandTypes.TextCommand }; }
        }

        /// <summary>
        /// Handles the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="rootModel">The root model.</param>
        /// <param name="isImport">if set to <c>true</c> [is import].</param>
        public override void HandleCommand(Command command, RootModel rootModel, bool isImport = false)
        {
            var toggleCommand = command as ToggleFullScreenModeCommand;
            if (toggleCommand != null)
            {
                _mainWindow.ToggleFullScreenMode();
                command.Handled = true;
                return;
            }

            var textCommand = command as TextCommand;
            if (textCommand != null)
            {
                if(string.Equals(textCommand.CommandText.Trim(), "#togglefullscreen", StringComparison.OrdinalIgnoreCase))
                {
                    _mainWindow.ToggleFullScreenMode();
                    command.Handled = true;
                }
            }

        }
    }
}
