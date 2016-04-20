using System;
using System.Collections.Generic;
using System.Windows;
using Adan.Client.Common.Controls;
using Adan.Client.Common.Model;
using CSLib.Net.Annotations;

namespace Adan.Client.Common.Settings
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SettingsSerializer
    {
        //private StringCollection _mainOutputs;
        private List<Group> _globalGroups;
        private char _commandChar;
        private char _commandDelimiter;
        private int _commandsHistorySize;
        private int _connectPort;
        private string _connectHostName;
        private string _colorTheme;
        private bool _autoConnect;
        private bool _isLogCommands;
        private int _scrollBuffer;
        private CursorPositionHistory _cursorPosition;
        private int _mainOutputWindowSecondaryScrollHeight;
        private bool _autoClearInput;
        private bool _autoReconnect;
        private int _minLengthHistory;
        private int? _mudFontSize;
        private string _mudFontName;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<SettingsChangedEventArgs> OnSettingsChanged;

        /// <summary>
        /// Gets or sets the connect port.
        /// </summary>
        /// <value>
        /// The connect port.
        /// </value>
        [NotNull]
        public int ConnectPort
        {
            get
            {
                return _connectPort;
            }
            set
            {
                _connectPort = value;
                SettingsChanged("ConnectPort", value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the connect host.
        /// </summary>
        /// <value>
        /// The name of the connect host.
        /// </value>
        [NotNull]
        public string ConnectHostName
        {
            get
            {
                return _connectHostName;
            }
            set
            {
                _connectHostName = value;
                SettingsChanged("ConnectHostName", value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the connect host.
        /// </summary>
        /// <value>
        /// The name of the connect host.
        /// </value>
        [NotNull]
        public string ColorTheme
        {
            get
            {
                return _colorTheme;
            }
            set
            {
                _colorTheme = value;
                SettingsChanged("ColorTheme", value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the connect host.
        /// </summary>
        /// <value>
        /// The name of the connect host.
        /// </value>
        [NotNull]
        public string MUDFontName
        {
            get
            {
                return _mudFontName ?? "Consolas";
            }
            set
            {
                _mudFontName = value;
                SettingsChanged("MUDFontName", value);
            }
        }


        /// <summary>
        /// Gets or sets the name of the connect host.
        /// </summary>
        /// <value>
        /// The name of the connect host.
        /// </value>
        [NotNull]
        public int MUDFontSize
        {
            get
            {
                return _mudFontSize ?? 11;
            }
            set
            {
                _mudFontSize = value;
                SettingsChanged("MUDFontSize", value);
            }
        }
        
        /// <summary>
                 /// 
                 /// </summary>
        [NotNull]
        public List<Group> GlobalGroups
        {
            get
            {
                if (_globalGroups == null)
                    _globalGroups = new List<Group>();

                return _globalGroups;
            }
        }

        /// <summary>
        /// For backwards compatibility
        /// </summary>
        public List<Hotkey> GlobalHotkeys { get; set; }

        ///// <summary>
        ///// Get MainOutput windows
        ///// </summary>
        //[NotNull]
        //public StringCollection MainOutputs
        //{
        //    get
        //    {
        //        if (_mainOutputs == null)
        //            _mainOutputs = new StringCollection();

        //        return _mainOutputs;
        //    }
        //}

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
                SettingsChanged("AutoConnect", value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public bool IsLogCommands
        {
            get
            {
                return _isLogCommands;
            }
            set
            {
                _isLogCommands = value;
                SettingsChanged("IsLogCommands", value);
            }
        }

        /// <summary>
        /// Scroll buffer
        /// </summary>
        public int ScrollBuffer
        {
            get
            {
                return _scrollBuffer;
            }
            set
            {
                _scrollBuffer = value;
                SettingsChanged("ScrollBuffer", value);
            }
        }

        /// <summary>
        /// Cursor Position
        /// </summary>
        public CursorPositionHistory CursorPosition
        {
            get
            {
                return _cursorPosition;
            }
            set
            {
                _cursorPosition = value;
                SettingsChanged("CursorPosition", value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the main output window secondary scroll.
        /// </summary>
        /// <value>
        /// The height of the main output window secondary scroll.
        /// </value>
        public int MainOutputWindowSecondaryScrollHeight
        {
            get
            {
                return _mainOutputWindowSecondaryScrollHeight;
            }
            set
            {
                _mainOutputWindowSecondaryScrollHeight = value;
                SettingsChanged("MainOutputWindowSecondaryScrollHeight", value);
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
                _commandChar = value;
                SettingsChanged("CommandChar", value);
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
                _commandDelimiter = value;
                SettingsChanged("CommandDelimiter", value);
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
                SettingsChanged("AutoClearInput", value);
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
                SettingsChanged("AutoReconnect", value);
            }
        }

        /// <summary>
        /// Get MinLengthHistory
        /// </summary>
        public int MinLengthHistory
        {
            get
            {
                return _minLengthHistory;
            }
            set
            {
                _minLengthHistory = value;
                SettingsChanged("MinLengthHistory", value);
            }
        }

        /// <summary>
        /// Get HistorySize
        /// </summary>
        public int CommandsHistorySize
        {
            get
            {
                return _commandsHistorySize;
            }
            set
            {
                _commandsHistorySize = value;
                SettingsChanged("CommandsHistorySize", value);
            }
        }

        /// <summary>
        /// Get the state of main window as it was before it was closed last time.
        /// </summary>
        public WindowState MainWindowState
        {
            get; set;
        }

        /// <summary>
        /// Get the Y coordinate of main window as it was before it was closed last time.
        /// </summary>
        public int MainWindowTop
        {
            get; set;
        }

        /// <summary>
        /// Get the X coordinate of main window as it was before it was closed last time.
        /// </summary>
        public int MainWindowLeft
        {
            get;
            set;
        }

        /// <summary>
        /// Get the width of main window as it was before it was closed last time.
        /// </summary>
        public int MainWindowWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Get the Height of main window as it was before it was closed last time.
        /// </summary>
        public int MainWindowHeight
        {
            get;
            set;
        }

        private void SettingsChanged(string name, object value)
        {
            if(OnSettingsChanged != null)
                OnSettingsChanged(this, new SettingsChangedEventArgs(name, value));
        }
    }
}
