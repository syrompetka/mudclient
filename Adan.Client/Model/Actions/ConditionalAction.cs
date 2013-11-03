// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConditionalAction.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ConditionalAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml.Serialization;

    using ActionParameters;

    using Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Action that executes only if certain condition is true.
    /// </summary>
    [Serializable]
    public class ConditionalAction : ActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalAction"/> class.
        /// </summary>
        public ConditionalAction()
        {
            LeftConditionParameter = new TriggerOrCommandParameter();
            RightConditionParameter = new TriggerOrCommandParameter();
            Condition = ActionCondition.Equals;
            ActionsToExecute = new List<ActionBase>();
        }

        /// <summary>
        /// Gets or sets the left condition parameter.
        /// </summary>
        /// <value>
        /// The left condition parameter.
        /// </value>
        [NotNull]
        public ActionParameterBase LeftConditionParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the right condition parameter.
        /// </summary>
        /// <value>
        /// The right condition parameter.
        /// </value>
        [NotNull]
        public ActionParameterBase RightConditionParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        [XmlAttribute]
        public ActionCondition Condition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the actions to execute.
        /// </summary>
        /// <value>
        /// The actions to execute.
        /// </value>
        [NotNull]
        public List<ActionBase> ActionsToExecute
        {
            get;
            set;
        }

        /// <summary>
        /// Executes this action.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public override void Execute(RootModel model, ActionExecutionContext context)
        {
            Assert.ArgumentNotNull(model, "model");
            Assert.ArgumentNotNull(context, "context");

            var firstVal = LeftConditionParameter.GetParameterValue(model, context);
            var secondVal = RightConditionParameter.GetParameterValue(model, context);

            double firstValDouble;

            double secondValDouble;
            bool isFirstValDouble = double.TryParse(firstVal, NumberStyles.Any, CultureInfo.InvariantCulture, out firstValDouble);
            bool isSecondValDouble = double.TryParse(secondVal, NumberStyles.Any, CultureInfo.InvariantCulture, out secondValDouble);

            bool isSuccess = false;
            if (isFirstValDouble && isSecondValDouble)
            {
                switch (Condition)
                {
                    case ActionCondition.Equals:
                        isSuccess = firstValDouble == secondValDouble;
                        break;
                    case ActionCondition.Greater:
                        isSuccess = firstValDouble > secondValDouble;
                        break;
                    case ActionCondition.GreaterOrEquals:
                        isSuccess = firstValDouble >= secondValDouble;
                        break;
                    case ActionCondition.Less:
                        isSuccess = firstValDouble < secondValDouble;
                        break;
                    case ActionCondition.LessOrEquals:
                        isSuccess = firstValDouble <= secondValDouble;
                        break;
                    case ActionCondition.NotEquals:
                        isSuccess = firstValDouble != secondValDouble;
                        break;
                }
            }
            else
            {
                switch (Condition)
                {
                    case ActionCondition.Equals:
                        isSuccess = firstVal == secondVal;
                        break;
                    case ActionCondition.NotEquals:
                        isSuccess = firstVal != secondVal;
                        break;
                    case ActionCondition.IsEmpty:
                        isSuccess = string.IsNullOrEmpty(firstVal);
                        break;
                    case ActionCondition.IsNotEmpty:
                        isSuccess = !string.IsNullOrEmpty(firstVal);
                        break;
                }
            }

            if (isSuccess)
            {
                foreach (var action in ActionsToExecute)
                {
                    action.Execute(model, context);
                }
            }
        }
    }
}
