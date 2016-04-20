namespace Adan.Client.Plugins.OutputWindow.Model.Actions
{
    using System;
    using System.Xml.Serialization;

    using Common.Messages;
    using Common.Model;
    using Common.Themes;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;
    using System.Text;

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
        /// 
        /// </summary>
        public override bool IsGlobal
        {
            get
            {
                return false;
            }
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

            //TODO: Enable OutputEntoreMessageKeepingColors only for triggers?
            //CurrentMessage work only from triggers
            var contextMessage = context.CurrentMessage as TextMessage;
            if (OutputEntireMessageKeepingColors && contextMessage != null)
            {
                model.PushMessageToConveyor(new OutputToAdditionalWindowMessage(contextMessage) { SkipTriggers = true, SkipSubstitution = true, SkipHighlight = true });
            }
            else
            {
                string str = PostProcessString(TextToOutput + GetParametersString(model, context), model, context);
                if (OutputEntireMessageKeepingColors)
                {
                    model.PushMessageToConveyor(new OutputToAdditionalWindowMessage(str) { SkipTriggers = true, SkipSubstitution = true, SkipHighlight = true });
                }
                else
                {
                    model.PushMessageToConveyor(new OutputToAdditionalWindowMessage(str, TextColor, BackgroundColor) { SkipTriggers = true, SkipSubstitution = true, SkipHighlight = true });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="rootModel"></param>
        /// <returns></returns>
        [NotNull]
        protected override string ReplaceVariables([NotNull] string input, [NotNull] RootModel rootModel)
        {
            StringBuilder sb = new StringBuilder();

            int lastPos = 0;
            int i = 0;
            while (i < input.Length)
            {
                if (input[i] == '$')
                {
                    if (i - lastPos > 0)
                        sb.Append(input, lastPos, i - lastPos);

                    i++;
                    int startPos = i;

                    while (i < input.Length && char.IsLetterOrDigit(input[i]))
                        i++;

                    sb.Append(rootModel.GetVariableValue(input.Substring(startPos, i - startPos)));
                    lastPos = i;
                }

                i++;
            }

            if (lastPos < input.Length)
                sb.Append(input, lastPos, input.Length - lastPos);

            return sb.ToString();
        }

        protected override string ReplaceParameters(string input, ActionExecutionContext context)
        {
            StringBuilder sb = new StringBuilder();
            
            int lastPos = 0;
            int i = 0;
            while (i < input.Length - 1)
            {
                if (input[i] == '%')
                {
                    if (i < input.Length - 2 && input[i + 1] == '%' && char.IsDigit(input[i + 2]))
                    {
                        if (i - lastPos > 0)
                            sb.Append(input, lastPos, i - lastPos);

                        sb.Append(GetParameter((int)Char.GetNumericValue(input[i + 2]), context));

                        lastPos = i + 3;
                        i += 2;
                    }
                    else if (char.IsDigit(input[i + 1]))
                    {
                        if (i - lastPos > 0)
                            sb.Append(input, lastPos, i - lastPos);

                        sb.Append(GetParameter((int)Char.GetNumericValue(input[i + 1]), context));
                        lastPos = i + 2;
                        i++;
                    }
                }

                i++;
            }

            if (lastPos < input.Length)
                sb.Append(input, lastPos, input.Length - lastPos);

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#output {").Append(TextToOutput).Append("}");

            return sb.ToString();
        }
    }
}
