using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Adan.Client.Common.Model;
using Adan.Client.Common.Plugins;
using Adan.Client.Common.ViewModel;
using Adan.Client.Model.Actions;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.ViewModel.Actions
{
    /// <summary>
    /// A view model for <see cref="SendToWindowAction"/>.
    /// </summary>
    public sealed class SendToWindowActionViewModel : ActionViewModelBase
    {
        private readonly SendToWindowAction _action;
        private readonly IEnumerable<ActionDescription> _allDescriptors;
        private ActionsViewModel _actionsToExecute;
        /// <summary>
        /// Initializes a new instance of the <see cref="SendToWindowActionViewModel"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <param name="allActionDescriptions">All action descriptions.</param>
        public SendToWindowActionViewModel([NotNull] SendToWindowAction action, [NotNull] ActionDescription actionDescriptor, [NotNull] IEnumerable<ActionDescription> allActionDescriptions)
            : base(action, actionDescriptor, allActionDescriptions)
        {
            _action = action;
            _allDescriptors = allActionDescriptions;
            _actionsToExecute = new ActionsViewModel(action.ActionsToExecute, allActionDescriptions);
            _actionsToExecute.PropertyChanged += HandleActionDescriptionChange;
        }

        /// <summary>
        /// Gets or sets a value indicating whether specified command should be sent to all windows.
        /// </summary>
        public bool SendToAllWindows
        {
            get { return _action.SendToAllWindows; }
            set
            {
                _action.SendToAllWindows = value;
                OnPropertyChanged("SendToAllWindows");
                OnPropertyChanged("ActionDescription");
            }
        }

        /// <summary>
        /// Get or set the name of window to send commands to.
        /// </summary>
        [CanBeNull]
        public string OutputWindowName
        {
            get
            {
                return _action.OutputWindowName;
            }

            set
            {
                _action.OutputWindowName = value;
                OnPropertyChanged("OutputWindowName");
                OnPropertyChanged("ActionDescription");
            }
        }

        /// <summary>
        /// Gets or sets the actions to execute.
        /// </summary>
        /// <value>
        /// The actions to execute.
        /// </value>
        [NotNull]
        public ActionsViewModel ActionsToExecute
        {
            get
            {
                return _actionsToExecute;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                if (_actionsToExecute != null)
                {
                    _actionsToExecute.PropertyChanged -= HandleActionDescriptionChange;
                }

                _actionsToExecute = value;
                _actionsToExecute.PropertyChanged += HandleActionDescriptionChange;
                OnPropertyChanged("ActionsToExecute");
                OnPropertyChanged("ActionDescription");
            }
        }
        /// <summary>
        /// Gets the action description.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string ActionDescription
        {
            get
            {
                if (SendToAllWindows)
                {
                    return "#sendall " + ActionsToExecute.ActionsDescription;
                }

                return "#send " + ActionsToExecute.ActionsDescription + " " + OutputWindowName;
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this instance.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override ActionViewModelBase Clone()
        {
            var conditionalAction = new SendToWindowAction();
            return new SendToWindowActionViewModel(conditionalAction, ActionDescriptor, _allDescriptors)
            {
                OutputWindowName = OutputWindowName,
                SendToAllWindows = SendToAllWindows,
                ActionsToExecute = ActionsToExecute.Clone(conditionalAction.ActionsToExecute)
            };
        }

        private void HandleActionDescriptionChange([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (e.PropertyName == "ActionsDescription")
            {
                OnPropertyChanged("ActionDescription");
            }
        }
    }
}
