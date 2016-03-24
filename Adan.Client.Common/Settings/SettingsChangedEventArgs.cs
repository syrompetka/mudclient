using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adan.Client.Common.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SettingsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="changedValue"></param>
        public SettingsChangedEventArgs(string name, object changedValue)
        {
            Name = name;
            Value = changedValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get;
            private set;
        }
    }
}
