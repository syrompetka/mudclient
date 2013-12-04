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
        private readonly Regex _regexOutput = new Regex(@"#outp?u?t?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly AdditionalOutputWindow _window;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputToAdditionalWindowConveyorUnit"/> class.
        /// </summary>
        /// <param name="window">The window.</param>
        public OutputToAdditionalWindowConveyorUnit([NotNull] AdditionalOutputWindow window)
            : base()
        {
            Assert.ArgumentNotNull(window, "window");

            _window = window;
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

                _window.AddMessage(new OutputToAdditionalWindowMessage(
                    match.Groups[1].ToString().Replace("{", String.Empty).Replace("}", String.Empty),
                    (TextColor)new TextColorToBrushConverter().ConvertBack(ThemeManager.Instance.ActiveTheme.DefaultTextColor, typeof(TextColor), new object(), CultureInfo.InvariantCulture),
                    (TextColor)new TextColorToBrushConverter().ConvertBack(ThemeManager.Instance.ActiveTheme.DefaultBackGroundColor, typeof(TextColor), new object(), CultureInfo.InvariantCulture)) { SkipTriggers = true });

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
            Assert.ArgumentNotNull(message, "message");

            var outputToAdditionalWindowMessage = message as OutputToAdditionalWindowMessage;
            if (outputToAdditionalWindowMessage != null)
            {
                _window.AddMessage(outputToAdditionalWindowMessage);
                outputToAdditionalWindowMessage.Handled = true;
            }
        }
    }
}
