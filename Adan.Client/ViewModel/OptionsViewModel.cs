using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adan.Client.Common.Controls;
using Adan.Client.Common.Model;
using Adan.Client.Common.ViewModel;
using Adan.Client.Common.Settings;

namespace Adan.Client.ViewModel
{
    /// <summary>
    /// View Model for Options
    /// </summary>
    public class OptionsViewModel : ViewModelBase
    {
        private char _commandChar;
        private char _commandDelimiter;
        private bool _autoReconnect;
        private bool _autoClearInput;
        private string _minLengthHistory;
        private string _historySize;
        private string _scrollBuffer;
        private bool _settingsFolder;
        private bool _autoConnect;
        //private bool _startOfLine;
        //private bool _endOfLine;
        private CursorPositionHistory _selectedCursorPosition;
        private SettingsOutputWindowForm _selectedOutputWindowForms;

        /// <summary>
        /// 
        /// </summary>
        public bool SettingsFolder
        {
            get
            {
                return _settingsFolder;
            }
            set
            {
                _settingsFolder = value;
                OnPropertyChanged("SettingsFolder");
            }
        }

        /// <summary>
        /// Scroll buffer
        /// </summary>
        public string ScrollBuffer
        {
            get
            {
                return _scrollBuffer;
            }
            set
            {
                _scrollBuffer = value;
                OnPropertyChanged("ScrollBuffer");
            }
        }

        /// <summary>
        /// Gets cursor history position
        /// </summary>
        public CursorPositionHistory SelectedCursorPosition
        {
            get
            {
                return _selectedCursorPosition;
            }
            set
            {
                _selectedCursorPosition = value;
                OnPropertyChanged("SelectedCursorPosition");
            }
        }

        ///// <summary>
        ///// Gets cursor history
        ///// </summary>
        //public bool StartOfLine
        //{
        //    get
        //    {
        //        return _startOfLine;
        //    }
        //    set
        //    {
        //        _startOfLine = value;
        //        OnPropertyChanged("StartOfLine");
        //    }
        //}

        ///// <summary>
        ///// Gets cursor history
        ///// </summary>
        //public bool EndOfLine
        //{
        //    get
        //    {
        //        return _endOfLine;
        //    }
        //    set
        //    {
        //        _endOfLine = value;
        //        OnPropertyChanged("EndOfLine");
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        public SettingsOutputWindowForm SelectedOutputWindowForm
        {
            get
            {
                return _selectedOutputWindowForms;
            }
            set
            {
                _selectedOutputWindowForms = value;
                OnPropertyChanged("SelectedOutputWindowForm");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool AutoConnect
        {
            get
            {
                return _autoConnect;
            }
            set
            {
                _autoConnect = value;
                OnPropertyChanged("AutoConnect");
            }
        }

        /// <summary>
        /// Gets Char Separator
        /// </summary>
        public char CommandChar
        {
            get
            {
                return _commandChar;
            }
            set
            {
                if (!Char.IsWhiteSpace(value))
                {
                    _commandChar = value;
                    OnPropertyChanged("CharSeparator");
                }
            }
        }

        /// <summary>
        /// Get Char delimiter
        /// </summary>
        public char CommandDelimiter
        {
            get
            {
                return _commandDelimiter;
            }
            set
            {
                if (!Char.IsWhiteSpace(value))
                {
                    _commandDelimiter = value;
                    OnPropertyChanged("CharDelimiter");
                }
            }
        }

        /// <summary>
        /// Get AutoClearInput
        /// </summary>
        public bool AutoClearInput
        {
            get
            {
                return _autoClearInput;
            }
            set
            {
                _autoClearInput = value;
                OnPropertyChanged("AutoClearInput");
            }
        }

        /// <summary>
        /// Get AutoReconnect
        /// </summary>
        public bool AutoReconnect
        {
            get
            {
                return _autoReconnect;
            }
            set
            {
                _autoReconnect = value;
                OnPropertyChanged("AutoReconnect");
            }
        }

        /// <summary>
        /// Get MinLengthHistory
        /// </summary>
        public string MinLengthHistory
        {
            get
            {
                return _minLengthHistory;
            }
            set
            {
                _minLengthHistory = value;
                OnPropertyChanged("MinLengthHistory");
            }
        }

        /// <summary>
        /// Get HistorySize
        /// </summary>
        public string HistorySize
        {
            get
            {
                return _historySize;
            }
            set
            {
                _historySize = value;
                OnPropertyChanged("HistorySize");
            }
        }
    }
}
