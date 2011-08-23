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
    using System.Windows.Controls;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Description of a single plugin widget.
    /// </summary>
    public class WidgetDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetDescription"/> class.
        /// </summary>
        /// <param name="name">The name of the widget.</param>
        /// <param name="description">The description of the widget.</param>
        /// <param name="control">The control that displays data.</param>
        public WidgetDescription([NotNull] string name, [NotNull] string description, [NotNull] Control control)
        {
            Assert.ArgumentNotNullOrWhiteSpace(name, "name");
            Assert.ArgumentNotNullOrWhiteSpace(description, "description");
            Assert.ArgumentNotNull(control, "control");
            Name = name;
            Description = description;
            Control = control;
        }

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
        public Control Control
        {
            get;
            private set;
        }
    }
}
