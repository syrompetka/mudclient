namespace Adan.Client.Plugins.GroupWidget.ViewModel
{
    using System.Collections.Generic;
    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Model.ActionParameters;

    /// <summary>
    /// A view model for <see cref="SelectedGroupMateParameter"/>.
    /// </summary>
    public class SelectedGroupMateParameterViewModel : ActionParameterViewModelBase
    {
        private readonly SelectedGroupMateParameter _selectedGroupMateParameter;
        private bool _isNumberDisabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedGroupMateParameterViewModel"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterDescriptor">The parameter descriptor.</param>
        /// <param name="allParameterDescriptions">All parameter descriptions.</param>
        public SelectedGroupMateParameterViewModel([NotNull] SelectedGroupMateParameter parameter, [NotNull] ParameterDescription parameterDescriptor, [NotNull] IEnumerable<ParameterDescription> allParameterDescriptions)
            : base(parameter, parameterDescriptor, allParameterDescriptions)
        {
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(parameterDescriptor, "parameterDescriptor");
            Assert.ArgumentNotNull(allParameterDescriptions, "allParameterDescriptions");
            _selectedGroupMateParameter = parameter;

            _isNumberDisabled = _selectedGroupMateParameter.GroupMateNumber == 0;
        }

        public int GroupMateNumber
        {
            get { return _selectedGroupMateParameter.GroupMateNumber; }
            set
            {
                if (_selectedGroupMateParameter.GroupMateNumber != value)
                {
                    if (value < 1 && !_isNumberDisabled)
                    {
                        return;
                    }

                    _selectedGroupMateParameter.GroupMateNumber = value;
                    OnPropertyChanged("GroupMateNumber");
                    OnPropertyChanged("ParameterDescription");
                }
            }
        }

        public bool IsNumberDisabled
        {
            get { return _isNumberDisabled; }
            set
            {
                if (_isNumberDisabled != value)
                {
                    if (value)
                    {
                        _selectedGroupMateParameter.GroupMateNumber = 0;
                    }
                    else
                    {
                        _selectedGroupMateParameter.GroupMateNumber = 1;
                    }

                    _isNumberDisabled = value;
                    OnPropertyChanged("GroupMateNumber");
                    OnPropertyChanged("IsNumberDisabled");
                    OnPropertyChanged("ParameterDescription");
                }
            }
        }

        /// <summary>
        /// Gets the parameter description.
        /// </summary>
        public override string ParameterDescription
        {
            get
            {
                return Parameter.GetParameterValue();
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionParameterViewModelBase Clone()
        {
            return new SelectedGroupMateParameterViewModel(new SelectedGroupMateParameter(), ParameterDescriptor, AllParameterDescriptions);
        }
    }
}
