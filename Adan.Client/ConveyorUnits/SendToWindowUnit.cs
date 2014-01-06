using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Adan.Client.Commands;
using Adan.Client.Common.Commands;
using Adan.Client.Common.ConveyorUnits;
using Adan.Client.Common.Model;
using Adan.Client.Common.Utils;

namespace Adan.Client.ConveyorUnits
{
    /// <summary>
    /// 
    /// </summary>
    public class SendToWindowUnit : ConveyorUnit
    {
        private MainWindow _mainWindow;

        private Regex _regexSendToWindow = new Regex(@"^\#send\s+(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex _regexSendToAllWindow = new Regex(@"^\#sendal?l?\s+(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainWindow"></param>
        public SendToWindowUnit(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
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
                return new int[] { BuiltInCommandTypes.SendToWindow, BuiltInCommandTypes.TextCommand };
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
            var showOutputCommand = command as SendToWindowCommand;
            if (showOutputCommand != null)
            {
                if (showOutputCommand.ToAll)
                    _mainWindow.SendToAllWindows(showOutputCommand.Command);
                else
                    _mainWindow.SendToWindow(showOutputCommand.OutputWindowName, showOutputCommand.Command);

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
                    if(args.Length > 1)
                    {
                        _mainWindow.SendToWindow(args[1], new TextCommand(args[0]));
                    }

                    command.Handled = true;
                    return;
                }

                m = _regexSendToAllWindow.Match(textCommand.CommandText);
                if (m.Success)
                {
                    _mainWindow.SendToAllWindows(new TextCommand(m.Groups[1].ToString()));

                    command.Handled = true;
                    return;
                }

                return;
            }
        }
    }
}
