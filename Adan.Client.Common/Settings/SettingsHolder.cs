namespace Adan.Client.Common.Settings
{
    using Controls;
    using Model;
    using Themes;
    using CSLib.Net.Annotations;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Xml.Serialization;

    /// <summary>
    /// Class whos hold settings like colors, windows sizes etc.
    /// </summary>
    public class SettingsHolder
    {
        #region Constants and Fields
        private static readonly SettingsHolder _instance = new SettingsHolder();

        private SettingsFolder _settingsFolder;
        private readonly IList<ProfileHolder> _profiles;
        private IList<string> _allProfiles;
        private string _folder;

        /// <summary>
        /// This event will be raised when a profile is changed via SetProfile
        /// </summary>
        public event EventHandler<ProfileChangedEventArgs> ProfilesChanged;

        /// <summary>
        /// This event will be raised when where is a non-critical error with Settings
        /// </summary>
        public event EventHandler<SettingsErrorEventArgs> ErrorOccurred;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// 
        /// </summary>
        public SettingsHolder()
        {
            _profiles = new List<ProfileHolder>();
            _allProfiles = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsHolder"/> class.
        /// </summary>
        public void Initialize(SettingsFolder settingsFolder, IList<Type> types)
        {
            SettingsFolder = settingsFolder;
            AllSerializationTypes = types;

            ReloadSettings();
            LoadAllProfiles();
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
                return _instance ?? new SettingsHolder();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<string> AllProfiles
        {
            get
            {
                if (_allProfiles == null)
                {
                    _allProfiles = new List<string>();
                }

                return _allProfiles;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<Type> AllSerializationTypes
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public SettingsSerializer Settings
        {
            get;
            private set;
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

                switch (SettingsFolder)
                {
                    case SettingsFolder.DocumentsAndSettings:
                        _folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Adan client");
                        break;
                    case SettingsFolder.ProgramFolder:
                        _folder = Path.Combine(Environment.CurrentDirectory);
                        break;
                }

                LoadAllProfiles();
            }
        }

        /// <summary>
        /// Get Settings Folder
        /// </summary>
        public string Folder
        {
            get
            {
                return _folder;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        public void ReloadSettings()
        {
            if (!File.Exists(Path.Combine(Folder, "Options.xml")))
            {
                LoadDefaultSettings();

                return;
            }

            var serializer = new XmlSerializer(typeof(SettingsSerializer), AllSerializationTypes.ToArray());

            try
            {
                using (var stream = File.OpenRead(Path.Combine(Folder, "Options.xml")))
                {
                    Settings = (SettingsSerializer)serializer.Deserialize(stream);
                    if (Settings.GlobalGroups.Count == 0)
                        Settings.GlobalGroups.Add(new Group() { IsBuildIn = true, Name = "GlobalDefault", IsEnabled = true });

                    if (Settings.GlobalHotkeys != null && Settings.GlobalHotkeys.Count > 0)
                    {
                        var defaultGroup = Settings.GlobalGroups.FirstOrDefault(g => g.IsBuildIn);
                        foreach (var hk in Settings.GlobalHotkeys)
                        {
                            defaultGroup.Hotkeys.Add(hk);
                        }

                        Settings.GlobalHotkeys.Clear();
                    }

                }
            }
            catch(Exception)
            {
                LoadDefaultSettings();
            }
        }

        private void LoadDefaultSettings()
        {
            Settings = new SettingsSerializer()
            {
                AutoClearInput = false,
                AutoReconnect = false,
                CommandChar = '#',
                CommandDelimiter = ';',
                ConnectHostName = "adan.ru",
                ConnectPort = 4000,
                CursorPosition = CursorPositionHistory.EndOfLine,
                CommandsHistorySize = 1000,
                MainOutputWindowSecondaryScrollHeight = 500,
                MinLengthHistory = 2,
                ScrollBuffer = 5000,
                IsLogCommands = false,
                AutoConnect = true,
                MainWindowLeft = 0,
                MainWindowTop = 0,
                MainWindowWidth = 800,
                MainWindowHeight = 600,
                MainWindowState = WindowState.Maximized,
            };
        }

        /// <summary>
        /// Saves current settings.
        /// </summary>
        public void SaveAllSettings()
        {
            SaveCommonSettings();
            SaveProfiles();
            ThemeManager.SaveSettings();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveCommonSettings()
        {
            var serializer = new XmlSerializer(typeof(SettingsSerializer), AllSerializationTypes.ToArray());

            if (File.Exists(Path.Combine(Folder, "Options.xml")))
            {
                File.Delete(Path.Combine(Folder, "Options.xml"));
            }

            using (var stream = File.OpenWrite(Path.Combine(Folder, "Options.xml")))
            {
                serializer.Serialize(stream, Settings);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveProfiles()
        {
            lock (_profiles)
            {
                foreach (var profile in _profiles)
                    profile.Save();
            }
        }

        /// <summary>
        /// Create new profile folder
        /// </summary>
        /// <param name="name">Name profile</param>
        public void CreateNewProfile(string name)
        {
            if (_allProfiles.Contains(name))
                return;

            lock (_profiles)
            {
                var newProfile = new ProfileHolder(name);
                newProfile.ErrorOccurred += (s, e) => { if (this.ErrorOccurred != null) this.ErrorOccurred(s, e); };
                _profiles.Add(newProfile);
            }

            _allProfiles.Add(name);
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
                catch (Exception)
                { }
            }

            lock (_profiles)
            {
                var profile = _profiles.FirstOrDefault(prof => prof.Name == name);
                if (profile != null)
                    _profiles.Remove(profile);
            }

            _allProfiles.Remove(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ProfileHolder GetProfile(string name)
        {
            lock (_profiles)
            {
                var retVal = _profiles.FirstOrDefault(profile => profile.Name == name);

                if (retVal == null)
                {
                    retVal = new ProfileHolder(name);
                    retVal.ErrorOccurred += (s, e) => { if (this.ErrorOccurred != null) this.ErrorOccurred(s, e); };
                    _profiles.Add(retVal);

                    if (_allProfiles.Contains(name))
                        retVal.ReloadProfile();
                    else
                        _allProfiles.Add(name);
                }

                //return retVal.Clone();
                return retVal;
            }
        }

        /// <summary>
        /// A profile has been changed
        /// </summary>
        /// <param name="name">Name of the profile that has been changed</param>
        public void SetProfile(string name)
        {
            if (ProfilesChanged != null)
            {
                ProfilesChanged(this, new ProfileChangedEventArgs(name, name=="Global"));
            }

            if (name == "Global")
            {
                SaveCommonSettings();
            }
            else
            {
                var profile = _profiles.FirstOrDefault(p => p.Name == name);
                profile.Save();
            }
        }

        #endregion

        #region Private Methods

        [NotNull]
        private string GetProfileSettingsFolder(string name)
        {
            return Path.Combine(GetSettingsFolder(), name);
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
                catch { }
            }

            return dir;
        }

        private void LoadAllProfiles()
        {
            lock (_profiles)
            {
                _profiles.Clear();
            }

            _allProfiles.Clear();

            var root = GetSettingsFolder();
            if (Directory.Exists(root))
            {
                try
                {

                    foreach (string dir in Directory.GetDirectories(root))
                    {
                        if (File.Exists(Path.Combine(dir, "Settings.xml")))
                        {
                            _allProfiles.Add(new DirectoryInfo(dir).Name);
                        }
                    }
                }
                catch { }

                if (_allProfiles.Count == 0)
                {
                    CreateNewProfile("Default");
                }
            }
        }

        #endregion
    }
}