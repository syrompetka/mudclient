// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the MainWindowModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel
{
    using System.Collections.Generic;

    using Common.Model;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A model for main window
    /// </summary>
    public class MainWindowModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowModel"/> class.
        /// </summary>
        /// <param name="allGroups">All groups.</param>
        /// <param name="allVariables">All variables.</param>
        /// <param name="rootModel">The RootModel.</param>
        public MainWindowModel([NotNull] IList<Group> allGroups, [NotNull] IEnumerable<Variable> allVariables, [NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(allGroups, "allGroups");
            Assert.ArgumentNotNull(allVariables, "allVariables");
            Assert.ArgumentNotNull(rootModel, "rootModel");

            //AllVariables = allVariables;
            RootModel = rootModel;

            ConnectionStatus = new ConnectionStatusModel();
           // GroupsModel = new GroupsViewModel(allGroups, rootModel.AllActionDescriptions, rootModel);
            //AllGroups = allGroups;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowModel"/> class.
        /// </summary>
        /// <param name="rootModel">The RootModel.</param>
        public MainWindowModel([NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");

            //AllVariables = allVariables;
            RootModel = rootModel;

            ConnectionStatus = new ConnectionStatusModel();
            // GroupsModel = new GroupsViewModel(allGroups, rootModel.AllActionDescriptions, rootModel);
            //AllGroups = allGroups;
        }

        /// <summary>
        /// Gets all groups.
        /// </summary>
        public IList<Group> AllGroups
        {
            //get;
            //private set;
            get
            {
                return RootModel.Groups;
            }
        }

        /// <summary>
        /// Gets all variables.
        /// </summary>
        [NotNull]
        public IEnumerable<Variable> AllVariables
        {
            //get;
            //private set;
            get
            {
                return RootModel.Variables;
            }
        }

        /// <summary>
        /// Gets the RootModel.
        /// </summary>
        [NotNull]
        public RootModel RootModel
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the connection status.
        /// </summary>
        [NotNull]
        public ConnectionStatusModel ConnectionStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the group model.
        /// </summary>
        [NotNull]
        public GroupsViewModel GroupsModel
        {
            get
            {
                //return new GroupsViewModel(AllGroups, RootModel.AllActionDescriptions);
                return new GroupsViewModel(AllGroups, RootModel.AllActionDescriptions);
            }
            private set { }
        }
    }
}
