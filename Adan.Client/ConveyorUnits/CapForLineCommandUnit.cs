using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adan.Client.Common.Commands;
using Adan.Client.Common.Conveyor;
using Adan.Client.Common.ConveyorUnits;
using Adan.Client.Common.Model;
using Adan.Client.Common.Settings;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.ConveyorUnits
{
    /// <summary>
    /// A <see cref="CapForLineCommandUnit"/> implementation that will stop command line.
    /// </summary>
    public class CapForLineCommandUnit : ConveyorUnit
    {
        /// <summary>
        /// 
        /// </summary>
        public CapForLineCommandUnit()
            : base()
        {
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
                return new[] { BuiltInCommandTypes.TextCommand };
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

            var textCommand = command as TextCommand;
            if (textCommand == null)
            {
                return;
            }

            if (textCommand.CommandText.StartsWith(SettingsHolder.Instance.Settings.CommandChar.ToString()))
                command.Handled = true;
        }
    }
}