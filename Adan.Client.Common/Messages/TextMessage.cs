// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TextMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Themes;

    /// <summary>
    /// Plain text message.
    /// </summary>
    public abstract class TextMessage : Message
    {
        private bool _isInnerTextComputed;
        private string _innerText;
        private string _coloredText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessage"/> class.
        /// </summary>
        /// <param name="originalMessage">The original message.</param>
        protected TextMessage([NotNull] TextMessage originalMessage)
        {
            Assert.ArgumentNotNull(originalMessage, "originalMessage");

            this.SkipSubstitution = originalMessage.SkipSubstitution;
            this.SkipTriggers = originalMessage.SkipTriggers;

            if (originalMessage._isInnerTextComputed)
            {
                _isInnerTextComputed = true;
                _innerText = originalMessage.InnerText;
            }

            _coloredText = originalMessage.ColoredText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isColored"></param>
        protected TextMessage([NotNull] string text, bool isColored)
        {
            Assert.ArgumentNotNull(text, "text");

            if (isColored)
                _coloredText = text;
            else
            {
                _innerText = text;
                _coloredText = text;
            }

            _isInnerTextComputed = !isColored;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="foregroundColor">Color of the foreground.</param>
        protected TextMessage([NotNull] string text, TextColor foregroundColor)
        {
            Assert.ArgumentNotNull(text, "text");

            this.AddText(text, foregroundColor);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="foregroundColor">Color of the foreground.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        protected TextMessage([NotNull] string text, TextColor foregroundColor, TextColor backgroundColor)
        {
            Assert.ArgumentNotNull(text, "text");

            this.AddText(text, foregroundColor, backgroundColor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void HighlightColoredText(TextColor foreground, TextColor background, int offset, int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException();

            StringBuilder sb = new StringBuilder(_coloredText.Length + 20);
            int coloredPosition = offset;

            if (coloredPosition > 0)
                sb.Append(_coloredText, 0, coloredPosition);

            sb.Append("\x1B[2;");

            if (foreground == TextColor.None && background == TextColor.None)
            {
                sb.Append("0");
            }
            else
            {
                if (foreground != TextColor.None)
                {
                    sb.Append(ConvertTextColorToAnsi(foreground, false));
                    sb.Append(';');
                }
                if (background != TextColor.None)
                    sb.Append(ConvertTextColorToAnsi(background, true));
            }

            sb.Append('m');

            sb.Append(_coloredText, coloredPosition, length);
            sb.Append("\x1B[3m");

            int endPosition = coloredPosition + length;
            if (endPosition < _coloredText.Length)
                sb.Append(_coloredText, endPosition, _coloredText.Length - endPosition);

            _coloredText = sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void HighlightInnerText(TextColor foreground, TextColor background, int offset, int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException();

            StringBuilder sb = new StringBuilder(_coloredText.Length + 20);
            int coloredPosition = 0;

            if (offset > 0)
            {
                int count = 0;
                while (count < offset)
                {
                    if (_coloredText[coloredPosition] == '\x1B')
                    {
                        coloredPosition += 2;
                        var tt = _coloredText[coloredPosition];

                        while (coloredPosition < _coloredText.Length && _coloredText[coloredPosition] != 'm')
                            coloredPosition++;

                        if (coloredPosition == _coloredText.Length)
                            break;

                        coloredPosition++;
                    }
                    else
                    {
                        count++;
                        coloredPosition++;
                    }
                }
            }

            while (_coloredText[coloredPosition] == '\x1B')
            {
                coloredPosition += 2;
                var tt = _coloredText[coloredPosition];

                while (coloredPosition < _coloredText.Length && _coloredText[coloredPosition] != 'm')
                    coloredPosition++;

                coloredPosition++;
            }

            if (coloredPosition > 0)
                sb.Append(_coloredText, 0, coloredPosition);

            sb.Append("\x1B[2;");

            if (foreground == TextColor.None && background == TextColor.None)
            {
                sb.Append("0");
            }
            else
            {
                if (foreground != TextColor.None)
                {
                    sb.Append(ConvertTextColorToAnsi(foreground, false));
                    sb.Append(';');
                }
                if (background != TextColor.None)
                    sb.Append(ConvertTextColorToAnsi(background, true));
            }

            sb.Append('m');

            int startPosition = coloredPosition;

            {
                int count = 0;
                while (count < length)                    
                {
                    if (_coloredText[coloredPosition] == '\x1B')
                    {
                        coloredPosition += 2;
                        var tt = _coloredText[coloredPosition];

                        while (coloredPosition < _coloredText.Length && _coloredText[coloredPosition] != 'm')
                            coloredPosition++;

                        if (coloredPosition == _coloredText.Length)
                            break;

                        coloredPosition++;
                    }
                    else
                    {
                        count++;
                        coloredPosition++;
                    }
                }
            }

            sb.Append(_coloredText, startPosition, coloredPosition - startPosition);
            sb.Append("\x1B[3m");

            if (coloredPosition < _coloredText.Length)
                sb.Append(_coloredText, coloredPosition, _coloredText.Length - coloredPosition);

            _coloredText = sb.ToString();
        }

        private string ConvertTextColorToAnsi(TextColor color, bool isBackground)
        {
            StringBuilder sb = new StringBuilder(4);

            switch(color)
            {
                case TextColor.White:
                    sb.Append("0;");
                    sb.Append((7 + (isBackground ? 40 : 30)).ToString());
                    break;
                case TextColor.BrightWhite:
                    sb.Append("1;");
                    sb.Append((7 + (isBackground ? 40 : 30)).ToString());
                    break;
                case TextColor.Black:
                    sb.Append("0;");
                    sb.Append(isBackground ? "40" : "30");
                    break;
                case TextColor.BrightBlack:                    
                    sb.Append("1;");
                    sb.Append(isBackground ? "40" : "30");
                    break;
                case TextColor.Red:
                    sb.Append("0;");
                    sb.Append((1 + (isBackground ? 40 : 30)).ToString());
                    break;
                case TextColor.BrightRed:
                    sb.Append("1;");
                    sb.Append((1 + (isBackground ? 40 : 30)).ToString());
                    break;
                case TextColor.Green:
                    sb.Append("0;");
                    sb.Append((2 + (isBackground ? 40 : 30)).ToString());
                    break;
                case TextColor.BrightGreen:
                    sb.Append("1;");
                    sb.Append((2 + (isBackground ? 40 : 30)).ToString());
                    break;
                case TextColor.Yellow:
                    sb.Append("0;");
                    sb.Append((3 + (isBackground ? 40 : 30)).ToString());
                    break;
                case TextColor.BrightYellow:
                    sb.Append("1;");
                    sb.Append((3 + (isBackground ? 40 : 30)).ToString());
                    break;
                case TextColor.Blue:
                    sb.Append("0;");
                    sb.Append((4 + (isBackground ? 40 : 30)).ToString());
                    break;
                case TextColor.BrightBlue:
                    sb.Append("0;");
                    sb.Append((4 + (isBackground ? 40 : 30)).ToString());
                    break;
                case TextColor.Magenta:
                    sb.Append("0;");
                    sb.Append((5 + (isBackground ? 40 : 30)).ToString());
                    break;
                case TextColor.BrightMagenta:
                    sb.Append("1;");
                    sb.Append((5 + (isBackground ? 40 : 30)).ToString());
                    break;
                case TextColor.Cyan:
                    sb.Append("0;");
                    sb.Append((6 + (isBackground ? 40 : 30)).ToString());
                    break;
                case TextColor.BrightCyan:
                    sb.Append("1;");
                    sb.Append((6 + (isBackground ? 40 : 30)).ToString());
                    break;
                case TextColor.RepeatCommandTextColor:
                    sb.Append("4");
                    break;
                case TextColor.None:
                default:
                    sb.Append("0");
                    break;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the inner text of this message.
        /// </summary>
        [NotNull]
        public string InnerText
        {
            get
            {
                if (!_isInnerTextComputed)
                {
                    int offset = 0;
                    int startIndex = 0;
                    StringBuilder sb = new StringBuilder();

                    while (offset < _coloredText.Length)
                    {
                        if (_coloredText[offset] == '\x1B')
                        {
                            if (offset > 0)
                                sb.Append(_coloredText.Substring(startIndex, offset - startIndex));

                            offset += 2;

                            while (offset < _coloredText.Length && _coloredText[offset] != 'm')
                                ++offset;

                            if (offset == _coloredText.Length)
                                break;

                            startIndex = offset + 1;
                        }

                        ++offset;
                    }

                    if (startIndex != _coloredText.Length)
                        sb.Append(_coloredText.Substring(startIndex, offset - startIndex));

                    _innerText = sb.ToString();

                    _isInnerTextComputed = true;
                }
                
                return _innerText;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public string ColoredText
        {
            get
            {
                return _coloredText;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void AddText(string text)
        {
            StringBuilder sb = new StringBuilder(_coloredText);
            sb.Append(text);
            _coloredText = sb.ToString();
            _isInnerTextComputed = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="foreground"></param>
        public void AddText(string text, TextColor foreground)
        {
            int offset = _coloredText.Length;
            StringBuilder sb = new StringBuilder(_coloredText);
            sb.Append(text);
            _coloredText = sb.ToString();
            _isInnerTextComputed = false;
            if (!String.IsNullOrEmpty(_coloredText))
            {
                this.HighlightColoredText(foreground, TextColor.None, offset, _coloredText.Length - offset);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        public void AddText(string text, TextColor foreground, TextColor background)
        {
            int offset = _coloredText.Length;
            StringBuilder sb = new StringBuilder(_coloredText);
            sb.Append(text);
            _coloredText = sb.ToString();
            _isInnerTextComputed = false;
            if (!String.IsNullOrEmpty(_coloredText))
            {
                this.HighlightColoredText(foreground, background, offset, _coloredText.Length - offset);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _coloredText = string.Empty;
            _isInnerTextComputed = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract TextMessage NewInstance();
    }
}
