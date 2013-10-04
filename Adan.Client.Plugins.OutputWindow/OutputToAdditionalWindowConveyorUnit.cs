// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputToAdditionalWindowConveyorUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the OutputToAdditionalWindowConveyorUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.OutputWindow
{
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
        /// <param name="messageConveyor">The message conveyor.</param>
        /// <param name="window">The window.</param>
        public OutputToAdditionalWindowConveyorUnit([NotNull] MessageConveyor messageConveyor, [NotNull] AdditionalOutputWindow window)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
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
        /// Handles the command.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        public override void HandleCommand(Common.Commands.Command command)
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
                    match.Groups[1].ToString(),
                    (TextColor)new TextColorToBrushConverter().ConvertBack(ThemeManager.Instance.ActiveTheme.DefaultTextColor, typeof(TextColor), new object(), CultureInfo.InvariantCulture),
                    (TextColor)new TextColorToBrushConverter().ConvertBack(ThemeManager.Instance.ActiveTheme.DefaultBackGroundColor, typeof(TextColor), new object(), CultureInfo.InvariantCulture)) { SkipTriggers = true });

                return;
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        public override void HandleMessage(Message message)
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
