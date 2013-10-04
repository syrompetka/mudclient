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
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Adan.Client.Common;
    using Common.Model;
    using Common.Themes;
    using CSLib.Net.Annotations;
    using Model.ActionParameters;
    using Model.Actions;
    using Properties;

    /// <summary>
    /// Class how hold settings like colors, windows sizes etc.
    /// </summary>
    public class SettingsHolder
    {
        #region Constants and Fields

        private static readonly SettingsHolder _instance = new SettingsHolder();
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
            MainOutputWindowSecondaryScrollHeight = _settings.MainOutputWindowSecondaryScrollHeight;
            ConnectHostName = _settings.ConnectHostName;
            ConnectPort = _settings.ConnectPort;

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
                                typeof(ConstantStringParameter)
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

            ProfileHolder.Instance.Initialize(types);
            ProfileHolder.Instance.Name = _settings.ProfileName;
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

        ///// <summary>
        ///// TODO: Profile Name
        ///// </summary>
        ////[NotNull]
        //public string ProfileName
        //{
        //    get
        //    {
        //        return _settings.ProfileName;
        //    }
        //    //set
        //    //{
        //    //    //Save();

        //    //    _settings.ProfileName = value;

        //    //    //ReadVariables();
        //    //    //ReadGroups();

        //    //    //if (SettingsChanged != null)
        //    //        //SettingsChanged(this, EventArgs.Empty);
        //    //}
        //}


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
            _settings.MainOutputWindowSecondaryScrollHeight = MainOutputWindowSecondaryScrollHeight;
            ThemeManager.SaveSettings();
            _settings.ProfileName = ProfileHolder.Instance.Name;
            _settings.Save();
            //SaveGroups();
            //SaveVariables();
            ProfileHolder.Instance.Save();
        }

        #endregion

        #region Methods

        [NotNull]
        private static string GetSettingsFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Adan client", "Settings");
        }

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