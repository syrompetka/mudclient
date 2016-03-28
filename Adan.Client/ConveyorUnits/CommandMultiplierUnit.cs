namespace Adan.Client.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Common.Model;
    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementation that "multiplies" commands string with "#number"
    /// </summary>
    public class CommandMultiplierUnit : ConveyorUnit
    {
        private readonly Regex _regex = new Regex(@"^\#([0-9]+)\s*(.*)", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMultiplierUnit"/> class.
        /// </summary>
        public CommandMultiplierUnit(MessageConveyor conveyor)
            : base(conveyor)
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
        
        public override void HandleCommand(Command command, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");

            var textCommand = command as TextCommand;
            if (textCommand == null)
            {
                return;
            }

            var match = _regex.Match(textCommand.CommandText.Trim());
            if(match.Success)
            {
                command.Handled = true;
                int count;
                string str;
                if (match.Groups[2].Value[0] == '{' && match.Groups[2].Value[match.Groups[2].Length - 1] == '}')
                    str = match.Groups[2].Value.Substring(1, match.Groups[2].Value.Length - 2);
                else
                    str = match.Groups[2].Value;

                if (int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        PushCommandToConveyor(new TextCommand(str));
                    }
                }
            }
        }
    }
}