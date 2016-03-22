using Adan.Client.Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Adan.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MainOutputWindowBase : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public abstract void AddMessages(IList<TextMessage> messages);

        /// <summary>
        /// 
        /// </summary>
        public abstract TextBoxWithHistory CommandInput
        {
            get;
        }
    }
}
