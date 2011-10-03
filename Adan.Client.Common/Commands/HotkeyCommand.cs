// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotkeyCommand.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the HotkeyCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Commands
{
    using System.Windows.Input;

    /// <summary>
    /// A command of pressing some key on the keyboard.
    /// </summary>
    public class HotkeyCommand : Command
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key that should be pressed.
        /// </value>
        public Key Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the modifier keys.
        /// </summary>
        /// <value>
        /// The modifier keys.
        /// </value>
        public ModifierKeys ModifierKeys
        {
            get;
            set;
        }

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
                return BuiltInCommandTypes.HotkeyCommand;
            }
        }
    }
}
