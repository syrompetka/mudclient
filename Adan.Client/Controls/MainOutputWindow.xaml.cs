namespace Adan.Client.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    using Common.Messages;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using System.Windows.Input;
    using Common.Settings;
    using Common.Model;
    using Common.Commands;

    /// <summary>
    /// Control to contain all text and allow scrolling.
    /// </summary>
    public partial class MainOutputWindow : IScrollInfo
    {
        private readonly MainWindow _mainWindow;
        private bool _isDisplayingStatusBar;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainOutputWindow"/> class.
        /// </summary>
        public MainOutputWindow(MainWindow mainWindow, RootModel rootModel)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            RootModel = rootModel;
            txtCommandInput.RootModel = rootModel;
            txtCommandInput.GotFocus += HandleGotFocus;
            txtCommandInput.GotKeyboardFocus += HandleGotFocus;
            txtCommandInput.LoadHistory(rootModel.Profile);
            secondaryScrollViewer.ScrollChanged += HandleScrollChanged;

            for (byte i = 1; i < 6; i++)
            {
                string msg = RootModel.GetVariableValue("statusBar"+ i);
                string col = RootModel.GetVariableValue("statusBar" + i + "Col");
                SetStatusBar(i.ToString(), msg, col, false);
            }

            string displayStatusBar = rootModel.GetVariableValue("DisplayStatusBar");
            bool shouldDisplay = false;
            if (displayStatusBar != "")
            {
                bool.TryParse(displayStatusBar, out shouldDisplay);
            }

            DisplayStatusBar(shouldDisplay, false);
        }

        public RootModel RootModel
        {
            get;
            private set;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.PreviewKeyDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown([NotNull] KeyEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            var hotkeyCommand = new HotkeyCommand
            {
                Key = e.Key == Key.System ? e.SystemKey : e.Key,
                ModifierKeys = Keyboard.Modifiers,
                Handled = false,
            };

            // For hotkeys, only Numpad Enter is processed
            var canPush = true;
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
            {
                var isExtended = (bool)typeof(KeyEventArgs).InvokeMember("IsExtendedKey", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, e, null);
                if (!isExtended)
                    canPush = false;
            }

            if (canPush)
                RootModel.PushCommandToConveyor(hotkeyCommand);

            if (hotkeyCommand.Handled)
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Up && Keyboard.Modifiers == 0 && txtCommandInput.IsFocused)
            {
                txtCommandInput.ShowPreviousCommand();
                e.Handled = true;
            }

            if (e.Key == Key.Down && Keyboard.Modifiers == 0 && txtCommandInput.IsFocused)
            {
                txtCommandInput.ShowNextCommand();
                e.Handled = true;
            }

            if (e.Key == Key.Enter && txtCommandInput.IsFocused)
            {
                txtCommandInput.SendCurrentCommand();
                e.Handled = true;
            }

            if (e.Key == Key.PageUp && Keyboard.Modifiers == 0)
            {
                PageUp();
                e.Handled = true;
            }

            if (e.Key == Key.PageDown && Keyboard.Modifiers == 0)
            {
                PageDown();
                e.Handled = true;
            }

            if (e.Key == Key.End && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ScrollToEnd();
                e.Handled = true;
            }

            //Очищение коммандной строки клавишей escape
            if (e.Key == Key.Escape && Keyboard.Modifiers == 0 && txtCommandInput.IsFocused)
            {
                txtCommandInput.Clear();
                e.Handled = true;
            }

            if (!e.Handled)
            {
                base.OnPreviewKeyDown(e);
            }
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

        /// <summary>
        /// Displays/hides status bar.
        /// </summary>
        /// <param name="state"></param>
        public void DisplayStatusBar(bool state, bool verbose)
        {
            _isDisplayingStatusBar = state;

            if (_isDisplayingStatusBar)
            {
                txtCommandInput.Margin = new Thickness(-1, 0, -1, 17);
                StatusBar.Visibility = Visibility.Visible;

                if (verbose)
                {
                    RootModel.PushMessageToConveyor(new InfoMessage("#Status bar displayed."));
                }
                RootModel.SetVariableValue("DisplayStatusBar", _isDisplayingStatusBar.ToString(), true);
            }
            else
            {
                txtCommandInput.Margin = new Thickness(-1, 0, -1, 0);
                StatusBar.Visibility = Visibility.Collapsed;
                StatusBar1.Text = "";
                StatusBar2.Text = "";
                StatusBar3.Text = "";
                StatusBar4.Text = "";
                StatusBar5.Text = "";

                for (byte i = 1; i < 6; i++)
                    RootModel.SetVariableValue("statusBar" + i, "", true);

                if (verbose)
                {
                    RootModel.PushMessageToConveyor(new InfoMessage("#Status bar hidden."));
                }
                RootModel.SetVariableValue("DisplayStatusBar", _isDisplayingStatusBar.ToString(), true);
            }
        }

        /// <summary>
        /// Displays/hides status bar.
        /// </summary>
        public void SetStatusBar(string idString, string message, string colorString, bool verbose = true)
        {
            byte Id;
            if (!byte.TryParse(idString, out Id))
            {
                // should never occur, but just in case
                return;
            }

            // If setting a status, but the status bar is hidden - display it.
            if (!_isDisplayingStatusBar && message != "")
            {
                DisplayStatusBar(true, false);
            }


            TextBlock textBlock;
            if (Id == 1) textBlock = StatusBar1;
            else if (Id == 2) textBlock = StatusBar2;
            else if (Id == 3) textBlock = StatusBar3;
            else if (Id == 4) textBlock = StatusBar4;
            else if (Id == 5) textBlock = StatusBar5;
            else
            {
                // Notify user that id number is wrong.
                // Print explanations about correct usage.
                RootModel.PushMessageToConveyor(new InfoMessage("Incorrect usage of #Status [1|2|3|4|5] {Text} [color].", Common.Themes.TextColor.BrightYellow));
                RootModel.PushMessageToConveyor(new InfoMessage("Error message: Only numbers between 1 and 5 are accepted.", Common.Themes.TextColor.BrightYellow));
                RootModel.PushMessageToConveyor(new InfoMessage("Example: #Status 1 {Hello} green", Common.Themes.TextColor.BrightYellow));
                return;
            }

            textBlock.Text = message;

            if (colorString == "" || colorString.Length < 2)
            {
                textBlock.Foreground = new SolidColorBrush(Color.FromRgb(192, 192, 192)); //#c0c0c0
            }
            else
            {
                if (colorString.StartsWith("#"))
                {
                    if (colorString.Length == 7)
                    {
                        Color color = (Color)ColorConverter.ConvertFromString(colorString);
                        textBlock.Foreground = new SolidColorBrush(color);
                    }
                    else
                    {
                        //notify user that color is wrong
                        // print examples of colors
                        RootModel.PushMessageToConveyor(new InfoMessage("Incorrect usage of #Status [1|2|3|4|5] {Text} [color].", Common.Themes.TextColor.BrightYellow));
                        RootModel.PushMessageToConveyor(new InfoMessage("Error message: Incorrect color. Acceptable formats are: #FFFFFF (hex rgb) or plain color name (e.g. green).", Common.Themes.TextColor.BrightYellow));
                        RootModel.PushMessageToConveyor(new InfoMessage("Example: #Status 1 {Hello} green", Common.Themes.TextColor.BrightYellow));
                        RootModel.PushMessageToConveyor(new InfoMessage("Example: #Status 1 {Hello} #00FF00", Common.Themes.TextColor.BrightYellow));
                        return;
                    }
                }
                else
                {
                    colorString = colorString.ToLower();
                    colorString = colorString[0].ToString().ToUpper() + colorString.Substring(1);
                    var colorProperty = typeof(Colors).GetProperty(colorString);
                    if (colorProperty != null)
                    {
                        Color color = (Color)colorProperty.GetValue(this);
                        textBlock.Foreground = new SolidColorBrush(color);
                    }
                    else
                    {
                        //notify that color is wrong
                        // print examples of colors
                        RootModel.PushMessageToConveyor(new InfoMessage("Incorrect usage of #Status [1|2|3|4|5] {Text} [color].", Common.Themes.TextColor.BrightYellow));
                        RootModel.PushMessageToConveyor(new InfoMessage("Error message: Incorrect color. The color you entered isn't in the color table.", Common.Themes.TextColor.BrightYellow));
                        RootModel.PushMessageToConveyor(new InfoMessage("Example: #Status 1 {Hello} green", Common.Themes.TextColor.BrightYellow));
                        RootModel.PushMessageToConveyor(new InfoMessage("Example: #Status 1 {Hello} #00FF00", Common.Themes.TextColor.BrightYellow));
                        RootModel.PushMessageToConveyor(new InfoMessage("For full list of acceptable colors, please refer to: ", Common.Themes.TextColor.BrightYellow));
                        RootModel.PushMessageToConveyor(new InfoMessage("https://i-msdn.sec.s-msft.com/en-us/library/system.windows.media.colors.43e06ea3-fdb6-448a-bb66-2e032ab1a12a(v=vs.110).jpeg", Common.Themes.TextColor.BrightYellow));
                        return;
                    }
                }
            }

            if (verbose)
            {
                if (message == "")
                    RootModel.PushMessageToConveyor(new InfoMessage("#StatusBar" + Id + " emptied.", Common.Themes.TextColor.BrightWhite));
                else
                    RootModel.PushMessageToConveyor(new InfoMessage("#StatusBar" + Id + ": " + message, Common.Themes.TextColor.BrightWhite));
            }

            RootModel.SetVariableValue("statusBar" + Id, message, true);
            RootModel.SetVariableValue("statusBar" + Id + "Col", colorString, true);

            //// If all statuses are empty, hide the status bar.
            //if (StatusBar1.Text == "" && StatusBar2.Text == "" && StatusBar3.Text == "" && StatusBar4.Text == "" && StatusBar5.Text == "")
            //{
            //    DisplayStatusBar(false, false);
            //}
        }

        #endregion

        #region Methods

        public void SaveCurrentHistory(ProfileHolder profile)
        {
            txtCommandInput.SaveCurrentHistory(profile);
        }

        public void LoadHistory(ProfileHolder profile)
        {
            txtCommandInput.LoadHistory(profile);
        }

        private void CheckIsScrolling()
        {
            if (secondaryScrollOutput.VerticalOffset < secondaryScrollOutput.ExtentHeight)
            {
                secondaryScrollOutput.AutoScroll = false;
                if (scrollGridRow.Height.Value == 0)
                {
                    var scrollHeightPercent = Math.Max(10, Math.Min(80, SettingsHolder.Instance.Settings.MainOutputWindowSecondaryScrollHeight)) / 100.0f;

                    scrollGridRow.Height = new GridLength(ActualHeight * scrollHeightPercent);
                    splitterRow.Height = new GridLength(5);
                }
            }
            else
            {
                secondaryScrollOutput.AutoScroll = true;
                if (scrollGridRow.Height.Value > 0)
                {
                    SettingsHolder.Instance.Settings.MainOutputWindowSecondaryScrollHeight = (int)(scrollGridRow.Height.Value / ActualHeight * 100.0f);
                    scrollGridRow.Height = new GridLength(0);
                    splitterRow.Height = new GridLength(0);
                }
            }
        }

        private void HandleGotFocus(object sender, RoutedEventArgs e)
        {
            PluginHost.Instance.OutputWindowChanged(RootModel);
        }

        #endregion

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                secondaryScrollOutput.MouseWheelUp();
            }

            if (e.Delta < 0)
            {
                secondaryScrollOutput.MouseWheelDown();
            }
            e.Handled = true;
            CheckIsScrolling();
        }

        private void HandleScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            CheckIsScrolling();
        }
    }
}
