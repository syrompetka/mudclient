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
    using Adan.Client.Common.Settings;
    using Adan.Client.Common.Controls;
    using Adan.Client.Common.Model;

    /// <summary>
    /// Control that enhances basic text box to support input history.
    /// </summary>
    public class TextBoxWithHistory : TextBox
    {
        private int _queueSize;
        private List<string> _enteredCommandsQueue;
        private int _currentQueueElementIndex = -1;
        private bool _selfChanges = false;
        private string oldText = String.Empty;

        private int oldCaretIndex;
        private int realCaretIndex = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxWithHistory"/> class.
        /// </summary>
        public TextBoxWithHistory()
        {
            AcceptsReturn = false;
            AcceptsTab = false;

            _queueSize = SettingsHolder.Instance.Settings.CommandsHistorySize;
            _enteredCommandsQueue = new List<string>();
        }

        /// <summary>
        /// Gets or sets the conveyor.
        /// </summary>
        [NotNull]
        public RootModel RootModel
        {
            get;
            set;
        }

        private int QueueSize
        {
            get
            {
                return _queueSize;
            }
            set
            {
                lock (_enteredCommandsQueue)
                {
                    if (_enteredCommandsQueue.Count > value)
                        _enteredCommandsQueue.RemoveRange(0, _enteredCommandsQueue.Count - value);

                    _enteredCommandsQueue.Capacity = value;
                    _queueSize = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        public void LoadHistory(ProfileHolder profile)
        {
            lock (_enteredCommandsQueue)
            {
                var history = profile.CommandsHistory;

                if (history.Count > _queueSize)
                {
                    _enteredCommandsQueue = profile.CommandsHistory.GetRange(_queueSize, profile.CommandsHistory.Count);
                }
                else
                {
                    _enteredCommandsQueue.AddRange(profile.CommandsHistory);
                    _enteredCommandsQueue.Capacity = _queueSize;
                }

                _currentQueueElementIndex = _enteredCommandsQueue.Count;
            }
        }

        /// <summary>
        /// Save current history to settings
        /// </summary>
        public void SaveCurrentHistory(ProfileHolder profile)
        {
            profile.CommandsHistory.Clear();

            lock (_enteredCommandsQueue)
            {
                profile.CommandsHistory.AddRange(_enteredCommandsQueue.ToArray());
            }
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

            if (!string.IsNullOrEmpty(command) && command.Length >= SettingsHolder.Instance.Settings.MinLengthHistory)
            {
                lock (_enteredCommandsQueue)
                {
                    _enteredCommandsQueue.RemoveAll(x => x == command);

                    _enteredCommandsQueue.Add(command);

                    if (_enteredCommandsQueue.Count > _queueSize)
                    {
                        _enteredCommandsQueue.RemoveAt(0);
                    }

                    _currentQueueElementIndex = _enteredCommandsQueue.Count;
                }
            }

            oldText = String.Empty;

            RootModel.PushCommandToConveyor(new TextCommand(command));
            if (SettingsHolder.Instance.Settings.AutoClearInput)
            {
                _selfChanges = true;
                Clear();
            }
            else
            {
                SelectAll();
                realCaretIndex = Text.Length;
            }
        }

        private void FindTextInHistoryAndUpdateTextBox(bool lookBackWard)
        {
            int ind = -1;
            lock (_enteredCommandsQueue)
            {
                if (lookBackWard)
                {
                    if (_currentQueueElementIndex > 0)
                    {
                        ind = _enteredCommandsQueue.FindLastIndex(_currentQueueElementIndex - 1, _currentQueueElementIndex,
                            x => x.StartsWith(oldText));
                    }
                }
                else
                {
                    if (_currentQueueElementIndex < _enteredCommandsQueue.Count)
                    {
                        ind = _enteredCommandsQueue.FindIndex(_currentQueueElementIndex + 1, _enteredCommandsQueue.Count - (_currentQueueElementIndex + 1),
                            x => x.StartsWith(oldText));

                        if (ind == -1)
                        {
                            _selfChanges = true;
                            Text = oldText;
                            _currentQueueElementIndex = _enteredCommandsQueue.Count;
                        }
                    }
                }

                if (ind >= 0)
                {
                    _selfChanges = true;
                    Text = _enteredCommandsQueue[ind];
                    _currentQueueElementIndex = ind;
                }
            }

            if (SettingsHolder.Instance.Settings.CursorPosition == CursorPositionHistory.StartOfLine)
                CaretIndex = 0;
            else
                CaretIndex = Text.Length;
        }

        /// <summary>
        /// Override OnTextChanged method
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

            base.OnTextChanged(e);
        }

        /// <summary>
        /// Улучшенное поведение каретки.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectionChanged(System.Windows.RoutedEventArgs e)
        {
            if (SelectionLength > 0)
            {
                if (oldCaretIndex == CaretIndex)
                {
                    realCaretIndex = CaretIndex + SelectionLength;
                }
                else
                {
                    realCaretIndex = CaretIndex;
                }
            }
            else
            {
                if (realCaretIndex != -1)
                {
                    int newCaretIndex = -1;
                    if (oldCaretIndex == CaretIndex)
                    {
                        if (realCaretIndex > 0)
                            newCaretIndex = realCaretIndex - 1;
                        else
                            newCaretIndex = realCaretIndex;
                    }
                    else if (oldCaretIndex < CaretIndex)
                    {
                        if (realCaretIndex < Text.Length)
                            newCaretIndex = realCaretIndex + 1;
                        else
                            newCaretIndex = realCaretIndex;
                    }
                    else
                    {
                        RootModel.PushMessageToConveyor(new ErrorMessage(string.Format("#Ошибка, просьба передать это разработчикам: TextBoxWithHistory: oldCaretIndex = {0}, CaretIndex = {1}", oldCaretIndex, CaretIndex)));
                    }

                    realCaretIndex = -1;
                    CaretIndex = newCaretIndex;
                }
            }

            oldCaretIndex = CaretIndex;

            base.OnSelectionChanged(e);
        }

        private void OnSettingsChanged(object sender, SettingsChangedEventArgs e)
        {
            if (e.Name == "CommandsHistorySize")
                QueueSize = (int) e.Value;
        }
    }
}
