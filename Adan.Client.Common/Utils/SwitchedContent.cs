// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchedContent.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   An element whose content changes depending on a set of conditions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Markup;

    using CSLib.Net.Annotations;

    /// <summary>
    /// An element whose content changes depending on a set of conditions.
    /// </summary>
    [ContentProperty("Cases")]
    [TemplatePart(Name = "Content", Type = typeof(ContentPresenter))]
    public class SwitchedContent : Control
    {
        private readonly SwitchConverter _converter;
        private ContentPresenter _content;
        private Binding _binding;

        /// <summary>
        /// Initializes static members of the <see cref="SwitchedContent"/> class. 
        /// </summary>
        static SwitchedContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchedContent), new FrameworkPropertyMetadata(typeof(SwitchedContent)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchedContent"/> class.
        /// </summary>
        public SwitchedContent()
        {
            _converter = new SwitchConverter();
        }

        /// <summary>
        /// Gets or sets the binding for the content property.
        /// </summary>
        [CanBeNull]
        public Binding Binding
        {
            get
            {
                return _binding;
            }

            set
            {
                if (value == _binding)
                {
                    return;
                }

                _binding = value;

                if (_binding != null)
                {
                    _binding.Converter = _converter;
                }

                UpdateBindings();
            }
        }

        /// <summary>
        /// Gets a collection of switch cases that determine the content used.
        /// </summary>
        [NotNull]
        public SwitchCaseCollection Cases
        {
            get
            {
                return _converter.Cases;
            }
        }

        /// <summary>
        /// Gets or sets the value to use when none of the cases match.
        /// </summary>
        [CanBeNull]
        public object Else
        {
            get
            {
                return _converter.Else;
            }

            set
            {
                _converter.Else = value;
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call 
        /// <see cref="FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _content = GetTemplateChild("Content") as ContentPresenter;

            UpdateBindings();
        }

        /// <summary>
        /// Binds the ContentPresenter's Content property to the Binding set on this control.
        /// </summary>
        private void UpdateBindings()
        {
            if (_content == null)
            {
                return;
            }

            if (_binding != null)
            {
                _content.SetBinding(ContentPresenter.ContentProperty, _binding);
            }
            else
            {
                _content.ClearValue(ContentPresenter.ContentProperty);
            }
        }
    } 
}
