// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupWidgetControl.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for GroupWidgetControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget
{
    using System;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;
    using Adan.Client.Plugins.GroupWidget.Messages;
    using Adan.Client.Common.Model;
    using System.Collections.Generic;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for GroupWidgetControl.xaml
    /// </summary>
    public partial class GroupWidgetControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupWidgetControl"/> class.
        /// </summary>
        public GroupWidgetControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public string ViewModelUid
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public void NextGroupMate()
        {
            var executeToAct = (Action) (() =>
            {
                var groupWidgetControl = (GroupStatusViewModel)DataContext;

                if (groupWidgetControl.SelectedGroupMate == null
                    || groupWidgetControl.GroupMates.IndexOf(groupWidgetControl.SelectedGroupMate) == groupWidgetControl.GroupMates.Count - 1)
                {
                    groupWidgetControl.SelectedGroupMate = groupWidgetControl.GroupMates.FirstOrDefault();
                    return;
                }

                var index = groupWidgetControl.GroupMates.IndexOf(groupWidgetControl.SelectedGroupMate);
                groupWidgetControl.SelectedGroupMate = groupWidgetControl.GroupMates[index + 1];
            });

            Dispatcher.BeginInvoke(DispatcherPriority.Normal, executeToAct);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PreviousGroupMate()
        {
            var executeToAct = (Action)(() =>
            {
                var groupWidgetControl = (GroupStatusViewModel)DataContext;

                if (groupWidgetControl.SelectedGroupMate == null
                    || groupWidgetControl.GroupMates.IndexOf(groupWidgetControl.SelectedGroupMate) == 0)
                {
                    groupWidgetControl.SelectedGroupMate = groupWidgetControl.GroupMates.LastOrDefault();
                    return;
                }

                var index = groupWidgetControl.GroupMates.IndexOf(groupWidgetControl.SelectedGroupMate);
                groupWidgetControl.SelectedGroupMate = groupWidgetControl.GroupMates[index - 1];
            });

            Dispatcher.BeginInvoke(DispatcherPriority.Normal, executeToAct);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        /// <param name="characters"></param>
        public void UpdateModel([NotNull] RootModel rootModel, [NotNull] List<CharacterStatus> characters)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");
            Assert.ArgumentNotNull(characters, "characters");

            Action actToExecute = () => 
                {
                    GroupStatusViewModel viewModel = DataContext as GroupStatusViewModel;
                    viewModel.UpdateRootModel(rootModel);
                    viewModel.UpdateModel(characters);
                };

            Dispatcher.BeginInvoke(actToExecute);
        }

        private void CancelFocusingListBoxItem([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            ((ListBoxItem)sender).IsSelected = true;
            e.Handled = true;
        }
    }
}