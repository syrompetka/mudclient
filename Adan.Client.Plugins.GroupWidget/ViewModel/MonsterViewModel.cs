// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonsterViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the MonsterViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget.ViewModel
{
    using System.Collections.Generic;

    using Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// View model for single monster in the players room.
    /// </summary>
    public class MonsterViewModel : GroupMateViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonsterViewModel"/> class.
        /// </summary>
        /// <param name="monsterStatus">The monster status.</param>
        /// <param name="affectsToDisplay">The affects to display.</param>
        public MonsterViewModel([NotNull] MonsterStatus monsterStatus, [NotNull] IEnumerable<AffectDescription> affectsToDisplay)
            : base(monsterStatus, affectsToDisplay)
        {
            Assert.ArgumentNotNull(monsterStatus, "monsterStatus");
            Assert.ArgumentNotNull(affectsToDisplay, "affectsToDisplay");

            MonsterStatus = monsterStatus;
        }

        /// <summary>
        /// Gets the monster status.
        /// </summary>
        [NotNull]
        public MonsterStatus MonsterStatus
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this monster is player character.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this monsters is player character; otherwise, <c>false</c>.
        /// </value>
        public bool PlayerCharacter
        {
            get
            {
                return MonsterStatus.IsPlayerCharacter;
            }

            set
            {
                if (value != MonsterStatus.IsPlayerCharacter)
                {
                    MonsterStatus.IsPlayerCharacter = value;
                    OnPropertyChanged("PlayerCharacter");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this monster is boss.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this monster is boss; otherwise, <c>false</c>.
        /// </value>
        public bool Boss
        {
            get
            {
                return MonsterStatus.IsBoss;
            }

            set
            {
                if (value != MonsterStatus.IsBoss)
                {
                    MonsterStatus.IsBoss = value;
                    OnPropertyChanged("Boss");
                }
            }
        }

        /// <summary>
        /// Updates this view model from model.
        /// </summary>
        /// <param name="characterStatus">The character status model.</param>
        public override void UpdateFromModel(CharacterStatus characterStatus)
        {
            Assert.ArgumentNotNull(characterStatus, "characterStatus");
            var monsterStatus = characterStatus as MonsterStatus;
            if (monsterStatus != null)
            {
                PlayerCharacter = monsterStatus.IsPlayerCharacter;
                Boss = monsterStatus.IsBoss;
            }

            base.UpdateFromModel(characterStatus);
        }
    }
}
