// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputToMainWindowActionViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the OutputToMainWindowActionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel.Actions
{
    using System.Collections.Generic;

    using Common.Plugins;
    using Common.Themes;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model.Actions;

    /// <summary>
    /// View model for "output text to main window" action.
    /// </summary>
    public class OutputToMainWindowActionViewModel : ActionWithParametersViewModelBase
    {
        private readonly OutputToMainWindowAction _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputToMainWindowActionViewModel"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        /// <param name="allDescriptions">All descriptions.</param>
        public OutputToMainWindowActionViewModel([NotNull] OutputToMainWindowAction action, [NotNull] ActionDescription actionDescriptor, [NotNull] IEnumerable<ParameterDescription> parameterDescriptions, [NotNull] IEnumerable<ActionDescription> allDescriptions)
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
        /// Gets the action description.
        /// </summary>
        public override string ActionDescription
        {
            get
            {
                return "#OutputToWindow " + TextToOutput + ParametersModel.ActionParametersDescription;
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionViewModelBase Clone()
        {
            var outputToMainWindowAction = new OutputToMainWindowAction();
            return new OutputToMainWindowActionViewModel(outputToMainWindowAction, ActionDescriptor, ParametersModel.ParameterDescriptions, AllActionDescriptions)
            {
                BackgroundColor = BackgroundColor,
                TextColor = TextColor,
                TextToOutput = TextToOutput,
                ParametersModel = ParametersModel.Clone(outputToMainWindowAction.Parameters)
            };
        }
    }
}
