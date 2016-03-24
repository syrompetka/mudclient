// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionWithParameters.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ActionWithParameters type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Base class for actions that have parameters.
    /// </summary>
    [Serializable]
    public abstract class ActionWithParameters : ActionBase
    {
        private StringBuilder _parametersStringBuilder;
       // private Regex _variableSearchRegex;
        private List<ActionParameterBase> _parameters;

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        [NotNull]
        public List<ActionParameterBase> Parameters
        {
            get
            {
                return _parameters ?? (_parameters = new List<ActionParameterBase>());
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _parameters = value;
            }
        }

        [NotNull]
        private StringBuilder ParametersStringBuilder
        {
            get
            {
                return _parametersStringBuilder ?? (_parametersStringBuilder = new StringBuilder());
            }
        }

        //[NotNull]
        //private Regex VariableSearchRegex
        //{
        //    get
        //    {
        //        return _variableSearchRegex ?? (_variableSearchRegex = new Regex(@"\$(\w+)"));
        //    }
        //}

        /// <summary>
        /// Gets the parameters string.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns>A string contaning all parameter values separated by a space.</returns>
        [NotNull]
        protected string GetParametersString([NotNull] RootModel model, [NotNull] ActionExecutionContext context)
        {
            Assert.ArgumentNotNull(model, "model");
            Assert.ArgumentNotNull(context, "context");

            ParametersStringBuilder.Clear();
            foreach (var parameter in Parameters)
            {
                ParametersStringBuilder.Append(" ");
                ParametersStringBuilder.Append(parameter.GetParameterValue(model, context));
            }

            return ParametersStringBuilder.ToString();
        }

        /// <summary>
        /// Postprocesses the result string replacing all %1.
        /// </summary>
        /// <param name="valueToProcess">The value to process.</param>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A result of post processing.
        /// </returns>
        [NotNull]
        protected virtual string PostProcessString([NotNull] string valueToProcess, [NotNull] RootModel model, [NotNull] ActionExecutionContext context)
        {
            Assert.ArgumentNotNull(valueToProcess, "valueToProcess");
            Assert.ArgumentNotNull(model, "model");
            Assert.ArgumentNotNull(context, "context");

            var res = valueToProcess;
            for (int i = 0; i < 10; i++)
            {
                if (context.Parameters.ContainsKey(i))
                    res = ReplaceParameters(res, i.ToString()[0], context.Parameters[i]);
                else
                    res = ReplaceParameters(res, i.ToString()[0], string.Empty);
            }
            //for (int i = 0; i < 10; i++)
            //{
            //    if (context.Parameters.ContainsKey(i))
            //        res = res.Replace("%" + i, context.Parameters[i]);
            //    else
            //        res = res.Replace("%" + i, string.Empty);
            //}

            return ReplaceVariables(res, model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="rootModel"></param>
        /// <returns></returns>
        [NotNull]
        protected string ReplaceVariables([NotNull] string input, [NotNull] RootModel rootModel)
        {
            StringBuilder sb = new StringBuilder();

            int nest = 0;
            int lastPos = 0;
            int i = 0;
            while (i < input.Length)
            {
                if (input[i] == '$')
                {
                    if (nest == 0)
                    {
                        if (i - lastPos > 0)
                            sb.Append(input, lastPos, i - lastPos);

                        i++;
                        int startPos = i;

                        while (i < input.Length && char.IsLetterOrDigit(input[i]))
                            i++;

                        sb.Append(rootModel.GetVariableValue(input.Substring(startPos, i - startPos)));
                        lastPos = i;
                    }
                }
                else if (input[i] == '{')
                {
                    nest++;
                }
                else if (input[i] == '}')
                {
                    nest--;
                }

                i++;
            }

            if (lastPos < input.Length)
                sb.Append(input, lastPos, input.Length - lastPos);

            return sb.ToString();
        }

        private string ReplaceParameters(string input, char number, string replacement)
        {
            StringBuilder sb = new StringBuilder();

            int nest = 0;
            int lastPos = 0;
            int i = 0;
            while (i < input.Length - 1)
            {
                if (input[i] == '%')
                {
                    if (input[i + 1] == number && nest == 0)
                    {
                        if (i - lastPos > 0)
                            sb.Append(input, lastPos, i - lastPos);

                        sb.Append(replacement);
                        lastPos = i + 2;
                        i++;
                    }
                }
                else if (input[i] == '{')
                {
                    nest++;
                }
                else if (input[i] == '}')
                {
                    nest--;
                }

                i++;
            }

            if (lastPos < input.Length)
                sb.Append(input, lastPos, input.Length - lastPos);

            return sb.ToString();
        }
    }
}
