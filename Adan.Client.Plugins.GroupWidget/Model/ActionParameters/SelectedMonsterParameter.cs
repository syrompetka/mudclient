namespace Adan.Client.Plugins.GroupWidget.Model.ActionParameters
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using Common.Model;

    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A parameter that return a name of selected group mate.
    /// </summary>
    [Serializable]
    public class SelectedMonsterParameter : ActionParameterBase
    {
        [XmlAttribute]
        [DefaultValue(0)]
        public int MonsterNumber { get; set; }

        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// Parameter value.
        /// </returns>
        public override string GetParameterValue(RootModel rootModel, ActionExecutionContext context)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");
            Assert.ArgumentNotNull(context, "context");
            
            if (MonsterNumber == 0)
            {
                return rootModel.GetVariableValue("Monster");
            }

            return rootModel.GetVariableValue("Monster" + MonsterNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetParameterValue()
        {
            if (MonsterNumber == 0)
            {
                return "$Monster";
            }

            return "$Monster" + MonsterNumber;

        }
    }
}
