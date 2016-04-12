// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectedGroupMateParameter.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the SelectedGroupMateParameter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    public class SelectedGroupMateParameter : ActionParameterBase
    {
        [XmlAttribute]
        [DefaultValue(0)]
        public int GroupMateNumber { get; set; }

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

            if (GroupMateNumber == 0)
            {
                return rootModel.GetVariableValue("GroupMate");
            }

            return rootModel.GetVariableValue("GroupMate" + GroupMateNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetParameterValue()
        {
            if (GroupMateNumber == 0)
            {
                return "$GroupMate";
            }

            return "$GroupMate" + GroupMateNumber;
        }
    }
}
