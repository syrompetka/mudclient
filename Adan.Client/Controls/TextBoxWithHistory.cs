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
    using System.Linq;
    using System.Globalization;

    using Common.Commands;
    using Common.Conveyor;

    using CSLib.Net.Annotations;
    using System.Collections.Specialized;
    using Adan.Client.Common.Messages;

    /// <summary>
    /// Control that enhances basic text box to support input history.
    /// </summary>
    public class TextBoxWithHistory : TextBox
    {
        //private const int _queueSize = 20;
        //private readonly List<string> _enteredCommandsQueue = new List<string>(_queueSize + 1);
        private int _queueSize;
        private List<string> _enteredCommandsQueue;
        private int _currentQueueElementIndex = -1;
        private bool _selfChanges = false;
        private string oldText = String.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxWithHistory"/> class.
        /// </summary>
        public TextBoxWithHistory()
        {
            AcceptsReturn = false;
            AcceptsTab = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            var history = SettingsHolder.Instance.CommandsHistory;
            _queueSize = SettingsHolder.Instance.HistorySize;

            if (history != null)
            {
                _enteredCommandsQueue = history.Cast<string>().ToList();

                if (_enteredCommandsQueue.Count > _queueSize)
                    _enteredCommandsQueue.RemoveRange(0, _enteredCommandsQueue.Count - _queueSize);

                _enteredCommandsQueue.Capacity = _queueSize;

                _currentQueueElementIndex = _enteredCommandsQueue.Count;
            }
            else
            {
                _enteredCommandsQueue = new List<string>(_queueSize);
            }
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
        /// Save current history to settings
        /// </summary>
        public void SaveCurrentHistory()
        {
            SettingsHolder.Instance.CommandsHistory.Clear();
            SettingsHolder.Instance.CommandsHistory.AddRange(_enteredCommandsQueue.ToArray());
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

            //Временное решение обновления размера истории комманд
            if (_enteredCommandsQueue.Capacity != SettingsHolder.Instance.HistorySize + 1)
            {
                _queueSize = SettingsHolder.Instance.HistorySize;

                if (_enteredCommandsQueue.Count > _queueSize + 1)
                    _enteredCommandsQueue.RemoveRange(0, _enteredCommandsQueue.Count - _queueSize);
                
                _enteredCommandsQueue.Capacity = _queueSize + 1;
            }

            if (!string.IsNullOrEmpty(command) && command.Length >= SettingsHolder.Instance.MinLengthHistory)
            {
                //WTF?
                //while (_enteredCommandsQueue.Contains(command))
                //{
                //    _enteredCommandsQueue.Remove(command);
                //}
                _enteredCommandsQueue.RemoveAll(x => x == command);

                _enteredCommandsQueue.Add(command);

                if (_enteredCommandsQueue.Count > _queueSize)
                {
                    _enteredCommandsQueue.RemoveAt(0);
                }

                _currentQueueElementIndex = _enteredCommandsQueue.Count;
            }

            Conveyor.PushCommand(new TextCommand(command));
            if (SettingsHolder.Instance.AutoClearInput)
            {
                _selfChanges = true;
                Clear();
            }
            else
                SelectAll();
        }

        private void FindTextInHistoryAndUpdateTextBox(bool lookBackWard)
        {
        //    var userTextLength = SelectionStart;
        //    var userText = Text.Substring(0, userTextLength);
        //    while (true)
        //    {
        //        if (!lookBackWard)
        //        {
        //            if (_currentQueueElementIndex >= _enteredCommandsQueue.Count - 1)
        //            {
        //                Text = userText;
        //                _currentQueueElementIndex = _enteredCommandsQueue.Count;
        //                Select(userText.Length, 0);
        //                break;
        //            }

        //            _currentQueueElementIndex++;
        //        }
        //        else
        //        {
        //            if (_currentQueueElementIndex <= 0)
        //            {
        //                break;
        //            }

        //            _currentQueueElementIndex--;
        //        }

        //        var newText = _enteredCommandsQueue[_currentQueueElementIndex];
        //        if (newText.StartsWith(userText, StringComparison.CurrentCultureIgnoreCase))
        //        {
        //            Text = newText;
        //            Select(userTextLength, newText.Length - userTextLength);
        //            break;
        //        }
        //    }

            int ind = -1;
            if (lookBackWard)
            {
                if (_currentQueueElementIndex > 0)
                {
                    ind = _enteredCommandsQueue.FindLastIndex(_currentQueueElementIndex - 1, _currentQueueElementIndex,
                        x => x.StartsWith(oldText, true, CultureInfo.InvariantCulture));
                }
            }
            else
            {
                if (_currentQueueElementIndex < _enteredCommandsQueue.Count)
                {
                    ind = _enteredCommandsQueue.FindIndex(_currentQueueElementIndex + 1, _enteredCommandsQueue.Count - (_currentQueueElementIndex + 1),
                        x => x.StartsWith(oldText, true, CultureInfo.InvariantCulture));

                    if (ind == -1)
                    {
                        _selfChanges = true;
                        Text = oldText;
                        _currentQueueElementIndex = _enteredCommandsQueue.Count;
                    }
                }
            }

            if(ind >= 0)
            {
                _selfChanges = true;
                Text = _enteredCommandsQueue[ind];
                _currentQueueElementIndex = ind;
            }

            if (SettingsHolder.Instance.CursorPosition == CursorPositionHistory.StartOfLine)
                CaretIndex = 0;
            else
                CaretIndex = Text.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (!_selfChanges)
            {
                oldText = Text;
                _currentQueueElementIndex = _enteredCommandsQueue.Count;
            }
            else
                _selfChanges = false;
        }
    }
}
