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
    public class SelectedMonsterParameterViewModel : ActionParameterViewModelBase
    {
        private readonly SelectedMonsterParameter _selectedMonsterParameter;
        private bool _isNumberDisabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedMonsterParameterViewModel"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterDescriptor">The parameter descriptor.</param>
        /// <param name="allParameterDescriptions">All parameter descriptions.</param>
        public SelectedMonsterParameterViewModel([NotNull] SelectedMonsterParameter parameter, [NotNull] ParameterDescription parameterDescriptor, [NotNull] IEnumerable<ParameterDescription> allParameterDescriptions)
            : base(parameter, parameterDescriptor, allParameterDescriptions)
        {
            _selectedMonsterParameter = parameter;
            _isNumberDisabled = _selectedMonsterParameter.MonsterNumber == 0;
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(parameterDescriptor, "parameterDescriptor");
            Assert.ArgumentNotNull(allParameterDescriptions, "allParameterDescriptions");
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

        public int MonsterNumber
        {
            get { return _selectedMonsterParameter.MonsterNumber; }
            set
            {
                if (_selectedMonsterParameter.MonsterNumber != value)
                {
                    if (value < 1 && !_isNumberDisabled)
                    {
                        return;
                    }

                    _selectedMonsterParameter.MonsterNumber = value;
                    OnPropertyChanged("MonsterNumber");
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
                        _selectedMonsterParameter.MonsterNumber = 0;
                    }
                    else
                    {
                        _selectedMonsterParameter.MonsterNumber = 1;
                    }

                    _isNumberDisabled = value;
                    OnPropertyChanged("MonsterNumber");
                    OnPropertyChanged("IsNumberDisabled");
                    OnPropertyChanged("ParameterDescription");
                }
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        [NotNull]
        public override ActionParameterViewModelBase Clone()
        {
            return new SelectedMonsterParameterViewModel(new SelectedMonsterParameter {MonsterNumber = MonsterNumber}, ParameterDescriptor, AllParameterDescriptions) ;
        }
    }
}
