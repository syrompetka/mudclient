// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputToAdditionalWindowActionViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the OutputToAdditionalWindowActionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.OutputWindow
{
    using System.Collections.Generic;

    using Common.Plugins;
    using Common.Themes;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Adan.Client.Plugins.OutputWindow.Model.Actions;

    /// <summary>
    /// View model for output to additional window action.
    /// </summary>
    public class OutputToAdditionalWindowActionViewModel : ActionWithParametersViewModelBase
    {
        private readonly OutputToAdditionalWindowAction _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputToAdditionalWindowActionViewModel"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        /// <param name="allDescriptions">All descriptions.</param>
        public OutputToAdditionalWindowActionViewModel([NotNull] OutputToAdditionalWindowAction action, [NotNull] ActionDescription actionDescriptor, [NotNull] IEnumerable<ParameterDescription> parameterDescriptions, [NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base(action, actionDescriptor, parameterDescriptions, allDescriptions)
        {
            Assert.ArgumentNotNull(action, "action");
            Assert.ArgumentNotNull(actionDescriptor, "actionDescriptor");
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");

            _action = action;
        }

        /// <summary>
        /// Gets or sets the text to output.
        /// </summary>
        /// <value>
        /// The text to output.
        /// </value>
        [NotNull]
        public string TextToOutput
        {
            get
            {
                return _action.TextToOutput;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _action.TextToOutput = value;
                OnPropertyChanged("TextToOutput");
                OnPropertyChanged("ActionDescription");
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>
        /// The color of the text.
        /// </value>
        public TextColor TextColor
        {
            get
            {
                return _action.TextColor;
            }

            set
            {
                _action.TextColor = value;
                OnPropertyChanged("TextColor");
            }
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        public TextColor BackgroundColor
        {
            get
            {
                return _action.BackgroundColor;
            }

            set
            {
                _action.BackgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to output entire message keeping colors.
        /// </summary>
        /// <value>
        /// <c>true</c> if to output entire message keeping colors; otherwise, <c>false</c>.
        /// </value>
        public bool OutputEntireMessageKeepingColors
        {
            get
            {
                return _action.OutputEntireMessageKeepingColors;
            }

            set
            {
                _action.OutputEntireMessageKeepingColors = value;
                OnPropertyChanged("OutputEntireMessageKeepingColors");
                OnPropertyChanged("ActionDescription");
            }
        }

        /// <summary>
        /// Gets the action description.
        /// </summary>
        public override string ActionDescription
        {
            get
            {
                if (OutputEntireMessageKeepingColors)
                {
                    return "#output";
                }

                return "#output " + TextToOutput + ParametersModel.ActionParametersDescription;
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionViewModelBase Clone()
        {
            var outputToAdditionalWindowAction = new OutputToAdditionalWindowAction();
            return new OutputToAdditionalWindowActionViewModel(outputToAdditionalWindowAction, ActionDescriptor, ParametersModel.ParameterDescriptions, AllActionDescriptions)
                       {
                           BackgroundColor = BackgroundColor,
                           TextColor = TextColor,
                           TextToOutput = TextToOutput,
                           OutputEntireMessageKeepingColors = OutputEntireMessageKeepingColors,
                           ParametersModel = ParametersModel.Clone(outputToAdditionalWindowAction.Parameters)
                       };
        }
    }
}
