using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Adan.Client.Commands;
using Adan.Client.Common.Commands;
using Adan.Client.Common.ConveyorUnits;
using Adan.Client.Common.Utils;

namespace Adan.Client.ConveyorUnits
{
    using Common.Conveyor;
    using Messages;

    public class ShowMainOutputUnit : ConveyorUnit
    {
        private readonly Regex _regexShowOutputWindow = new Regex(@"^\#wind?o?w?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public ShowMainOutputUnit(MessageConveyor conveyor):base(conveyor)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get 
            {
                return Enumerable.Empty<int>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override IEnumerable<int> HandledCommandTypes
        {
            get 
            {
                return new[] { BuiltInCommandTypes.ShowMainOutputCommand, BuiltInCommandTypes.TextCommand };
            }
        }

        public override void HandleCommand(Command command, bool isImport = false)
        {
            var showOutputCommand = command as ShowMainOutputCommand;
            if (showOutputCommand != null)
            {
                PushMessageToConveyor(new ShowOutputWindowMessage(showOutputCommand.OutputWindowName));
                command.Handled = true;
                return;
            }

            var textCommand = command as TextCommand;
            if (textCommand != null)
            {
                Match m = _regexShowOutputWindow.Match(textCommand.CommandText);
                if (m.Success)
                {
                    if (m.Groups[1].Length > 0)
                    {
                        PushMessageToConveyor(
                            new ShowOutputWindowMessage(CommandLineParser.GetArgs(m.Groups[1].ToString())[0]));
                    }
                    else
                    {
                        PushMessageToConveyor(new ShowOutputWindowMessage(string.Empty));
                    }

                    command.Handled = true;
                }
            }
        }
    }
}