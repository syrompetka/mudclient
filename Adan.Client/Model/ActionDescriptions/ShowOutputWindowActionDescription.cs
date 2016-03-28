using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adan.Client.Common.Plugins;
using Adan.Client.Common.Model;
using Adan.Client.Common.ViewModel;
using Adan.Client.Model.Actions;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using Adan.Client.ViewModel.Actions;

namespace Adan.Client.Model.ActionDescriptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ShowOutputWindowActionDescription : ActionDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClearVariableValueActionDescription"/> class.
        /// </summary>
        /// <param name="allDescriptions">All descriptions.</param>
        public ShowOutputWindowActionDescription([NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base("Switch output window", allDescriptions)
        {
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ActionBase CreateAction()
        {
            return new ShowOutputWindowAction();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public override ActionViewModelBase CreateActionViewModel([NotNull] ActionBase action)
        {
            Assert.ArgumentNotNull(action, "action");

            var showOutputWindowAction = action as ShowOutputWindowAction;
            if (showOutputWindowAction != null)
            {
                return new ShowOutputWindowActionViewModel(showOutputWindowAction, this, AllDescriptions);
            }

            return null;
        }
    }
}
