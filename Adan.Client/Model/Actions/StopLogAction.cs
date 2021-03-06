﻿// -----------------------------------------public ---------------------------------------------------------------------------
// <copyright file="StopLogAction.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the StopLogAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.Actions
{
    using System;

    using Common.Model;

    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Action that stops logging to file.
    /// </summary>
    [Serializable]
    public class StopLogAction : ActionBase
    {
        #region Overrides of ActionBase

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
        /// Executes this action.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public override void Execute(RootModel model, ActionExecutionContext context)
        {
            Assert.ArgumentNotNull(model, "model");
            Assert.ArgumentNotNull(context, "context");

            model.PushMessageToConveyor(new Messages.StopLoggingMessage());
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "#stoplog";
        }
    }
}
