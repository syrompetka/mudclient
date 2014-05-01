using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.Common.Model
{
    /// <summary>
    /// Base class for actions that have 1 parameter.
    /// </summary>
    [Serializable]
    public abstract class ActionWithOneParameterOrDefault : ActionWithParameters
    {
        ///// <summary>
        ///// Заменяет %0 переменные на контекст. В случае отсутствия перемнных %0 добавляет в конец дефолт контекст.
        ///// </summary>
        ///// <param name="valueToProcess">The value to process.</param>
        ///// <param name="model">The model.</param>
        ///// <param name="context">The context.</param>
        ///// <returns>
        ///// A result of post processing.
        ///// </returns>
        //[NotNull]
        //protected override string PostProcessString([NotNull] string valueToProcess, [NotNull] RootModel model, [NotNull] ActionExecutionContext context)
        //{
        //    Assert.ArgumentNotNull(valueToProcess, "valueToProcess");
        //    Assert.ArgumentNotNull(model, "model");
        //    Assert.ArgumentNotNull(context, "context");

        //    //bool ret = false;
        //    for (int i = 0; i < 10; i++)
        //    {
        //        if(context.Parameters.ContainsKey(i))
        //            valueToProcess = ReplaceParameters(valueToProcess, i.ToString()[0], context.Parameters[i]);
        //        else
        //            valueToProcess = ReplaceParameters(valueToProcess, i.ToString()[0], string.Empty);
        //    }

        //    //if (context.Parameters.Count > 0)
        //    //{
        //    //    valueToProcess = Regex.Replace(valueToProcess, "%0", 
        //    //        x => 
        //    //        {
        //    //            ret = true; 
        //    //            return context.Parameters[0];
        //    //        });

        //    //    if (!ret)
        //    //        valueToProcess += " " + context.Parameters[0];
        //    //}

        //    return valueToProcess;
        //}
    }
}
