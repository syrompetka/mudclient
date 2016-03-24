using System.Collections.Generic;
using Adan.Client.Common.Model;
using Adan.Client.Common.Plugins;
using Adan.Client.Common.ViewModel;
using Adan.Client.Model.Actions;
using Adan.Client.ViewModel.Actions;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.Model.ActionDescriptions
{

    /// <summary>
    ///  A <see cref="ActionDescription"/> implementation for <see cref="SendToWindowAction"/>.
    /// </summary>
    public sealed class SendToWindowActionDescription : ActionDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendToWindowActionDescription"/> class.
        /// </summary>
        /// <param name="allDescriptions">All descriptions.</param>
        public SendToWindowActionDescription( [NotNull] IEnumerable<ActionDescription> allDescriptions) 
            : base("Send to output window", allDescriptions)
        {
        }

        /// <summary>
        /// Creates the <see cref="ActionBase" /> derived class instace.
        /// </summary>
        /// <returns>
        /// The instance of <see cref="ActionBase" /> derived class.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override ActionBase CreateAction()
        {
            var result = new SendToWindowAction();
            result.ActionsToExecute.Add(new SendTextAction());
            return result;
        }

        /// <summary>
        /// Creates the action view model by specified <see cref="ActionBase" /> instance.
        /// </summary>
        /// <param name="action">The <see cref="ActionBase" /> instance to create view model for.</param>
        /// <returns>
        /// Created action view model or <c>null</c> if specified action is not supported by this description.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override ActionViewModelBase CreateActionViewModel(ActionBase action)
        {
            Assert.ArgumentNotNull(action, "action");

            var sendToWindowAction = action as SendToWindowAction;
            if (sendToWindowAction != null)
            {
                return new SendToWindowActionViewModel(sendToWindowAction, this, AllDescriptions);
            }

            return null;
        }
    }
}
