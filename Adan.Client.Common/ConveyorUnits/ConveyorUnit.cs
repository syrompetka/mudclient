﻿// --------------------------------------------------------------------------------------------------------------------
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
    public abstract class ConveyorUnit : IDisposable
    {
        #region Constants and Fields

        //private readonly MessageConveyor _messageConveyor;

        #endregion

        #region Constructors and Destructors

        #endregion

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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="rootModel"></param>
        /// <param name="isImport"></param>
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