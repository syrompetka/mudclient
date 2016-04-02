using System.Collections.Generic;
using Adan.Client.Common.ViewModel;
using CSLib.Net.Annotations;
using Adan.Client.Common.Plugins;
using CSLib.Net.Diagnostics;
using Adan.Client.Model.Actions;

namespace Adan.Client.ViewModel.Actions
{
    /// <summary>
    /// 
    /// </summary>
    public class ShowOutputWindowActionViewModel : ActionViewModelBase
    {
        private readonly ShowOutputWindowAction _action;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="actionDescriptor"></param>
        /// <param name="allDescriptions"></param>
        public ShowOutputWindowActionViewModel([NotNull] ShowOutputWindowAction action, [NotNull] ActionDescription actionDescriptor, [NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base(action, actionDescriptor, allDescriptions)
        {
            Assert.ArgumentNotNull(action, "action");
            Assert.ArgumentNotNull(actionDescriptor, "actionDescriptor");
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");

            _action = action;
        }

        /// <summary>
        /// Gets the action description.
        /// </summary>
        public override string ActionDescription
        {
            get
            {
                return string.Format("#window {0}", _action.OutputWindowName);
            }
        }

        /// <summary>
        /// Get or set the name of window to switch to.
        /// </summary>
        [NotNull]
        public string OutputWindowName
        {
            get
            {
                return _action.OutputWindowName;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _action.OutputWindowName = value;
                OnPropertyChanged("OutputWindowName");
                OnPropertyChanged("ActionDescription");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ActionViewModelBase Clone()
        {
            return new ShowOutputWindowActionViewModel(new ShowOutputWindowAction(), ActionDescriptor, AllActionDescriptions) { OutputWindowName = OutputWindowName };
        }
    }
}
