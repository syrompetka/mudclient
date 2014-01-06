using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Adan.Client.Common.Controls.AvalonEdit.VisualLineElements
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAdanVisualLineElement
    {
        /// <summary>
        /// 
        /// </summary>
        Brush Foreground
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        Brush Background
        {
            get;
        }
    }
}
