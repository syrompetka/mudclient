using System.Collections.Generic;
using Adan.Client.Common.Model;
using Adan.Client.Common.Plugins;
using Adan.Client.Common.ViewModel;
using Adan.Client.Model.Actions;
using CSLib.Net.Annotations;

namespace Adan.Client.ViewModel
{
    /// <summary>
    /// A view model for <see cref="ToggleFullScreenModeAction"/>
    /// </summary>
    public sealed class ToggleFullScreenModeActionViewModel : ActionViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleFullScreenModeActionViewModel"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <param name="allActionDescriptions">All action descriptions.</param>
        public ToggleFullScreenModeActionViewModel([NotNull] ToggleFullScreenModeAction action, [NotNull] ActionDescription actionDescriptor, [NotNull] IEnumerable<ActionDescription> allActionDescriptions)
            : base(action, actionDescriptor, allActionDescriptions)
        {
        }

        /// <summary>
        /// Gets the action description.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string ActionDescription
        {
            get
            {
                return "#togglefullscreen";
                
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this instance.
        /// </returns>
        public override ActionViewModelBase Clone()
        {
            return new ToggleFullScreenModeActionViewModel(new ToggleFullScreenModeAction(), ActionDescriptor, AllActionDescriptions);
        }
    }
}
