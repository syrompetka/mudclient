// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionDialogViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ConnectionDialogViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel
{
    using CSLib.Net.Annotations;

    /// <summary>
    /// View model for connection dialog.
    /// </summary>
    public class ConnectionDialogViewModel
    {
        /// <summary>
        /// Gets or sets the name of the host.
        /// </summary>
        /// <value>
        /// The name of the host.
        /// </value>
        [NotNull]
        public string HostName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port to connect to.
        /// </value>
        public int Port
        {
            get;
            set;
        }
    }
}
