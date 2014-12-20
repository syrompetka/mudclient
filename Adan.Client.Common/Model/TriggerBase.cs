// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerBase.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TriggerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;
    using Messages;

    /// <summary>
    /// Class that performs actions depending on server message.
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(TextTrigger))]
    public abstract class TriggerBase : IUndo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerBase"/> class.
        /// </summary>
        protected TriggerBase()
        {
            Actions = new List<ActionBase>();
            StopProcessingTriggersAfterThis = true;
            Priority = 5;
            Group = null;
            Operation = UndoOperation.None;
        }

        /// <summary>
        /// Gets or sets the actions to be performed when this trigger toggles.
        /// </summary>
        [NotNull]
        public List<ActionBase> Actions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        [XmlAttribute]
        public int Priority
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether message should not be processed against triggers or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if message should not be processed against triggers; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool StopProcessingTriggersAfterThis
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to hide message that caused this trigger or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if to hide original message; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool DoNotDisplayOriginalMessage
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
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
        public abstract string GetPatternString();

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="rootModel">The RootModel.</param>
        public abstract void HandleMessage([NotNull] Message message, [NotNull] RootModel rootModel);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string UndoInfo();

        /// <summary>
        /// 
        /// </summary>
        public abstract void Undo(RootModel rootModel);
    }
}
