// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgetDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the WidgetDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Plugins
{
    using System.Windows;
    using System.Windows.Controls;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Description of a single plugin widget.
    /// </summary>
    public class WidgetDescription
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="control"></param>
        /// <param name="icon"></param>
        public WidgetDescription([NotNull] string name, [NotNull] string description, [NotNull] FrameworkElement control, [NotNull] string icon)
        {
            Assert.ArgumentNotNullOrWhiteSpace(name, "name");
            Assert.ArgumentNotNullOrWhiteSpace(description, "description");
            Assert.ArgumentNotNull(control, "control");
            Assert.ArgumentNotNull(icon, "icon");

            Name = string.Format("Plugin{0}", name);
            Description = description;
            Control = control;
            Icon = icon;
            Height = 300;
            Width = 300;
            Left = 50;
            Top = 50;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="control"></param>
        public WidgetDescription([NotNull] string name, [NotNull] string description, [NotNull] FrameworkElement control)
            : this(name, description, control, string.Empty) { }

        /// <summary>
        /// Gets the name of this widget.
        /// The name must be unique among all widgets of all plugins.
        /// </summary>
        [NotNull]
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the description of this widget.
        /// Will be shown to user as a title of widget window.
        /// </summary>
        [NotNull]
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the control that displays widget data.
        /// </summary>
        [NotNull]
        public FrameworkElement Control
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the icon of this widge.
        /// </summary>
        [NotNull]
        public string Icon
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Height
        {
            get;
            set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public int Width
        {
            get;
            set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public int Left
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Top
        {
            get;
            set;
        }
    }
}
