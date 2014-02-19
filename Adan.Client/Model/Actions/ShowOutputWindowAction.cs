using System;
using Adan.Client.Commands;
using Adan.Client.Common.Model;

namespace Adan.Client.Model.Actions
{
    /// <summary>
    /// Action that changes currently active output window.
    /// </summary>
    [Serializable]
    public class ShowOutputWindowAction : ActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowOutputWindowAction"/> class.
        /// </summary>
        public ShowOutputWindowAction()
        {
            OutputWindowName = string.Empty;
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
        /// Gets or sets the name of the output window to show.
        /// </summary>
        public string OutputWindowName
        {
            get;
            set;
        }

        /// <summary>
        /// Executes this action.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public override void Execute(RootModel model, ActionExecutionContext context)
        {
            model.PushCommandToConveyor(new ShowMainOutputCommand(OutputWindowName));
        }
    }
}
