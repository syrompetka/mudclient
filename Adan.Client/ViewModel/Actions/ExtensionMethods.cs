// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ExtensionMethods type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel.Actions
{
    using System;

    using CSLib.Net.Annotations;

    using Model.Actions;

    /// <summary>
    /// Class to hold misc extension methods for parameters.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Convers the action condition to sign.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>Converted string representation of specified action condition.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><c>condition</c> is out of range.</exception>
        [NotNull]
        public static string ConvertToSign(this ActionCondition condition)
        {
            switch (condition)
            {
                case ActionCondition.Equals:
                    return "==";
                case ActionCondition.NotEquals:
                    return "!=";
                case ActionCondition.Greater:
                    return ">";
                case ActionCondition.GreaterOrEquals:
                    return ">=";
                case ActionCondition.Less:
                    return "<";
                case ActionCondition.LessOrEquals:
                    return "<=";
                case ActionCondition.IsEmpty:
                    return "Is Empty";
                case ActionCondition.IsNotEmpty:
                    return "Is Not Empty";
            }

            throw new ArgumentOutOfRangeException("condition", @"Supplied operation is not supported.");
        }

        /// <summary>
        /// Determines whether the specified condition is binary operator or not.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        ///  <c>true</c> if the specified condition is binary operator; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBinaryOperator(this ActionCondition condition)
        {
            switch (condition)
            {
                case ActionCondition.IsEmpty:
                case ActionCondition.IsNotEmpty:
                    return false;
                default:
                    return true;
            }
        }
    }
}
