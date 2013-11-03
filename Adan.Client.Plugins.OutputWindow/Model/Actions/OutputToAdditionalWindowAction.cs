// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputToAdditionalWindowAction.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the OutputToAdditionalWindowAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.OutputWindow.Model.Actions
{
    using System;
    using System.Xml.Serialization;

    using Common.Messages;
    using Common.Model;
    using Common.Themes;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Adan.Client.Plugins.OutputWindow.Messages;

    /// <summary>
    /// Action that outputs some text to additional window.
    /// </summary>
    [Serializable]
    public class OutputToAdditionalWindowAction : ActionWithParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputToAdditionalWindowAction"/> class.
        /// </summary>
        public OutputToAdditionalWindowAction()
        {
            TextToOutput = string.Empty;
            TextColor = TextColor.None;
            BackgroundColor = TextColor.None;
        }

        /// <summary>
        /// Gets or sets the text to ouput.
        /// </summary>
        /// <value>
        /// The text to ouput.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string TextToOutput
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>
        /// The color of the text.
        /// </value>
        [XmlAttribute]
        public TextColor TextColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        [XmlAttribute]
        public TextColor BackgroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to output entire message keeping colors.
        /// </summary>
        /// <value>
        /// <c>true</c> if to output entire message keeping colors; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool OutputEntireMessageKeepingColors
        {
            get;
            set;
        }

        /// <summary>
        /// Executes this action.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public override void Execute(RootModel model, ActionExecutionContext context)
        {
            Assert.ArgumentNotNull(model, "model");
            Assert.ArgumentNotNull(context, "context");
            if (OutputEntireMessageKeepingColors)
            {
                var textMessage = context.CurrentMessage as TextMessage;
                if (textMessage != null)
                {
                    model.PushMessageToConveyor(new OutputToAdditionalWindowMessage(textMessage) { SkipTriggers = true });
                }
            }
            else
            {
                model.PushMessageToConveyor(new OutputToAdditionalWindowMessage(PostProcessString(TextToOutput + GetParametersString(model, context), model, context), TextColor, BackgroundColor) { SkipTriggers = true });
            }
        }
    }
}
