// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeHelper.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TreeHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A static class to store tree navigating helper methods.
    /// </summary>
    public static class TreeHelper
    {
        /// <summary>
        /// Finds the visual children of specified type.
        /// </summary>
        /// <typeparam name="T">Type of child to look for.</typeparam>
        /// <param name="dependencyObject">The dependency object to scan.</param>
        /// <returns>Found children of specified type.</returns>
        [NotNull]
        public static IEnumerable<T> FindVisualChildren<T>([NotNull] DependencyObject dependencyObject) where T : DependencyObject
        {
            Assert.ArgumentNotNull(dependencyObject, "dependencyObject");
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, i);
                if (child != null && child is T)
                {
                    yield return (T)child;
                }

                if (child == null)
                {
                    continue;
                }

                foreach (T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }
    }
}
