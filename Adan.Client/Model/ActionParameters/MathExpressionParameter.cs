// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MathExpressionParameter.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the MathExpressionParameter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.ActionParameters
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Common.Model;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A parameter value of which is a result of evaluating some math expression.
    /// </summary>
    [Serializable]
    public class MathExpressionParameter : ActionParameterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MathExpressionParameter"/> class.
        /// </summary>
        public MathExpressionParameter()
        {
            FirstOperand = new TriggerOrCommandParameter();
            SecondOperand = new TriggerOrCommandParameter();
            Operation = MathOperation.Plus;
        }

        /// <summary>
        /// Gets or sets the first operand.
        /// </summary>
        /// <value>
        /// The first operand.
        /// </value>
        [NotNull]
        public ActionParameterBase FirstOperand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second operand.
        /// </summary>
        /// <value>
        /// The second operand.
        /// </value>
        [NotNull]
        public ActionParameterBase SecondOperand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        [XmlAttribute]
        public MathOperation Operation
        {
            get;
            set;
        }

        #region Overrides of ActionParameterBase

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

            var leftVal = FirstOperand.GetParameterValue(rootModel, context);
            var rightVal = SecondOperand.GetParameterValue(rootModel, context);

            double leftValueNumeric;
            double rightValueNumertic;

            bool isLeftValueNumeric = double.TryParse(leftVal, NumberStyles.Any, CultureInfo.InvariantCulture, out leftValueNumeric);
            bool isRightValueNumeric = double.TryParse(rightVal, NumberStyles.Any, CultureInfo.InvariantCulture, out rightValueNumertic);
            if (isRightValueNumeric && isLeftValueNumeric)
            {
                switch (Operation)
                {
                    case MathOperation.Plus:
                        return (leftValueNumeric + rightValueNumertic).ToString(CultureInfo.InvariantCulture);
                    case MathOperation.Minus:
                        return (leftValueNumeric - rightValueNumertic).ToString(CultureInfo.InvariantCulture);
                    case MathOperation.Multiplication:
                        return (leftValueNumeric * rightValueNumertic).ToString(CultureInfo.InvariantCulture);
                    case MathOperation.Division:
                        return rightValueNumertic == 0
                                   ? (leftValueNumeric / rightValueNumertic).ToString(CultureInfo.InvariantCulture)
                                   : string.Empty;
                }
            }
            else
            {
                if (Operation == MathOperation.Plus)
                {
                    return leftVal + rightVal;
                }
            }

            return string.Empty;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetParameterValue()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(FirstOperand.GetParameterValue());
            switch(Operation)
            {
                case MathOperation.Division:
                    sb.Append("/");
                    break;
                case MathOperation.Minus:
                    sb.Append("-");
                    break;
                case MathOperation.Multiplication:
                    sb.Append("*");
                    break;
                case MathOperation.Plus:
                    sb.Append("+");
                    break;
            }
            sb.Append(SecondOperand.GetParameterValue());

            return sb.ToString();
        }
    }
}
