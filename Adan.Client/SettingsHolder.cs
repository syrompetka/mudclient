// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsHolder.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Settings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Adan.Client.Common;
    using Adan.Client.Common.Controls;
    using Common.Model;
    using Common.Themes;
    using CSLib.Net.Annotations;
    using Model.ActionParameters;
    using Model.Actions;
    using Properties;

    /// <summary>
    /// Class whos hold settings like colors, windows sizes etc.
    /// </summary>
    public class SettingsHolder
    {
        #region Constants and Fields

        private static SettingsHolder _instance = new SettingsHolder();

        private readonly Settings _settings;
        //private readonly XmlSerializer _groupsSerializer;
        //private readonly XmlSerializer _variablesSerializer;
        //private IList<Group> _groups;
        //private IList<Variable> _variables;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsHolder"/> class.
        /// </summary>
        public SettingsHolder()
        {
            _settings = Settings.Default;
            _settings.Reload();

            RootModel.CommandChar = _settings.CommandChar;
            RootModel.CommandDelimiter = _settings.CommandDelimiter;

            var types = new List<Type>
                            {
                                typeof(SendTextAction),
                                typeof(OutputToMainWindowAction),
                                typeof(ClearVariableValueAction),
                                typeof(ConditionalAction),
                                typeof(DisableGroupAction),
                                typeof(EnableGroupAction),
                                typeof(SetVariableValueAction),
                                typeof(StartLogAction),
                                typeof(StopLogAction),
                                typeof(TriggerOrCommandParameter),
                                typeof(VariableReferenceParameter),
                                typeof(MathExpressionParameter),
                                typeof(ConstantStringParameter),
                                typeof(SendTextOneParameterAction),
                            };

            foreach (var plugin in PluginHost.Instance.Plugins)
            {
                foreach (var customType in plugin.CustomSerializationTypes)
                {
                    types.Add(customType);
                }
            }

            //_groupsSerializer = new XmlSerializer(typeof(List<Group>), types.ToArray());
            //_variablesSerializer = new XmlSerializer(typeof(List<Variable>));

            ProfileHolder.Instance.SettingsFolder = (SettingsFolder)_settings.Folder;

            ProfileHolder.Instance.Initialize(types);

            if (ProfileHolder.Instance.AllProfiles.Contains(_settings.ProfileName))
                ProfileHolder.Instance.Name = _settings.ProfileName;
            else if (!ProfileHolder.Instance.AllProfiles.Contains("Default"))
                ProfileHolder.Instance.Name = "Default";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        [NotNull]
        public static SettingsHolder Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Get SettingsFolder
        /// </summary>
        [NotNull]
        public SettingsFolder SettingsFolder
        {
            get
            {
                return (SettingsFolder)_settings.Folder;
            }
            set
            {
                _settings.Folder = (int)value;
                ProfileHolder.Instance.SettingsFolder = value;
            }
        }

        /// <summary>
        /// Get Settings Folder
        /// </summary>
        public string Folder
        {
            get
            {
                return ProfileHolder.Instance.Folder;
            }
        }

        /// <summary>
        /// Command history
        /// </summary>
        public StringCollection CommandsHistory
        {
            get
            {
                if (_settings.CommandsHistory == null)
                    _settings.CommandsHistory = new StringCollection();

                return _settings.CommandsHistory;
            }
        }

        /// <summary>
        /// Scroll buffer
        /// </summary>
        public int ScrollBuffer
        {
            get
            {
                return _settings.ScrollBuffer;
            }
            set
            {
                _settings.ScrollBuffer = value;
            }
        }

        /// <summary>
        /// Cursor Position
        /// </summary>
        public CursorPositionHistory CursorPosition
        {
            get
            {
                return (CursorPositionHistory)_settings.HistoryCursorPosition;
            }
            set
            {
                _settings.HistoryCursorPosition = (int) value;
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
                return _settings.MainOutputWindowSecondaryScrollHeight;
            }
            set
            {
                _settings.MainOutputWindowSecondaryScrollHeight = value;
            }
        }

        /// <summary>
        /// Gets Char Separator
        /// </summary>
        public char CommandChar
        {
            get
            {
                return _settings.CommandChar;
            }
            set
            {
                _settings.CommandChar = value;
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
                return _settings.CommandDelimiter;
            }
            set
            {
                _settings.CommandDelimiter = value;
                RootModel.CommandDelimiter = value;
            }
        }

        /// <summary>
        /// Get AutoClearInput
        /// </summary>
        public bool AutoClearInput
        {
            get
            {
                return _settings.AutoClearInput;
            }
            set
            {
                _settings.AutoClearInput = value;
            }
        }

        /// <summary>
        /// Get AutoReconnect
        /// </summary>
        public bool AutoReconnect
        {
            get
            {
                return _settings.AutoReconnect;
            }
            set
            {
                _settings.AutoReconnect = value;
            }
        }

        /// <summary>
        /// Get MinLengthHistory
        /// </summary>
        public int MinLengthHistory
        {
            get
            {
                return _settings.MinLengthHistory;
            }
            set
            {
                _settings.MinLengthHistory = value;
            }
        }

        /// <summary>
        /// Get HistorySize
        /// </summary>
        public int HistorySize
        {
            get
            {
                return _settings.HistorySize;
            }
            set
            {
                _settings.HistorySize = value;
            }
        } 

        /// <summary>
        /// Gets the groups.
        /// </summary>
        [NotNull]
        public IList<Group> Groups
        {
            get
            {
                return ProfileHolder.Instance.Groups;
            }
        }

        #endregion

        #region Connection settings

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
                return _settings.ConnectPort;
            }
            set
            {
                _settings.ConnectPort = value;
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
                return _settings.ConnectHostName;
            }
            set
            {
                _settings.ConnectHostName = value;
            }
        }

        /// <summary>
        /// Gets the variables.
        /// </summary>
        /// <value>
        /// The variables.
        /// </value>
        [NotNull]
        public IList<Variable> Variables
        {
            get
            {
                return ProfileHolder.Instance.Variables;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves current settings.
        /// </summary>
        public void Save()
        {
            //_settings.MainOutputWindowSecondaryScrollHeight = MainOutputWindowSecondaryScrollHeight;
            ThemeManager.SaveSettings();
            _settings.ProfileName = ProfileHolder.Instance.Name;
            _settings.Save();
            //SaveGroups();
            //SaveVariables();
            ProfileHolder.Instance.Save();
        }

        #endregion

        #region Methods

        //[NotNull]
        //private static string GetSettingsFolder()
        //{
        //    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Adan client", "Settings");
        //}

        //private void SaveGroups()
        //{
        //    FileStream stream = null;
        //    try
        //    {
        //        if (!Directory.Exists(GetSettingsFolder()))
        //        {
        //            Directory.CreateDirectory(GetSettingsFolder());
        //        }

        //        stream = File.Open(Path.Combine(GetSettingsFolder(), "Settings.xml"), FileMode.Create, FileAccess.Write);
        //        using (var streamWriter = new XmlTextWriter(stream, Encoding.UTF8))
        //        {
        //            stream = null;
        //            streamWriter.Formatting = Formatting.Indented;
        //            _groupsSerializer.Serialize(streamWriter, _groups);
        //        }
        //    }
        //    finally
        //    {
        //        if (stream != null)
        //        {
        //            stream.Dispose();
        //        }
        //    }
        //}

        //private void ReadGroups()
        //{
        //    var settingsFileFullPath = Path.Combine(GetSettingsFolder(), "Settings.xml");
        //    if (!File.Exists(settingsFileFullPath))
        //    {
        //        _groups = new List<Group>();
        //        return;
        //    }

        //    using (var stream = File.OpenRead(settingsFileFullPath))
        //    {
        //        _groups = (IList<Group>)_groupsSerializer.Deserialize(stream);
        //    }
        //}

        //[NotNull]
        //private IList<Variable> ReadVariables()
        //{
        //    var variablesFileFullPath = Path.Combine(GetSettingsFolder(), "Variables.xml");
        //    if (!File.Exists(variablesFileFullPath))
        //    {
        //        _variables = new List<Variable>();
        //        return _variables;
        //    }

        //    using (var stream = File.OpenRead(variablesFileFullPath))
        //    {
        //        _variables = (IList<Variable>)_variablesSerializer.Deserialize(stream);
        //        return _variables;
        //    }
        //}

        //private void SaveVariables()
        //{
        //    FileStream stream = null;
        //    try
        //    {
        //        if (!Directory.Exists(GetSettingsFolder()))
        //        {
        //            Directory.CreateDirectory(GetSettingsFolder());
        //        }

        //        stream = File.Open(Path.Combine(GetSettingsFolder(), "Variables.xml"), FileMode.Create, FileAccess.Write);
        //        using (var streamWriter = new XmlTextWriter(stream, Encoding.UTF8))
        //        {
        //            stream = null;
        //            streamWriter.Formatting = Formatting.Indented;
        //            _variablesSerializer.Serialize(streamWriter, _variables);
        //        }
        //    }
        //    finally
        //    {
        //        if (stream != null)
        //        {
        //            stream.Dispose();
        //        }
        //    }
        //}

        #endregion
    }
}