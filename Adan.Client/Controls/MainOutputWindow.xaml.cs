﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainOutputWindow.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for MainOutputWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Controls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    using Common.Messages;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Messages;

    /// <summary>
    /// Control to contain all text and allow scrolling.
    /// </summary>
    public partial class MainOutputWindow : IScrollInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainOutputWindow"/> class.
        /// </summary>
        public MainOutputWindow()
        {
            InitializeComponent();
        }

        #region Implementation of IScrollInfo properties

        /// <summary>
        /// Gets or sets a value indicating whether scrolling on the vertical axis is possible. 
        /// </summary>
        /// <returns>
        /// true if scrolling is possible; otherwise, false. This property has no default value.
        /// </returns>
        public bool CanVerticallyScroll
        {
            get
            {
                return true;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether scrolling on the horizontal axis is possible.
        /// </summary>
        /// <returns>
        /// true if scrolling is possible; otherwise, false. This property has no default value.
        /// </returns>
        public bool CanHorizontallyScroll
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets the horizontal size of the extent.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Double"/> that represents, in device independent pixels, the horizontal size of the extent. This property has no default value.
        /// </returns>
        public double ExtentWidth
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the vertical size of the extent.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Double"/> that represents, in device independent pixels, the vertical size of the extent.This property has no default value.
        /// </returns>
        public double ExtentHeight
        {
            get
            {
                return secondaryScrollOutput.ExtentHeight;
            }
        }

        /// <summary>
        /// Gets the horizontal size of the viewport for this content.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Double"/> that represents, in device independent pixels, the horizontal size of the viewport for this content. This property has no default value.
        /// </returns>
        public double ViewportWidth
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the vertical size of the viewport for this content.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Double"/> that represents, in device independent pixels, the vertical size of the viewport for this content. This property has no default value.
        /// </returns>
        public double ViewportHeight
        {
            get
            {
                return mainScrollOutput.ViewportHeight + secondaryScrollOutput.ViewportHeight;
            }
        }

        /// <summary>
        /// Gets the horizontal offset of the scrolled content.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Double"/> that represents, in device independent pixels, the horizontal offset. This property has no default value.
        /// </returns>
        public double HorizontalOffset
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the vertical offset of the scrolled content.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Double"/> that represents, in device independent pixels, the vertical offset of the scrolled content. Valid values are between zero and the <see cref="P:System.Windows.Controls.Primitives.IScrollInfo.ExtentHeight"/> minus the <see cref="P:System.Windows.Controls.Primitives.IScrollInfo.ViewportHeight"/>. This property has no default value.
        /// </returns>
        public double VerticalOffset
        {
            get
            {
                return secondaryScrollOutput.VerticalOffset;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="T:System.Windows.Controls.ScrollViewer"/> element that controls scrolling behavior.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.ScrollViewer"/> element that controls scrolling behavior. This property has no default value.
        /// </returns>
        [CanBeNull]
        public ScrollViewer ScrollOwner
        {
            get
            {
                return secondaryScrollOutput.ScrollOwner;
            }

            set
            {
                secondaryScrollOutput.ScrollOwner = value;
                secondaryScrollOutput.ParentScrollInfo = mainScrollOutput;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds the messages.
        /// </summary>
        /// <param name="messages">The messages.</param>
        public void AddMessages([NotNull] IEnumerable<TextMessage> messages)
        {
            Assert.ArgumentNotNull(messages, "messages");
            
            mainScrollOutput.AddMessages(messages);

            secondaryScrollOutput.AddMessages(messages);
        }

        /// <summary>
        /// Scrolls to end.
        /// </summary>
        public void ScrollToEnd()
        {
            secondaryScrollOutput.ScrollToEnd();
            CheckIsScrolling();
        }

        #endregion

        #region Implementation of IScrollInfo methods

        /// <summary>
        /// Scrolls up within content by one logical unit. 
        /// </summary>
        public void LineUp()
        {
            secondaryScrollOutput.LineUp();
            CheckIsScrolling();
        }

        /// <summary>
        /// Scrolls down within content by one logical unit. 
        /// </summary>
        public void LineDown()
        {
            secondaryScrollOutput.LineDown();
            CheckIsScrolling();
        }

        /// <summary>
        /// Scrolls left within content by one logical unit.
        /// </summary>
        public void LineLeft()
        {
        }

        /// <summary>
        /// Scrolls right within content by one logical unit.
        /// </summary>
        public void LineRight()
        {
        }

        /// <summary>
        /// Scrolls up within content by one page.
        /// </summary>
        public void PageUp()
        {
            secondaryScrollOutput.PageUp();
            CheckIsScrolling();
        }

        /// <summary>
        /// Scrolls down within content by one page.
        /// </summary>
        public void PageDown()
        {
            secondaryScrollOutput.PageDown();
            CheckIsScrolling();
        }

        /// <summary>
        /// Scrolls left within content by one page.
        /// </summary>
        public void PageLeft()
        {
        }

        /// <summary>
        /// Scrolls right within content by one page.
        /// </summary>
        public void PageRight()
        {
        }

        /// <summary>
        /// Scrolls up within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelUp()
        {
            secondaryScrollOutput.MouseWheelUp();
            CheckIsScrolling();
        }

        /// <summary>
        /// Scrolls down within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelDown()
        {
            secondaryScrollOutput.MouseWheelDown();
            CheckIsScrolling();
        }

        /// <summary>
        /// Scrolls left within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelLeft()
        {
        }

        /// <summary>
        /// Scrolls right within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelRight()
        {
        }

        /// <summary>
        /// Sets the amount of horizontal offset.
        /// </summary>
        /// <param name="offset">The degree to which content is horizontally offset from the containing viewport.</param>
        public void SetHorizontalOffset(double offset)
        {
        }

        /// <summary>
        /// Sets the amount of vertical offset.
        /// </summary>
        /// <param name="offset">The degree to which content is vertically offset from the containing viewport.</param>
        public void SetVerticalOffset(double offset)
        {
            secondaryScrollOutput.SetVerticalOffset(offset);
            CheckIsScrolling();
        }

        /// <summary>
        /// Forces content to scroll until the coordinate space of a <see cref="T:System.Windows.Media.Visual"/> object is visible. 
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Rect"/> that is visible.
        /// </returns>
        /// <param name="visual">A <see cref="T:System.Windows.Media.Visual"/> that becomes visible.</param><param name="rectangle">A bounding rectangle that identifies the coordinate space to make visible.</param>
        public Rect MakeVisible([NotNull] Visual visual, Rect rectangle)
        {
            Assert.ArgumentNotNull(visual, "visual");

            return secondaryScrollOutput.MakeVisible(visual, rectangle);
        }

        #endregion

        #region Methods

        private void CheckIsScrolling()
        {
            if (secondaryScrollOutput.VerticalOffset < secondaryScrollOutput.ExtentHeight)
            {
                secondaryScrollOutput.AutoScroll = false;
                if (scrollGridRow.Height.Value == 0)
                {
                    scrollGridRow.Height = new GridLength(SettingsHolder.Instance.MainOutputWindowSecondaryScrollHeight);
                    splitterRow.Height = new GridLength(5);
                }
            }
            else
            {
                secondaryScrollOutput.AutoScroll = true;
                if (scrollGridRow.Height.Value > 0)
                {
                    SettingsHolder.Instance.MainOutputWindowSecondaryScrollHeight = (int)scrollGridRow.Height.Value;
                    scrollGridRow.Height = new GridLength(0);
                    splitterRow.Height = new GridLength(0);
                }
            }
        }
        #endregion
    }
}
