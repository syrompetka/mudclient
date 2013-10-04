using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Adan.Client.Common.ViewModel;

namespace Adan.Client.ViewModel
{
    /// <summary>
    /// Profile View Model
    /// </summary>
    [Serializable]
    public class ProfileViewModel : ViewModelBase
    {
        private readonly string _name;
        private readonly bool _isDefault;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="isDefault">Is Default Profile</param>
        public ProfileViewModel(string name, bool isDefault)
        {
            _name = name;
            _isDefault = isDefault;
        }

        /// <summary>
        /// Get Name
        /// </summary>
        [XmlAttribute]
        public string NameProfile
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Get isDefault
        /// </summary>
        [XmlAttribute]
        public bool IsDefault
        {
            get
            {
                return _isDefault;
            }
        }
    }
}
