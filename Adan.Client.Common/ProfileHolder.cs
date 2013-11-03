using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using Adan.Client.Common.Commands;
using Adan.Client.Common.Controls;
using Adan.Client.Common.Model;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.Common
{
    /// <summary>
    /// Class how hold settings like triggers, actions etc.
    /// </summary>
    public class ProfileHolder
    {
        private static readonly ProfileHolder _instance = new ProfileHolder();

        private XmlSerializer _groupsSerializer;
        private XmlSerializer _variablesSerializer;
        private XmlSerializer _commonSerializer;
        private IList<Group> _groups;
        private IList<Variable> _variables;
        private string _name;
        private bool _firstTime = true;
        private IList<string> _profiles;
        private SettingsFolder _settingsFolder;
        //private static object _lockobject = new object();

        /// <summary>
        /// Initialize class
        /// </summary>
        /// <param name="types">Types</param>
        public void Initialize([NotNull] IList<Type> types)
        {
            Assert.ArgumentNotNull(types, "types");

            _groupsSerializer = new XmlSerializer(typeof(List<Group>), types.ToArray());
            _variablesSerializer = new XmlSerializer(typeof(List<Variable>));
            _commonSerializer = new XmlSerializer(typeof(CommonProfileSettings));

            LoadAllProfiles();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        [NotNull]
        public static ProfileHolder Instance
        {
            get
            {
               // lock(_lockobject)
                    return _instance;
            }
        }

        /// <summary>
        /// Get Settings Folder
        /// </summary>
        public SettingsFolder SettingsFolder
        {
            get
            {
                return _settingsFolder;
            }
            set
            {
                _settingsFolder = value;

                if (!_firstTime)
                {
                    LoadAllProfiles();

                    if (!AllProfiles.Contains(Name))
                        AllProfiles.Add(Name);
                    else if (!AllProfiles.Contains("Default"))
                        CreateNewProfile("Default");
                }
            }
        }

        /// <summary>
        /// Get Settings Folder Folder
        /// </summary>
        public string Folder
        {
            get
            {
                string dir = String.Empty;
                switch (SettingsFolder)
                {
                    case SettingsFolder.DocumentsAndSettings:
                        dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Adan client");
                        break;
                    case SettingsFolder.ProgramFolder:
                        dir = Path.Combine(Environment.CurrentDirectory);
                        break;
                }

                return dir;
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
                if (_groups == null)
                {
                    ReadGroups();
                }

                return _groups ?? new List<Group>();
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
                if(_variables == null)
                    ReadVariables();

                return _variables ?? new List<Variable>();
            }
        }

        /// <summary>
        /// Profile name.
        /// </summary>
        [NotNull]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value)
                    return;

                if (!_firstTime)
                    Save();
                else
                    _firstTime = false;

                _name = value;

                if (!AllProfiles.Contains(value))
                    CreateNewProfile(value);
                else
                {
                    ReadVariables();
                    ReadGroups();
                    ReadCommonSettings();
                }


                //if (SettingsChanged != null)
                //SettingsChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CommonProfileSettings CommonSettings
        {
            get;
            private set;
        }

        /// <summary>
        /// Get all profiles
        /// </summary>
        public IList<string> AllProfiles
        {
            get
            {
                if (_profiles == null)
                    _profiles = new List<string>();

                return _profiles;
            }
            private set
            {
                _profiles = value;
            }
        }

        /// <summary>
        /// Saves current settings.
        /// </summary>
        public void Save()
        {
            SaveGroups();
            SaveVariables();
            SaveCommonSettings();
        }

        /// <summary>
        /// Create new profile folder
        /// </summary>
        /// <param name="name">Name profile</param>
        public void CreateNewProfile(string name)
        {
            FileStream stream = null;
            string dir = GetProfileSettingsFolder(name);
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                stream = File.Open(Path.Combine(dir, "Settings.xml"), FileMode.Create, FileAccess.Write);
                using (var streamWriter = new XmlTextWriter(stream, Encoding.Default))
                {
                    stream = null;
                    streamWriter.Formatting = Formatting.Indented;
                    var gr = new List<Group>();
                    gr.Add(new Group() { Name = "Default", IsEnabled = true, IsBuildIn = true });
                    _groupsSerializer.Serialize(streamWriter, gr);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                stream = File.Open(Path.Combine(dir, "Variables.xml"), FileMode.Create, FileAccess.Write);
                using (var streamWriter = new XmlTextWriter(stream, Encoding.Default))
                {
                    stream = null;
                    streamWriter.Formatting = Formatting.Indented;
                    _variablesSerializer.Serialize(streamWriter, null);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            try
            {
                if (!Directory.Exists(GetProfileSettingsFolder()))
                {
                    Directory.CreateDirectory(GetProfileSettingsFolder());
                }

                stream = File.Open(Path.Combine(GetProfileSettingsFolder(), "Common.xml"), FileMode.Create, FileAccess.Write);
                using (var streamWriter = new XmlTextWriter(stream, Encoding.Default))
                {
                    streamWriter.Formatting = Formatting.Indented;
                    var profileSettings = new CommonProfileSettings() 
                    {
                        MultiAction = false 
                    };
                    _commonSerializer.Serialize(streamWriter, profileSettings);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            AllProfiles.Add(name);
        }

        /// <summary>
        /// Delete Profile folder
        /// </summary>
        /// <param name="name">Name profile</param>
        public void DeleteProfile(string name)
        {
            var dir = GetProfileSettingsFolder(name);
            if (Directory.Exists(dir))
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK);
                }
            }
            
            AllProfiles.Remove(name);
        }

        /// <summary>
        /// Import jmc config to current profile
        /// </summary>
        /// <param name="file">File name</param>
        /// <param name="rootModel">Root Model</param>
        public void ImportJmcConfig(string file, RootModel rootModel)
        {
            if (!File.Exists(file))
                return;

            Task.Factory.StartNew(() =>
                {
                    using (var stream = new StreamReader(file, Encoding.Default, false, 1024))
                    {
                        string line;
                        while ((line = stream.ReadLine()) != null)
                        {
                            //XML не читает символ \x001B
                            //TODO: Need FIX IT
                            if (!line.Contains("\x001B"))
                                rootModel.PushCommandToConveyor(new TextCommand(line));
                        }
                    }
                });
        }

        [NotNull]
        private string GetProfileSettingsFolder(string name = null)
        {
            return Path.Combine(GetSettingsFolder(), name == null ? Name : name);
        }

        [NotNull]
        private string GetSettingsFolder()
        {
            string dir = Path.Combine(Folder, "Settings");

            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (Exception) { }
            }
            return dir;
        }

        private void SaveGroups()
        {
            FileStream stream = null;
            try
            {
                if (!Directory.Exists(GetProfileSettingsFolder()))
                {
                    Directory.CreateDirectory(GetProfileSettingsFolder());
                }

                stream = File.Open(Path.Combine(GetProfileSettingsFolder(), "Settings.xml"), FileMode.Create, FileAccess.Write);
                using (var streamWriter = new XmlTextWriter(stream, Encoding.Default))
                {
                    stream = null;
                    streamWriter.Formatting = Formatting.Indented;
                    _groupsSerializer.Serialize(streamWriter, _groups);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        private void ReadCommonSettings()
        {
            var commonFileFullPath = Path.Combine(GetProfileSettingsFolder(), "Common.xml");
            if (!File.Exists(commonFileFullPath))
            {
                CommonSettings = new CommonProfileSettings() { MultiAction = false };
                return;
            }

            using (var stream = File.OpenRead(commonFileFullPath))
            {
                CommonSettings = (CommonProfileSettings) _commonSerializer.Deserialize(stream);
            }
        }

        private void SaveCommonSettings()
        {
            FileStream stream = null;
            try
            {
                if (!Directory.Exists(GetProfileSettingsFolder()))
                {
                    Directory.CreateDirectory(GetProfileSettingsFolder());
                }

                stream = File.Open(Path.Combine(GetProfileSettingsFolder(), "Common.xml"), FileMode.Create, FileAccess.Write);
                using (var streamWriter = new XmlTextWriter(stream, Encoding.Default))
                {
                    streamWriter.Formatting = Formatting.Indented;
                    _commonSerializer.Serialize(streamWriter, CommonSettings);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        private void ReadGroups()
        {
            var settingsFileFullPath = Path.Combine(GetProfileSettingsFolder(), "Settings.xml");
            if (!File.Exists(settingsFileFullPath))
            {
                _groups = new List<Group>();
                return;
            }

            using (var stream = File.OpenRead(settingsFileFullPath))
            {
                try
                {
                    _groups = (IList<Group>)_groupsSerializer.Deserialize(stream);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ошибка", e.Message, MessageBoxButton.OK);
                }
            }
        }

        private void ReadVariables()
        {
            var variablesFileFullPath = Path.Combine(GetProfileSettingsFolder(), "Variables.xml");
            if (!File.Exists(variablesFileFullPath))
            {
                _variables = new List<Variable>();
            }

            using (var stream = File.OpenRead(variablesFileFullPath))
            {
                try
                {
                    _variables = (IList<Variable>)_variablesSerializer.Deserialize(stream);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ошибка", e.Message, MessageBoxButton.OK);
                }
            }
        }

        private void SaveVariables()
        {
            FileStream stream = null;
            try
            {
                if (!Directory.Exists(GetProfileSettingsFolder()))
                {
                    Directory.CreateDirectory(GetProfileSettingsFolder());
                }

                stream = File.Open(Path.Combine(GetProfileSettingsFolder(), "Variables.xml"), FileMode.Create, FileAccess.Write);
                using (var streamWriter = new XmlTextWriter(stream, Encoding.Default))
                {
                    streamWriter.Formatting = Formatting.Indented;
                    _variablesSerializer.Serialize(streamWriter, _variables);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        private void LoadAllProfiles()
        {
            AllProfiles.Clear();

            var root = GetSettingsFolder();
            if (Directory.Exists(root))
            {
                foreach (string dir in Directory.GetDirectories(root))
                {
                    if (File.Exists(Path.Combine(dir, "Settings.xml")))
                    {
                        AllProfiles.Add(new DirectoryInfo(dir).Name);
                    }
                }
            }
        }
    }
}
