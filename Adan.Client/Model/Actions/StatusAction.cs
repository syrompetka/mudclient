// -----------------------------------------public ---------------------------------------------------------------------------
// <copyright file="StatusAction.cs" company="Adamand MUD">
//   Copyright (c) Bester
// </copyright>
// <summary>
//   Defines the StatusAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.Actions
{
    using System;
    using System.Xml.Serialization;
    using Common.Commands;
    using Common.Model;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Action that operates on the Status Bar.
    /// </summary>
    [Serializable]
    public class StatusAction : ActionWithParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusAction"/> class.
        /// </summary>
        public StatusAction()
        {
            CommandText = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsGlobal
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        /// <value>
        /// The command text.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string CommandText
        {
            get;
            set;
        }

        #region Overrides of ActionBase

        /// <summary>
        /// Executes this action.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public override void Execute(RootModel model, ActionExecutionContext context)
        {
            Assert.ArgumentNotNull(model, "model");
            Assert.ArgumentNotNull(context, "context");

            var cmd = new TextCommand("#status " + CommandText);
            model.PushCommandToConveyor(cmd);
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return CommandText;
        }
    }
}
