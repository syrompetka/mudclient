﻿// --------------------------------------------------------------------------------------------------------------------
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
        private readonly MessageConveyor _conveyor;
        //private readonly IDictionary<string, Variable> _variablesDictionary = new Dictionary<string, Variable>();
        //private readonly IList<Variable> _variables;
        private List<TriggerBase> _enabledTriggersOrderedByPriority;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootModel"/> class.
        /// </summary>
        /// <param name="conveyor">The conveyor.</param>
        /// <param name="groups">The groups.</param>
        /// <param name="variables">The variables.</param>
        /// <param name="allActionDescriptions">All action descriptions.</param>
        /// <param name="allParameterDescriptions">All parameter descriptions.</param>
        public RootModel([NotNull] MessageConveyor conveyor, [NotNull] IList<Group> groups, [NotNull] IList<Variable> variables, [NotNull] IList<ActionDescription> allActionDescriptions, [NotNull] IList<ParameterDescription> allParameterDescriptions)
        {
            Assert.ArgumentNotNull(conveyor, "conveyor");
            Assert.ArgumentNotNull(groups, "groups");
            Assert.ArgumentNotNull(variables, "variables");
            Assert.ArgumentNotNull(allActionDescriptions, "allActionDescriptions");
            Assert.ArgumentNotNull(allParameterDescriptions, "allParameterDescriptions");

            _conveyor = conveyor;
            //Groups = groups;
            //_variables = variables;
            AllActionDescriptions = allActionDescriptions;
            AllParameterDescriptions = allParameterDescriptions;
            GroupStatus = new List<CharacterStatus>(10);
            RoomMonstersStatus = new List<MonsterStatus>(20);

            //foreach (var variable in _variables)
            //foreach (var variable in variables)
            //{
            //    //if (!_variablesDictionary.ContainsKey(variable.Name))
            //    //{
            //       // _variablesDictionary.Add(variable.Name, variable);
            //   // }
            //}

            CustomSerializationTypes = new List<Type>();
        }

        /// <summary>
        /// Initialize additional parameters
        /// </summary>
        /// <param name="allActionDescriptions">All action descriptions.</param>
        /// <param name="allParameterDescriptions">All parameter descriptions.</param>
        public void Initialize([NotNull] IList<ActionDescription> allActionDescriptions, [NotNull] IList<ParameterDescription> allParameterDescriptions)
        {
            AllActionDescriptions = allActionDescriptions;
            AllParameterDescriptions = allParameterDescriptions;
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

        /// <summary>
        /// Gets the custom serialization types.
        /// </summary>
        [NotNull]
        public IList<Type> CustomSerializationTypes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the groups.
        /// </summary>
        [NotNull]
        public IList<Group> Groups
        {
            //get;
            //private set;
            get
            {
                return ProfileHolder.Instance.Groups;
            }
        }

        /// <summary>
        /// Gets all action descriptions.
        /// </summary>
        [NotNull]
        public IList<ActionDescription> AllActionDescriptions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all parameter descriptions.
        /// </summary>
        [NotNull]
        public IList<ParameterDescription> AllParameterDescriptions
        {
            get;
            private set;
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
        /// Gets variables
        /// </summary>
        [NotNull]
        public IList<Variable> Variables
        {
            //get;
            //private set;
            get
            {
                return ProfileHolder.Instance.Variables;
            }
        }

        /// <summary>
        /// Pushes the command to conveyor.
        /// </summary>
        /// <param name="command">The command.</param>
        public void PushCommandToConveyor([NotNull] Command command)
        {
            Assert.ArgumentNotNull(command, "command");

            _conveyor.PushCommand(command);
        }

        /// <summary>
        /// Pushes the message to conveyor.
        /// </summary>
        /// <param name="message">The message.</param>
        public void PushMessageToConveyor([NotNull] Message message)
        {
            Assert.ArgumentNotNull(message, "message");

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
            //if (!_variablesDictionary.ContainsKey(variableName))
            if (v != null)
            {
                v.Value = value;
                //var variable = new Variable { Name = variableName, Value = value };
                //_variablesDictionary.Add(variableName, variable);
                //_variables.Add(variable);
            }
            else
                Variables.Add(new Variable() { Name = variableName, Value = value });

            //_variablesDictionary[variableName].Value = value;

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

            //if (_variablesDictionary.ContainsKey(variableName))
            //{
            //    return _variablesDictionary[variableName].Value;
            //}

            var v = Variables.FirstOrDefault(var => var.Name == variableName);
            if (v != null)
            {
                return v.Value;
            }

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
            //if (_variablesDictionary.ContainsKey(variableName))
            //{
            //    //_variables.Remove(_variablesDictionary[variableName]);
            //    _variablesDictionary.Remove(variableName);
            //}

            var v = Variables.FirstOrDefault(var => var.Name == variableName);
            if (v != null)
            {
                Variables.Remove(v);
            }

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
                    PushMessageToConveyor(new ErrorMessage("#Возникла Ошибка при удалении группы"));
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
    }
}
