using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Adan.Client.Common.Commands;
using Adan.Client.Common.Conveyor;
using Adan.Client.Common.ConveyorUnits;
using Adan.Client.Common.Model;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.ConveyorUnits
{
    /// <summary>
    /// Unit for replace variables to value
    /// </summary>
    public class VariableReplaceUnit : ConveyorUnit
    {
        private Regex VariableRegex = new Regex(@"\$([\w\d]+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private RootModel _rooteModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSeparatorUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        /// <param name="rootModel">Root Model</param>
        public VariableReplaceUnit([NotNull] MessageConveyor messageConveyor, [NotNull] RootModel rootModel)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
            Assert.ArgumentNotNull(rootModel, "rootModel");

            _rooteModel = rootModel;
        }

        #region Overrides of ConveyorUnit

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
        /// Handles the command.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        public override void HandleCommand(Command command)
        {
            Assert.ArgumentNotNull(command, "command");

            var textCommand = command as TextCommand;
            if (textCommand == null)
            {
                return;
            }

            bool ret;

            do
            {
                ret = false;
                textCommand.CommandText = VariableRegex.Replace(textCommand.CommandText,
                    m =>
                    {
                        ret = true;
                        return _rooteModel.GetVariableValue(m.Groups[1].Value);
                    });
            } while (ret);
        }

        #endregion
    }
}
