// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MathExpressionParameterViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the MathExpressionParameterViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel.ActionParameters
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model.ActionParameters;

    /// <summary>
    /// View model for math expression parameter.
    /// </summary>
    public class MathExpressionParameterViewModel : ActionParameterViewModelBase
    {
        private readonly MathExpressionParameter _parameter;
        private readonly ParameterDescription _parameterDescriptor;
        private ActionParameterViewModelBase _firstOperand;
        private ActionParameterViewModelBase _secondOperand;

        /// <summary>
        /// Initializes a new instance of the <see cref="MathExpressionParameterViewModel"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterDescriptor">The parameter descriptor.</param>
        /// <param name="allParameterDescriptions">All parameter descriptions.</param>
        public MathExpressionParameterViewModel([NotNull] MathExpressionParameter parameter, [NotNull] ParameterDescription parameterDescriptor, [NotNull] IEnumerable<ParameterDescription> allParameterDescriptions)
            : base(parameter, parameterDescriptor, allParameterDescriptions)
        {
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(parameterDescriptor, "parameterDescriptor");
            Assert.ArgumentNotNull(allParameterDescriptions, "allParameterDescriptions");

            _parameter = parameter;
            _parameterDescriptor = parameterDescriptor;
            _firstOperand = ActionParametersViewModel.CreateParameterViewModel(parameter.FirstOperand, allParameterDescriptions);
            _secondOperand = ActionParametersViewModel.CreateParameterViewModel(parameter.SecondOperand, allParameterDescriptions);

            _firstOperand.PropertyChanged += HandleOperandDescriptionChange;
            _secondOperand.PropertyChanged += HandleOperandDescriptionChange;

            _firstOperand.ParameterTypeChanged += HandleFirstOperandTypeChange;
            _secondOperand.ParameterTypeChanged += HandleSecondOperandTypeChange;
        }

        /// <summary>
        /// Gets the parameter description.
        /// </summary>
        public override string ParameterDescription
        {
            get
            {
                return string.Format(CultureInfo.CurrentUICulture, "Calculate({0} {1} {2})", FirstOperand.ParameterDescription, Operation.ConvertOperationTypeToSign(), SecondOperand.ParameterDescription);
            }
        }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public MathOperation Operation
        {
            get
            {
                return _parameter.Operation;
            }

            set
            {
                _parameter.Operation = value;
                OnPropertyChanged("Operation");
                OnPropertyChanged("ParameterDescription");
            }
        }

        /// <summary>
        /// Gets or sets the first operand descriptor.
        /// </summary>
        /// <value>
        /// The first operand descriptor.
        /// </value>
        [NotNull]
        public ParameterDescription FirstOperandDescriptor
        {
            get
            {
                return _firstOperand.ParameterDescriptor;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _firstOperand.ParameterDescriptor = value;
                OnPropertyChanged("FirstOperandDescriptor");
            }
        }

        /// <summary>
        /// Gets or sets the second operand descriptor.
        /// </summary>
        /// <value>
        /// The second operand descriptor.
        /// </value>
        [NotNull]
        public ParameterDescription SecondOperandDescriptor
        {
            get
            {
                return _secondOperand.ParameterDescriptor;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _secondOperand.ParameterDescriptor = value;
                OnPropertyChanged("SecondOperandDescriptor");
            }
        }

        /// <summary>
        /// Gets or sets the first operand.
        /// </summary>
        /// <value>
        /// The first operand.
        /// </value>
        [NotNull]
        public ActionParameterViewModelBase FirstOperand
        {
            get
            {
                return _firstOperand;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                if (_firstOperand != null)
                {
                    _firstOperand.PropertyChanged -= HandleOperandDescriptionChange;
                    _firstOperand.ParameterTypeChanged -= HandleFirstOperandTypeChange;
                }

                _firstOperand = value;
                _firstOperand.ParameterTypeChanged += HandleFirstOperandTypeChange;
                _parameter.FirstOperand = value.Parameter;
                _firstOperand.PropertyChanged += HandleOperandDescriptionChange;
                OnPropertyChanged("FirstOperand");
                OnPropertyChanged("ParameterDescription");
            }
        }

        /// <summary>
        /// Gets or sets the second operand.
        /// </summary>
        /// <value>
        /// The second operand.
        /// </value>
        [NotNull]
        public ActionParameterViewModelBase SecondOperand
        {
            get
            {
                return _secondOperand;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                if (_secondOperand != null)
                {
                    _secondOperand.PropertyChanged -= HandleOperandDescriptionChange;
                    _secondOperand.ParameterTypeChanged -= HandleSecondOperandTypeChange;
                }

                _secondOperand = value;
                _parameter.SecondOperand = value.Parameter;
                _secondOperand.PropertyChanged += HandleOperandDescriptionChange;
                _secondOperand.ParameterTypeChanged += HandleSecondOperandTypeChange;
                OnPropertyChanged("SecondOperand");
                OnPropertyChanged("ParameterDescription");
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionParameterViewModelBase Clone()
        {
            return new MathExpressionParameterViewModel(new MathExpressionParameter(), _parameterDescriptor, AllParameterDescriptions)
                       {
                           FirstOperand = FirstOperand.Clone(),
                           SecondOperand = SecondOperand.Clone(),
                           Operation = Operation
                       };
        }

        private void HandleOperandDescriptionChange([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (e.PropertyName == "ParameterDescription")
            {
                OnPropertyChanged("ParameterDescription");
            }
        }

        private void HandleFirstOperandTypeChange([NotNull] ActionParameterViewModelBase parameter, [NotNull] ParameterDescription newparameterdescription)
        {
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(newparameterdescription, "newparameterdescription");

            FirstOperand = ActionParametersViewModel.CreateNewActionParameterFromType(newparameterdescription);
        }

        private void HandleSecondOperandTypeChange([NotNull] ActionParameterViewModelBase parameter, [NotNull] ParameterDescription newparameterdescription)
        {
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(newparameterdescription, "newparameterdescription");

            SecondOperand = ActionParametersViewModel.CreateNewActionParameterFromType(newparameterdescription);
        }
    }
}
