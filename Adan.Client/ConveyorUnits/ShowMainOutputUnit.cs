using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Adan.Client.Common.Commands;
using Adan.Client.Common.ConveyorUnits;
using Adan.Client.Common.Model;

namespace Adan.Client.ConveyorUnits
{
    /// <summary>
    /// 
    /// </summary>
    public class ShowMainOutputUnit : ConveyorUnit
    {
        private static MainWindow _mainWindow;

        private Regex _regexShowOutputWindow = new Regex(@"^\#window\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainWindow"></param>
        public ShowMainOutputUnit(MainWindow mainWindow)
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
                return new int[] { BuiltInCommandTypes.ShowMainOutputCommand, BuiltInCommandTypes.TextCommand };
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
            var showOutputCommand = command as ShowMainOutputCommand;
            if (showOutputCommand != null)
            {
                Application.Current.Dispatcher.BeginInvoke((Action<string>)_mainWindow.ShowOutputWindow, ((ShowMainOutputCommand)command).OutputWindowName);
                command.Handled = true;
                return;
            }

            var textCommand = command as TextCommand;
            if (textCommand != null)
            {
                Match m = _regexShowOutputWindow.Match(textCommand.CommandText);
                if (m.Success)
                {
                    Application.Current.Dispatcher.BeginInvoke((Action<string>)_mainWindow.ShowOutputWindow, m.Groups[1].ToString());
                    command.Handled = true;
                }

                return;
            }
        }
    }
}