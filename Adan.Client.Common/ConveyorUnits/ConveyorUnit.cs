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

        private readonly MessageConveyor _messageConveyor;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConveyorUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        protected ConveyorUnit([NotNull] MessageConveyor messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");

            _messageConveyor = messageConveyor;
        }

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

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the command.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        public virtual void HandleCommand([NotNull] Command command)
        {
            Assert.ArgumentNotNull(command, "command");
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        public virtual void HandleMessage([NotNull] Message message)
        {
            Assert.ArgumentNotNull(message, "message");
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
        /// Pushes the command to conveyor.
        /// </summary>
        /// <param name="command">The command.</param>
        protected void PushCommandToConveyor([NotNull] Command command)
        {
            Assert.ArgumentNotNull(command, "command");

            _messageConveyor.PushCommand(command);
        }

        /// <summary>
        /// Pushes the message to conveyor.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void PushMessageToConveyor([NotNull] Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            _messageConveyor.PushMessage(message);
        }

        #endregion
    }
}