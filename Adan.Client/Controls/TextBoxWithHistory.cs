// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextBoxWithHistory.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for TextBoxWithHistory.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;

    using Common.Commands;
    using Common.Conveyor;

    using CSLib.Net.Annotations;

    /// <summary>
    /// Control that enhances basic text box to support input history.
    /// </summary>
    public class TextBoxWithHistory : TextBox
    {
        private const int _queueSize = 20;
        private readonly List<string> _enteredCommandsQueue = new List<string>(_queueSize + 1);
        private int _currentQueueElementIndex = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxWithHistory"/> class.
        /// </summary>
        public TextBoxWithHistory()
        {
            AcceptsReturn = false;
            AcceptsTab = false;
        }

        /// <summary>
        /// Gets or sets the conveyor.
        /// </summary>
        [NotNull]
        public MessageConveyor Conveyor
        {
            get;
            set;
        }

        /// <summary>
        /// Shows the next command.
        /// </summary>
        public void ShowNextCommand()
        {
            FindTextInHistoryAndUpdateTextBox(false);
        }

        /// <summary>
        /// Shows the previous command.
        /// </summary>
        public void ShowPreviousCommand()
        {
            FindTextInHistoryAndUpdateTextBox(true);
        }

        /// <summary>
        /// Sends the current command.
        /// </summary>
        public void SendCurrentCommand()
        {
            var command = Text ?? string.Empty;
            if (!string.IsNullOrEmpty(command) && command.Length > 1)
            {
                while (_enteredCommandsQueue.Contains(command))
                {
                    _enteredCommandsQueue.Remove(command);
                }

                _enteredCommandsQueue.Add(command);

                if (_enteredCommandsQueue.Count > _queueSize)
                {
                    _enteredCommandsQueue.RemoveAt(0);
                }

                _currentQueueElementIndex = _enteredCommandsQueue.Count;
            }

            Conveyor.PushCommand(new TextCommand(command));
            Text = string.Empty;
        }

        private void FindTextInHistoryAndUpdateTextBox(bool lookBackWard)
        {
            var userTextLength = SelectionStart;
            var userText = Text.Substring(0, userTextLength);
            while (true)
            {
                if (!lookBackWard)
                {
                    if (_currentQueueElementIndex >= _enteredCommandsQueue.Count - 1)
                    {
                        Text = userText;
                        _currentQueueElementIndex = _enteredCommandsQueue.Count;
                        Select(userText.Length, 0);
                        break;
                    }

                    _currentQueueElementIndex++;
                }
                else
                {
                    if (_currentQueueElementIndex <= 0)
                    {
                        break;
                    }

                    _currentQueueElementIndex--;
                }

                var newText = _enteredCommandsQueue[_currentQueueElementIndex];
                if (newText.StartsWith(userText, StringComparison.CurrentCultureIgnoreCase))
                {
                    Text = newText;
                    Select(userTextLength, newText.Length - userTextLength);
                    break;
                }
            }
        }
    }
}
