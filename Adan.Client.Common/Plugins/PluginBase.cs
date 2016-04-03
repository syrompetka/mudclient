namespace Adan.Client.Common.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Conveyor;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Model;
    using ViewModel;

    /// <summary>
    /// Base class for all plugins.
    /// </summary>
    public abstract class PluginBase : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Gets the widgets of this plugin.
        /// </summary>
        [NotNull]
        public virtual IEnumerable<WidgetDescription> Widgets
        {
            get
            {
                return Enumerable.Empty<WidgetDescription>();
            }
        }

        /// <summary>
        /// Gets the custom action descriptions of this plugin.
        /// </summary>
        [NotNull]
        public virtual IEnumerable<ActionDescription> CustomActions
        {
            get
            {
                return Enumerable.Empty<ActionDescription>();
            }
        }

        /// <summary>
        /// Gets the custom action parameters descriptions of this plugin.
        /// </summary>
        [NotNull]
        public virtual IEnumerable<ParameterDescription> CustomActionParameters
        {
            get
            {
                return Enumerable.Empty<ParameterDescription>();
            }
        }

        /// <summary>
        /// Gets the plugin xaml resources to merge.
        /// </summary>
        [NotNull]
        public virtual IEnumerable<string> PluginXamlResourcesToMerge
        {
            get
            {
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Gets the custom serialization types of this plugin.
        /// </summary>
        [NotNull]
        public virtual IEnumerable<Type> CustomSerializationTypes
        {
            get
            {
                return Enumerable.Empty<Type>();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this plugin has options dialog.
        /// </summary>
        /// <value>
        /// <c>true</c> if this plugin has options dialog; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasOptions
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the options menu item text.
        /// </summary>
        [NotNull]
        public virtual string OptionsMenuItemText
        {
            get
            {
                return string.Empty;
            }
        }

        public abstract void InitializeConveyor(MessageConveyor conveyor);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public virtual void OnCreatedOutputWindow(RootModel rootModel)
        {

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public virtual void OnChangedOutputWindow(RootModel rootModel) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public virtual void OnClosedOutputWindow(RootModel rootModel) { }

        /// <summary>
        /// Initializes this plugins with a specified <see cref="MessageConveyor"/> and <see cref="RootModel"/>.
        /// </summary>
        /// <param name="initializationStatusModel">The initialization status model.</param>
        /// <param name="MainWindowEx">The main window.</param>
        public abstract void Initialize([NotNull] InitializationStatusModel initializationStatusModel, [NotNull] Window MainWindowEx);

        /// <summary>
        /// Shows the options dialog.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        public virtual void ShowOptionsDialog([NotNull]Window parentWindow)
        {
            Assert.ArgumentNotNull(parentWindow, "parentWindow");
        }
    }
}
