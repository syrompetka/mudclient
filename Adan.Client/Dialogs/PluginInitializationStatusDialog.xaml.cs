// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginInitializationStatusDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for PluginInitializationStatusDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Dialogs
{
    using System;

    using Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Adan.Client.Common.ViewModel;
    using System.Threading;

    /// <summary>
    /// Interaction logic for PluginInitializationStatusDialog.xaml
    /// </summary>
    public partial class PluginInitializationStatusDialog
    {
        private InitializationStatusModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInitializationStatusDialog"/> class.
        /// </summary>
        public PluginInitializationStatusDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        [NotNull]
        public InitializationStatusModel ViewModel
        {
            get
            {
                return _viewModel;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _viewModel = value;
                _viewModel.PropertyChanged += (o, e) => Dispatcher.Invoke((Action)HandleStatusChange);
            }
        }

        private void HandleStatusChange()
        {
            txtPluginName.Text = _viewModel.CurrentPluginName;
            txtStatus.Text = _viewModel.PluginInitializationStatus;
            //Thread.Sleep(1000);
        }
    }
}
