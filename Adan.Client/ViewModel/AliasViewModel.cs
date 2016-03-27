// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AliasViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the AliasViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel
{
    #region Namespace Imports

    using System.Collections.Generic;

    using Actions;

    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    #endregion

    /// <summary>
    /// View model for alias editor.
    /// </summary>
    public class AliasViewModel : ViewModelBase
    {
        #region Constants and Fields

        private readonly IEnumerable<ActionDescription> _actionDescriptions;
        private GroupViewModel _aliasGroup;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AliasViewModel"/> class.
        /// </summary>
        /// <param name="allGroups">All groups.</param>
        /// <param name="aliasGroup">The alias group.</param>
        /// <param name="commandAlias">The alias.</param>
        /// <param name="actionDescriptions">The action descriptions.</param>
        public AliasViewModel([NotNull] IEnumerable<GroupViewModel> allGroups, [NotNull] GroupViewModel aliasGroup, [NotNull] CommandAlias commandAlias, [NotNull] IEnumerable<ActionDescription> actionDescriptions)
        {
            Assert.ArgumentNotNull(allGroups, "allGroups");
            Assert.ArgumentNotNull(aliasGroup, "aliasGroup");
            Assert.ArgumentNotNull(commandAlias, "commandAlias");
            Assert.ArgumentNotNull(actionDescriptions, "actionDescriptions");

            _aliasGroup = aliasGroup;
            _actionDescriptions = actionDescriptions;
            CommandAlias = commandAlias;
            AllGroups = allGroups;
            ActionsViewModel = new ActionsViewModel(commandAlias.Actions, actionDescriptions);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the action view model.
        /// </summary>
        [NotNull]
        public ActionsViewModel ActionsViewModel
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        [NotNull]
        public CommandAlias CommandAlias
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the alias description.
        /// </summary>
        [NotNull]
        public string AliasDescription
        {
            get
            {
                return Command;
            }
        }

        /// <summary>
        /// Gets or sets the alias group.
        /// </summary>
        /// <value>
        /// The alias group.
        /// </value>
        [NotNull]
        public GroupViewModel AliasGroup
        {
            get
            {
                return _aliasGroup;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _aliasGroup = value;
                OnPropertyChanged("AliasGroup");
            }
        }

        /// <summary>
        /// Gets all groups.
        /// </summary>
        [NotNull]
        public IEnumerable<GroupViewModel> AllGroups
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        [NotNull]
        public string Command
        {
            get
            {
                return CommandAlias.Command;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                CommandAlias.Command = value;
                OnPropertyChanged("Command");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this instance.
        /// </returns>
        [NotNull]
        public AliasViewModel Clone()
        {
            var clonedAlias = new CommandAlias();
            return new AliasViewModel(AllGroups, AliasGroup, clonedAlias, _actionDescriptions)
                       {
                           ActionsViewModel = ActionsViewModel.Clone(clonedAlias.Actions),
                           Command = Command
                       };
        }

        #endregion
    }
}