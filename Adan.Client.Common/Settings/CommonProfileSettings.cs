using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Adan.Client.Common.Settings
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CommonProfileSettings
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public bool MultiAction
        {
            get;
            set;
        }
    }
}
