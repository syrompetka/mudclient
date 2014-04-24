using System;
using Adan.Client.Commands;
using Adan.Client.Common.Model;

namespace Adan.Client.Model.Actions
{
    /// <summary>
    /// Action that toggles full screen mode of the main window.
    /// </summary>
    [Serializable]
    public sealed class ToggleFullScreenModeAction : ActionBase
    {
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
        /// Executes this action.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public override void Execute(RootModel model, ActionExecutionContext context)
        {
            model.PushCommandToConveyor(new ToggleFullScreenModeCommand());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "#togglefullscreen";
        }
    }
}
