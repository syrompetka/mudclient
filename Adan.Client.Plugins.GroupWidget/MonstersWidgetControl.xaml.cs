// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonstersWidgetControl.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for GroupWidgetControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;
    using Adan.Client.Plugins.GroupWidget.Messages;
    using Adan.Client.Common.Model;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for GroupWidgetControl.xaml
    /// </summary>
    public partial class MonstersWidgetControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonstersWidgetControl"/> class.
        /// </summary>
        public MonstersWidgetControl()
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
        /// <param name="characters"></param>
        public void UpdateModel([NotNull] List<MonsterStatus> characters)
        {
            Assert.ArgumentNotNull(characters, "roomMonstersMessage");

            Action actToExecute = () =>
            {
                RoomMonstersViewModel viewModel = DataContext as RoomMonstersViewModel;
                viewModel.UpdateModel(characters);
            };

            Application.Current.Dispatcher.BeginInvoke(actToExecute, DispatcherPriority.Background);
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
