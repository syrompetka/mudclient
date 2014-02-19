using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Adan.Client.Common.Controls;
using Adan.Client.Common.Model;
using CSLib.Net.Annotations;
using System.Xml.Serialization;

namespace Adan.Client.Common.Settings
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SettingsSerializer
    {
        private StringCollection _mainOutputs;
        private List<Hotkey> _globalHotkeys;
        private char _commandChar;
        private char _commandDelimiter;
        private int _commandsHistorySize;
        private int _connectPort;
        private string _connectHostName;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<SettingsChangedEventArgs> SettingsChanged;

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
                if(SettingsChanged != null)
                    SettingsChanged(this, new SettingsChangedEventArgs("ConnectPort", value));
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
                if (SettingsChanged != null)
                    SettingsChanged(this, new SettingsChangedEventArgs("ConnectHostName", value));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public List<Hotkey> GlobalHotkeys
        {
            get
            {
                if (_globalHotkeys == null)
                    _globalHotkeys = new List<Hotkey>();

                return _globalHotkeys;
            }
        }

        /// <summary>
        /// Get MainOutput windows
        /// </summary>
        [NotNull]
        public StringCollection MainOutputs
        {
            get
            {
                if (_mainOutputs == null)
                    _mainOutputs = new StringCollection();

                return _mainOutputs;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AutoConnect
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public bool IsLogCommands
        {
            get;
            set;
        }

        /// <summary>
        /// Scroll buffer
        /// </summary>
        public int ScrollBuffer
        {
            get;
            set;
        }

        /// <summary>
        /// Cursor Position
        /// </summary>
        public CursorPositionHistory CursorPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height of the main output window secondary scroll.
        /// </summary>
        /// <value>
        /// The height of the main output window secondary scroll.
        /// </value>
        public int MainOutputWindowSecondaryScrollHeight
        {
            get;
            set;
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
                RootModel.CommandChar = value;
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
                RootModel.CommandDelimiter = value;
            }
        }

        /// <summary>
        /// Get AutoClearInput
        /// </summary>
        public bool AutoClearInput
        {
            get;
            set;
        }

        /// <summary>
        /// Get AutoReconnect
        /// </summary>
        public bool AutoReconnect
        {
            get;
            set;
        }

        /// <summary>
        /// Get MinLengthHistory
        /// </summary>
        public int MinLengthHistory
        {
            get;
            set;
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
                if (SettingsChanged != null)
                    SettingsChanged(this, new SettingsChangedEventArgs("CommandsHistorySize", value));
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
    }
}
