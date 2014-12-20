using Adan.Client.Common.Conveyor;
using Adan.Client.Common.Model;
using Adan.Client.Common.Utils;
using CSLib.Net.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Adan.Client.Common.Settings
{
    /// <summary>
    /// Class how hold settings like triggers, actions etc.
    /// </summary>
    public class ProfileHolder
    {
        #region Constants and Fields

        private ConcurrentBag<Group> _groups;
        private ConcurrentBag<Variable> _variables;
        private string _name;
        private List<string> _commandsHistory;

        #endregion

        #region Constructors and Destuctors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public ProfileHolder(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public List<string> CommandsHistory
        {
            get
            {
                if (_commandsHistory == null)
                    _commandsHistory = new List<string>();

                return _commandsHistory;
            }
            private set
            {
                _commandsHistory = value;
            }
        }

        /// <summary>
        /// Gets the groups.
        /// </summary>
        [NotNull]
        public ConcurrentBag<Group> Groups
        {
            get
            {
                if (_groups == null)
                {
                    ReadGroups();
                }

                return _groups;
            }
            set
            {
                _groups = value;
            }
        }

        /// <summary>
        /// Gets the variables.
        /// </summary>
        /// <value>
        /// The variables.
        /// </value>
        [NotNull]
        public ConcurrentBag<Variable> Variables
        {
            get
            {
                if(_variables == null)
                    ReadVariables();

                return _variables;
            }
            set
            {
                _variables = value;
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
            private set
            {
                _name = value;
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves current settings.
        /// </summary>
        public void Save()
        {
            SaveGroups();
            SaveVariables();
            SaveCommonSettings();
            SaveCommandHistory();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReloadProfile()
        {
            ReadGroups();
            ReadVariables();
            ReadCommonSettings();
            ReadCommandsHistory();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="profile"></param>
        public static void ImportJmcConfig(string file, ProfileHolder profile)
        {
            if (!File.Exists(file))
                return;

            //Task.Factory.StartNew(() =>
            //    {
            RootModel rootModel = new RootModel(profile);
                    try
                    {
                        using (var stream = new StreamReader(file, Encoding.Default, false, 1024))
                        {
                            string line;
                            while ((line = stream.ReadLine()) != null)
                            {
                                //XML не читает символ \x01B
                                //TODO: Need FIX IT
                                if (!line.Contains("\x001B"))
                                {
                                    //rootModel.PushCommandToConveyor(new TextCommand(line))
                                    MessageConveyor.ImportJMC(line, rootModel);
                                }
                            }
                        }
                    }
                    catch 
                    {
                        return;
                    }

                    //SettingsHolder.Instance.SetProfile(rootModel.Profile);
                //});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ProfileHolder Clone()
        {
            return new ProfileHolder(this.Name)
                {
                    _groups = new ConcurrentBag<Group>(this.Groups),
                    _variables = new ConcurrentBag<Variable>(this.Variables),
                    CommonSettings = this.CommonSettings,
                    CommandsHistory = new List<string>(this.CommandsHistory),
                };
        }

        #endregion

        #region Private Methods

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
                try
                {
                    var serializer = new XmlSerializer(typeof(CommonProfileSettings));

                    CommonSettings = (CommonProfileSettings)serializer.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Instance.Write(string.Format("Error read common settings: {0}\r\n{1}", ex.Message, ex.StackTrace));
                }
            }
        }

        private void SaveCommonSettings()
        {
            if (!Directory.Exists(GetProfileSettingsFolder()))
            {
                Directory.CreateDirectory(GetProfileSettingsFolder());
            }

            using (var stream = File.Open(Path.Combine(GetProfileSettingsFolder(), "Common.xml"), FileMode.Create, FileAccess.Write))
            using (var streamWriter = new XmlTextWriter(stream, Encoding.Default))
            {
                streamWriter.Formatting = Formatting.Indented;

                try
                {
                    var serializer = new XmlSerializer(typeof(CommonProfileSettings));
                    serializer.Serialize(streamWriter, CommonSettings);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Instance.Write(string.Format("Error save common settings: {0}\r\n{1}", ex.Message, ex.StackTrace));
                }
            }
        }

        private void ReadGroups()
        {
            var settingsFileFullPath = Path.Combine(GetProfileSettingsFolder(), "Settings.xml");

            if (!File.Exists(settingsFileFullPath))
            {
                Groups = new ConcurrentBag<Group>();
                Groups.Add(new Group() { Name = "Default", IsBuildIn = true, IsEnabled = true });
                return;
            }

            using (var stream = File.OpenRead(settingsFileFullPath))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(ConcurrentBag<Group>), SettingsHolder.Instance.AllSerializationTypes.ToArray());
                    Groups = (ConcurrentBag<Group>)serializer.Deserialize(stream);
                    var defGroup = Groups.FirstOrDefault(group => group.Name == "Default");
                    if (defGroup == null)
                    {
                        Groups.Add(new Group() { Name = "Default", IsBuildIn = true, IsEnabled = true });
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.Instance.Write(string.Format("Error read groups: {0}\r\n{1}", ex.Message, ex.StackTrace));

                    Groups = new ConcurrentBag<Group>();
                    Groups.Add(new Group() { Name = "Default", IsBuildIn = true, IsEnabled = true });
                }
            }
        }

        private void SaveGroups()
        {
            if (!Directory.Exists(GetProfileSettingsFolder()))
            {
                Directory.CreateDirectory(GetProfileSettingsFolder());
            }

            using (var stream = File.Open(Path.Combine(GetProfileSettingsFolder(), "Settings.xml"), FileMode.Create, FileAccess.Write))
            using (var streamWriter = new XmlTextWriter(stream, Encoding.Default))
            {
                streamWriter.Formatting = Formatting.Indented;

                try
                {
                    var serializer = new XmlSerializer(typeof(List<Group>), SettingsHolder.Instance.AllSerializationTypes.ToArray());
                    serializer.Serialize(streamWriter, Groups);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Instance.Write(string.Format("Error save groups: {0}\r\n{1}", ex.Message, ex.StackTrace));
                }
            }
        }

        private void ReadVariables()
        {
            var variablesFileFullPath = Path.Combine(GetProfileSettingsFolder(), "Variables.xml");
            if (!File.Exists(variablesFileFullPath))
            {
                Variables = new ConcurrentBag<Variable>();
                return;
            }

            using (var stream = File.OpenRead(variablesFileFullPath))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(ConcurrentBag<Variable>));
                    Variables = (ConcurrentBag<Variable>)serializer.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Instance.Write(string.Format("Error read variables: {0}\r\n{1}", ex.Message, ex.StackTrace));

                    Variables = new ConcurrentBag<Variable>();
                }
            }
        }

        private void SaveVariables()
        {
            if (!Directory.Exists(GetProfileSettingsFolder()))
            {
                Directory.CreateDirectory(GetProfileSettingsFolder());
            }

            using (var stream = File.Open(Path.Combine(GetProfileSettingsFolder(), "Variables.xml"), FileMode.Create, FileAccess.Write))
            using (var streamWriter = new XmlTextWriter(stream, Encoding.Default))
            {
                streamWriter.Formatting = Formatting.Indented;

                try
                {
                    var serializer = new XmlSerializer(typeof(List<Variable>));
                    serializer.Serialize(streamWriter, Variables);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Instance.Write(string.Format("Error save variables: {0}\r\n{1}", ex.Message, ex.StackTrace));
                }
            }
        }

        private void ReadCommandsHistory()
        {
            var commandsHistoryPath = Path.Combine(GetProfileSettingsFolder(), "History.xml");

            if (!File.Exists(commandsHistoryPath))
            {
                CommandsHistory = new List<string>();
                return;
            }

            using (var stream = File.OpenRead(commandsHistoryPath))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(List<string>));
                    CommandsHistory = (List<string>) serializer.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Instance.Write(string.Format("Error read command history: {0}\r\n{1}", ex.Message, ex.StackTrace));

                    CommandsHistory = new List<string>();
                }
            }
        }

        private void SaveCommandHistory()
        {
            if (!Directory.Exists(GetProfileSettingsFolder()))
            {
                Directory.CreateDirectory(GetProfileSettingsFolder());
            }

            using (var stream = File.Open(Path.Combine(GetProfileSettingsFolder(), "History.xml"), FileMode.Create, FileAccess.Write))
            using (var streamWriter = new XmlTextWriter(stream, Encoding.Default))
            {
                streamWriter.Formatting = Formatting.Indented;

                try
                {
                    var serializer = new XmlSerializer(typeof(List<string>));
                    serializer.Serialize(streamWriter, CommandsHistory);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Instance.Write(string.Format("Error save command history: {0}\r\n{1}", ex.Message, ex.StackTrace));
                }
            }
        }

        [NotNull]
        private string GetProfileSettingsFolder(string name = null)
        {
            return Path.Combine(GetSettingsFolder(), name == null ? Name : name);
        }

        [NotNull]
        private string GetSettingsFolder()
        {
            string dir = Path.Combine(SettingsHolder.Instance.Folder, "Settings");

            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (Exception ex) 
                { 
                    ErrorLogger.Instance.Write(string.Format("Error create settings directory: {0}\r\n{1]", ex.Message, ex.StackTrace));
                }
            }
            return dir;
        }

        #endregion
    }
}
