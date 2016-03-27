using System.Collections.Generic;
using Adan.Client.Common.Model;
using Adan.Client.Common.Plugins;
using Adan.Client.Common.ViewModel;
using Adan.Client.Model.Actions;
using Adan.Client.ViewModel;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.Model.ActionDescriptions
{
    /// <summary>
    ///  A <see cref="ActionDescription"/> implementation for <see cref="ToggleFullScreenModeAction"/>.
    /// </summary>
    public sealed class ToggleFullScreenModeActionDescription : ActionDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleFullScreenModeActionDescription"/> class.
        /// </summary>
        /// <param name="allDescriptions">All descriptions.</param>
        public ToggleFullScreenModeActionDescription( [NotNull] IEnumerable<ActionDescription> allDescriptions) 
            : base("Toggle full screen mode", allDescriptions)
        {
        }

        /// <summary>
        /// Creates the <see cref="ActionBase" /> derived class instace.
        /// </summary>
        /// <returns>
        /// The instance of <see cref="ActionBase" /> derived class.
        /// </returns>
        public override ActionBase CreateAction()
        {
            return new ToggleFullScreenModeAction();
        }

        /// <summary>
        /// Creates the action view model by specified <see cref="ActionBase" /> instance.
        /// </summary>
        /// <param name="action">The <see cref="ActionBase" /> instance to create view model for.</param>
        /// <returns>
        /// Created action view model or <c>null</c> if specified action is not supported by this description.
        /// </returns>
        public override ActionViewModelBase CreateActionViewModel(ActionBase action)
        {
            Assert.ArgumentNotNull(action, "action");

            var sendToWindowAction = action as ToggleFullScreenModeAction;
            if (sendToWindowAction != null)
            {
                return new ToggleFullScreenModeActionViewModel(sendToWindowAction, this, AllDescriptions);
            }

            return null;
        }
    }
}
