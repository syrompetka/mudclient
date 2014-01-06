﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using Adan.Client.Common.Controls.AvalonEdit.VisualLineElements;
using Adan.Client.Common.Themes;
using Adan.Client.Messages;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace Adan.Client.Common.Controls.AvalonEdit
{
    /// <summary>
    /// 
    /// </summary>
    public class AdanElementGenerator : VisualLineElementGenerator
    {
        private const int _asciBackGroundCodeBase = 40;
        private const int _asciForeGroundCodeBase = 30;

#if DEBUG

        Stopwatch sw = new Stopwatch();
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler RenderTimeChanged;

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan RenderTime
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void StartGeneration(ITextRunConstructionContext context)
        {
            sw.Reset();
            base.StartGeneration(context);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void FinishGeneration()
        {
            base.FinishGeneration();
            sw.Stop();
            if (RenderTimeChanged != null)
            {
                RenderTime += sw.Elapsed;
                RenderTimeChanged(this, EventArgs.Empty);
            }
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public override VisualLineElement ConstructElement(int offset)
        {
            int curOffset = offset;
            int endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
            var document = CurrentContext.Document;
            bool isHighlight = false;

            if (document.TextLength == 0)
                return null;

            TextColor currentBackColor = TextColor.None;
            TextColor currentForeColor = TextColor.None;
            bool isBright = false;

            if (document.GetCharAt(curOffset) == '\x1B')
            {
                curOffset += 2;

                while (curOffset < endOffset && document.GetCharAt(curOffset) != 'm')
                {
                    int startOffset = curOffset;
                    while (char.IsDigit(document.GetCharAt(curOffset)))
                        curOffset++;

                    if (curOffset == startOffset)
                    {
                        curOffset++;
                        currentBackColor = TextColor.None;
                        currentForeColor = TextColor.None;
                    }
                    else
                    {
                        int value = int.Parse(document.GetText(startOffset, curOffset - startOffset));
                        if (value == 0)
                        {
                            isBright = false;
                        }
                        else if (value == 1)
                        {
                            isBright = true;
                        }
                        //Недокументированный код для хайлайта
                        else if (value == 2)
                        {
                            isHighlight = true;
                        }
                        //Недокументированный код обозначающий конец хайлайта
                        else if (value == 3)
                        {
                            return new AdanStopHighlightVisualElement(4);
                        }
                        //Недокументированный код для TextColor.RepeatCommand
                        else if (value == 4)
                        {
                            currentForeColor = TextColor.RepeatCommandTextColor;
                            currentBackColor = TextColor.None;
                        }
                        else if (value >= _asciBackGroundCodeBase
                                 && value <= _asciBackGroundCodeBase + (int)AnsiColor.White)
                        {
                            currentBackColor = ConvertAnsiColorToTextColor((AnsiColor)(value - _asciBackGroundCodeBase), isBright);
                        }
                        else if (value >= _asciForeGroundCodeBase
                                 && value <= _asciForeGroundCodeBase + (int)AnsiColor.White)
                        {
                            currentForeColor = ConvertAnsiColorToTextColor((AnsiColor)(value - _asciForeGroundCodeBase), isBright);
                        }
                        else
                        {
                            return null;
                        }

                        if (document.GetCharAt(curOffset) == ';')
                            curOffset++;
                    }
                }

                if (curOffset < endOffset)
                    curOffset++;

                if (isHighlight)
                    return new AdanStartHighlightVisualElement(currentForeColor, currentBackColor, curOffset - offset);

                if (currentBackColor == TextColor.None && currentForeColor == TextColor.None)
                    return new AdanResetVisualLineElement(curOffset - offset);
                else
                    return new AdanColorVisualLineElement(currentForeColor, currentBackColor, curOffset - offset);
            }
            else if (offset == CurrentContext.VisualLine.FirstDocumentLine.Offset && offset > 0)
            {
                while (curOffset >= 0)
                {
                    if (document.GetCharAt(curOffset) == '\x1B')
                    {
                        var newOffset = curOffset;
                        newOffset += 2;

                        while (newOffset < endOffset && document.GetCharAt(newOffset) != 'm')
                        {
                            int startOffset = newOffset;
                            while (char.IsDigit(document.GetCharAt(newOffset)))
                                newOffset++;

                            if (newOffset == startOffset)
                            {
                                newOffset++;
                                currentBackColor = TextColor.None;
                                currentForeColor = TextColor.None;
                            }
                            else
                            {
                                int value = int.Parse(document.GetText(startOffset, newOffset - startOffset));
                                if (value == 0)
                                {
                                    isBright = false;
                                }
                                else if (value == 1)
                                {
                                    isBright = true;
                                }
                                //Недокументированный код для хайлайта
                                else if (value == 2)
                                {
                                    goto label;
                                }
                                //Недокументированный код обозначающий конец хайлайта
                                else if (value == 3)
                                {
                                    goto label;
                                }
                                //Недокументированный код для TextColor.RepeatCommand
                                else if (value == 4)
                                {
                                    goto label;
                                }
                                else if (value >= _asciBackGroundCodeBase
                                         && value <= _asciBackGroundCodeBase + (int)AnsiColor.White)
                                {
                                    currentBackColor = ConvertAnsiColorToTextColor((AnsiColor)(value - _asciBackGroundCodeBase), isBright);
                                }
                                else if (value >= _asciForeGroundCodeBase
                                         && value <= _asciForeGroundCodeBase + (int)AnsiColor.White)
                                {
                                    currentForeColor = ConvertAnsiColorToTextColor((AnsiColor)(value - _asciForeGroundCodeBase), isBright);
                                }
                                else
                                {
                                    return null;
                                }

                                if (document.GetCharAt(newOffset) == ';')
                                    newOffset++;
                            }
                        }

                        if (currentBackColor == TextColor.None && currentForeColor == TextColor.None)
                            return new AdanResetVisualLineElement(0);
                        else
                            return new AdanColorVisualLineElement(currentForeColor, currentBackColor, 0);
                    }

                label:
                    curOffset--;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startOffset"></param>
        /// <returns></returns>
        public override int GetFirstInterestedOffset(int startOffset)
        {
            int curOffset = startOffset;
            int endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
            var document = CurrentContext.Document;

            if (startOffset == CurrentContext.VisualLine.FirstDocumentLine.Offset)
                return startOffset;

            while (curOffset < endOffset && document.GetCharAt(curOffset) != '\x1B')
                curOffset++;

            return curOffset == endOffset ? -1 : curOffset;
        }

        private static TextColor ConvertAnsiColorToTextColor(AnsiColor ansiColor, bool isBright)
        {
            if (isBright)
            {
                switch (ansiColor)
                {
                    case AnsiColor.Black:
                        return TextColor.BrightBlack;
                    case AnsiColor.Blue:
                        return TextColor.BrightBlue;
                    case AnsiColor.Cyan:
                        return TextColor.BrightCyan;
                    case AnsiColor.Green:
                        return TextColor.BrightGreen;
                    case AnsiColor.Magenta:
                        return TextColor.BrightMagenta;
                    case AnsiColor.Red:
                        return TextColor.BrightRed;
                    case AnsiColor.White:
                        return TextColor.BrightWhite;
                    case AnsiColor.Yellow:
                        return TextColor.BrightYellow;
                }
            }
            else
            {
                switch (ansiColor)
                {
                    case AnsiColor.Black:
                        return TextColor.Black;
                    case AnsiColor.Blue:
                        return TextColor.Blue;
                    case AnsiColor.Cyan:
                        return TextColor.Cyan;
                    case AnsiColor.Green:
                        return TextColor.Green;
                    case AnsiColor.Magenta:
                        return TextColor.Magenta;
                    case AnsiColor.Red:
                        return TextColor.Red;
                    case AnsiColor.White:
                        return TextColor.White;
                    case AnsiColor.Yellow:
                        return TextColor.Yellow;
                }
            }

            return TextColor.None;
        }
    }
}