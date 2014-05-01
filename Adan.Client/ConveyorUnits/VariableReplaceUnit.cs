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
        private Regex VariableRegex = new Regex(@"\$(\w+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// 
        /// </summary>
        public VariableReplaceUnit()
            : base()
        {
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

            bool ret;

            do
            {
                ret = false;
                textCommand.CommandText = VariableRegex.Replace(textCommand.CommandText,
                    m =>
                    {
                        ret = true;
                        return rootModel.GetVariableValue(m.Groups[1].Value);
                    });
            } while (ret);
        }

        #endregion
    }
}
