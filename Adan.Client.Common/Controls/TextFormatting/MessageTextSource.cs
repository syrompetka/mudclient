// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageTextSource.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the MessageTextSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Controls.TextFormatting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.TextFormatting;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Messages;

    using Themes;

    /// <summary>
    /// A <see cref="TextSource"/> implementation that wraps <see cref="TextMessage"/>.
    /// </summary>
    public class MessageTextSource : TextSource
    {
        private readonly TextSelectionSettings _selectionSettings;
        private readonly IDictionary<Brush, IDictionary<Brush, CustomTextRunProperties>> _customPropertiesByColor = new Dictionary<Brush, IDictionary<Brush, CustomTextRunProperties>>();
        private readonly IDictionary<Brush, Brush> _invertedBrushes = new Dictionary<Brush, Brush>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTextSource"/> class.
        /// </summary>
        /// <param name="selectionSettings">The selection settings.</param>
        public MessageTextSource([NotNull] TextSelectionSettings selectionSettings)
        {
            Assert.ArgumentNotNull(selectionSettings, "selectionSettings");
            _selectionSettings = selectionSettings;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [CanBeNull]
        public TextMessage Message
        {
            get;
            set;
        }

        #region Overrides of TextSource

        /// <summary>
        /// Retrieves a <see cref="T:System.Windows.Media.TextFormatting.TextRun"/> starting at a specified <see cref="T:System.Windows.Media.TextFormatting.TextSource"/> position.
        /// </summary>
        /// <returns>
        /// A value that represents a <see cref="T:System.Windows.Media.TextFormatting.TextRun"/>, or an object derived from <see cref="T:System.Windows.Media.TextFormatting.TextRun"/>.
        /// </returns>
        /// <param name="textSourceCharacterIndex">Specifies the character index position in the <see cref="T:System.Windows.Media.TextFormatting.TextSource"/> where the <see cref="T:System.Windows.Media.TextFormatting.TextRun"/> is retrieved.</param>
        public override TextRun GetTextRun(int textSourceCharacterIndex)
        {
            //if (Message == null || textSourceCharacterIndex >= Message.InnerText.Length)
            //{
            //    return new TextEndOfLine(1);
            //}

            //var currentIdx = 0;
            //bool isSelecting = false;
            //int selectionStartIndex = 0;
            //int selectionEndIndex = Message.InnerText.Length;

            //if (_selectionSettings.SelectedMessages.Contains(Message))
            //{
            //    isSelecting = true;
            //    if (Message == _selectionSettings.SelectedMessages.Last())
            //    {
            //        selectionStartIndex = _selectionSettings.SelectionStartCharacterNumber;
            //    }

            //    if (Message == _selectionSettings.SelectedMessages.First())
            //    {
            //        selectionEndIndex = _selectionSettings.SelectionEndCharacterNumber;
            //    }
            //}

            ////foreach (var messageBlock in Message.MessageBlocks)
            ////{
            ////    if (messageBlock.Text.Length + currentIdx > textSourceCharacterIndex)
            ////    {
            ////        if (isSelecting)
            ////        {
            ////            if (selectionStartIndex >= currentIdx && selectionStartIndex < messageBlock.Text.Length + currentIdx)
            ////            {
            ////                if (textSourceCharacterIndex < selectionStartIndex)
            ////                {
            ////                    return new TextCharacters(
            ////                        messageBlock.Text,
            ////                        textSourceCharacterIndex - currentIdx,
            ////                        selectionStartIndex - textSourceCharacterIndex,
            ////                        GetTextRunPropertiesByBrushes(ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(messageBlock.Foreground, false), ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(messageBlock.Background, true)));
            ////                }

            ////                if (textSourceCharacterIndex >= selectionEndIndex)
            ////                {
            ////                    return new TextCharacters(
            ////                        messageBlock.Text,
            ////                        textSourceCharacterIndex - currentIdx,
            ////                        messageBlock.Text.Length - selectionEndIndex + currentIdx,
            ////                        GetTextRunPropertiesByBrushes(ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(messageBlock.Foreground, false), ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(messageBlock.Background, true)));
            ////                }

            ////                return new TextCharacters(
            ////                    messageBlock.Text,
            ////                    textSourceCharacterIndex - currentIdx,
            ////                    Math.Min(selectionEndIndex - selectionStartIndex, messageBlock.Text.Length - textSourceCharacterIndex + currentIdx),
            ////                    GetTextRunPropertiesByBrushes(InvertBrush(ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(messageBlock.Foreground, false)), InvertBrush(ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(messageBlock.Background, true))));
            ////            }

            ////            if (selectionStartIndex < currentIdx && selectionEndIndex > messageBlock.Text.Length + currentIdx)
            ////            {
            ////                return new TextCharacters(
            ////                    messageBlock.Text,
            ////                    textSourceCharacterIndex - currentIdx,
            ////                    messageBlock.Text.Length - textSourceCharacterIndex + currentIdx,
            ////                    GetTextRunPropertiesByBrushes(InvertBrush(ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(messageBlock.Foreground, false)), InvertBrush(ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(messageBlock.Background, true))));
            ////            }

            ////            if (selectionStartIndex < selectionEndIndex && selectionEndIndex > currentIdx && selectionEndIndex <= messageBlock.Text.Length + currentIdx)
            ////            {
            ////                if (selectionEndIndex > textSourceCharacterIndex)
            ////                {
            ////                    return new TextCharacters(
            ////                        messageBlock.Text,
            ////                        textSourceCharacterIndex - currentIdx,
            ////                        selectionEndIndex - textSourceCharacterIndex,
            ////                        GetTextRunPropertiesByBrushes(InvertBrush(ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(messageBlock.Foreground, false)), InvertBrush(ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(messageBlock.Background, true))));
            ////                }
            ////            }
            ////        }

            ////        return new TextCharacters(
            ////            messageBlock.Text,
            ////            textSourceCharacterIndex - currentIdx,
            ////            messageBlock.Text.Length - textSourceCharacterIndex + currentIdx,
            ////            GetTextRunPropertiesByBrushes(ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(messageBlock.Foreground, false), ThemeManager.Instance.ActiveTheme.GetBrushByTextColor(messageBlock.Background, true)));
            ////    }

            //    //currentIdx += messageBlock.Text.Length;
            //}

            return new TextEndOfLine(1);
        }

        /// <summary>
        /// Retrieves the text span immediately before the specified <see cref="T:System.Windows.Media.TextFormatting.TextSource"/> position.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Media.TextFormatting.CultureSpecificCharacterBufferRange"/> value that represents the text span immediately before <paramref name="textSourceCharacterIndexLimit"/>.
        /// </returns>
        /// <param name="textSourceCharacterIndexLimit">An <see cref="T:System.Int32"/> value that specifies the character index position where text retrieval stops.</param>
        /// <exception cref="NotImplementedException"><c>NotImplementedException</c>.</exception>
        public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a value that maps a <see cref="T:System.Windows.Media.TextFormatting.TextSource"/> character index to a <see cref="T:System.Windows.Media.TextEffect"/> character index.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Int32"/> value that represents the <see cref="T:System.Windows.Media.TextEffect"/> character index.
        /// </returns>
        /// <param name="textSourceCharacterIndex">An <see cref="T:System.Int32"/> value that specifies the <see cref="T:System.Windows.Media.TextFormatting.TextSource"/> character index to map.</param>
        /// <exception cref="NotImplementedException"><c>NotImplementedException</c>.</exception>
        public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
        {
            throw new NotImplementedException();
        }

        #endregion

        [NotNull]
        private Brush InvertBrush([NotNull]SolidColorBrush brushToInvert)
        {
            Assert.ArgumentNotNull(brushToInvert, "brushToInvert");

            Brush result;
            if (!_invertedBrushes.TryGetValue(brushToInvert, out result))
            {
                var invertedBrush =
                    new SolidColorBrush(
                        Color.FromRgb(
                            (byte)(byte.MaxValue - brushToInvert.Color.R),
                            (byte)(byte.MaxValue - brushToInvert.Color.G),
                            (byte)(byte.MaxValue - brushToInvert.Color.B)));
                _invertedBrushes[brushToInvert] = invertedBrush;
                result = invertedBrush;
            }

            return result;
        }

        [NotNull]
        private CustomTextRunProperties GetTextRunPropertiesByBrushes([NotNull] Brush foreground, [NotNull] Brush background)
        {
            Assert.ArgumentNotNull(foreground, "foreground");
            Assert.ArgumentNotNull(background, "background");

            IDictionary<Brush, CustomTextRunProperties> foreGroundValues;
            if (!_customPropertiesByColor.TryGetValue(foreground, out foreGroundValues))
            {
                foreGroundValues = new Dictionary<Brush, CustomTextRunProperties>();
                _customPropertiesByColor[foreground] = foreGroundValues;
            }

            CustomTextRunProperties result;
            if (!foreGroundValues.TryGetValue(background, out result))
            {
                result = new CustomTextRunProperties(foreground, background);
                foreGroundValues[background] = result;
            }

            return result;
        }
    }
}
