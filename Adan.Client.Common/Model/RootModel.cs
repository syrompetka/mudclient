// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the RootModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Adan.Client.Common.Settings;
    using Commands;
    using Conveyor;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;
    using Plugins;
    using Properties;

    /// <summary>
    /// A root object for all model objects.
    /// </summary>
    public class RootModel
    {
        #region #Constants and Fields

        private static IList<Type> _customSerializationTypes;

        private MessageConveyor _conveyor;
        private List<TriggerBase> _enabledTriggersOrderedByPriority;
        private ProfileHolder _profile;
        private object _profileLockObject = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conveyor"></param>
        /// <param name="profile"></param>
        public RootModel([NotNull] MessageConveyor conveyor, ProfileHolder profile) : this(profile)
        {
            Assert.ArgumentNotNull(conveyor, "conveyor");

            _profile = profile;
            MessageConveyor = conveyor;

            SettingsHolder.Instance.ProfilesChanged += OnProfileChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        public RootModel(ProfileHolder profile)
        {
            _profile = profile;

            //TODO: Разобраться с кол-вом монстров, персонажей в группе
            GroupStatus = new List<CharacterStatus>(10);
            RoomMonstersStatus = new List<MonsterStatus>(20);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public static IList<ParameterDescription> AllParameterDescriptions
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IList<ActionDescription> AllActionDescriptions
        {
            get;
            set;
        }

        /// <summary>
        /// Get char delimiter
        /// </summary>
        [NotNull]
        public static char CommandDelimiter
        {
            get;
            set;
        }

        /// <summary>
        /// Get char commands
        /// </summary>
        [NotNull]
        public static char CommandChar
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the custom serialization types.
        /// </summary>
        [NotNull]
        public static IList<Type> CustomSerializationTypes
        {
            get
            {
                if (_customSerializationTypes == null)
                    _customSerializationTypes = new List<Type>();

                return _customSerializationTypes;
            }
            private set
            {
                _customSerializationTypes = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Uid
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsLogging
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public MessageConveyor MessageConveyor
        {
            get
            {
                return _conveyor;
            }
            private set
            {
                _conveyor = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Connected
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ConnectionInProgress
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ProfileHolder Profile
        {
            get
            {
                lock (_profileLockObject)
                {
                    return _profile;
                }
            }
            set
            {
                lock (_profileLockObject)
                {
                    _profile = value;
                }
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
                lock (_profileLockObject)
                {
                    return Profile.Groups;
                }
            }
        }

        /// <summary>
        /// Gets variables
        /// </summary>
        [NotNull]
        public IList<Variable> Variables
        {
            get
            {
                lock (_profileLockObject)
                {
                    return Profile.Variables;
                }
            }
        }

        /// <summary>
        /// Gets the group status.
        /// </summary>
        [NotNull]
        public IList<CharacterStatus> GroupStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the selected group mate.
        /// </summary>
        /// <value>
        /// The selected group mate.
        /// </value>
        [CanBeNull]
        public CharacterStatus SelectedGroupMate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the room monsters status.
        /// </summary>
        [NotNull]
        public IList<MonsterStatus> RoomMonstersStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the selected room monster.
        /// </summary>
        /// <value>
        /// The selected room monster.
        /// </value>
        [CanBeNull]
        public MonsterStatus SelectedRoomMonster
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the enabled triggers ordered by priority.
        /// </summary>
        [NotNull]
        public IEnumerable<TriggerBase> EnabledTriggersOrderedByPriority
        {
            get
            {
                return _enabledTriggersOrderedByPriority ?? RecalculatedEnabledTriggersPriorities();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Pushes the command to conveyor.
        /// </summary>
        /// <param name="command">The command.</param>
        public void PushCommandToConveyor([NotNull] Command command)
        {
            Assert.ArgumentNotNull(command, "command");

            if(_conveyor != null)
                _conveyor.PushCommand(command);
        }

        /// <summary>
        /// Pushes the message to conveyor.
        /// </summary>
        /// <param name="message">The message.</param>
        public void PushMessageToConveyor([NotNull] Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            if(_conveyor != null)
                _conveyor.PushMessage(message);
        }

        /// <summary>
        /// Sets the variable value.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="value">The value.</param>
        /// <param name="isSilent">IsSilent</param>
        public void SetVariableValue([NotNull]string variableName, [NotNull] string value, [NotNull] bool isSilent)
        {
            Assert.ArgumentNotNullOrEmpty(variableName, "variableName");
            Assert.ArgumentNotNull(value, "value");

            var v = Variables.FirstOrDefault(var => var.Name == variableName);

            if (v != null)
                v.Value = value;
            else
                Variables.Add(new Variable() { Name = variableName, Value = value });

            if(!isSilent)
                _conveyor.PushMessage(new InfoMessage(string.Format(CultureInfo.InvariantCulture, Resources.VariableValueSet, variableName, value)));
        }

        /// <summary>
        /// Gets the variable value.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <returns>
        /// The value if speciefed variable or empty string if there is no such one.
        /// </returns>
        [NotNull]
        public string GetVariableValue([NotNull] string variableName)
        {
            Assert.ArgumentNotNullOrEmpty(variableName, "variableName");

            var v = Variables.FirstOrDefault(var => var.Name == variableName);
            if (v != null)
                return v.Value;

            if (variableName.Equals("DATE", StringComparison.InvariantCultureIgnoreCase))
            {
                return DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            else if (variableName.Equals("TIME", StringComparison.InvariantCultureIgnoreCase))
            {
                return DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }

        /// <summary>
        /// Clears the variable value.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="isSilent">Is Silent</param>
        public void ClearVariableValue([NotNull] string variableName, bool isSilent)
        {
            Assert.ArgumentNotNullOrEmpty(variableName, "variableName");

            var v = Variables.FirstOrDefault(var => var.Name == variableName);
            if (v != null)
                Variables.Remove(v);

            if(!isSilent)
                _conveyor.PushMessage(new InfoMessage(string.Format(CultureInfo.InvariantCulture, Resources.VariableValueClear, variableName)));
        }

        /// <summary>
        /// Enables the group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        public void EnableGroup([NotNull]string groupName)
        {
            Assert.ArgumentNotNullOrEmpty(groupName, "groupName");
            var group = Groups.FirstOrDefault(gr => gr.Name == groupName);
            if (group != null && !group.IsBuildIn)
            {
                group.IsEnabled = true;
                PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.InvariantCulture, Resources.GroupEnabled, group.Name)));
            }

            RecalculatedEnabledTriggersPriorities();
        }

        /// <summary>
        /// Disables the group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        public void DisableGroup([NotNull]string groupName)
        {
            Assert.ArgumentNotNullOrEmpty(groupName, "groupName");
            var group = Groups.FirstOrDefault(gr => gr.Name == groupName);
            if (group != null && !group.IsBuildIn)
            {
                group.IsEnabled = false;
                PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.InvariantCulture, Resources.GroupDisabled, group.Name)));
            }

            RecalculatedEnabledTriggersPriorities();
        }

        /// <summary>
        /// Add the group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        public void AddGroup([NotNull]string groupName)
        {
            Assert.ArgumentNotNull(groupName, "groupName");
            var group = Groups.FirstOrDefault(gr => gr.Name == groupName);
            if (group == null)
            {
                Groups.Add(new Group() { Name = groupName, IsEnabled = true, IsBuildIn = false });
            }
        }

        /// <summary>
        /// Delete the group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        public void DeleteGroup([NotNull]string groupName)
        {
            Assert.ArgumentNotNull(groupName, "groupName");
            var group = Groups.FirstOrDefault(gr => gr.Name == groupName);
            if (group != null && !group.IsBuildIn)
            {
                if (!Groups.Remove(group))
                    PushMessageToConveyor(new ErrorMessage("#Ошибка удаления группы"));
            }
        }

        /// <summary>
        /// Recalculateds the enabled triggers priorities.
        /// </summary>
        /// <returns>A collection of enable triggers ordered by priority.</returns>
        [NotNull]
        public IEnumerable<TriggerBase> RecalculatedEnabledTriggersPriorities()
        {
            return _enabledTriggersOrderedByPriority = Groups.Where(g => g.IsEnabled).SelectMany(g => g.Triggers).OrderBy(trg => trg.Priority).ToList();
        }


        #endregion

        private void OnProfileChanged(object sender, SettingsChangedEventArgs e)
        {
            if (e.Name == Profile.Name)
            {
                Profile = SettingsHolder.Instance.GetProfile(e.Name);
                RecalculatedEnabledTriggersPriorities();
            }
        }
    }
}
