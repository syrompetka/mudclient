namespace Adan.Client.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Common.Commands;
    using Common.Controls;
    using Common.Model;
    using Common.Settings;
    using CSLib.Net.Annotations;

    /* В WPF при выделении CaretIndex определяет крайнее левое положение выделения, а SelectionLength его длину.
     * При перемещении каретки влево или вправо она передвигается всегда относительно CaretIndex,
     * независимо от выделения слева направо или справо налево.
     * Этот класс реализоввывает более привычное и понятное поведение каретки.
     */

    /// <summary>
    /// Control that enhances basic text box to support input history.
    /// Улучшено поведение каретки.
    /// </summary>
    public class TextBoxWithHistory : TextBox
    {
        private int _queueSize;
        private List<string> _enteredCommandsQueue;
        private int _currentQueueElementIndex = -1;
        private bool _selfTextChanges;
        private string oldText = String.Empty;

        private int oldCaretIndex;
        private int realStartSelection = -1;
        private int correctCaretIndex;
        private bool needCorrectCaret;
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxWithHistory"/> class.
        /// </summary>
        public TextBoxWithHistory()
        {
            AcceptsReturn = false;
            AcceptsTab = false;
            AutoWordSelection = false;

            _queueSize = SettingsHolder.Instance.Settings.CommandsHistorySize;
            _enteredCommandsQueue = new List<string>();
            SettingsHolder.Instance.Settings.OnSettingsChanged += OnSettingsChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Left && Keyboard.Modifiers == ModifierKeys.None && SelectionLength > 0)
            {
                if (CaretIndex == realStartSelection)
                {
                    correctCaretIndex = CaretIndex + SelectionLength - 1;
                    needCorrectCaret = true;
                }
                else if (CaretIndex < realStartSelection)
                {
                    correctCaretIndex = CaretIndex > 0 ? CaretIndex - 1 : 0;
                    needCorrectCaret = true;
                }
            }
            else if (e.Key == Key.Right && Keyboard.Modifiers == ModifierKeys.None && SelectionLength > 0)
            {
                if (CaretIndex == realStartSelection)
                {
                    correctCaretIndex = CaretIndex + SelectionLength == Text.Length ? CaretIndex + SelectionLength : CaretIndex + SelectionLength + 1;
                    needCorrectCaret = true;
                }
                else if (CaretIndex < realStartSelection)
                {
                    correctCaretIndex = CaretIndex + 1;
                    needCorrectCaret = true;
                }
            }

            base.OnPreviewKeyDown(e);
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
        /// Load command's history from profile.
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
            RootModel.PushCommandToConveyor(FlushOutputQueueCommand.Instance);
            if (SettingsHolder.Instance.Settings.AutoClearInput)
            {
                _selfTextChanges = true;
                Clear();
            }
            else
            {
                SelectAll();
                realStartSelection = 0;
            }
        }

        private void FindTextInHistoryAndUpdateTextBox(bool lookBackWard)
        {
            int index = -1;
            lock (_enteredCommandsQueue)
            {
                if (lookBackWard)
                {
                    if (_currentQueueElementIndex > 0)
                    {
                        index = _enteredCommandsQueue.FindLastIndex(_currentQueueElementIndex - 1, _currentQueueElementIndex,
                            x => x.StartsWith(oldText));
                    }
                }
                else
                {
                    if (_currentQueueElementIndex < _enteredCommandsQueue.Count)
                    {
                        index = _enteredCommandsQueue.FindIndex(_currentQueueElementIndex + 1, _enteredCommandsQueue.Count - (_currentQueueElementIndex + 1),
                            x => x.StartsWith(oldText));

                        if (index == -1)
                        {
                            _selfTextChanges = true;
                            Text = oldText;
                            _currentQueueElementIndex = _enteredCommandsQueue.Count;
                        }
                    }
                }

                if (index >= 0)
                {
                    _selfTextChanges = true;
                    Text = _enteredCommandsQueue[index];
                    _currentQueueElementIndex = index;
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
            if (!_selfTextChanges)
            {
                oldText = Text;
                _currentQueueElementIndex = _enteredCommandsQueue.Count;
            }
            else
                _selfTextChanges = false;

            base.OnTextChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectionChanged(RoutedEventArgs e)
        {
            if (SelectionLength > 0)
            {
                if(!isSelected)
                {
                    realStartSelection = oldCaretIndex;
                    isSelected = true;
                }
            }
            else
            {
                if (needCorrectCaret)
                {
                    needCorrectCaret = false;
                    CaretIndex = correctCaretIndex;
                }

                isSelected = false;
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
