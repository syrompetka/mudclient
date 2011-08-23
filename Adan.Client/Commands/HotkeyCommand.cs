// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotkeyCommand.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the HotkeyCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Commands
{
    using System;
    using System.Windows.Input;

    using Common.Commands;

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
        /// Gets or sets a value indicating whether thos command was processed or not.
        /// </summary>
        /// <value>
        ///  <c>true</c> if this hot key was processed; otherwise, <c>false</c>.
        /// </value>
        public bool HotkeyProcessed
        {
            get;
            set;
        }
    }
}
