namespace Adan.Client.Plugins.GroupWidget.Model.ParameterDescriptions

{
    using System.Collections.Generic;

    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;
    using ActionParameters;

    /// <summary>
    /// A description of "selected group mate" parameter.
    /// </summary>
    public class SelectedGroupMateParameterDescription : ParameterDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedGroupMateParameterDescription"/> class.
        /// </summary>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        public SelectedGroupMateParameterDescription([NotNull] IEnumerable<ParameterDescription> parameterDescriptions)
            : base("Group mate", parameterDescriptions)
        {
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");
        }

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <returns>Created parameter.</returns>
        public override ActionParameterBase CreateParameter()
        {
            return new SelectedGroupMateParameter();
        }

        /// <summary>
        /// Creates the parameter view model.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// Created parameter view model.
        /// </returns>
        public override ActionParameterViewModelBase CreateParameterViewModel(ActionParameterBase parameter)
        {
            Assert.ArgumentNotNull(parameter, "parameter");

            var selectedGroupMateParameter = parameter as SelectedGroupMateParameter;
            if (selectedGroupMateParameter != null)
            {
                return new SelectedGroupMateParameterViewModel(selectedGroupMateParameter, this, ParameterDescriptions);
            }

            return null;
        }
    }
}
