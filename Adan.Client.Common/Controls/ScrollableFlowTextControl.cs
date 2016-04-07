
namespace Adan.Client.Common.Controls
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.TextFormatting;
    using System.Windows.Threading;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Messages;

    using TextFormatting;
    using Settings;
    using Utils;

    #endregion

    /// <summary>
    /// Interaction logic for ScrollableFlowTextControl.xaml
    /// </summary>
    public class ScrollableFlowTextControl : Control, IScrollInfo
    {
        #region Constants and Fields

        private readonly List<TextMessage> _messages = new List<TextMessage>();
        private readonly TextFormatter _formatter = TextFormatter.Create(TextFormattingMode.Ideal);
        private readonly TextSelectionSettings _selectionSettings = new TextSelectionSettings();
        private readonly TextSelectionSettings _tempSelectionSettings = new TextSelectionSettings();
        private readonly MessageTextSource _textSource;
        private readonly CustomTextParagraphProperties _customTextParagraphProperties = new CustomTextParagraphProperties();
        private readonly TextRunCache _textRunCache = new TextRunCache();
        private readonly Stack<TextLine> _linesToRenderStack = new Stack<TextLine>();


        private readonly DispatcherTimer _doubleClickTimer;

        private int _currentLineNumber;
        private int _currentNumberOfLinesInView;
        private double _lineHeight;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollableFlowTextControl"/> class.
        /// </summary>
        public ScrollableFlowTextControl()
        {
            AutoScroll = true;
            ClipToBounds = true;
            _textSource = new MessageTextSource(_selectionSettings);
            _doubleClickTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 150), DispatcherPriority.Background, (o, e) => ClearTextSelection(), Dispatcher.CurrentDispatcher);
            _doubleClickTimer.Stop();

            MaximumLinesToStore = Math.Max(SettingsHolder.Instance.Settings.ScrollBuffer, 100);
            SettingsHolder.Instance.Settings.OnSettingsChanged += HandleSettingsChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the lines overflow percent before cleanup.
        /// </summary>
        /// <value>
        /// The lines overflow percent before cleanup.
        /// </value>
        public int LinesOverflowPercentBeforeCleanup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum lines to store.
        /// </summary>
        /// <value>
        /// The maximum lines to store.
        /// </value>
        public int MaximumLinesToStore
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether content will be automatically scrolled down to last line.
        /// </summary>
        /// <value>
        ///   <c>true</c> if content will be automatically scrolled down to last line; otherwise, <c>false</c>.
        /// </value>
        public bool AutoScroll
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parent scroll info.
        /// </summary>
        /// <value>
        /// The parent scroll info.
        /// </value>
        [CanBeNull]
        public IScrollInfo ParentScrollInfo
        {
            get;
            set;
        }

        #endregion

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
                return _messages.Count;
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
                if (ParentScrollInfo != null && _currentNumberOfLinesInView == 0)
                {
                    return ParentScrollInfo.ViewportHeight;
                }

                return _currentNumberOfLinesInView;
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
                return _currentLineNumber;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="T:System.Windows.Controls.ScrollViewer"/> element that controls scrolling behavior.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.ScrollViewer"/> element that controls scrolling behavior. This property has no default value.
        /// </returns>
        public ScrollViewer ScrollOwner
        {
            get;
            set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the messages.
        /// </summary>
        /// <param name="messages">The messages.</param>
        public void AddMessages([NotNull] IEnumerable<TextMessage> messages)
        {
            Assert.ArgumentNotNull(messages, "messages");
            _messages.AddRange(messages);

            CheckMessagesOverflow();

            if (ScrollOwner != null)
            {
                ScrollOwner.InvalidateScrollInfo();
            }

            if (!AutoScroll)
            {
                return;
            }

            if (_selectionSettings.Dragging)
            {
                ClearTextSelection();
            }

            _currentLineNumber = _messages.Count;

            InvalidateVisual();
        }

        /// <summary>
        /// Scrolls to end.
        /// </summary>
        public void ScrollToEnd()
        {
            _currentLineNumber = _messages.Count;
            InvalidateVisual();
        }

        #endregion

        #region Implementation of IScrollInfo methods

        /// <summary>
        /// Scrolls up within content by one logical unit. 
        /// </summary>
        public void LineUp()
        {
            if (_currentLineNumber <= 0 || _currentLineNumber <= _currentNumberOfLinesInView)
            {
                return;
            }

            _currentLineNumber--;

            if (ScrollOwner != null)
            {
                ScrollOwner.InvalidateScrollInfo();
            }

            InvalidateVisual();
        }

        /// <summary>
        /// Scrolls down within content by one logical unit. 
        /// </summary>
        public void LineDown()
        {
            if (_currentLineNumber >= _messages.Count)
            {
                return;
            }

            _currentLineNumber++;

            if (ScrollOwner != null)
            {
                ScrollOwner.InvalidateScrollInfo();
            }

            InvalidateVisual();
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
            if (_currentNumberOfLinesInView == 0)
            {
                if (ParentScrollInfo != null)
                {
                    _currentLineNumber -= (int)ParentScrollInfo.ViewportHeight;
                }
                else
                {
                    _currentLineNumber -= 10;
                }
            }
            else
            {
                _currentLineNumber -= (_currentNumberOfLinesInView / 2) - 1;
            }

            if (_currentLineNumber == _messages.Count)
            {
                _currentLineNumber--;
            }

            if (_currentLineNumber < _currentNumberOfLinesInView)
            {
                _currentLineNumber = _currentNumberOfLinesInView;
            }

            if (ScrollOwner != null)
            {
                ScrollOwner.InvalidateScrollInfo();
            }

            InvalidateVisual();
        }

        /// <summary>
        /// Scrolls down within content by one page.
        /// </summary>
        public void PageDown()
        {
            if (_currentNumberOfLinesInView == 0)
            {
                return;
            }

            _currentLineNumber += (_currentNumberOfLinesInView / 2) - 1;
            if (_currentLineNumber > _messages.Count)
            {
                _currentLineNumber = _messages.Count;
            }

            if (ScrollOwner != null)
            {
                ScrollOwner.InvalidateScrollInfo();
            }

            InvalidateVisual();
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
            LineUp();
            LineUp();
            LineUp();
        }

        /// <summary>
        /// Scrolls down within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelDown()
        {
            LineDown();
            LineDown();
            LineDown();
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
            _currentLineNumber = (int)Math.Ceiling(offset) + _currentNumberOfLinesInView;
            if (_currentLineNumber < _currentNumberOfLinesInView)
            {
                _currentLineNumber = _currentNumberOfLinesInView;
            }

            if (_currentLineNumber > _messages.Count)
            {
                _currentLineNumber = _messages.Count;
            }

            InvalidateVisual();
        }

        /// <summary>
        /// Forces content to scroll until the coordinate space of a <see cref="T:System.Windows.Media.Visual"/> object is visible.
        /// </summary>
        /// <param name="visual">A <see cref="T:System.Windows.Media.Visual"/> that becomes visible.</param>
        /// <param name="rectangle">A bounding rectangle that identifies the coordinate space to make visible.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Rect"/> that is visible.
        /// </returns>
        public Rect MakeVisible([NotNull] Visual visual, Rect rectangle)
        {
            Assert.ArgumentNotNull(visual, "visual");

            return rectangle;
        }

        #endregion

        #region TextSelecting

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick"/> routed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick([NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            base.OnMouseDoubleClick(e);

            _selectionSettings.SelectedMessages.Clear();

            int lineNumber = 0;
            if (_lineHeight > 0)
            {
                lineNumber = _currentLineNumber - (int)Math.Floor((ActualHeight - e.GetPosition(this).Y) / _lineHeight);
            }

            if (_messages.Count >= lineNumber && lineNumber > 0)
            {
                _selectionSettings.SelectedMessages.Add(_messages[lineNumber - 1]);
                _selectionSettings.SelectionStartCharacterNumber = 0;
                _selectionSettings.SelectionEndCharacterNumber = _messages[lineNumber - 1].InnerText.Length;
                try
                {
                    Clipboard.SetText(_messages[lineNumber - 1].InnerText);
                }
                catch
                {
                }

                _doubleClickTimer.Start();
            }

            InvalidateVisual();
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown([NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            base.OnMouseLeftButtonDown(e);
            _selectionSettings.Dragging = true;
            _selectionSettings.NeedUpdate = true;
            _selectionSettings.SetSelectionStartPosition(e.GetPosition(this));
            CaptureMouse();
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.MouseMove"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove([NotNull] MouseEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            base.OnMouseMove(e);
            if (_selectionSettings.Dragging)
            {
                Mouse.OverrideCursor = Cursors.IBeam;
                _selectionSettings.SetSelectionEndPosition(e.GetPosition(this));
                _selectionSettings.NeedUpdate = true;
                e.Handled = true;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp([NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            base.OnMouseLeftButtonUp(e);

            if (_selectionSettings.Dragging)
            {
                _selectionSettings.Dragging = false;
                CopySelectedMessagesToClipboard();
                ReleaseMouseCapture();
                Mouse.OverrideCursor = null;
                ClearTextSelection();
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.MouseLeave"/> attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave([NotNull] MouseEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            if (!_selectionSettings.Dragging)
            {
                Mouse.OverrideCursor = null;
            }

            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.LostMouseCapture"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains event data.</param>
        protected override void OnLostMouseCapture([NotNull] MouseEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            _selectionSettings.Dragging = false;
            ClearTextSelection();
            base.OnLostMouseCapture(e);
        }

        #endregion

        #region Rendering

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system.
        /// The rendering instructions for this element are not used directly when this method is invoked,
        /// and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender([NotNull] DrawingContext drawingContext)
        {
            Assert.ArgumentNotNull(drawingContext, "drawingContext");

            try
            {
                if (Visibility != Visibility.Visible)
                {
                    return;
                }

                drawingContext.DrawRectangle(Themes.ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(Themes.TextColor.None, true), new Pen(Themes.ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(Themes.TextColor.None, true), 0), new Rect(0, 0, ActualWidth, ActualHeight));
                var renderedLines = 0;
                if (_selectionSettings.NeedUpdate)
                {
                    _tempSelectionSettings.SelectedMessages.Clear();
                }

                if (_messages.Count > 0)
                {
                    var currentHeight = ActualHeight;
                    var lineNumber = _currentLineNumber;
                    while (currentHeight > 0 && lineNumber > 0)
                    {
                        _textSource.Message = _messages[lineNumber - 1];
                        var textStorePosition = 0;

                        _linesToRenderStack.Clear();

                        do
                        {
                            var line = _formatter.FormatLine(_textSource, textStorePosition, ActualWidth, _customTextParagraphProperties, null, _textRunCache);
                            _linesToRenderStack.Push(line);
                            textStorePosition += line.Length;
                        }
                        while (textStorePosition < _messages[lineNumber - 1].InnerText.Length);

                        var drawnChars = 0;
                        while (_linesToRenderStack.Count > 0)
                        {
                            var line = _linesToRenderStack.Pop();
                            line.Draw(drawingContext, new Point(0, currentHeight - line.Height), InvertAxes.None);

                            _lineHeight = line.Height;
                            drawnChars += line.Length;
                            if (_selectionSettings.NeedUpdate)
                            {
                                ProcessLineForSelection(line, _messages[lineNumber - 1], currentHeight, drawnChars);
                            }

                            currentHeight -= line.Height;
                            line.Dispose();
                        }

                        renderedLines++;
                        lineNumber--;
                        _textRunCache.Invalidate();
                    }
                }

                _currentNumberOfLinesInView = renderedLines;
                if (_selectionSettings.NeedUpdate)
                {
                    _selectionSettings.SelectionEndCharacterNumber = _tempSelectionSettings.SelectionEndCharacterNumber;
                    _selectionSettings.SelectionStartCharacterNumber = _tempSelectionSettings.SelectionStartCharacterNumber;
                    _selectionSettings.SelectedMessages.Clear();
                    _selectionSettings.SelectedMessages.AddRange(_tempSelectionSettings.SelectedMessages);
                    _selectionSettings.NeedUpdate = false;
                    InvalidateVisual();
                }
            }
            catch (Exception ex)
            {
                    ErrorLogger.Instance.Write(string.Format("Error rendering text {0}", ex));
            }

        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged"/> event, using the specified information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged([NotNull] SizeChangedInfo sizeInfo)
        {
            Assert.ArgumentNotNull(sizeInfo, "sizeInfo");

            base.OnRenderSizeChanged(sizeInfo);
            if (ScrollOwner != null)
            {
                ScrollOwner.InvalidateScrollInfo();
            }
        }

        #endregion

        #region Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Ok")]
        private void ProcessLineForSelection([NotNull] TextLine line, [NotNull] TextMessage message, double bottomY, int alreadyDrawCharsForMessage)
        {
            Assert.ArgumentNotNull(line, "line");
            Assert.ArgumentNotNull(message, "message");

            bool alreadyAdded = _tempSelectionSettings.SelectedMessages.Contains(message);

            double selectionTop = _selectionSettings.SelectionStartPosition.Y;
            double selectionBottom = _selectionSettings.SelectionEndPosition.Y;
            if (selectionBottom < selectionTop)
            {
                selectionTop = _selectionSettings.SelectionEndPosition.Y;
                selectionBottom = _selectionSettings.SelectionStartPosition.Y;
            }

            double selectionLeft = _selectionSettings.SelectionStartPosition.X;
            double selectionRight = _selectionSettings.SelectionEndPosition.X;
            if (selectionLeft > selectionRight)
            {
                selectionLeft = _selectionSettings.SelectionEndPosition.X;
                selectionRight = _selectionSettings.SelectionStartPosition.X;
            }

            double lineBottom = bottomY;
            double lineTop = bottomY - line.Height;

            if (lineBottom < selectionBottom && lineTop > selectionTop && alreadyDrawCharsForMessage > 0)
            {
                if (!alreadyAdded)
                {
                    _tempSelectionSettings.SelectedMessages.Add(message);
                }
            }
            else if ((selectionBottom > lineTop && selectionBottom < lineBottom) && (selectionTop > lineTop && selectionTop < lineBottom))
            {
                var leftCharIndex = selectionLeft > line.Width ? line.Length : line.GetCharacterHitFromDistance(selectionLeft).FirstCharacterIndex;
                var rightCharIndex = selectionRight > line.Width ? line.Length : line.GetCharacterHitFromDistance(selectionRight).FirstCharacterIndex;
                if (rightCharIndex > leftCharIndex)
                {
                    _tempSelectionSettings.SelectionStartCharacterNumber = leftCharIndex;
                    _tempSelectionSettings.SelectionEndCharacterNumber = rightCharIndex;

                    if (!alreadyAdded)
                    {
                        _tempSelectionSettings.SelectedMessages.Add(message);
                    }
                }
            }
            else if (selectionTop > lineTop && selectionTop < lineBottom)
            {
                double distance = _selectionSettings.SelectionEndPosition.X;
                if (selectionTop == _selectionSettings.SelectionStartPosition.Y)
                {
                    distance = _selectionSettings.SelectionStartPosition.X;
                }

                var charIndex = distance > line.Width ? line.Length : line.GetCharacterHitFromDistance(distance).FirstCharacterIndex;
                _tempSelectionSettings.SelectionStartCharacterNumber = charIndex;

                if (!alreadyAdded)
                {
                    _tempSelectionSettings.SelectedMessages.Add(message);
                }
            }
            else if (selectionBottom > lineTop && selectionBottom < lineBottom)
            {
                double distance = _selectionSettings.SelectionEndPosition.X;
                if (selectionBottom == _selectionSettings.SelectionStartPosition.Y)
                {
                    distance = _selectionSettings.SelectionStartPosition.X;
                }

                var charIndex = distance > line.Width ? line.Length : line.GetCharacterHitFromDistance(distance).FirstCharacterIndex;
                _tempSelectionSettings.SelectionEndCharacterNumber = charIndex;

                if (!alreadyAdded)
                {
                    _tempSelectionSettings.SelectedMessages.Add(message);
                }
            }
            else
            {
                if (alreadyAdded)
                {
                    _tempSelectionSettings.SelectedMessages.Remove(message);
                }
            }
        }

        private void ClearTextSelection()
        {
            _selectionSettings.SelectedMessages.Clear();
            _doubleClickTimer.Stop();
            InvalidateVisual();
        }

        private void CopySelectedMessagesToClipboard()
        {
            string result = string.Empty;
            if (_selectionSettings.SelectedMessages.Count == 1)
            {
                var text = _selectionSettings.SelectedMessages[0].InnerText;
                if (_selectionSettings.SelectionStartCharacterNumber < text.Length)
                {
                    var length = Math.Min(text.Length, _selectionSettings.SelectionEndCharacterNumber);
                    result = text.Substring(_selectionSettings.SelectionStartCharacterNumber, length - _selectionSettings.SelectionStartCharacterNumber);
                }
            }

            if (_selectionSettings.SelectedMessages.Count > 1)
            {
                var text = _selectionSettings.SelectedMessages[_selectionSettings.SelectedMessages.Count - 1].InnerText;
                result = text.Substring(Math.Min(_selectionSettings.SelectionStartCharacterNumber, text.Length));

                for (int i = _selectionSettings.SelectedMessages.Count - 2; i > 0; i--)
                {
                    result += "\r\n";
                    result += _selectionSettings.SelectedMessages[i].InnerText;
                }

                result += "\r\n";
                text = _selectionSettings.SelectedMessages[0].InnerText;
                var length = Math.Min(text.Length, _selectionSettings.SelectionEndCharacterNumber);
                result += text.Substring(0, length);
            }

            try
            {
                Clipboard.SetText(result);
            }
            catch
            {
            }
        }

        private void CheckMessagesOverflow()
        {
            var currentNumberOfMessages = _messages.Count;
            if (currentNumberOfMessages > MaximumLinesToStore * (100 + LinesOverflowPercentBeforeCleanup) / 100.0)
            {
                _messages.RemoveRange(0, currentNumberOfMessages - MaximumLinesToStore);
                _currentLineNumber = _currentLineNumber - (currentNumberOfMessages - _messages.Count);
                if (_currentLineNumber <= 0)
                {
                    _currentLineNumber = 1;
                }

                InvalidateVisual();
                if (ScrollOwner != null)
                {
                    ScrollOwner.InvalidateScrollInfo();
                }
            }
        }

        private void HandleSettingsChanged(object sender, SettingsChangedEventArgs e)
        {
            if (e.Name == "MUDFontName" || e.Name == "MUDFontSize" || e.Name == "ColorTheme")
            {
                _textRunCache.Invalidate();
                InvalidateVisual();
            }

            if (e.Name == "ScrollBuffer")
            {
                MaximumLinesToStore = Math.Max(SettingsHolder.Instance.Settings.ScrollBuffer, 100);
            }
        }

        #endregion
    }
}