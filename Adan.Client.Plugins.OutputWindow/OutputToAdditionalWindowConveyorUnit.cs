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
    using System.Linq;

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
                return Enumerable.Empty<int>();
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
