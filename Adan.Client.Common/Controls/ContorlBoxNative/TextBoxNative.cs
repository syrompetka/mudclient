using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms.Integration;
using System.Windows.Markup;
using System.Windows.Media;
using Adan.Client.Common.Controls.ContorlBoxNative;
using Adan.Client.Common.Messages;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Adan.Client.Common.Controls
{
    /// <summary>
    /// 
    /// </summary>
    /// TODO: TEST TEST TEST!!!
    public class TextBoxNative : Control, IScrollInfo
    {
        #region Constants and Fields

        private readonly WindowsFormsHost _host = new WindowsFormsHost();
        private TextView _view;
        private int _currentLineNumber = 0;
        private int _currentNumberOfLinesInView = 0;
        private List<TextMessage> _messages;

        private bool _isSelected = false;
        private bool _isEndOfScroll = false;
        private int _startSelectLine = 0;
        private int _startSelectIndex = 0;
        private int _endSelectLine = 0;
        private int _endSelectIndex = 0;

        private object _lockObject = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public TextBoxNative()
        {
            this.AddVisualChild(_host);
            this.AddLogicalChild(_host);
            _host.Child = _view = new TextView();
            _messages = new List<TextMessage>();

            DependencyPropertyDescriptor.FromProperty(Control.FontFamilyProperty, typeof(Control)).AddValueChanged(this, TextBoxNative_FontChanged);
            DependencyPropertyDescriptor.FromProperty(Control.FontSizeProperty, typeof(Control)).AddValueChanged(this, TextBoxNative_FontChanged);
            DependencyPropertyDescriptor.FromProperty(Control.FontStyleProperty, typeof(Control)).AddValueChanged(this, TextBoxNative_FontChanged);
            DependencyPropertyDescriptor.FromProperty(Control.FontWeightProperty, typeof(Control)).AddValueChanged(this, TextBoxNative_FontChanged);

            this.TextBoxNative_FontChanged(this, EventArgs.Empty);
            this.SizeChanged += TextBoxNative_SizeChanged;
            this._view.MouseCaptureChanged += _view_LostMouseCapture;
            this._view.MouseDown += _view_MouseDown;
            this._view.MouseMove += _view_MouseMove;
            this._view.MouseUp += _view_MouseUp;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public TextView TextViewNative
        {
            get
            {
                return _view;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<TextMessage> Messages
        {
            get
            {
                return _messages.AsReadOnly();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public void AddMessage(IList<TextMessage> messages)
        {
            lock (_lockObject)
            {
                _messages.AddRange(messages);
                if (_messages.Count >= _currentNumberOfLinesInView &&
                    ((_messages.Count - messages.Count) - _currentNumberOfLinesInView) <= 0 ||
                    (_currentLineNumber == (_messages.Count - messages.Count) - _currentNumberOfLinesInView))
                {
                    this.ScrollToEnd();
                }
            }

            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();

            this.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public void ChangeMessageList(List<TextMessage> list)
        {
            lock (_lockObject)
            {
                _messages = list;
                this.ScrollToEnd();
            }

            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();

            this.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void RemoveMessage(int offset, int count)
        {
            lock (_lockObject)
            {
                _messages.RemoveRange(offset, count);
                if (_messages.Count > _currentNumberOfLinesInView && _currentLineNumber > _messages.Count - _currentNumberOfLinesInView)
                    this.ScrollToEnd();
            }

            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();

            this.Refresh();
        }

        #endregion

        #region Selection
        
        private void _view_LostMouseCapture(object sender, EventArgs e)
        {

        }

        private void _view_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _isSelected = true;

                if (_messages.Count <= this.ViewportHeight || _currentLineNumber == this.ExtentHeight - this.ViewportHeight)
                    _isEndOfScroll = true;

                _startSelectLine = getLineFromCoords(e.Y);
                _startSelectIndex = e.X;
                _endSelectLine = getLineFromCoords(e.Y);
                _endSelectIndex = e.X;

                this.Refresh();
            }
        }

        private void _view_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (_isSelected)
                {
                    _isSelected = false;
                    _isEndOfScroll = false;

                    if (_isEndOfScroll)
                        this.ScrollToEnd();

                    if (this.ScrollOwner != null)
                        this.ScrollOwner.InvalidateScrollInfo();

                    this.Refresh();
                }
            }
        }

        private void _view_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(_isSelected)
            {
                var line = getLineFromCoords(e.Y);
                if (_endSelectLine == line && _endSelectIndex == e.X)
                    return;

                _endSelectLine = line;
                _endSelectIndex = e.X;

                this.Refresh();
            }
        }

        private int getLineFromCoords(int y)
        {
            return _currentLineNumber + (int)Math.Ceiling(y / (this.FontSize * 3 / 4));
        }

        #endregion

        #region ScrollInfo

        /// <summary>
        /// 
        /// </summary>
        public bool CanHorizontallyScroll
        {
            get
            {
                return false;
            }
            set { }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanVerticallyScroll
        {
            get
            {
                return true;
            }
            set { }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ExtentHeight
        {
            get
            {
                return Math.Max(_messages.Count, _currentNumberOfLinesInView);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ExtentWidth
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ScrollViewer ScrollOwner
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public double VerticalOffset
        {
            get
            {
                return _currentLineNumber;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double HorizontalOffset
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ViewportHeight
        {
            get
            {
                return _currentNumberOfLinesInView;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ViewportWidth
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void LineLeft() { }

        /// <summary>
        /// 
        /// </summary>
        public void LineRight() { }

        /// <summary>
        /// 
        /// </summary>
        public void LineUp()
        {
            this.SetVerticalOffset(_currentLineNumber - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        public void LineDown()
        {
            this.SetVerticalOffset(_currentLineNumber + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        public void MouseWheelLeft() { }

        /// <summary>
        /// 
        /// </summary>
        public void MouseWheelRight() { }

        /// <summary>
        /// 
        /// </summary>
        public void MouseWheelDown()
        {
            this.SetVerticalOffset(_currentLineNumber + 3);
        }

        /// <summary>
        /// 
        /// </summary>
        public void MouseWheelUp()
        {
            this.SetVerticalOffset(_currentLineNumber - 3);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PageDown()
        {
            this.SetVerticalOffset(_currentLineNumber + 3);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PageUp()
        {
            this.SetVerticalOffset(_currentLineNumber - 3);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PageLeft() { }

        /// <summary>
        /// 
        /// </summary>
        public void PageRight() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        public void SetHorizontalOffset(double offset) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        public void SetVerticalOffset(double offset)
        {
            int off = (int)offset;

            if (_messages.Count <= _currentNumberOfLinesInView || off <= 0)
                _currentLineNumber = 0;
            else if (off >= this.ExtentHeight - this.ViewportHeight)
                this.ScrollToEnd();
            else
                _currentLineNumber = off;

            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();

            this.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ScrollToEnd()
        {
            _currentLineNumber = (int)(this.ExtentHeight - this.ViewportHeight);
        }

        #endregion

        #region Control Interface

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return rectangle;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            return _host;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            _host.Measure(availableSize);
            return _host.DesiredSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            _host.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {
            lock (_lockObject)
            {
                var num = Math.Min(_messages.Count, _currentNumberOfLinesInView);
                if (num > 0)
                    _view.Messages = _messages.GetRange(_currentLineNumber, num);
                else
                    _view.Messages = new List<TextMessage>();
            }

            if (_isSelected)
            {

            }

            _host.Child.Refresh();
        }

        #endregion

        private void TextBoxNative_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newCurrentNumberLines = (int)Math.Ceiling(e.NewSize.Height / _view.Font.Height);

            if (newCurrentNumberLines == _currentNumberOfLinesInView)
                return;

            lock (_lockObject)
            {
                if (_messages.Count < newCurrentNumberLines)
                {
                    _currentLineNumber = 0;
                }
                else if (_messages.Count > 0)
                {
                    if (_messages.Count <= _currentNumberOfLinesInView)
                        _currentLineNumber = _messages.Count - newCurrentNumberLines;
                    else if (_currentLineNumber == _messages.Count - _currentNumberOfLinesInView)
                    {
                        _currentLineNumber = _messages.Count - newCurrentNumberLines;
                    }
                    else if (newCurrentNumberLines >= _messages.Count - _currentLineNumber)
                    {
                        _currentLineNumber = _messages.Count - newCurrentNumberLines;
                    }
                }
            }

            _currentNumberOfLinesInView = newCurrentNumberLines;

            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();

            this.Refresh();
        }

        private void TextBoxNative_FontChanged(object sender, EventArgs e)
        {
            var fontStyle = System.Drawing.FontStyle.Regular;

            if (this.FontStyle == FontStyles.Italic)
                fontStyle |= System.Drawing.FontStyle.Italic;

            if (this.FontWeight == FontWeights.Bold)
                fontStyle |= System.Drawing.FontStyle.Bold;

            var font = new System.Drawing.Font(new System.Drawing.FontFamily(this.FontFamily.Source), (float)(this.FontSize * 3 / 4), fontStyle);
            _currentNumberOfLinesInView = (int)Math.Ceiling(this.ActualHeight / font.Height);
            _view.Font = font;

            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();

            this.Refresh();
        }
    }
}