using System;
using System.Collections.Generic;
using Adan.Client.Commands;
using Adan.Client.Common.Model;
using CSLib.Net.Annotations;

namespace Adan.Client.Model.Actions
{
    /// <summary>
    /// The action that sends certain command to a specific window conveyor.
    /// </summary>
    [Serializable]
    public class SendToWindowAction : ActionBase
    {
        private List<ActionBase> _actionsToExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendToWindowAction"/> class.
        /// </summary>
        public SendToWindowAction()
        {
            ActionsToExecute = new List<ActionBase>();
        }

        /// <summary>
        /// Gets a value indicating whether this action can be executed in global scope or not.
        /// </summary>
        public override bool IsGlobal
        {
            get 
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the name of the output window to send command to.
        /// </summary>
        public string OutputWindowName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether specified command should be sent to all windows.
        /// </summary>
        public bool SendToAllWindows
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the actions to execute.
        /// </summary>
        [NotNull]
        public List<ActionBase> ActionsToExecute
        {
            get
            {
                return _actionsToExecute;
            }
            set
            {
                _actionsToExecute = value;
            }
        }

        /// <summary>
        /// Executes this action.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public override void Execute(RootModel model, ActionExecutionContext context)
        {
            model.PushCommandToConveyor(new SendToWindowCommand(OutputWindowName, ActionsToExecute, SendToAllWindows, context));
        }
    }
}
