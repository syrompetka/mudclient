using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adan.Client.Common.ViewModel;
using CSLib.Net.Annotations;
using Adan.Client.Common.Plugins;
using CSLib.Net.Diagnostics;
using Adan.Client.Common.Model;
using Adan.Client.Model.Actions;

namespace Adan.Client.ViewModel.Actions
{
    /// <summary>
    /// 
    /// </summary>
    public class ShowOutputWindowActionViewModel : ActionViewModelBase
    {
        private ShowOutputWindowAction _action;

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
            Assert.ArgumentNotNull(actionDescriptor, "actionDescription");
            Assert.ArgumentNotNull(allDescriptions, "allDescription");

            _action = action;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ActionDescription
        {
            get
            {
                return "SS";
                //return string.Format("#window {0}", _action.OutputWindowName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ActionViewModelBase Clone()
        {
            return new ShowOutputWindowActionViewModel(_action, ActionDescriptor, AllActionDescriptions);
        }
    }
}
