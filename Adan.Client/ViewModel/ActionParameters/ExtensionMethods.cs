// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ExtensionMethods type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel.ActionParameters
{
    using System;

    using CSLib.Net.Annotations;

    using Model.ActionParameters;

    /// <summary>
    /// Class to hold misc extension methods for parameters.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Converts the operation type to sign.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns>Converted string representation of specified operation type.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><c>operation</c> is out of range.</exception>
        [NotNull]
        public static string ConvertOperationTypeToSign(this MathOperation operation)
        {
            switch (operation)
            {
                case MathOperation.Plus:
                    return "+";
                case MathOperation.Minus:
                    return "-";
                case MathOperation.Division:
                    return "/";
                case MathOperation.Multiplication:
                    return "*";
            }

            throw new ArgumentOutOfRangeException("operation", @"Supplied operation is not supported.");
        }
    }
}
