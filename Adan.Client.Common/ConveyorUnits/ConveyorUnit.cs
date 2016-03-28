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

        protected ConveyorUnit(MessageConveyor conveyor)
        {
            Conveyor = conveyor;
        }

        #region Properties

        public MessageConveyor Conveyor { get; private set; }

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
        /// <param name="isImport">if set to <c>true</c> [is import].</param>
        public virtual void HandleCommand([NotNull] Command command, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");
        }

        public virtual void HandleMessage([NotNull] Message message)
        {
            Assert.ArgumentNotNull(message, "message");
        }

        protected void PushCommandToConveyor([NotNull] Command command)
        {
            Assert.ArgumentNotNull(command, "command");
            
            Conveyor.PushCommand(command);
        }

        protected void PushMessageToConveyor([NotNull] Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            
            Conveyor.PushMessage(message);
        }

        #endregion

        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}