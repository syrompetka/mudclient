// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputToAdditionalWindowConveyorUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the OutputToAdditionalWindowConveyorUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.OutputWindow.Models.ConveyorUnits
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Adan.Client.Common.Commands;
    using Adan.Client.Common.Themes;
    using Adan.Client.Common.Utils;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Adan.Client.Plugins.OutputWindow.Messages;
    using Adan.Client.Common.Model;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementaion to handle <see cref="OutputToAdditionalWindowMessage"/>.
    /// </summary>
    public class OutputToAdditionalWindowConveyorUnit : ConveyorUnit
    {
        private readonly Regex _regexOutput = new Regex(@"#outp?u?t?\s+\{?(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private AdditionalOutputWindowManager _manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputToAdditionalWindowConveyorUnit"/> class.
        /// </summary>
        public OutputToAdditionalWindowConveyorUnit(AdditionalOutputWindowManager manager)
            : base()
        {
            _manager = manager;
        }

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return new[] { BuiltInMessageTypes.TextMessage };
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

            string commandText = textCommand.CommandText.Trim();

            Match match = _regexOutput.Match(commandText);
            if (match.Success)
            {
                textCommand.Handled = true;

                if (!match.Groups[1].Success)
                {
                    return;
                }

                string str = match.Groups[1].Value;

                rootModel.PushMessageToConveyor(new OutputToAdditionalWindowMessage(str[str.Length - 1] == '}' ? str.Substring(0, str.Length - 1) : str));

                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="rootModel"></param>
        public override void HandleMessage(Message message, RootModel rootModel)
        {
            var outputMessage = message as OutputToAdditionalWindowMessage;
            if (outputMessage != null)
            {
                outputMessage.Handled = true;

                _manager.AddText(rootModel, outputMessage);
            }
        }
    }
}
