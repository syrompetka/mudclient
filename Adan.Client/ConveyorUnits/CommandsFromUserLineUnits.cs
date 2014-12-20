
namespace Adan.Client.ConveyorUnits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Input;

    using Common.Commands;
    using Common.ConveyorUnits;
    using Common.Model;
    using Common.Messages;
    using Common.Themes;
    using Common.Utils;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Commands;
    using Model.Actions;
    using Messages;
    using System.Text;
    using System.Diagnostics;
    using Adan.Client.Common.Settings;
    using System.Globalization;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementation that use commands string from line
    /// e.g. #alias, #action, #highlight, #substitution, #hotkey etc.
    /// </summary>
    public class CommandsFromUserLineUnit : ConveyorUnit
    {
        private readonly Regex _regexAction = new Regex(@"^\#act?i?o?n?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexUnAction = new Regex(@"^\#unact?i?o?n?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexAlias = new Regex(@"^\#ali?a?s?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexUnAlias = new Regex(@"^\#unali?a?s?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexHighLight = new Regex(@"^\#hig?h?l?i?g?h?t?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexUnHighLight = new Regex(@"^\#unhig?h?l?i?g?h?t?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexSubstitution = new Regex(@"^#sub?s?t?i?t?u?t?e?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexUnSubstitution = new Regex(@"^#unsub?s?t?i?t?u?t?e?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexHotkey = new Regex(@"^#hot?k?e?y?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexUnHotkey = new Regex(@"^#unhot?k?e?y?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexGag = new Regex(@"^#gag?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexUnGag = new Regex(@"^#ungag?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexVariable = new Regex(@"^#var?i?a?b?l?e?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexUnVariable = new Regex(@"^#unvar?i?a?b?l?e?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _regexLog = new Regex(@"#log?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regesStopLog = new Regex(@"#stoplog", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexShowme = new Regex(@"#sho?w?m?e?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexTickOn = new Regex(@"#tickon\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexTickOff = new Regex(@"#tickoff?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexTickSize = new Regex(@"#ticksi?z?e?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexZap = new Regex(@"#za?p?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexConnect = new Regex(@"#conn?e?c?t?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexStatus = new Regex(@"#stat?u?s?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexGroup = new Regex(@"#gr?o?u?p?\s*(enable|disable)\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _regexUndo = new Regex(@"#undo(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return Enumerable.Empty<int>();
            }
        }

        /// <summary>
        /// Gets a set of command types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledCommandTypes
        {
            get
            {
                return new[] { BuiltInCommandTypes.TextCommand };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="rootModel"></param>
        /// <param name="isImport"></param>
        public override void HandleCommand([NotNull] Command command, RootModel rootModel, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");

            var textCommand = command as TextCommand;
            if (textCommand == null)
            {
                return;
            }

            var commandText = textCommand.CommandText.Trim();

            if (!commandText.StartsWith(SettingsHolder.Instance.Settings.CommandChar.ToString()))
                return;

            if (this.TriggerCheck(commandText, rootModel, isImport) || this.AliasCheck(commandText, rootModel, isImport)
                || this.ConnectCheck(commandText, rootModel, isImport) || this.GroupCheck(commandText, rootModel, isImport)
                || this.HighLightCheck(commandText, rootModel, isImport) || this.HotkeyCheck(commandText, rootModel, isImport)
                || this.LogCheck(commandText, rootModel, isImport) || this.ShowmeCheck(commandText, rootModel, isImport)
                || this.StatusCheck(commandText, rootModel, isImport) || this.SubstitutionCheck(commandText, rootModel, isImport)
                || this.VariableCheck(commandText, rootModel, isImport) || this.ZapCheck(commandText, rootModel, isImport)
                || this.UndoCheck(commandText, rootModel, isImport))
            {
                command.Handled = true;
                return;
            }
        }

        private bool UndoCheck(string commandText, RootModel rootModel, bool isImport)
        {
            var match = _regexUndo.Match(commandText);
            if (match.Success)
            {
                var undoStack = rootModel.UndoStack;
                if (undoStack.Count > 0)
                {
                    IUndo undo = undoStack.Pop();
                    undo.Undo(rootModel);

                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage(undo.UndoInfo()), rootModel);
                }
                else
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Undo стэк пуст."), rootModel);
                }

                return true;
            }

            return false;
        }

        private bool AliasCheck(string commandText, RootModel rootModel, bool isImport)
        {
            var match = _regexAlias.Match(commandText);
            if (match.Success)
            {
                var aliasList = rootModel.AliasList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                    {
                        aliasList.Clear();
                        aliasList.AddRange(rootModel.Groups.SelectMany(gr => gr.Aliases));
                        if (aliasList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список алиасов:"), rootModel);
                            for (int i = 0; i < aliasList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #alias {{{1}}} {{{2}}}",
                                    i + 1, aliasList[i].Command, String.Join(";", aliasList[i].Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список алиасов пуст."), rootModel);
                        }
                    }

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 2)
                {
                    if (!isImport)
                    {
                        aliasList.Clear();
                        aliasList.AddRange(rootModel.Groups.SelectMany(gr => gr.Aliases
                            .Where(alias => Regex.IsMatch(alias.Command, args[0], RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))));

                        if (aliasList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Список алиасов, содержащих строку \"{0}\":", args[0])), rootModel);
                            for (int i = 0; i < aliasList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #alias {{{1}}} {{{2}}}",
                                    i + 1, aliasList[i].Command, String.Join(";", aliasList[i].Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Алиасы, содержащие строку \"{0}\" не найдены.", args[0])), rootModel);
                        }
                    }

                    return true;
                }

                string groupName = "Default";

                if (args.Length == 3)
                    groupName = args[2];

                CommandAlias commandAlias = new CommandAlias()
                {
                    Command = Regex.Replace(args[0], "%?%[0-9]", "").Trim()
                };

                commandAlias.Actions.Add(new SendTextAction()
                {
                    CommandText = args[1]
                });

                var group = rootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                if (group == null)
                {
                    rootModel.AddGroup(groupName);
                    group = rootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                }

                {
                    var alias = group.Aliases.FirstOrDefault(all => all.Command == args[0]);
                    while (alias != null)
                    {
                        group.Aliases.TryTake(out alias);

                        alias.Group = group;
                        alias.Operation = UndoOperation.Add;
                        rootModel.UndoStack.Push(alias);

                        if (!isImport)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Алиас удален: #alias {{{0}}} {{{1}}}",
                                alias.Command, String.Join(";", alias.Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                        }

                        alias = group.Aliases.FirstOrDefault(x => x.Command == args[0]);
                    }
                }

                group.Aliases.Add(commandAlias);

                if (!isImport)
                {
                    commandAlias.Group = group;
                    commandAlias.Operation = UndoOperation.Remove;
                    rootModel.UndoStack.Push(commandAlias);

                    base.PushMessageToConveyor(new InfoMessage(string.Format("#Алиас добавлен: #alias {{{0}}} {{{1}}}",
                        commandAlias.Command, commandAlias.Actions[0].ToString())), rootModel);
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                }

                return true;
            }

            match = _regexUnAlias.Match(commandText);
            if (!isImport && match.Success)
            {
                var aliasList = rootModel.AliasList;

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return true;
                }

                int num;
                if (int.TryParse(args[0], out num))
                {
                    if (num > 0 && num <= aliasList.Count)
                    {
                        CommandAlias alias;
                        if(rootModel.Groups.FirstOrDefault(x => x.Aliases.Contains(aliasList[num - 1])).Aliases.TryTake(out alias))
                        {
                            aliasList[num - 1].Group = alias.Group;
                            aliasList[num - 1].Operation = UndoOperation.Add;
                            rootModel.UndoStack.Push(aliasList[num - 1]);
                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Алиас удален: #alias {{{0}}} {{{1}}}",
                                    aliasList[num - 1].Command, String.Join(";", aliasList[num - 1].Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                            }
                        }

                        //for (int i = 0; i < rootModel.Groups.Count; i++)
                        //{
                        //    if (rootModel.Groups[i].Aliases.Remove(aliasList[num - 1]))
                        //    {
                        //        aliasList[num - 1].Group = rootModel.Groups[i];
                        //        aliasList[num - 1].Operation = UndoOperation.Add;
                        //        rootModel.UndoStack.Push(aliasList[num - 1]);
                        //        if (!isImport)
                        //        {
                        //            base.PushMessageToConveyor(new InfoMessage(string.Format("#Алиас удален: #alias {{{0}}} {{{1}}}",
                        //                aliasList[num - 1].Command, String.Join(";", aliasList[num - 1].Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                        //            base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                        //        }

                        //        break;
                        //    }
                        //}
                    }
                    else
                    {
                        base.PushMessageToConveyor(new InfoMessage("#Неверный номер"), rootModel);
                    }

                    return true;
                }

                if (args.Length == 2)
                {
                    var group = rootModel.Groups.FirstOrDefault(gr => gr.Name == args[1]);
                    if (group != null)
                    {
                        var alias = group.Aliases.FirstOrDefault(x => x.Command == args[0]);
                        if (alias != null)
                        {
                            group.Aliases.TryTake(out alias);

                            alias.Group = group;
                            alias.Operation = UndoOperation.Add;
                            rootModel.UndoStack.Push(alias);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Алиас удален: #alias {{{0}}} {{{1}}}",
                                    alias.Command, String.Join(";", alias.Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var group in rootModel.Groups)
                    {
                        var alias = group.Aliases.FirstOrDefault(all => all.Command == args[0]);
                        while (alias != null)
                        {
                            group.Aliases.TryTake(out alias);

                            alias.Group = group;
                            alias.Operation = UndoOperation.Add;
                            rootModel.UndoStack.Push(alias);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Алиас удален: #alias {{{0}}} {{{1}}}",
                                    alias.Command, String.Join(";", alias.Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                            }

                            alias = group.Aliases.FirstOrDefault(all => all.Command == args[0]);
                        }
                    }
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены последнего удаления используйте #undo"), rootModel);
                }

                return true;
            }

            return false;
        }

        private bool TriggerCheck(string commandText, RootModel rootModel, bool isImport)
        {
            var match = _regexAction.Match(commandText);
            if (match.Success)
            {
                var triggersList = rootModel.TriggersList;

                if (match.Groups[1].Length == 0)
                {                   
                    if (!isImport)
                    {
                        triggersList.Clear();
                        triggersList.AddRange(rootModel.Groups.SelectMany(gr => gr.Triggers));

                        if(triggersList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список триггеров:"), rootModel);
                            for (int i = 0; i < triggersList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #action {{{1}}} {{{2}}}",
                                    i + 1, triggersList[i].GetPatternString(), String.Join(";", triggersList[i].Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список триггеров пуст."), rootModel);
                        }                        
                    }

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 2)
                {
                    if (!isImport)
                    {
                        triggersList.Clear();
                        triggersList.AddRange(rootModel.Groups.SelectMany(gr => gr.Triggers
                            .Where(trig => Regex.IsMatch(trig.GetPatternString(), args[0], RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))));

                        if (triggersList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Список триггеров, содержащих строку \"{0}\":", args[0])), rootModel);
                            for (int i = 0; i < triggersList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #action {{{1}}} {{{2}}}",
                                    i + 1, triggersList[i].GetPatternString(), String.Join(";", triggersList[i].Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Триггеры, содержащие строку \"{0}\" не найдены.", args[0])), rootModel);
                        }
                    }

                    return true;
                }

                string groupName = "Default";
                int priority = 5;

                if (args.Length == 4)
                {
                    groupName = args[3];
                    if (!int.TryParse(args[2], out priority))
                    {
                        if (!isImport)
                            base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);
                        return true;
                    }
                }
                else if (args.Length == 3)
                {
                    groupName = args[2];
                }

                TextTrigger trigger = new TextTrigger()
                {
                    MatchingPattern = args[0],
                    DoNotDisplayOriginalMessage = false,
                    Priority = priority,
                    StopProcessingTriggersAfterThis = false
                };

                trigger.Actions.Add(new SendTextAction()
                {
                    CommandText = args[1]
                });

                var group = rootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                if (group == null)
                {
                    rootModel.AddGroup(groupName);
                    group = rootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                }

                {
                    var trig = group.Triggers.FirstOrDefault(x => x.GetPatternString() == args[0]);
                    while (trig != null)
                    {
                        //group.Triggers.Remove(trig);
                        group.Triggers.TryTake(out trig);

                        trig.Group = group;
                        trig.Operation = UndoOperation.Add;
                        rootModel.UndoStack.Push(trig);

                        if (!isImport)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Триггер удален: #action {{{0}}} {{{1}}}",
                                trig.GetPatternString(), String.Join(";", trig.Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                        }

                        trig = group.Triggers.FirstOrDefault(x => x.GetPatternString() == args[0]);
                    }
                }

                group.Triggers.Add(trigger);

                rootModel.RecalculatedEnabledTriggersPriorities();

                if (!isImport)
                {
                    trigger.Group = group;
                    trigger.Operation = UndoOperation.Remove;
                    rootModel.UndoStack.Push(trigger);
                    base.PushMessageToConveyor(new InfoMessage(string.Format("#Триггер добавлен: #action {{{0}}} {{{1}}}",
                        trigger.GetPatternString(), trigger.Actions[0].ToString())), rootModel);
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                }

                return true;
            }

            match = _regexUnAction.Match(commandText);
            if (match.Success)
            {
                var triggersList = rootModel.TriggersList;

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return true;
                }

                int num;
                if (int.TryParse(args[0], out num))
                {
                    if (num > 0 && num <= triggersList.Count)
                    {
                        TriggerBase trigger;
                        if(rootModel.Groups.FirstOrDefault(x => x.Triggers.Contains(triggersList[num - 1])).Triggers.TryTake(out trigger))
                        {
                            triggersList[num - 1].Group = trigger.Group;
                            triggersList[num - 1].Operation = UndoOperation.Add;
                            rootModel.UndoStack.Push(triggersList[num - 1]);
                            
                            if (!isImport)
                            {
                                    base.PushMessageToConveyor(new InfoMessage(string.Format("#Триггер удален: #action {{{0}}} {{{1}}}",
                                        triggersList[num - 1].GetPatternString(), String.Join(";", triggersList[num - 1].Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                                    base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                            }
                        }

                        //for (int i = 0; i < rootModel.Groups.Count; i++)
                        //{
                        //    if (rootModel.Groups[i].Triggers.Remove(triggersList[num - 1]))
                        //    {
                        //        triggersList[num - 1].Group = rootModel.Groups[i];
                        //        triggersList[num - 1].Operation = UndoOperation.Add;
                        //        rootModel.UndoStack.Push(triggersList[num - 1]);
                        //        if (!isImport)
                        //        {
                        //            base.PushMessageToConveyor(new InfoMessage(string.Format("#Триггер удален: #action {{{0}}} {{{1}}}",
                        //                triggersList[num - 1].GetPatternString(), String.Join(";", triggersList[num - 1].Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                        //            base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                        //        }

                        //        rootModel.RecalculatedEnabledTriggersPriorities();
                        //        break;
                        //    }
                        //}
                    }
                    else
                    {
                        base.PushMessageToConveyor(new InfoMessage("#Неверный номер"), rootModel);
                    }

                    return true;
                }

                if (args.Length == 2)
                {
                    var group = rootModel.Groups.FirstOrDefault(gr => gr.Name == args[1]);
                    if (group != null)
                    {
                        var trig = group.Triggers.FirstOrDefault(x => x.GetPatternString() == args[0]);
                        if (trig != null)
                        {
                            group.Triggers.TryTake(out trig);

                            trig.Group = group;
                            trig.Operation = UndoOperation.Add;
                            rootModel.UndoStack.Push(trig);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Триггер удален: #action {{{0}}} {{{1}}}",
                                    trig.GetPatternString(), String.Join(";", trig.Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var group in rootModel.Groups)
                    {
                        var trig = group.Triggers.FirstOrDefault(x => x.GetPatternString() == args[0]);
                        while (trig != null)
                        {
                            group.Triggers.TryTake(out trig);

                            trig.Group = group;
                            trig.Operation = UndoOperation.Add;
                            rootModel.UndoStack.Push(trig);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Триггер удален: #action {{{0}}} {{{1}}}",
                                    trig.GetPatternString(), String.Join(";", trig.Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                            }

                            trig = group.Triggers.FirstOrDefault(x => x.GetPatternString() == args[0]);
                        }
                    }

                    base.PushMessageToConveyor(new InfoMessage("#Для отмены последнего удаления используйте #undo"), rootModel);
                }

                rootModel.RecalculatedEnabledTriggersPriorities();

                return true;
            }

            return false;
        }

        private bool HighLightCheck(string commandText, RootModel rootModel, bool isImport)
        {
            var match = _regexHighLight.Match(commandText);
            if (match.Success)
            {
                var highlightList = rootModel.HighlightList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                    {
                        highlightList.Clear();
                        highlightList.AddRange(rootModel.Groups.SelectMany(x => x.Highlights));

                        if (highlightList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список хайлайтов:"), rootModel);
                            for (int i = 0; i < highlightList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #highlight {{{1}}} {{{2},{3}}}",
                                    i + 1, highlightList[i].TextToHighlight, highlightList[i].ForegroundColor, highlightList[i].BackgroundColor)), rootModel);
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список хайлайтов пуст."), rootModel);
                        }
                    }

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 2)
                {
                    if (!isImport)
                    {
                        highlightList.Clear();
                        highlightList.AddRange(rootModel.Groups.SelectMany(x => x.Highlights
                            .Where(hi => Regex.IsMatch(hi.TextToHighlight, args[0], RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))));

                        if (highlightList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Список хайлайтов, содержащих строку \"{0}\":", args[0])), rootModel);
                            for (int i = 0; i < highlightList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #highlight {{{1}}} {{{2},{3}}}",
                                    i + 1, highlightList[i].TextToHighlight, highlightList[i].ForegroundColor, highlightList[i].BackgroundColor)), rootModel);
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Хайлаты, содержащие строку \"{0}\" не найдены.", args[0])), rootModel);
                        }
                    }

                    return true;
                }

                string groupName = "Default";

                if (args.Length == 3)
                    groupName = args[2];

                var group = rootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                if (group == null)
                {
                    rootModel.AddGroup(groupName);
                    group = rootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                }

                Highlight highlight = new Highlight()
                {
                    TextToHighlight = args[1]
                };

                string[] colors = args[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (colors.Length < 2)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return true;
                }

                TextColor foregroundColor = TextColorParser.Parse(colors[0].Trim());
                TextColor backgroundColor;
                if (colors.Length == 2)
                    backgroundColor = TextColorParser.Parse(colors[1].Trim());
                else
                    backgroundColor = TextColor.Black;

                if (foregroundColor == TextColor.None || backgroundColor == TextColor.None)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Ошибка цвета"), rootModel);

                    return true;
                }

                highlight.ForegroundColor = foregroundColor;
                highlight.BackgroundColor = backgroundColor;

                {
                    var high = group.Highlights.FirstOrDefault(x => x.TextToHighlight == args[0]);
                    while (high != null)
                    {
                        group.Highlights.TryTake(out high);

                        high.Group = group;
                        high.Operation = UndoOperation.Add;
                        rootModel.UndoStack.Push(high);

                        if (!isImport)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Хайлайт удален: #action {{{0}}} {{{1},{2}}}",
                                high.TextToHighlight, high.ForegroundColor, high.BackgroundColor)), rootModel);
                        }

                        high = group.Highlights.FirstOrDefault(x => x.TextToHighlight == args[0]);
                    }
                }

                group.Highlights.Add(highlight);

                highlight.Group = group;
                highlight.Operation = UndoOperation.Remove;
                rootModel.UndoStack.Push(highlight);

                if (!isImport)
                {
                    base.PushMessageToConveyor(new InfoMessage(string.Format("#Хайлайт добавлен: #highlight {{{0}}} {{{1}, {2}}}",
                           highlight.TextToHighlight, highlight.ForegroundColor, highlight.BackgroundColor)), rootModel);
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                }

                return true;
            }

            match = _regexUnHighLight.Match(commandText);
            if (match.Success)
            {
                var highlightList = rootModel.HighlightList;

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);
                    return true;
                }

                int num;
                if (int.TryParse(args[0], out num))
                {
                    if (num > 0 && num <= highlightList.Count)
                    {
                        Highlight highlight;
                        if(rootModel.Groups.FirstOrDefault(x => x.Highlights.Contains(highlightList[num-1])).Highlights.TryTake(out highlight))
                        {
                            highlightList[num - 1].Group = highlight.Group;
                            highlightList[num - 1].Operation = UndoOperation.Add;
                            rootModel.UndoStack.Push(highlightList[num - 1]);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Хайлайт удален: #action {{{0}}} {{{1}, {2}}}",
                                    highlightList[num - 1].TextToHighlight, highlightList[num - 1].ForegroundColor, highlightList[num - 1].BackgroundColor)), rootModel);
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                            }
                        }

                        //for (int i = 0; i < rootModel.Groups.Count; i++)
                        //{
                        //    if (rootModel.Groups[i].Highlights.Remove(highlightList[num - 1]))
                        //    {
                        //        highlightList[num - 1].Group = rootModel.Groups[i];
                        //        highlightList[num - 1].Operation = UndoOperation.Add;
                        //        rootModel.UndoStack.Push(highlightList[num - 1]);

                        //        if (!isImport)
                        //        {
                        //            base.PushMessageToConveyor(new InfoMessage(string.Format("#Хайлайт удален: #action {{{0}}} {{{1}, {2}}}",
                        //                highlightList[num - 1].TextToHighlight, highlightList[num - 1].ForegroundColor, highlightList[num - 1].BackgroundColor)), rootModel);
                        //            base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                        //        }

                        //        break;
                        //    }
                        //}
                    }
                    else
                    {
                        base.PushMessageToConveyor(new InfoMessage("#Неверный номер"), rootModel);
                    }

                    return true;
                }

                if (args.Length == 2)
                {
                    var group = rootModel.Groups.FirstOrDefault(gr => gr.Name == args[1]);
                    if (group != null)
                    {
                        var high = group.Highlights.FirstOrDefault(x => x.TextToHighlight == args[0]);
                        if (high != null)
                        {
                            group.Highlights.TryTake(out high);

                            high.Group = group;
                            high.Operation = UndoOperation.Add;
                            rootModel.UndoStack.Push(high);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Хайлайт удален: #action {{{0}}} {{{1},{2}}}",
                                    high.TextToHighlight, high.ForegroundColor, high.BackgroundColor)), rootModel);
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var group in rootModel.Groups)
                    {
                        var high = group.Highlights.FirstOrDefault(x => x.TextToHighlight == args[0]);
                        while (high != null)
                        {
                            group.Highlights.TryTake(out high);

                            high.Group = group;
                            high.Operation = UndoOperation.Add;
                            rootModel.UndoStack.Push(high);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Хайлайт удален: #action {{{0}}} {{{1},{2}}}",
                                    high.TextToHighlight, high.ForegroundColor, high.BackgroundColor)), rootModel);
                            }

                            high = group.Highlights.FirstOrDefault(x => x.TextToHighlight == args[0]);
                        }
                    }

                    base.PushMessageToConveyor(new InfoMessage("#Для отмены последнего удаления используйте #undo"), rootModel);
                }

                return true;
            }

            return false;
        }

        private bool SubstitutionCheck(string commandText, RootModel rootModel, bool isImport)
        {
            var match = _regexSubstitution.Match(commandText);
            if (match.Success)
            {
                var substitutionList = rootModel.SubstitutionList;
                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                    {
                        substitutionList.Clear();
                        substitutionList.AddRange(rootModel.Groups.SelectMany(x => x.Substitutions));

                        if (substitutionList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список замен:"), rootModel);
                            for (int i = 0; i < substitutionList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #substitution {{{1}}} {{{2}}}",
                                    i + 1, substitutionList[i].Pattern, substitutionList[i].SubstituteWith)), rootModel);
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список замен пуст."), rootModel);
                        }
                    }

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 2)
                {
                    if (!isImport)
                    {
                        substitutionList.Clear();
                        substitutionList.AddRange(rootModel.Groups.SelectMany(x => x.Substitutions
                            .Where(hi => Regex.IsMatch(hi.Pattern, args[0], RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))));

                        if (substitutionList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Список замен, содержащих строку \"{0}\":", args[0])), rootModel);
                            for (int i = 0; i < substitutionList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #substitution {{{1}}} {{{2}}}",
                                    i + 1, substitutionList[i].Pattern, substitutionList[i].SubstituteWith)), rootModel);
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Замены, содержащие строку \"{0}\" не найдены.", args[0])), rootModel);
                        }
                    }

                    return true;
                }

                string groupName = "Default";

                if (args.Length == 3)
                    groupName = args[2];

                var group = rootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                if (group == null)
                {
                    rootModel.AddGroup(groupName);
                    group = rootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                }

                Substitution subtitution = new Substitution() { Pattern = args[0], SubstituteWith = args[1] };

                {
                    var sub = group.Substitutions.FirstOrDefault(x => x.Pattern == args[0]);
                    while (sub != null)
                    {
                        group.Substitutions.TryTake(out sub);

                        sub.Group = group;
                        sub.Operation = UndoOperation.Add;
                        rootModel.UndoStack.Push(sub);

                        if (!isImport)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Замена удалена: #substitution {{{0}}} {{{1}}}",
                                sub.Pattern, sub.SubstituteWith)), rootModel);
                        }

                        sub = group.Substitutions.FirstOrDefault(x => x.Pattern == args[0]);
                    }
                }

                group.Substitutions.Add(subtitution);

                subtitution.Group = group;
                subtitution.Operation = UndoOperation.Remove;
                rootModel.UndoStack.Push(subtitution);

                if (!isImport)
                {
                    base.PushMessageToConveyor(new InfoMessage(string.Format("#Замена добавлена: #subtitution {{{0}}} {{{1}}}",
                           subtitution.Pattern, subtitution.SubstituteWith)), rootModel);
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                }

                return true;
            }

            match = _regexUnSubstitution.Match(commandText);
            if (match.Success)
            {
                var substitutionList = rootModel.SubstitutionList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return true;
                }

                int num;
                if (int.TryParse(args[0], out num))
                {
                    if (num > 0 && num <= substitutionList.Count)
                    {
                        Substitution sub;

                        if (rootModel.Groups.FirstOrDefault(x => x.Substitutions.Contains(substitutionList[num - 1])).Substitutions.TryPeek(out sub))
                        {
                            substitutionList[num - 1].Group = sub.Group;
                            substitutionList[num - 1].Operation = UndoOperation.Add;
                            rootModel.UndoStack.Push(substitutionList[num - 1]);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Замена удалена: #substitution {{{0}}} {{{1}}}",
                                    substitutionList[num - 1].Pattern, substitutionList[num - 1].SubstituteWith)), rootModel);
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                            }
                        }

                        //for (int i = 0; i < rootModel.Groups.Count; i++)
                        //{
                        //    if (rootModel.Groups[i].Substitutions.Remove(substitutionList[num - 1]))
                        //    {
                        //        substitutionList[num - 1].Group = rootModel.Groups[i];
                        //        substitutionList[num - 1].Operation = UndoOperation.Add;
                        //        rootModel.UndoStack.Push(substitutionList[num - 1]);

                        //        if (!isImport)
                        //        {
                        //            base.PushMessageToConveyor(new InfoMessage(string.Format("#Замена удалена: #substitution {{{0}}} {{{1}}}",
                        //                substitutionList[num - 1].Pattern, substitutionList[num - 1].SubstituteWith)), rootModel);
                        //            base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                        //        }

                        //        break;
                        //    }
                        //}
                    }
                    else
                    {
                        base.PushMessageToConveyor(new InfoMessage("#Неверный номер"), rootModel);
                    }

                    return true;
                }

                if (args.Length >= 2)
                {
                    var group = rootModel.Groups.FirstOrDefault(gr => gr.Name == args[1]);
                    if (group != null)
                    {
                        var sub = group.Substitutions.FirstOrDefault(x => x.Pattern == args[0]);
                        if (sub != null)
                        {
                            group.Substitutions.TryTake(out sub);

                            sub.Group = group;
                            sub.Operation = UndoOperation.Add;
                            rootModel.UndoStack.Push(sub);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Замена удалена: #substitution {{{0}}} {{{1}}}",
                                    sub.Pattern, sub.SubstituteWith)), rootModel);
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var group in rootModel.Groups)
                    {
                        var sub = group.Substitutions.FirstOrDefault(x => x.Pattern == args[0]);
                        while (sub != null)
                        {
                            group.Substitutions.TryTake(out sub);

                            sub.Group = group;
                            sub.Operation = UndoOperation.Add;
                            rootModel.UndoStack.Push(sub);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Замена удалена: #substitution {{{0}}} {{{1}}}",
                                    sub.Pattern, sub.SubstituteWith)), rootModel);
                            }

                            sub = group.Substitutions.FirstOrDefault(x => x.Pattern == args[0]);
                        }
                    }
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены последнего удаления используйте #undo"), rootModel);
                }

                return true;
            }

            return false;
        }

        private bool HotkeyCheck(string commandText, RootModel rootModel, bool isImport)
        {
            var match = _regexHotkey.Match(commandText);

            if (match.Success)
            {
                var hotkeysList = rootModel.HotkeyList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                    {
                        hotkeysList.Clear();
                        hotkeysList.AddRange(rootModel.Groups.SelectMany(x => x.Hotkeys));

                        if (hotkeysList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список хоткеев:"), rootModel);
                            for (int i = 0; i < hotkeysList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #hotkey {{{1}}} {{{2}}}",
                                    i + 1, hotkeysList[i].GetKeyToString(), String.Join(";", hotkeysList[i].Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список хоткеев пуст."), rootModel);
                        }
                    }

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 2)
                {
                    if (!isImport)
                    {
                        hotkeysList.Clear();
                        hotkeysList.AddRange(rootModel.Groups.SelectMany(x => x.Hotkeys
                            .Where(hot => Regex.IsMatch(hot.GetKeyToString(), args[0], RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))));

                        if (hotkeysList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Список хоткеев, содержащих строку \"{0}\":", args[0])), rootModel);
                            for (int i = 0; i < hotkeysList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #hotkey {{{1}}} {{{2}}}",
                                    i + 1, hotkeysList[i].GetKeyToString(), String.Join(";", hotkeysList[i].Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Хоткеи, содержащие строку \"{0}\" не найдены.", args[0])), rootModel);
                        }
                    }

                    return true;
                }

                string groupName = "Default";

                if (args.Length == 3)
                    groupName = args[2];

                var group = rootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                if (group == null)
                {
                    rootModel.AddGroup(groupName);
                    group = rootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                }

                args[0] = args[0].Replace("INS", "Insert").Replace("NUMDEL", "Decimal");
                args[0] = args[0].Replace("MUL", "Multiply").Replace("NUM", "Numpad").Replace("MIN", "Subtract").Replace("DIV", "Divide");
                args[0] = args[0].Replace("PGUP", "PageUp").Replace("PGDWN", "Next").Replace("`", "OemTilde").Replace("SP", "Space").Replace("DEL", "Delete");


                string[] hotkeys = args[0].Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);

                Hotkey hotk = new Hotkey();

                ModifierKeysConverter modifierKeysConverter = new ModifierKeysConverter();
                KeyConverter keyConverter = new KeyConverter();

                foreach (string hotkey in hotkeys)
                {
                    if (modifierKeysConverter.IsValid(hotkey))
                        hotk.ModifierKeys |= (ModifierKeys)modifierKeysConverter.ConvertFrom(hotkey);
                    else if (keyConverter.IsValid(hotkey))
                        hotk.Key |= (Key)keyConverter.ConvertFrom(hotkey);
                    else
                    {
                        if (!isImport)
                            base.PushMessageToConveyor(new ErrorMessage("#Неверный код хоткея"), rootModel);

                        return true;
                    }
                }

                hotk.Actions.Add(new SendTextAction()
                {
                    CommandText = args[1]
                });

                {
                    var hotkey = group.Hotkeys.FirstOrDefault(x => x.GetKeyToString() == args[0]);
                    while (hotkey != null)
                    {
                        group.Hotkeys.TryTake(out hotkey);

                        hotkey.Group = group;
                        hotkey.Operation = UndoOperation.Add;
                        rootModel.UndoStack.Push(hotkey);

                        if (!isImport)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Хоткей удален: #hotkey {{{0}}} {{{1}}}",
                                hotkey.GetKeyToString(), String.Join(";", hotkey.Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                        }

                        hotkey = group.Hotkeys.FirstOrDefault(x => x.GetKeyToString() == args[0]);
                    }
                }

                group.Hotkeys.Add(hotk);

                hotk.Group = group;
                hotk.Operation = UndoOperation.Remove;
                rootModel.UndoStack.Push(hotk);

                rootModel.RecalculatedEnabledTriggersPriorities();

                if (!isImport)
                {
                    base.PushMessageToConveyor(new InfoMessage(string.Format("#Хоткей добавлен: #hotkey {{{0}}} {{{1}}}",
                        hotk.GetKeyToString(), hotk.Actions[0].ToString())), rootModel);
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                }

                return true;
            }

            match = _regexUnHotkey.Match(commandText);
            if (match.Success)
            {
                var hotkeyList = rootModel.HotkeyList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return true;
                }


                int num;
                if (int.TryParse(args[0], out num))
                {
                    if (num > 0 && num <= hotkeyList.Count)
                    {
                        Hotkey hotkey;
                        if(rootModel.Groups.FirstOrDefault(x => x.Hotkeys.Contains(hotkeyList[num - 1])).Hotkeys.TryTake(out hotkey))
                        {
                            hotkeyList[num - 1].Group = hotkey.Group;
                                hotkeyList[num - 1].Operation = UndoOperation.Add;
                                rootModel.UndoStack.Push(hotkeyList[num - 1]);

                                if (!isImport)
                                {
                                    base.PushMessageToConveyor(new InfoMessage(string.Format("#Хоткей удален: #hotkey {{{0}}} {{{1}}}",
                                        hotkeyList[num - 1].GetKeyToString(), String.Join(";", hotkeyList[num - 1].Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                                    base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                                }
                        }

                        //for (int i = 0; i < rootModel.Groups.Count; i++)
                        //{
                        //    if (rootModel.Groups[i].Hotkeys.Remove(hotkeyList[num - 1]))
                        //    {
                        //        hotkeyList[num - 1].Group = rootModel.Groups[i];
                        //        hotkeyList[num - 1].Operation = UndoOperation.Add;
                        //        rootModel.UndoStack.Push(hotkeyList[num - 1]);

                        //        if (!isImport)
                        //        {
                        //            base.PushMessageToConveyor(new InfoMessage(string.Format("#Хоткей удален: #hotkey {{{0}}} {{{1}}}",
                        //                hotkeyList[num - 1].GetKeyToString(), String.Join(";", hotkeyList[num - 1].Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                        //            base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                        //        }

                        //        break;
                        //    }
                        //}
                        
                    return true;
                    }
                    else
                    {
                        base.PushMessageToConveyor(new InfoMessage("#Неверный номер"), rootModel);
                    }

                }
            

                ModifierKeysConverter modifierKeysConverter = new ModifierKeysConverter();
                KeyConverter keyConverter = new KeyConverter();

                ModifierKeys modifierKeys = ModifierKeys.None;
                Key key = Key.None;

                string[] hotkeys = args[0].Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string hotkey in hotkeys)
                {
                    if (modifierKeysConverter.IsValid(hotkey))
                        modifierKeys |= (ModifierKeys)modifierKeysConverter.ConvertFrom(hotkey);
                    else if (keyConverter.IsValid(hotkey))
                        key |= (Key)keyConverter.ConvertFrom(hotkey);
                    else
                    {
                        if (!isImport)
                            base.PushMessageToConveyor(new ErrorMessage("#Неверный код хоткея"), rootModel);
                    }
                }

                if (args.Length == 2)
                {
                    var group = rootModel.Groups.FirstOrDefault(gr => gr.Name == args[1]);
                    if (group != null)
                    {
                        var hotkey = group.Hotkeys.FirstOrDefault(x => x.GetKeyToString() == args[0]);
                        if (hotkey != null)
                        {
                            group.Hotkeys.TryTake(out hotkey);

                            hotkey.Group = group;
                            hotkey.Operation = UndoOperation.Add;
                            rootModel.UndoStack.Push(hotkey);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Хоткей удален: #hotkey {{{0}}} {{{1}}}",
                                    hotkey.GetKeyToString(), String.Join(";", hotkey.Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"), rootModel);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var group in rootModel.Groups)
                    {
                        var hotkey = group.Hotkeys.FirstOrDefault(x => x.GetKeyToString() == args[0]);
                        while (hotkey != null)
                        {
                            group.Hotkeys.TryTake(out hotkey);

                            hotkey.Group = group;
                            hotkey.Operation = UndoOperation.Add;
                            rootModel.UndoStack.Push(hotkey);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Хоткей удален: #hotkey {{{0}}} {{{1}}}",
                                    hotkey.GetKeyToString(), String.Join(";", hotkey.Actions.Select(action => action.ToString()).ToArray()))), rootModel);
                            }

                            hotkey = group.Hotkeys.FirstOrDefault(x => x.GetKeyToString() == args[0]);
                        }
                    }
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены последнего удаления используйте #undo"), rootModel);
                }

                return true;
            }
            return false;
        }

        private bool VariableCheck(string commandText, RootModel rootModel, bool isImport)
        {
            var match = _regexVariable.Match(commandText);
            if (match.Success)
            {
                var variableList = rootModel.VariableList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                    {
                        variableList.Clear();
                        variableList.AddRange(rootModel.Variables);

                        if (variableList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список переменных:"), rootModel);
                            for (int i = 0; i < variableList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #variable {{{1}}} {{{2}}}",
                                    i + 1, variableList[i].Name, variableList[i].Value)), rootModel);
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список переменных пуст."), rootModel);
                        }
                    }

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 2 || (args.Length > 0 && String.IsNullOrEmpty(args[0])))
                {
                    if (!isImport)
                    {
                        variableList.Clear();
                        variableList.AddRange(rootModel.Variables.Where(x => Regex.IsMatch(x.Value, args[0], RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)));

                        if (variableList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Список переменных, содержащих строку \"{0}\":", args[0])), rootModel);
                            for (int i = 0; i < variableList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #variable {{{1}}} {{{2}}}",
                                    i + 1, variableList[i].Name, variableList[i].Value)), rootModel);
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Переменные, содержащие строку \"{0}\" не найдены.", args[0])), rootModel);
                        }
                    }

                    return true;
                }

                string varName = args[0];
                string varValue = args[1];

                do
                {
                    if (!varName.StartsWith("$"))
                        break;

                    string str = rootModel.GetVariableValue(varName);
                    if (String.IsNullOrEmpty(str))
                        break;

                    varName = str;
                } while(true);

                do
                {
                    if (!varValue.StartsWith("$"))
                        break;

                    string str = rootModel.GetVariableValue(varValue);
                    if (String.IsNullOrEmpty(str))
                        break;

                    varValue = str;
                } while (true);

                if (!isImport)
                    rootModel.SetVariableValue(varName, varValue, false);
                else
                    rootModel.SetVariableValue(varName, varValue, true);

                return true;
            }

            match = _regexUnVariable.Match(commandText);
            if (match.Success)
            {
                var variableList = rootModel.VariableList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return true;
                }

                int num;
                if (int.TryParse(args[0], out num))
                {
                    if (num > 0 && num <= variableList.Count)
                    {

                        rootModel.ClearVariableValue(variableList[num - 1].Name, true);
                        if (!isImport)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Переменная удалена: #variable {{{0}}}",
                                variableList[num - 1].Name)), rootModel);
                        }
                    }
                    else
                    {
                        base.PushMessageToConveyor(new InfoMessage("#Неверный номер"), rootModel);
                    }

                    return true;
                }

                rootModel.ClearVariableValue(args[0], false);

                if (!isImport)
                {
                    base.PushMessageToConveyor(new InfoMessage(string.Format("#Переменная удалена: #variable {{{0}}}",
                        args[0])), rootModel);
                }

                return true;
            }

            return false;
        }

        private bool LogCheck(string commandText, RootModel rootModel, bool isImport)
        {
            var match = _regexLog.Match(commandText);
            if (match.Success)
            {
                if (rootModel.IsLogging)
                {
                    rootModel.PushMessageToConveyor(new StopLoggingMessage());
                }
                else
                {
                    if (match.Groups[1].Length == 0)
                    {
                        rootModel.PushMessageToConveyor(new StartLoggingMessage(string.Format("{0}-{1}", rootModel.Profile.Name, DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))));
                    }
                    else
                    {
                        rootModel.PushMessageToConveyor(new StartLoggingMessage(CommandLineParser.GetArgs(match.Groups[1].ToString())[0]));
                    }
                }

                return true;
            }

            match = _regesStopLog.Match(commandText);
            if (match.Success)
            {
                if (rootModel.IsLogging)
                {
                    rootModel.PushMessageToConveyor(new StopLoggingMessage());
                }
                else
                {
                    rootModel.PushMessageToConveyor(new ErrorMessage("#Лог не записывается."));
                }

                return true;
            }

            return false;
        }

        private bool ShowmeCheck(string commandText, RootModel rootModel, bool isImport)
        {
            var match = _regexShowme.Match(commandText);
            if (match.Success)
            {
                if (!match.Groups[1].Success)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Action List"), rootModel);

                    return true;
                }

                if (!isImport)
                    base.PushMessageToConveyor(new InfoMessage(match.Groups[1].ToString().Replace("{", String.Empty).Replace("}", String.Empty)), rootModel);

                return true;
            }

            return false;
        }

        private bool ConnectCheck(string commandText, RootModel rootModel, bool isImport)
        {
            var match = _regexConnect.Match(commandText);
            if (match.Success)
            {
                if (!match.Groups[1].Success)
                {
                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 2)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return true;
                }

                int port;
                if (!int.TryParse(args[1], out port))
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return true;
                }

                if (!isImport)
                    base.PushCommandToConveyor(new ConnectCommand(args[0], port), rootModel);

                return true;
            }

            return false;
        }

        private bool ZapCheck(string commandText, RootModel rootModel, bool isImport)
        {
            var match = _regexZap.Match(commandText);
            if (match.Success)
            {
                if (!isImport)
                    base.PushCommandToConveyor(new DisconnectCommand(), rootModel);

                return true;
            }

            return false;
        }

        private bool GroupCheck(string commandText, RootModel rootModel, bool isImport)
        {
            var match = _regexGroup.Match(commandText);
            if (match.Success)
            {
                if (!match.Groups[2].Success)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[2].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return true;
                }

                if (match.Groups[1].ToString() == "enable")
                {
                    rootModel.EnableGroup(args[0]);
                }
                else if (match.Groups[1].ToString() == "disable")
                {
                    rootModel.DisableGroup(args[0]);
                }
                else
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);
                }

                return true;
            }

            return false;
        }

        //TODO
        private bool StatusCheck(string commandText, RootModel rootModel, bool isImport)
        {
            var match = _regexStatus.Match(commandText);
            if (match.Success)
            {
                return true;
            }

            return false;
        }
    }
}
