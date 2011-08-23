// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Hotkey.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Hotkey type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Windows.Input;

    using CSLib.Net.Annotations;

    /// <summary>
    /// A hot key to a set of actions to perform.
    /// </summary>
    [DataContract]
    public class Hotkey
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hotkey"/> class.
        /// </summary>
        public Hotkey()
        {
            Key = Key.None;
            ModifierKeys = ModifierKeys.None;
            Actions = new List<ActionBase>();
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key that should be pressed.
        /// </value>
        [DataMember]
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
        [DataMember]
        public ModifierKeys ModifierKeys
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the actions to be performed when user uses hits this hotkey.
        /// </summary>
        [NotNull]
        [DataMember]
        public List<ActionBase> Actions
        {
            get;
            set;
        }
    }
}
