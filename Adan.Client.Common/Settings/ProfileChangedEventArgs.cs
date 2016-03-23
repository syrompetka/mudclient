using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adan.Client.Common.Settings
{
    /// <summary>
    /// Event arguments for the ProfileChanged event of ProfileHolder
    /// </summary>
    public sealed class ProfileChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the profile</param>
        /// <param name="globalChange">Is the change global?</param>
        public ProfileChangedEventArgs(string name, bool globalChange = false)
        {
            Name = name;
            Global = globalChange;
        }

        /// <summary>
        /// Name of the profile that has been changed
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Is the change global?
        /// </summary>
        public bool Global { get; set; }

    }
}
