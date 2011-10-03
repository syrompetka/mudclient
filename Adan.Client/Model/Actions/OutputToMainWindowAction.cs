// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputToMainWindowAction.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the OutputToMainWindowAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.Actions
{
    using System;
    using System.Xml.Serialization;

    using Common.Messages;
    using Common.Model;
    using Common.Themes;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Action that outputs some text to main window.
    /// </summary>
    [Serializable]
    public class OutputToMainWindowAction : ActionWithParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputToMainWindowAction"/> class.
        /// </summary>
        public OutputToMainWindowAction()
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

        #region Overrides of ActionBase

        /// <summary>
        /// Executes this action.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public override void Execute(RootModel model, ActionExecutionContext context)
        {
            Assert.ArgumentNotNull(model, "model");
            Assert.ArgumentNotNull(context, "context");

            model.PushMessageToConveyor(new OutputToMainWindowMessage(PostProcessString(TextToOutput + GetParametersString(model, context), model, context), TextColor, BackgroundColor) { SkipTriggers = true });
        }

        #endregion
    }
}
