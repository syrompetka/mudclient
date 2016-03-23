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
    using System.Collections.Generic;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="SubstitutionUnit"/> class.
        /// </summary>
        public SubstitutionUnit()
            : base()
        {
        }

        #region Overrides of ConveyorUnit

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return new[] { BuiltInMessageTypes.TextMessage };
            }
        }

        /// <summary>
        /// Gets a set of command types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledCommandTypes
        {
            get
            {
                return Enumerable.Empty<int>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="rootModel"></param>
        public override void HandleMessage(Message message, RootModel rootModel)
        {
            Assert.ArgumentNotNull(message, "message");

            var textMessage = message as TextMessage;
            if (textMessage != null)
            {
                if (!textMessage.SkipSubstitution)
                {
                    foreach (var group in rootModel.Groups.Where(g => g.IsEnabled))
                    {
                        foreach (var substitution in group.Substitutions)
                        {
                            substitution.HandleMessage(textMessage, rootModel);
                        }
                    }
                }
            }

        }

        #endregion
    }
}
