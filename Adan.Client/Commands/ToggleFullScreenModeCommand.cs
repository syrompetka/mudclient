using Adan.Client.Common.Commands;

namespace Adan.Client.Commands
{
    /// <summary>
    /// Command to toggle full screen mode of the main window.
    /// </summary>
    public sealed class ToggleFullScreenModeCommand : Command
    {
        /// <summary>
        /// Gets the type of this command.
        /// </summary>
        /// <value>
        /// The type of this command.
        /// </value>
        public override int CommandType
        {
            get
            {
                return BuiltInCommandTypes.ToggleFullScreenMode;
            }
        }
    }
}
