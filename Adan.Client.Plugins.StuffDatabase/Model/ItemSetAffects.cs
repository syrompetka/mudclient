using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Adan.Client.Plugins.StuffDatabase.Model.Affects;
using CSLib.Net.Annotations;

namespace Adan.Client.Plugins.StuffDatabase.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ItemSetAffects
    {
        private readonly List<AppliedAffect> _appliedAffects;

        /// <summary>
        /// 
        /// </summary>
        public ItemSetAffects()
        {
            _appliedAffects = new List<AppliedAffect>();
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(typeof(SkillResist))]
        [XmlElement(typeof(SkillEnhance))]
        [XmlElement(typeof(MagicArrows))]
        [XmlElement(typeof(Envenom))]
        [XmlElement(typeof(Enhance))]
        [NotNull]
        public List<AppliedAffect> AppliedAffects
        {
            get
            {
                return _appliedAffects;
            }
        }
    }
}
