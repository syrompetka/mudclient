// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConveyorUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ConveyorUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.ConveyorUnits
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using Adan.Client.Common.Model;
    using Commands;
    using Conveyor;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;

    #endregion

    /// <summary>
    /// Base class for all conveyor units.
    /// </summary>
    public abstract class ConveyorUnit
    {

        #region Properties

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        [NotNull]
        public abstract IEnumerable<int> HandledMessageTypes
        {
            get;
        }

        /// <summary>
        /// Gets a set of command types that this unit can handle.
        /// </summary>
        [NotNull]
        public abstract IEnumerable<int> HandledCommandTypes
        {
            get;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="rootModel">The root model.</param>
        /// <param name="isImport">if set to <c>true</c> [is import].</param>
        public virtual void HandleCommand([NotNull] Command command, [NotNull] RootModel rootModel, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");
            Assert.ArgumentNotNull(rootModel, "rootModel");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="rootModel"></param>
        public virtual void HandleMessage([NotNull] Message message, [NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(message, "message");
            Assert.ArgumentNotNull(rootModel, "rootModel");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="rootModel"></param>
        protected void PushCommandToConveyor([NotNull] Command command, [NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(command, "command");
            Assert.ArgumentNotNull(rootModel, "rootModel");

            rootModel.PushCommandToConveyor(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="rootModel"></param>
        protected void PushMessageToConveyor([NotNull] Message message, [NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(message, "message");
            Assert.ArgumentNotNull(rootModel, "rootModel");

            rootModel.PushMessageToConveyor(message);
        }

        #endregion
    }
}