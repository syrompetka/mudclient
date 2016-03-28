// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializationStatusModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the InitializationStatusModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;

    /// <summary>
    /// A view model for initialization status dialog.
    /// </summary>
    public class InitializationStatusModel : ViewModelBase
    {
        private string _currentPluginName;
        private string _pluginInitializationStatus;

        /// <summary>
        /// Gets or sets the name of plugin currently being initialized.
        /// </summary>
        /// <value>
        /// The name of plugin currently being initialized.
        /// </value>
        [NotNull]
        public string CurrentPluginName
        {
            get
            {
                return _currentPluginName;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _currentPluginName = value;
                OnPropertyChanged("CurrentPluginName");
            }
        }

        /// <summary>
        /// Gets or sets the plugin initialization status.
        /// </summary>
        /// <value>
        /// The plugin initialization status.
        /// </value>
        [NotNull]
        public string PluginInitializationStatus
        {
            get
            {
                return _pluginInitializationStatus;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _pluginInitializationStatus = value;
                OnPropertyChanged("PluginInitializationStatus");
            }
        }
    }
}
