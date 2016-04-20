// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStatusModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ConnectionStatusModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel
{
    using Common.ViewModel;

    /// <summary>
    /// Model for connection status.
    /// </summary>
    public class ConnectionStatusViewModel : ViewModelBase
    {
        private bool _connected;
        private bool _connectionInProgress;

        /// <summary>
        /// Gets or sets a value indicating whether application is currently connected to server or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected
        {
            get
            {
                return _connected;
            }

            set
            {
                _connected = value;
                OnPropertyChanged("Connected");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether connection is in progress or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if connection is in progress; otherwise, <c>false</c>.
        /// </value>
        public bool ConnectionInProgress
        {
            get
            {
                return _connectionInProgress;
            }

            set
            {
                _connectionInProgress = value;
                OnPropertyChanged("ConnectionInProgress");
            }
        }
    }
}
