using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adan.Client.Common.Plugins
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum Anchor
    {
        /// <summary>
        /// 
        /// </summary>
        None,

        /// <summary>
        /// /
        /// </summary>
        Top,

        /// <summary>
        /// 
        /// </summary>
        Bottom,

        /// <summary>
        /// 
        /// </summary>
        Left,

        /// <summary>
        /// 
        /// </summary>
        Right,
    }
}
