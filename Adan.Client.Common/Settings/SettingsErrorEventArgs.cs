using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adan.Client.Common.Settings
{
    public class SettingsErrorEventArgs: EventArgs
    {
        public SettingsErrorEventArgs(string message)
        {
            Message = message;
        }
        public string Message { get; private set; }
    }
}
