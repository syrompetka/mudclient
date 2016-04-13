﻿namespace Adan.Client.Common.Model
{
    using Settings;
    using Commands;
    using Conveyor;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;
    using Plugins;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A root object for all model objects.
    /// </summary>
    public class RootModel : IDisposable
    {
        #region Constants and Fields

        private MessageConveyor _conveyor;
        private List<TriggerBase> _enabledTriggersOrderedByPriority;
        private readonly string _name;
        private ProfileHolder _profile;
        private readonly IList<RootModel> _allModels;
        private readonly object _profileLockObject = new object();

        private readonly List<CommandAlias> _aliasList;
        private readonly List<TriggerBase> _triggersList;
        private readonly List<Highlight> _highlightList;
        private readonly List<Substitution> _substitutionList;
        private readonly List<Hotkey> _hotkeyList;
        private readonly List<Variable> _variableList;

        private List<CharacterStatus> _groupStatus;
        private List<MonsterStatus> _monsterStatus;

        private readonly Stack<IUndo> _undoStack;

        private readonly Regex _variableRegex = new Regex(@"\$(\w+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        #endregion

        #region Constructors and Destructors

        public RootModel([NotNull] MessageConveyor conveyor, ProfileHolder profile, IList<RootModel> allModels)
        {
            Assert.ArgumentNotNull(conveyor, "conveyor");

            _aliasList = new List<CommandAlias>();
            _triggersList = new List<TriggerBase>();
            _highlightList = new List<Highlight>();
            _substitutionList = new List<Substitution>();
            _hotkeyList = new List<Hotkey>();
            _variableList = new List<Variable>();

            _undoStack = new Stack<IUndo>();

            _name = profile.Name;
            _profile = profile;
            _allModels = allModels;
            MessageConveyor = conveyor;

            GroupStatus = new List<CharacterStatus>();
            RoomMonstersStatus = new List<MonsterStatus>();

            SettingsHolder.Instance.ProfilesChanged += OnProfileChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        public RootModel(ProfileHolder profile)
        {
            _name = profile.Name;
            _aliasList = new List<CommandAlias>();
            _triggersList = new List<TriggerBase>();
            _highlightList = new List<Highlight>();
            _substitutionList = new List<Substitution>();
            _hotkeyList = new List<Hotkey>();
            _variableList = new List<Variable>();

            _undoStack = new Stack<IUndo>();

            _profile = profile;

            GroupStatus = new List<CharacterStatus>();
            RoomMonstersStatus = new List<MonsterStatus>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int ServerVersion
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public List<CommandAlias> AliasList
        {
            get
            {
                return _aliasList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<TriggerBase> TriggersList
        {
            get
            {
                return _triggersList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Highlight> HighlightList
        {
            get
            {
                return _highlightList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Substitution> SubstitutionList
        {
            get
            {
                return _substitutionList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Hotkey> HotkeyList
        {
            get
            {
                return _hotkeyList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Variable> VariableList
        {
            get
            {
                return _variableList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Stack<IUndo> UndoStack
        {
            get
            {
                return _undoStack;
            }
        }

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
        public IEnumerable<Group> Groups
        {
            get
            {
                return SettingsHolder.Instance.Settings.GlobalGroups.Concat(Profile.Groups);
            }
        }

        /// <summary>
        /// Gets variables
        /// </summary>
        [NotNull]
        public List<Variable> Variables
        {
            get
            {
                return Profile.Variables;
            }
        }

        /// <summary>
        /// Gets the group status.
        /// </summary>
        [NotNull]
        public List<CharacterStatus> GroupStatus
        {
            get
            {
                if (_groupStatus == null)
                    _groupStatus = new List<CharacterStatus>();

                return _groupStatus;
            }
            set
            {
                _groupStatus = value;
            }
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
        public List<MonsterStatus> RoomMonstersStatus
        {
            get
            {
                if (_monsterStatus == null)
                    _monsterStatus = new List<MonsterStatus>();

                return _monsterStatus;
            }

            set
            {
                _monsterStatus = value;
            }
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

            if (_conveyor != null)
            {
                _conveyor.PushCommand(command);
            }
        }

        /// <summary>
        /// Pushes the message to conveyor.
        /// </summary>
        /// <param name="message">The message.</param>
        public void PushMessageToConveyor([NotNull] Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            if (_conveyor != null)
            {
                _conveyor.PushMessage(message);
            }
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
            {
                v.Value = value;
            }
            else
            {
                Variables.Add(new Variable() { Name = variableName, Value = value });
            }

            if (!isSilent)
            {
                PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.InvariantCulture, Resources.VariableValueSet, variableName, value)));
            }
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

            if (variableName.Equals("DATE", StringComparison.InvariantCultureIgnoreCase))
            {
                return DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            if (variableName.Equals("TIME", StringComparison.InvariantCultureIgnoreCase))
            {
                return DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            }

            if (variableName.StartsWith("Monster", StringComparison.OrdinalIgnoreCase))
            {
                return GetMonsterOrGroupMateTarget(variableName, "Monster", SelectedRoomMonster, RoomMonstersStatus);
            }

            if (variableName.StartsWith("GroupMate", StringComparison.OrdinalIgnoreCase))
            {
                return GetMonsterOrGroupMateTarget(variableName, "GroupMate", SelectedGroupMate, GroupStatus);
            }

            var variablesList = new List<Variable>();
            var v = Variables.FirstOrDefault(x => x.Name == variableName);
            while (v != null)
            {
                variablesList.Add(v);

                if (!v.Value.StartsWith("$"))
                    break;

                if (variablesList.Contains(v))
                {
                    this.PushMessageToConveyor(new ErrorMessage(string.Format("#Cyclical variable: ${0}", variableName)));
                    throw new Exception();
                }

                var searchValue = v.Value.Substring(1, v.Value.Length - 1);
                v = Variables.FirstOrDefault(x => x.Name == searchValue);
            }

            return variablesList.Count > 0 ? variablesList[variablesList.Count - 1].Value : string.Empty;
        }

        private string GetMonsterOrGroupMateTarget(string variableName, string prefix, CharacterStatus selectedCharacter, IEnumerable<CharacterStatus> characters)
        {
            CharacterStatus monsterToProcess = null;
            var monsterNumberSubstring = variableName.Substring(prefix.Length).Trim();
            if (!string.IsNullOrEmpty(monsterNumberSubstring))
            {
                int monsterNumber;
                if (!int.TryParse(monsterNumberSubstring, out monsterNumber))
                {
                    return string.Empty;
                }

                if (monsterNumber > characters.Count())
                {
                    return string.Empty;
                }

                monsterToProcess = characters.Skip(monsterNumber - 1).First();
            }
            else
            {
                monsterToProcess = selectedCharacter;
            }

            if (monsterToProcess == null)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(monsterToProcess.TargetName))
            {
                return string.Empty;
            }

            int targetNamePrefix = 1;
            var monsterFirstWordName = monsterToProcess.TargetName.Split(' ').First();
            foreach (var monster in characters)
            {
                if (monster == monsterToProcess)
                {
                    break;
                }
                if (string.IsNullOrEmpty(monsterToProcess.TargetName))
                {
                    continue;
                }

                if (monster.TargetName.Split(' ').First() == monsterFirstWordName)
                {
                    targetNamePrefix++;
                }
            }

            if (targetNamePrefix > 1)
            {
                return targetNamePrefix + "." + monsterFirstWordName;
            }

            return monsterFirstWordName;
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
            {
                if (!Variables.Remove(v))
                {
                    this.PushMessageToConveyor(new ErrorMessage(string.Format("#Error delete variable: ${0}", v)));
                    return;
                }
            }

            if (!isSilent)
                this.PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.InvariantCulture, Resources.VariableValueClear, variableName)));
        }

        public VarReplaceReply ReplaceVariables(string str)
        {
            bool ret;
            bool allVariables = true;

            do
            {
                ret = false;
                str = _variableRegex.Replace(str,
                    m =>
                    {
                        ret = true;
                        var variable = GetVariableValue(m.Groups[1].Value);
                        if (string.IsNullOrEmpty(variable))
                            allVariables = false;

                        return variable;
                    });
            } while (ret);

            return new VarReplaceReply(str, allVariables);
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
                Profile.Groups.Add(new Group() { Name = groupName, IsEnabled = true, IsBuildIn = false });
            }
        }

        /// <summary>
        /// Delete the group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        public void DeleteGroup([NotNull]string groupName)
        {
            Assert.ArgumentNotNull(groupName, "groupName");
            var group = Profile.Groups.FirstOrDefault(gr => gr.Name == groupName);
            if (group != null && !group.IsBuildIn)
            {
                Profile.Groups.Remove(group);
            }
            else
            {
                group = SettingsHolder.Instance.Settings.GlobalGroups.FirstOrDefault(gr => gr.Name == groupName);
                if (group != null && !group.IsBuildIn)
                {
                    Profile.Groups.Remove(group);
                }
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

        public void SendToWindow(string name, IEnumerable<ActionBase> actionsToExecute, ActionExecutionContext actionExecutionContext)
        {
            var rootModel = _allModels.FirstOrDefault(w => w._name == name);
            if (rootModel == null)
            {
                return;
            }

            foreach (var action in actionsToExecute)
            {
                try
                {
                    action.Execute(rootModel, actionExecutionContext);
                }
                catch (Exception)
                { }
            }

            rootModel.PushCommandToConveyor(FlushOutputQueueCommand.Instance);
        }

        public void SendToAllWindows(IEnumerable<ActionBase> actionsToExecute, ActionExecutionContext actionExecutionContext)
        {
            foreach (var rootModel in _allModels)
            {
                foreach (var action in actionsToExecute)
                {
                    try
                    {
                        action.Execute(rootModel, actionExecutionContext);
                    }
                    catch (Exception)
                    {
                    }
                }

                rootModel.PushCommandToConveyor(FlushOutputQueueCommand.Instance);
            }
        }
        #endregion

        private void OnProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            if (e.Name == Profile.Name)
                Profile = SettingsHolder.Instance.GetProfile(e.Name);

            if (e.Name == Profile.Name || e.Global)
                RecalculatedEnabledTriggersPriorities();
        }

        public void Dispose()
        {
            if (_allModels != null && _allModels.Contains(this))
            {
                _allModels.Remove(this);
            }

            if (MessageConveyor != null)
            {
                MessageConveyor.Dispose();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct VarReplaceReply
    {
        /// <summary>
        /// 
        /// </summary>
        public string Value;

        /// <summary>
        /// 
        /// </summary>
        public bool IsAllVariables;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isAllVariables"></param>
        public VarReplaceReply(string str, bool isAllVariables)
        {
            Value = str;
            IsAllVariables = isAllVariables;
        }
    }
}
