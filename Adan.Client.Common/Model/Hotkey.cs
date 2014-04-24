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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Input;
    using System.Xml.Serialization;
    using CSLib.Net.Annotations;

    /// <summary>
    /// A hot key to a set of actions to perform.
    /// </summary>
    [Serializable]
    public class Hotkey : IUndo
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
        [XmlAttribute]
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
        [XmlAttribute]
        public ModifierKeys ModifierKeys
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the actions to be performed when user uses hits this hotkey.
        /// </summary>
        [NotNull]
        public List<ActionBase> Actions
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public UndoOperation Operation
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
                [XmlIgnore]
        public Group Group
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetKeyToString()
        {
            if (Key == System.Windows.Input.Key.None)
                return "None";

            StringBuilder sb = new StringBuilder();

            if ((ModifierKeys & ModifierKeys.Control) == ModifierKeys.Control)
            {
                sb.Append("Control+");
            }

            if ((ModifierKeys & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                sb.Append("Shift+");
            }

            if ((ModifierKeys & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                sb.Append("Alt+");
            }

            if ((ModifierKeys & ModifierKeys.Windows) == ModifierKeys.Windows)
            {
                sb.Append("Windows+");
            }

            sb.Append(Key);

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string UndoInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#Хоткей {").Append(GetKeyToString()).Append("} ");

            switch (Operation)
            {
                case UndoOperation.Add:
                    sb.Append("восстановлен");
                    break;
                case UndoOperation.Remove:
                    sb.Append("удален");
                    break;
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Undo()
        {
            if (Group != null && Operation != UndoOperation.None)
            {
                switch (Operation)
                {
                    case UndoOperation.Add:
                        Group.Hotkeys.Add(this);
                        break;
                    case UndoOperation.Remove:
                        Group.Hotkeys.Remove(this);
                        break;
                }
            }
        }
    }
}
