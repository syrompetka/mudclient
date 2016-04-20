using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms.Integration;
using System.Windows.Markup;
using System.Windows.Media;
using Adan.Client.Common.Controls.ContorlBoxNative;
using Adan.Client.Common.Messages;

namespace Adan.Client.Common.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class TextBoxNative : Control, IScrollInfo
    {
        private readonly WindowsFormsHost _host = new WindowsFormsHost();
        private TextView _view;
        private int _currentLineNumber = 0;
        private int _currentNumberOfLinesInView = 0;

        /// <summary>
        /// 
        /// </summary>
        public TextBoxNative()
        {
            this.AddVisualChild(_host);
            this.AddLogicalChild(_host);
            _host.Child = _view = new TextView(this);
            Messages = new List<TextMessage>();

            DependencyPropertyDescriptor.FromProperty(Control.FontFamilyProperty, typeof(Control)).AddValueChanged(this, TextBoxNative_FontChanged);
            DependencyPropertyDescriptor.FromProperty(Control.FontSizeProperty, typeof(Control)).AddValueChanged(this, TextBoxNative_FontChanged);
            DependencyPropertyDescriptor.FromProperty(Control.FontStyleProperty, typeof(Control)).AddValueChanged(this, TextBoxNative_FontChanged);
            DependencyPropertyDescriptor.FromProperty(Control.FontWeightProperty, typeof(Control)).AddValueChanged(this, TextBoxNative_FontChanged);

            this.SizeChanged += TextBoxNative_SizeChanged;
        }

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
        public List<TextMessage> Messages
        {
            get;
            set;
        }

        private void TextBoxNative_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newCurrentNumberLines = (int)(e.NewSize.Height / _view.Font.Height);

            if (Messages.Count < newCurrentNumberLines)
                _currentLineNumber = 0;
            else if (Messages.Count > 0)
            {
                if(newCurrentNumberLines > _currentNumberOfLinesInView && _currentLineNumber > Math.Min(Messages.Count, Messages.Count - newCurrentNumberLines))
                    _currentLineNumber = Math.Min(Messages.Count, Messages.Count - newCurrentNumberLines);
                else if(newCurrentNumberLines < _currentNumberOfLinesInView
                    && (Messages.Count < _currentNumberOfLinesInView || _currentLineNumber == Messages.Count - _currentNumberOfLinesInView))
                    _currentLineNumber = Math.Min(Messages.Count, Messages.Count - newCurrentNumberLines);
            }

            _currentNumberOfLinesInView = newCurrentNumberLines;

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
            _currentNumberOfLinesInView = (int)(this.ActualHeight / font.Height);
            _view.Font = font;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public void AddMessage(IList<TextMessage> messages)
        {
            Messages.AddRange(messages);

            if (Messages.Count == _currentLineNumber + _currentNumberOfLinesInView + messages.Count
                || (Messages.Count - messages.Count < _currentNumberOfLinesInView && Messages.Count > _currentNumberOfLinesInView))
            {
                _currentLineNumber = Messages.Count - _currentNumberOfLinesInView;
            }

            this.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanHorizontallyScroll
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanVerticallyScroll
        {
            get { return true; }
            set { }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public double ExtentHeight
        {
            get { return Messages.Count > _currentNumberOfLinesInView ? Messages.Count : 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ExtentWidth
        {
            get { return 0; }
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
            get { return _currentLineNumber; }
        }

        /// <summary>
        /// 
        /// </summary>
        public double HorizontalOffset
        {
            get { return 0; }      
        }

        /// <summary>
        /// 
        /// </summary>
        public double ViewportHeight
        {
            get { return _currentNumberOfLinesInView; }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ViewportWidth
        {
            get { return 0; }
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
            if (_currentLineNumber <= 0)
                return;

            _currentLineNumber--;
            this.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LineDown()
        {
            if (_currentLineNumber >= Messages.Count - _currentNumberOfLinesInView)
                return;

            _currentLineNumber++;
            this.Refresh();
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
            if (Messages.Count - (_currentLineNumber+_currentNumberOfLinesInView) <= 0)
                return;

            _currentLineNumber = Math.Min(_currentLineNumber + 3, Messages.Count - _currentNumberOfLinesInView);
            this.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        public void MouseWheelUp()
        {
            if (_currentLineNumber <= 0)
                return;

            _currentLineNumber = Math.Max(0, _currentLineNumber - 3);
            this.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        public void PageDown()
        {
            if (Messages.Count - (_currentLineNumber + _currentNumberOfLinesInView) <= 0)
                return;

            _currentLineNumber = Math.Min(_currentLineNumber + _currentNumberOfLinesInView, Messages.Count - _currentNumberOfLinesInView);
            this.Refresh();
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
        public void PageUp()
        {
            if (_currentLineNumber <= 0)
                return;

            _currentLineNumber = Math.Max(0, _currentLineNumber - _currentNumberOfLinesInView);
            this.Refresh();
        }
        

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
            offset = Math.Max(0, Math.Min(Math.Ceiling(offset), ExtentHeight - ViewportHeight));

            if(offset != _currentLineNumber)
            {
                _currentLineNumber = (int)offset;
                this.Refresh();
            }
        }

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
            if(ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();

            _host.Child.Refresh();
        }
    }
}
