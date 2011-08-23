// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubstitutionUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the SubstitutionUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ConveyorUnits
{
    using System.Linq;

    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;
    using Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementation that handles substitutions.
    /// </summary>
    public class SubstitutionUnit : ConveyorUnit
    {
        private readonly RootModel _rootModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubstitutionUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        /// <param name="rootModel">The root model.</param>
        public SubstitutionUnit([NotNull] MessageConveyor messageConveyor, [NotNull] RootModel rootModel)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
            Assert.ArgumentNotNull(rootModel, "rootModel");
            _rootModel = rootModel;
        }

        #region Overrides of ConveyorUnit

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            var textMessage = message as TextMessage;
            if (textMessage == null)
            {
                return;
            }

            foreach (var group in _rootModel.Groups.Where(g => g.IsEnabled))
            {
                foreach (var substitution in group.Substitutions)
                {
                    substitution.HandleMessage(textMessage);
                }
            }
        }

        #endregion
    }
}
