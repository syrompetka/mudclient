// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemeManager.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ThemeManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Themes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Properties;

    /// <summary>
    /// A singleton class responcible for managing themes.
    /// </summary>
    public sealed class ThemeManager
    {
        private static ThemeManager _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeManager"/> class.
        /// </summary>
        public ThemeManager()
        {
            var themes = new List<ThemeDescription> { new ExpressionDarkThemeDescription() };
            AvailableThemes = themes;
            SwitchToTheme(themes.Where(theme => theme.Name == Settings.Default.SelectedThemeName).Single());
            _instance = this;
        }

        /// <summary>
        /// Gets the instance of <see cref="ThemeManager"/>.
        /// </summary>
        [NotNull]
        public static ThemeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ThemeManager();
                }

                return _instance;
            }
        }
       
        /// <summary>
        /// Gets all available themes.
        /// </summary>
        [NotNull]
        public IEnumerable<ThemeDescription> AvailableThemes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the active theme.
        /// </summary>
        [NotNull]
        public ThemeDescription ActiveTheme
        {
            get;
            private set;
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public static void SaveSettings()
        {
            Settings.Default.Save();
        }

        /// <summary>
        /// Switches to a specified theme.
        /// </summary>
        /// <param name="themeToSwitchTo">The theme to switch to.</param>
        public void SwitchToTheme([NotNull]ThemeDescription themeToSwitchTo)
        {
            Assert.ArgumentNotNull(themeToSwitchTo, "themeToSwitchTo");
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(@"Resources/Icons.xaml", UriKind.Relative) });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(@"Resources/CommonTemplates.xaml", UriKind.Relative) });

            foreach (var dictionary in themeToSwitchTo.DictionariesToMerge)
            {
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(dictionary, UriKind.Relative) });
            }

            ActiveTheme = themeToSwitchTo;
            Settings.Default.SelectedThemeName = themeToSwitchTo.Name;
        }
    }
}
