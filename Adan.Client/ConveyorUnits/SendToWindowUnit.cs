namespace Adan.Client.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Commands;
    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Model;
    using Common.Utils;
    using Model.Actions;

    public class SendToWindowUnit : ConveyorUnit
    {
        private readonly Regex _regexSendToWindow = new Regex(@"^\#send\s+(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexSendToAllWindow = new Regex(@"^\#sendal?l?\s+(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public SendToWindowUnit(MessageConveyor conveyor) : base(conveyor)
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
                return new[] { BuiltInCommandTypes.SendToWindow, BuiltInCommandTypes.TextCommand };
            }
        }

        public override void HandleCommand(Command command, bool isImport = false)
        {
            var showOutputCommand = command as SendToWindowCommand;
            if (showOutputCommand != null)
            {
                if (showOutputCommand.ToAll)
                {
                    Conveyor.RootModel.SendToAllWindows(showOutputCommand.ActionsToExecute, showOutputCommand.ActionExecutionContext);
                }
                else
                {
                    Conveyor.RootModel.SendToWindow(showOutputCommand.OutputWindowName, showOutputCommand.ActionsToExecute, showOutputCommand.ActionExecutionContext); 
                    
                }

                command.Handled = true;
                return;
            }

            var textCommand = command as TextCommand;
            if (textCommand != null)
            {
                Match m = _regexSendToWindow.Match(textCommand.CommandText);
                if (m.Success)
                {
                    var args = CommandLineParser.GetArgs(m.Groups[1].ToString());
                    if (args.Length > 1)
                    {
                        Conveyor.RootModel.SendToWindow(args[1], Enumerable.Repeat(new SendTextAction { CommandText = args[0] }, 1), ActionExecutionContext.Empty);
                    }

                    command.Handled = true;
                    return;
                }

                m = _regexSendToAllWindow.Match(textCommand.CommandText);
                if (m.Success)
                {
                    Conveyor.RootModel.SendToAllWindows(Enumerable.Repeat(new SendTextAction { CommandText = m.Groups[1].ToString() }, 1), ActionExecutionContext.Empty);

                    command.Handled = true;
                    return;
                }
            }
        }
    }
}
