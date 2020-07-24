
using Adan.Client.Common.Conveyor;

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
    using Common.Settings;
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
        private readonly Regex _regexZap = new Regex(@"#za?p?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexConnect = new Regex(@"#conn?e?c?t?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexStatusBar = new Regex(@"^#stat{1}u?s? (On|Off){1}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexStatus = new Regex(@"#stat{1}u?s? (\d{1}) \{{1}(.+)\}{1} ([^\s]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexStatusSimple = new Regex(@"#stat{1}u?s? (\d{1})\s*(.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexGroup = new Regex(@"#gr?o?u?p?\s*(enable|disable)\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _regexUndo = new Regex(@"#undo(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexTest = new Regex(@"#test (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public CommandsFromUserLineUnit(MessageConveyor conveyor) : base(conveyor)
        {
        }

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

        public override void HandleCommand([NotNull] Command command, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");

            var textCommand = command as TextCommand;
            if (textCommand == null)
            {
                return;
            }

            var commandText = textCommand.CommandText.Trim();

            if (commandText.Length > 1 && commandText[0] != SettingsHolder.Instance.Settings.CommandChar)
                return;
            
            if (TriggerCheck(commandText, isImport) || AliasCheck(commandText, isImport)
                || HighLightCheck(commandText, isImport) || HotkeyCheck(commandText, isImport)
                || StatusCheck(commandText) || SubstitutionCheck(commandText, isImport)
                || UndoCheck(commandText, isImport))
            {
                if (!isImport)
                {
                    SettingsHolder.Instance.SetProfile(Conveyor.RootModel.Profile.Name);
                    SettingsHolder.Instance.SetProfile("Global");
                }
                command.Handled = true;
                return;
            }

            if (VariableCheck(commandText, isImport) || ZapCheck(commandText, isImport)
                || ConnectCheck(commandText, isImport) || GroupCheck(commandText, isImport)
                || LogCheck(commandText) || ShowmeCheck(commandText, isImport)
                || TestCheck(commandText, isImport))
            {
                command.Handled = true;
            }

        }

        // Тест триггера
        // Эмулируем как будто строка пришла из мада и смотрим какие триги на нее сработают
        private bool TestCheck(string commandText, bool isImport)
        {
            // Нету такого в jmc
            if (isImport)
                return false;

            var match = _regexTest.Match(commandText);
            if (!match.Success)
                return false;

            var textToTest = match.Groups[1].ToString();
            var message = new OutputToMainWindowMessage(textToTest);
            PushMessageToConveyor(message);

            message.SkipTriggers = false;
            PushMessageToConveyor(new InfoMessage(string.Format("#Список триггеров, срабатывающих на строку '{0}':", textToTest)));
            var stopped = false;
            foreach (var trigger in Conveyor.RootModel.EnabledTriggersOrderedByPriority.OfType<TextTrigger>())
            {
                if (trigger.MatchMessage(message, Conveyor.RootModel))
                {
                    PushMessageToConveyor(new InfoMessage(trigger.ToString()));
                    if (!stopped && trigger.StopProcessingTriggersAfterThis)
                    {
                        PushMessageToConveyor(new InfoMessage("# ^--- После этого триггера строка дальше не будет обрабатываться (из-за флага StopProcessingTriggersAfterThis)!"));
                        stopped = true;
                    }
                }
            }

            return true;

        }

        private bool UndoCheck(string commandText, bool isImport)
        {
            if(isImport)
                return false;

            var match = _regexUndo.Match(commandText);
            if (match.Success)
            {
                var undoStack = Conveyor.RootModel.UndoStack;
                if (undoStack.Count > 0)
                {
                    IUndo undo = undoStack.Pop();
                    undo.Undo(Conveyor.RootModel);

                    if (!isImport)
                    {
                        PushMessageToConveyor(new InfoMessage(undo.UndoInfo()));
                    }
                }
                else
                {
                    if (!isImport)
                    {
                        PushMessageToConveyor(new InfoMessage("#Undo стэк пуст."));
                    }
                }

                return true;
            }

            return false;
        }

        private bool AliasCheck(string commandText, bool isImport)
        {
            var match = _regexAlias.Match(commandText);
            if (match.Success)
            {
                var aliasList = Conveyor.RootModel.AliasList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                    {
                        aliasList.Clear();
                        aliasList.AddRange(Conveyor.RootModel.Groups.SelectMany(gr => gr.Aliases));
                        if (aliasList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список алиасов:"));
                            for (int i = 0; i < aliasList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #alias {{{1}}} {{{2}}}",
                                    i + 1, aliasList[i].Command, String.Join(";", aliasList[i].Actions.Select(action => action.ToString()).ToArray()))));
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список алиасов пуст."));
                        }
                        base.PushMessageToConveyor(new InfoMessage("#Для отображения справки по алиасам, введите: #alias help"));
                    }

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length == 1 && args[0] == "help" && !isImport)
                {
                    base.PushMessageToConveyor(new InfoMessage("#Добавление алиаса: #alias {алиас} {действия}"));
                    base.PushMessageToConveyor(new InfoMessage("#Добавление алиаса в группу: #alias {алиас} {действия} {группа}"));
                    base.PushMessageToConveyor(new InfoMessage("#Действия можно разделять через ';', и внутри использовать %0 = все параметры, %1 = первый параметр, %2 второй параметр и т.п."));
                    base.PushMessageToConveyor(new InfoMessage("#Пример: #alias {б} {сбить %0;вст}"));
                    base.PushMessageToConveyor(new InfoMessage("#Теперь если вы напишете 'б шаман', в мад будут отправлены команды сбить шаман;встать."));
                    return true;
                }

                if (args.Length < 2)
                {
                    if (!isImport)
                    {
                        aliasList.Clear();
                        aliasList.AddRange(Conveyor.RootModel.Groups.SelectMany(gr => gr.Aliases
                            .Where(alias => Regex.IsMatch(alias.Command, args[0], RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))));

                        if (aliasList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Список алиасов, содержащих строку \"{0}\":", args[0])));
                            for (int i = 0; i < aliasList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #alias {{{1}}} {{{2}}}",
                                    i + 1, aliasList[i].Command, String.Join(";", aliasList[i].Actions.Select(action => action.ToString()).ToArray()))));
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Алиасы, содержащие строку \"{0}\" не найдены.", args[0])));
                        }
                    }

                    return true;
                }

                string groupName = "Default";
                
                if (args.Length >= 3)
                {
                    if (!isImport || args[2] != "default")
                    {
                        groupName = args[2];
                    }
                }

                CommandAlias commandAlias = new CommandAlias()
                {
                    Command = Regex.Replace(args[0], "%?%[0-9]", "").Trim()
                };

                commandAlias.Actions.Add(new SendTextAction()
                {
                    CommandText = args[1]
                });

                var group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                if (group == null)
                {
                    Conveyor.RootModel.AddGroup(groupName);
                    group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                }

                {
                    var alias = group.Aliases.FirstOrDefault(all => all.Command == args[0]);
                    while (alias != null)
                    {
                        group.Aliases.Remove(alias);

                        alias.Group = group;
                        alias.Operation = UndoOperation.Add;
                        Conveyor.RootModel.UndoStack.Push(alias);

                        if (!isImport)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Алиас удален: #alias {{{0}}} {{{1}}}",
                                alias.Command, String.Join(";", alias.Actions.Select(action => action.ToString()).ToArray()))));
                        }

                        alias = group.Aliases.FirstOrDefault(x => x.Command == args[0]);
                    }
                }

                group.Aliases.Add(commandAlias);

                if (!isImport)
                {
                    commandAlias.Group = group;
                    commandAlias.Operation = UndoOperation.Remove;
                    Conveyor.RootModel.UndoStack.Push(commandAlias);

                    base.PushMessageToConveyor(new InfoMessage(string.Format("#Алиас добавлен: #alias {{{0}}} {{{1}}}",
                        commandAlias.Command, commandAlias.Actions[0].ToString())));
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"));
                }

                return true;
            }

            match = _regexUnAlias.Match(commandText);
            if (!isImport && match.Success)
            {
                var aliasList = Conveyor.RootModel.AliasList;

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"));

                    return true;
                }

                int num;
                if (int.TryParse(args[0], out num))
                {
                    if (num > 0 && num <= aliasList.Count)
                    {
                        CommandAlias alias = aliasList[num - 1];
                        if (Conveyor.RootModel.Groups.FirstOrDefault(x => x.Aliases.Contains(alias)).Aliases.Remove(alias))
                        {
                            aliasList[num - 1].Group = alias.Group;
                            aliasList[num - 1].Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(aliasList[num - 1]);
                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Алиас удален: #alias {{{0}}} {{{1}}}",
                                    aliasList[num - 1].Command, String.Join(";", aliasList[num - 1].Actions.Select(action => action.ToString()).ToArray()))));
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"));
                            }
                        }

                    }
                    else
                    {
                        base.PushMessageToConveyor(new InfoMessage("#Неверный номер"));
                    }

                    return true;
                }

                if (args.Length == 2)
                {
                    var group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == args[1]);
                    if (group != null)
                    {
                        var alias = group.Aliases.FirstOrDefault(x => x.Command == args[0]);
                        if (alias != null)
                        {
                            group.Aliases.Remove(alias);

                            alias.Group = group;
                            alias.Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(alias);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Алиас удален: #alias {{{0}}} {{{1}}}",
                                    alias.Command, String.Join(";", alias.Actions.Select(action => action.ToString()).ToArray()))));
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"));
                            }
                        }
                    }
                }
                else
                {
                    foreach (var group in Conveyor.RootModel.Groups)
                    {
                        var alias = group.Aliases.FirstOrDefault(all => all.Command == args[0]);
                        while (alias != null)
                        {
                            group.Aliases.Remove(alias);

                            alias.Group = group;
                            alias.Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(alias);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Алиас удален: #alias {{{0}}} {{{1}}}",
                                    alias.Command, String.Join(";", alias.Actions.Select(action => action.ToString()).ToArray()))));
                            }

                            alias = group.Aliases.FirstOrDefault(all => all.Command == args[0]);
                        }
                    }
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены последнего удаления используйте #undo"));
                }

                return true;
            }

            return false;
        }

        private bool TriggerCheck(string commandText, bool isImport)
        {
            var match = _regexAction.Match(commandText);
            if (match.Success)
            {
                var triggersList = Conveyor.RootModel.TriggersList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                    {
                        triggersList.Clear();
                        triggersList.AddRange(Conveyor.RootModel.Groups.SelectMany(gr => gr.Triggers));

                        if (triggersList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список триггеров:"));
                            for (int i = 0; i < triggersList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. {1}", i + 1, triggersList[i])));
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список триггеров пуст."));
                        }
                        base.PushMessageToConveyor(new InfoMessage("#Для отображения справки по триггерам, введите: #action help"));
                    }

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length == 1 && args[0] == "help" && !isImport)
                {
                    PushMessageToConveyor(new InfoMessage("#Добавление триггера: #act {текст} {действия}"));
                    PushMessageToConveyor(new InfoMessage("#Добавление триггера с приоритетом: #act {текст} {действия} {приоритет}"));
                    PushMessageToConveyor(new InfoMessage("#Добавление триггера в указанную группу: #act {текст} {действия} {группа}"));
                    PushMessageToConveyor(new InfoMessage("#Добавление триггера с приоритетом и группой: #act {текст} {действия} {приоритет} {группа}"));
                    PushMessageToConveyor(new InfoMessage("#Приоритет обязательно должен быть числом. Триггеры с меньшим приоритетом выполняются первыми."));
                    PushMessageToConveyor(new InfoMessage("#По умолчанию, только один триггер может быть выполнен для одной строки пришедшей из MUD'а."));
                    PushMessageToConveyor(new InfoMessage("#Это поведение можно изменить через интерфейс (убрать галочку 'Stop processing trigger for this message')."));
                    PushMessageToConveyor(new InfoMessage("#Если текст начинается с ^ и не содержит %0 %1 %2 и т.п., он будет автоматически считаться регулярным выражением."));
                    PushMessageToConveyor(new InfoMessage("#Пример: #action {^Вы полетели на землю от мощного удара} {вст}"));
                    return true;
                }

                if (args.Length < 2)
                {
                    if (!isImport)
                    {
                        triggersList.Clear();
                        triggersList.AddRange(Conveyor.RootModel.Groups.SelectMany(gr => gr.Triggers
                            .Where(trig => Regex.IsMatch(trig.GetPatternString(), args[0], RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))));

                        if (triggersList.Count > 0)
                        {
                            PushMessageToConveyor(new InfoMessage(string.Format("#Список триггеров, содержащих строку \"{0}\":", args[0])));
                            for (int i = 0; i < triggersList.Count; i++)
                            {
                                PushMessageToConveyor(new InfoMessage(string.Format("{0}. {1}", i + 1, triggersList[i])));
                            }
                        }
                        else
                        {
                            PushMessageToConveyor(new InfoMessage(string.Format("#Триггеры, содержащие строку \"{0}\" не найдены.", args[0])));
                        }
                    }

                    return true;
                }

                string groupName = "Default";
                int priority = 5;

                if (args.Length >= 3)
                {
                    if (!int.TryParse(args[2], out priority))
                    {
                        if (!isImport)
                            PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка: приоритет (3й аргумент) должен быть числом."));
                        return true;
                    }
                }

                if (args.Length >= 4)
                {
                    if (!isImport || args[3] != "default")
                    {
                        groupName = args[3];
                    }
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

                var group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                if (group == null)
                {
                    Conveyor.RootModel.AddGroup(groupName);
                    group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                }

                {
                    var trig = group.Triggers.FirstOrDefault(x => x.GetPatternString() == args[0]);
                    while (trig != null)
                    {
                        group.Triggers.Remove(trig);

                        trig.Group = group;
                        trig.Operation = UndoOperation.Add;
                        Conveyor.RootModel.UndoStack.Push(trig);

                        if (!isImport)
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Триггер удален: " + trigger));
                        }

                        trig = group.Triggers.FirstOrDefault(x => x.GetPatternString() == args[0]);
                    }
                }

                group.Triggers.Add(trigger);

                Conveyor.RootModel.RecalculatedEnabledTriggersPriorities();

                if (!isImport)
                {
                    trigger.Group = group;
                    trigger.Operation = UndoOperation.Remove;
                    Conveyor.RootModel.UndoStack.Push(trigger);
                    base.PushMessageToConveyor(new InfoMessage("#Триггер добавлен (#undo - отменить): " + trigger));
                }

                return true;
            }

            match = _regexUnAction.Match(commandText);
            if (match.Success)
            {
                var triggersList = Conveyor.RootModel.TriggersList;

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"));

                    return true;
                }

                int num;
                if (int.TryParse(args[0], out num))
                {
                    if (num > 0 && num <= triggersList.Count)
                    {
                        TriggerBase trigger = triggersList[num - 1];
                        if (Conveyor.RootModel.Groups.FirstOrDefault(x => x.Triggers.Contains(trigger)).Triggers.Remove(trigger))
                        {
                            triggersList[num - 1].Group = trigger.Group;
                            triggersList[num - 1].Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(triggersList[num - 1]);

                            if (!isImport)
                            {
                                PushMessageToConveyor(new InfoMessage("#Триггер удален (#undo - отменить): " + trigger));
                            }
                        }
                    }
                    else
                    {
                        base.PushMessageToConveyor(new InfoMessage("#Неверный номер"));
                    }

                    return true;
                }

                if (args.Length == 2)
                {
                    var group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == args[1]);
                    if (group != null)
                    {
                        var trig = group.Triggers.FirstOrDefault(x => x.GetPatternString() == args[0]);
                        if (trig != null)
                        {
                            group.Triggers.Remove(trig);

                            trig.Group = group;
                            trig.Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(trig);

                            if (!isImport)
                            {
                                PushMessageToConveyor(new InfoMessage("#Триггер удален (#undo - отменить): " + trig));
                            }
                        }
                    }
                }
                else
                {
                    foreach (var group in Conveyor.RootModel.Groups)
                    {
                        var trig = group.Triggers.FirstOrDefault(x => x.GetPatternString() == args[0]);
                        while (trig != null)
                        {
                            group.Triggers.Remove(trig);

                            trig.Group = group;
                            trig.Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(trig);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Триггер удален: #action {{{0}}} {{{1}}}",
                                    trig.GetPatternString(), String.Join(";", trig.Actions.Select(action => action.ToString()).ToArray()))));
                            }

                            trig = group.Triggers.FirstOrDefault(x => x.GetPatternString() == args[0]);
                        }
                    }

                    base.PushMessageToConveyor(new InfoMessage("#Для отмены последнего удаления используйте #undo"));
                }

                Conveyor.RootModel.RecalculatedEnabledTriggersPriorities();

                return true;
            }

            return false;
        }

        private bool HighLightCheck(string commandText, bool isImport)
        {
            var match = _regexHighLight.Match(commandText);
            if (match.Success)
            {
                var highlightList = Conveyor.RootModel.HighlightList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                    {
                        highlightList.Clear();
                        highlightList.AddRange(Conveyor.RootModel.Groups.SelectMany(x => x.Highlights));

                        if (highlightList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список хайлайтов:"));
                            for (int i = 0; i < highlightList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #highlight {{{2},{3}}} {{{1}}}",
                                    i + 1, highlightList[i].TextToHighlight, highlightList[i].ForegroundColor, highlightList[i].BackgroundColor)));
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список хайлайтов пуст."));
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
                        highlightList.AddRange(Conveyor.RootModel.Groups.SelectMany(x => x.Highlights
                            .Where(hi => Regex.IsMatch(hi.TextToHighlight, args[0], RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))));

                        if (highlightList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Список хайлайтов, содержащих строку \"{0}\":", args[0])));
                            for (int i = 0; i < highlightList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #highlight {{{2},{3}}} {{{1}}}",
                                    i + 1, highlightList[i].TextToHighlight, highlightList[i].ForegroundColor, highlightList[i].BackgroundColor)));
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Хайлаты, содержащие строку \"{0}\" не найдены.", args[0])));
                        }
                    }

                    return true;
                }

                string groupName = "Default";

                if (args.Length >= 3)
                {
                    if (!isImport || args[2] != "default")
                    {
                        groupName = args[2];
                    }
                }

                var group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                if (group == null)
                {
                    Conveyor.RootModel.AddGroup(groupName);
                    group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                }

                Highlight highlight = new Highlight()
                {
                    TextToHighlight = args[1]
                };

                string[] colors = args[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                TextColor foregroundColor = TextColorParser.Parse(colors[0].Trim());
                TextColor backgroundColor;
                if (colors.Length == 2)
                    backgroundColor = TextColorParser.Parse(colors[1].Trim());
                else
                    backgroundColor = TextColor.Black;

                if (foregroundColor == TextColor.None && backgroundColor == TextColor.None)
                {
                    if (!isImport)
                    {
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка. Формат: #highlight {цвет текста,цвет фона} {текст который подсвечивать}"));
                        base.PushMessageToConveyor(new ErrorMessage("#Возможные варианты цвета: " + String.Join(", ", Enum.GetValues(typeof(TextColor)).OfType<TextColor>())));
                    }

                    return true;
                }

                highlight.ForegroundColor = foregroundColor;
                highlight.BackgroundColor = backgroundColor;

                {
                    var high = group.Highlights.FirstOrDefault(x => x.TextToHighlight == args[0]);
                    while (high != null)
                    {
                        group.Highlights.Remove(high);

                        high.Group = group;
                        high.Operation = UndoOperation.Add;
                        Conveyor.RootModel.UndoStack.Push(high);

                        if (!isImport)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Хайлайт удален: #highlight {{{1},{2}}} {{{0}}}",
                                high.TextToHighlight, high.ForegroundColor, high.BackgroundColor)));
                        }

                        high = group.Highlights.FirstOrDefault(x => x.TextToHighlight == args[0]);
                    }
                }

                group.Highlights.Add(highlight);

                highlight.Group = group;
                highlight.Operation = UndoOperation.Remove;
                Conveyor.RootModel.UndoStack.Push(highlight);

                if (!isImport)
                {
                    base.PushMessageToConveyor(new InfoMessage(string.Format("#Хайлайт добавлен: #highlight {{{1}, {2}}} {{{0}}}",
                           highlight.TextToHighlight, highlight.ForegroundColor, highlight.BackgroundColor)));
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"));
                }

                return true;
            }

            match = _regexUnHighLight.Match(commandText);
            if (match.Success)
            {
                var highlightList = Conveyor.RootModel.HighlightList;

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"));
                    return true;
                }

                int num;
                if (int.TryParse(args[0], out num))
                {
                    if (num > 0 && num <= highlightList.Count)
                    {
                        Highlight highlight = highlightList[num - 1];
                        if (Conveyor.RootModel.Groups.FirstOrDefault(x => x.Highlights.Contains(highlight)).Highlights.Remove(highlight))
                        {
                            highlightList[num - 1].Group = highlight.Group;
                            highlightList[num - 1].Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(highlightList[num - 1]);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Хайлайт удален: #highlight {{{1}, {2}}} {{{0}}}",
                                    highlightList[num - 1].TextToHighlight, highlightList[num - 1].ForegroundColor, highlightList[num - 1].BackgroundColor)));
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"));
                            }
                        }
                    }
                    else
                    {
                        base.PushMessageToConveyor(new InfoMessage("#Неверный номер"));
                    }

                    return true;
                }

                if (args.Length == 2)
                {
                    var group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == args[1]);
                    if (group != null)
                    {
                        var high = group.Highlights.FirstOrDefault(x => x.TextToHighlight == args[0]);
                        if (high != null)
                        {
                            group.Highlights.Remove(high);

                            high.Group = group;
                            high.Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(high);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Хайлайт удален: #highlight {{{1},{2}}} {{{0}}}",
                                    high.TextToHighlight, high.ForegroundColor, high.BackgroundColor)));
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"));
                            }
                        }
                    }
                }
                else
                {
                    foreach (var group in Conveyor.RootModel.Groups)
                    {
                        var high = group.Highlights.FirstOrDefault(x => x.TextToHighlight == args[0]);
                        while (high != null)
                        {
                            group.Highlights.Remove(high);

                            high.Group = group;
                            high.Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(high);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Хайлайт удален: #highlight {{{1},{2}}} {{{0}}}",
                                    high.TextToHighlight, high.ForegroundColor, high.BackgroundColor)));
                            }

                            high = group.Highlights.FirstOrDefault(x => x.TextToHighlight == args[0]);
                        }
                    }

                    base.PushMessageToConveyor(new InfoMessage("#Для отмены последнего удаления используйте #undo"));
                }

                return true;
            }

            return false;
        }

        private bool SubstitutionCheck(string commandText, bool isImport)
        {
            var match = _regexSubstitution.Match(commandText);
            if (match.Success)
            {
                var substitutionList = Conveyor.RootModel.SubstitutionList;
                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                    {
                        substitutionList.Clear();
                        substitutionList.AddRange(Conveyor.RootModel.Groups.SelectMany(x => x.Substitutions));

                        if (substitutionList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список замен:"));
                            for (int i = 0; i < substitutionList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #substitution {{{1}}} {{{2}}}",
                                    i + 1, substitutionList[i].Pattern, substitutionList[i].SubstituteWith)));
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список замен пуст."));
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
                        substitutionList.AddRange(Conveyor.RootModel.Groups.SelectMany(x => x.Substitutions
                            .Where(hi => Regex.IsMatch(hi.Pattern, args[0], RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))));

                        if (substitutionList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Список замен, содержащих строку \"{0}\":", args[0])));
                            for (int i = 0; i < substitutionList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #substitution {{{1}}} {{{2}}}",
                                    i + 1, substitutionList[i].Pattern, substitutionList[i].SubstituteWith)));
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Замены, содержащие строку \"{0}\" не найдены.", args[0])));
                        }
                    }

                    return true;
                }

                string groupName = "Default";

                if (args.Length >= 3)
                {
                    if (!isImport || args[2] != "default")
                    {
                        groupName = args[2];
                    }
                }

                var group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                if (group == null)
                {
                    Conveyor.RootModel.AddGroup(groupName);
                    group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                }

                Substitution subtitution = new Substitution() { Pattern = args[0], SubstituteWith = args[1] };

                {
                    var sub = group.Substitutions.FirstOrDefault(x => x.Pattern == args[0]);
                    while (sub != null)
                    {
                        group.Substitutions.Remove(sub);

                        sub.Group = group;
                        sub.Operation = UndoOperation.Add;
                        Conveyor.RootModel.UndoStack.Push(sub);

                        if (!isImport)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Замена удалена: #substitution {{{0}}} {{{1}}}",
                                sub.Pattern, sub.SubstituteWith)));
                        }

                        sub = group.Substitutions.FirstOrDefault(x => x.Pattern == args[0]);
                    }
                }

                group.Substitutions.Add(subtitution);

                subtitution.Group = group;
                subtitution.Operation = UndoOperation.Remove;
                Conveyor.RootModel.UndoStack.Push(subtitution);

                if (!isImport)
                {
                    base.PushMessageToConveyor(new InfoMessage(string.Format("#Замена добавлена: #subtitution {{{0}}} {{{1}}}",
                           subtitution.Pattern, subtitution.SubstituteWith)));
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"));
                }

                return true;
            }

            match = _regexUnSubstitution.Match(commandText);
            if (match.Success)
            {
                var substitutionList = Conveyor.RootModel.SubstitutionList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"));

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"));

                    return true;
                }

                int num;
                if (int.TryParse(args[0], out num))
                {
                    if (num > 0 && num <= substitutionList.Count)
                    {
                        Substitution sub = substitutionList[num - 1];
                        if (Conveyor.RootModel.Groups.FirstOrDefault(x => x.Substitutions.Contains(sub)).Substitutions.Remove(sub))
                        {
                            substitutionList[num - 1].Group = sub.Group;
                            substitutionList[num - 1].Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(substitutionList[num - 1]);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Замена удалена: #substitution {{{0}}} {{{1}}}",
                                    substitutionList[num - 1].Pattern, substitutionList[num - 1].SubstituteWith)));
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"));
                            }
                        }

                        //for (int i = 0; i < Conveyor.RootModel.Groups.Count; i++)
                        //{
                        //    if (Conveyor.RootModel.Groups[i].Substitutions.Remove(substitutionList[num - 1]))
                        //    {
                        //        substitutionList[num - 1].Group = Conveyor.RootModel.Groups[i];
                        //        substitutionList[num - 1].Operation = UndoOperation.Add;
                        //        Conveyor.RootModel.UndoStack.Push(substitutionList[num - 1]);

                        //        if (!isImport)
                        //        {
                        //            base.PushMessageToConveyor(new InfoMessage(string.Format("#Замена удалена: #substitution {{{0}}} {{{1}}}",
                        //                substitutionList[num - 1].Pattern, substitutionList[num - 1].SubstituteWith)));
                        //            base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"));
                        //        }

                        //        break;
                        //    }
                        //}
                    }
                    else
                    {
                        base.PushMessageToConveyor(new InfoMessage("#Неверный номер"));
                    }

                    return true;
                }

                if (args.Length >= 2)
                {
                    var group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == args[1]);
                    if (group != null)
                    {
                        var sub = group.Substitutions.FirstOrDefault(x => x.Pattern == args[0]);
                        if (sub != null)
                        {
                            group.Substitutions.Remove(sub);

                            sub.Group = group;
                            sub.Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(sub);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Замена удалена: #substitution {{{0}}} {{{1}}}",
                                    sub.Pattern, sub.SubstituteWith)));
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"));
                            }
                        }
                    }
                }
                else
                {
                    foreach (var group in Conveyor.RootModel.Groups)
                    {
                        var sub = group.Substitutions.FirstOrDefault(x => x.Pattern == args[0]);
                        while (sub != null)
                        {
                            group.Substitutions.Remove(sub);

                            sub.Group = group;
                            sub.Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(sub);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Замена удалена: #substitution {{{0}}} {{{1}}}",
                                    sub.Pattern, sub.SubstituteWith)));
                            }

                            sub = group.Substitutions.FirstOrDefault(x => x.Pattern == args[0]);
                        }
                    }
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены последнего удаления используйте #undo"));
                }

                return true;
            }

            return false;
        }

        private bool HotkeyCheck(string commandText, bool isImport)
        {
            var match = _regexHotkey.Match(commandText);

            if (match.Success)
            {
                var hotkeysList = Conveyor.RootModel.HotkeyList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                    {
                        hotkeysList.Clear();
                        hotkeysList.AddRange(Conveyor.RootModel.Groups.SelectMany(x => x.Hotkeys));

                        if (hotkeysList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список хоткеев:"));
                            for (int i = 0; i < hotkeysList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #hotkey {{{1}}} {{{2}}}",
                                    i + 1, hotkeysList[i].GetKeyToString(), String.Join(";", hotkeysList[i].Actions.Select(action => action.ToString()).ToArray()))));
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список хоткеев пуст."));
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
                        hotkeysList.AddRange(Conveyor.RootModel.Groups.SelectMany(x => x.Hotkeys
                            .Where(hot => Regex.IsMatch(hot.GetKeyToString(), args[0], RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))));

                        if (hotkeysList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Список хоткеев, содержащих строку \"{0}\":", args[0])));
                            for (int i = 0; i < hotkeysList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #hotkey {{{1}}} {{{2}}}",
                                    i + 1, hotkeysList[i].GetKeyToString(), String.Join(";", hotkeysList[i].Actions.Select(action => action.ToString()).ToArray()))));
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Хоткеи, содержащие строку \"{0}\" не найдены.", args[0])));
                        }
                    }

                    return true;
                }

                string groupName = "Default";

                if (args.Length >= 3)
                {
                    if (!isImport || args[2] != "default")
                    {
                        groupName = args[2];
                    }
                }

                var group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
                if (group == null)
                {
                    Conveyor.RootModel.AddGroup(groupName);
                    group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == groupName);
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
                            base.PushMessageToConveyor(new ErrorMessage("#Неверный код хоткея"));

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
                        group.Hotkeys.Remove(hotkey);

                        hotkey.Group = group;
                        hotkey.Operation = UndoOperation.Add;
                        Conveyor.RootModel.UndoStack.Push(hotkey);

                        if (!isImport)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Хоткей удален: #hotkey {{{0}}} {{{1}}}",
                                hotkey.GetKeyToString(), String.Join(";", hotkey.Actions.Select(action => action.ToString()).ToArray()))));
                        }

                        hotkey = group.Hotkeys.FirstOrDefault(x => x.GetKeyToString() == args[0]);
                    }
                }

                group.Hotkeys.Add(hotk);

                hotk.Group = group;
                hotk.Operation = UndoOperation.Remove;
                Conveyor.RootModel.UndoStack.Push(hotk);

                Conveyor.RootModel.RecalculatedEnabledTriggersPriorities();

                if (!isImport)
                {
                    base.PushMessageToConveyor(new InfoMessage(string.Format("#Хоткей добавлен: #hotkey {{{0}}} {{{1}}}",
                        hotk.GetKeyToString(), hotk.Actions[0].ToString())));
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"));
                }

                return true;
            }

            match = _regexUnHotkey.Match(commandText);
            if (match.Success)
            {
                var hotkeyList = Conveyor.RootModel.HotkeyList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"));

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"));

                    return true;
                }


                int num;
                if (int.TryParse(args[0], out num))
                {
                    if (num > 0 && num <= hotkeyList.Count)
                    {
                        Hotkey hotkey = hotkeyList[num - 1];
                        if (Conveyor.RootModel.Groups.FirstOrDefault(x => x.Hotkeys.Contains(hotkey)).Hotkeys.Remove(hotkey))
                        {
                            hotkeyList[num - 1].Group = hotkey.Group;
                            hotkeyList[num - 1].Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(hotkeyList[num - 1]);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Хоткей удален: #hotkey {{{0}}} {{{1}}}",
                                    hotkeyList[num - 1].GetKeyToString(), String.Join(";", hotkeyList[num - 1].Actions.Select(action => action.ToString()).ToArray()))));
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"));
                            }
                        }

                        //for (int i = 0; i < Conveyor.RootModel.Groups.Count; i++)
                        //{
                        //    if (Conveyor.RootModel.Groups[i].Hotkeys.Remove(hotkeyList[num - 1]))
                        //    {
                        //        hotkeyList[num - 1].Group = Conveyor.RootModel.Groups[i];
                        //        hotkeyList[num - 1].Operation = UndoOperation.Add;
                        //        Conveyor.RootModel.UndoStack.Push(hotkeyList[num - 1]);

                        //        if (!isImport)
                        //        {
                        //            base.PushMessageToConveyor(new InfoMessage(string.Format("#Хоткей удален: #hotkey {{{0}}} {{{1}}}",
                        //                hotkeyList[num - 1].GetKeyToString(), String.Join(";", hotkeyList[num - 1].Actions.Select(action => action.ToString()).ToArray()))));
                        //            base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"));
                        //        }

                        //        break;
                        //    }
                        //}

                        return true;
                    }
                    else
                    {
                        base.PushMessageToConveyor(new InfoMessage("#Неверный номер"));
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
                            base.PushMessageToConveyor(new ErrorMessage("#Неверный код хоткея"));
                    }
                }

                if (args.Length == 2)
                {
                    var group = Conveyor.RootModel.Groups.FirstOrDefault(gr => gr.Name == args[1]);
                    if (group != null)
                    {
                        var hotkey = group.Hotkeys.FirstOrDefault(x => x.GetKeyToString() == args[0]);
                        if (hotkey != null)
                        {
                            group.Hotkeys.Remove(hotkey);

                            hotkey.Group = group;
                            hotkey.Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(hotkey);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Хоткей удален: #hotkey {{{0}}} {{{1}}}",
                                    hotkey.GetKeyToString(), String.Join(";", hotkey.Actions.Select(action => action.ToString()).ToArray()))));
                                base.PushMessageToConveyor(new InfoMessage("#Для отмены используйте #undo"));
                            }
                        }
                    }
                }
                else
                {
                    foreach (var group in Conveyor.RootModel.Groups)
                    {
                        var hotkey = group.Hotkeys.FirstOrDefault(x => x.GetKeyToString() == args[0]);
                        while (hotkey != null)
                        {
                            group.Hotkeys.Remove(hotkey);

                            hotkey.Group = group;
                            hotkey.Operation = UndoOperation.Add;
                            Conveyor.RootModel.UndoStack.Push(hotkey);

                            if (!isImport)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("#Хоткей удален: #hotkey {{{0}}} {{{1}}}",
                                    hotkey.GetKeyToString(), String.Join(";", hotkey.Actions.Select(action => action.ToString()).ToArray()))));
                            }

                            hotkey = group.Hotkeys.FirstOrDefault(x => x.GetKeyToString() == args[0]);
                        }
                    }
                    base.PushMessageToConveyor(new InfoMessage("#Для отмены последнего удаления используйте #undo"));
                }

                return true;
            }
            return false;
        }

        //@TODO: Check for reserved variable names such as DATE, TIME, Monster, GroupMate, DisplayStatusBar, StatusBar1, etc.
        private bool VariableCheck(string commandText, bool isImport)
        {
            var match = _regexVariable.Match(commandText);
            if (match.Success)
            {
                var variableList = Conveyor.RootModel.VariableList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                    {
                        variableList.Clear();
                        variableList.AddRange(Conveyor.RootModel.Variables);

                        if (variableList.Count > 0)
                        {
                            PushMessageToConveyor(new InfoMessage("#Список переменных:"));
                            for (int i = 0; i < variableList.Count; i++)
                            {
                                PushMessageToConveyor(new InfoMessage(string.Format("{0}. #variable {{{1}}} {{{2}}}", i + 1, variableList[i].Name, variableList[i].Value)));
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage("#Список переменных пуст."));
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
                        variableList.AddRange(Conveyor.RootModel.Variables.Where(x => Regex.IsMatch(x.Value, args[0], RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)));

                        if (variableList.Count > 0)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Список переменных, содержащих строку \"{0}\":", args[0])));
                            for (int i = 0; i < variableList.Count; i++)
                            {
                                base.PushMessageToConveyor(new InfoMessage(string.Format("{0}. #variable {{{1}}} {{{2}}}",
                                    i + 1, variableList[i].Name, variableList[i].Value)));
                            }
                        }
                        else
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Переменные, содержащие строку \"{0}\" не найдены.", args[0])));
                        }
                    }

                    return true;
                }

                string varName = args[0].StartsWith("$") ? Conveyor.RootModel.GetVariableValue(args[0].Substring(1)) : args[0];
                string varValue = args[1].StartsWith("$") ? Conveyor.RootModel.GetVariableValue(args[1].Substring(1)) : args[1];                

                if (!isImport)
                {
                    Conveyor.RootModel.SetVariableValue(varName, varValue, false);
                }
                else
                {
                    Conveyor.RootModel.SetVariableValue(varName, varValue, true);
                }

                return true;
            }

            match = _regexUnVariable.Match(commandText);
            if (match.Success)
            {
                var variableList = Conveyor.RootModel.VariableList;

                if (match.Groups[1].Length == 0)
                {
                    if (!isImport)
                    {
                        PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"));
                    }

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"));

                    return true;
                }

                int num;
                if (int.TryParse(args[0], out num))
                {
                    if (num > 0 && num <= variableList.Count)
                    {

                        Conveyor.RootModel.ClearVariableValue(variableList[num - 1].Name, true);
                        if (!isImport)
                        {
                            base.PushMessageToConveyor(new InfoMessage(string.Format("#Переменная удалена: #variable {{{0}}}",
                                variableList[num - 1].Name)));
                        }
                    }
                    else
                    {
                        base.PushMessageToConveyor(new InfoMessage("#Неверный номер"));
                    }

                    return true;
                }

                Conveyor.RootModel.ClearVariableValue(args[0], false);

                if (!isImport)
                {
                    base.PushMessageToConveyor(new InfoMessage(string.Format("#Переменная удалена: #variable {{{0}}}",
                        args[0])));
                }

                return true;
            }

            return false;
        }

        private bool LogCheck(string commandText)
        {
            var match = _regexLog.Match(commandText);
            if (match.Success)
            {
                if (match.Groups[1].Length == 0)
                {
                    Conveyor.RootModel.PushMessageToConveyor(new StartLoggingMessage(string.Format("{0}-{1}", Conveyor.RootModel.Profile.Name, DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))));
                }
                else
                {
                    Conveyor.RootModel.PushMessageToConveyor(new StartLoggingMessage(CommandLineParser.GetArgs(match.Groups[1].ToString())[0]));
                }

                return true;
            }

            match = _regesStopLog.Match(commandText);
            if (match.Success)
            {
                Conveyor.RootModel.PushMessageToConveyor(new StopLoggingMessage());
                return true;
            }

            return false;
        }

        private bool ShowmeCheck(string commandText, bool isImport)
        {
            var match = _regexShowme.Match(commandText);
            if (match.Success)
            {
                if (!match.Groups[1].Success)
                {
                    if (!isImport)
                    {
                        PushMessageToConveyor(new InfoMessage("#Action List"));
                    }

                    return true;
                }

                if (!isImport)
                {
                    PushMessageToConveyor(new InfoMessage(match.Groups[1].ToString().Replace("{", String.Empty).Replace("}", String.Empty)));
                }

                return true;
            }

            return false;
        }

        private bool ConnectCheck(string commandText, bool isImport)
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
                    {
                        PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"));
                    }

                    return true;
                }

                int port;
                if (!int.TryParse(args[1], out port))
                {
                    if (!isImport)
                    {
                        PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"));
                    }

                    return true;
                }

                if (!isImport)
                {
                    PushCommandToConveyor(new ConnectCommand(args[0], port));
                }

                return true;
            }

            return false;
        }

        private bool ZapCheck(string commandText, bool isImport)
        {
            var match = _regexZap.Match(commandText);
            if (match.Success)
            {
                if (!isImport)
                {
                    PushCommandToConveyor(new DisconnectCommand());
                }

                return true;
            }

            return false;
        }

        private bool GroupCheck(string commandText, bool isImport)
        {
            var match = _regexGroup.Match(commandText);
            if (match.Success)
            {
                if (!match.Groups[2].Success)
                {
                    if (!isImport)
                    {
                        PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"));
                    }

                    return true;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[2].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"));

                    return true;
                }

                if (match.Groups[1].ToString() == "enable")
                {
                    Conveyor.RootModel.EnableGroup(args[0]);
                }
                else if (match.Groups[1].ToString() == "disable")
                {
                    Conveyor.RootModel.DisableGroup(args[0]);
                }
                else
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"));
                }

                return true;
            }

            return false;
        }

        private bool StatusCheck(string commandText)
        {
            // #status on|off
            var match = _regexStatusBar.Match(commandText);
            if (match.Success)
            {
                if (match.Groups.Count > 0)
                {
                    if (string.Equals(match.Groups[1].Value, "On", StringComparison.OrdinalIgnoreCase))
                        Conveyor.RootModel.PushMessageToConveyor(new ShowStatusBarMessage(true));
                    else
                        Conveyor.RootModel.PushMessageToConveyor(new ShowStatusBarMessage(false));
                }
                return true;
            }

            // #stat 1 {message} color
            match = _regexStatus.Match(commandText);
            if (match.Success)
            {
                if (match.Groups.Count == 4)
                {
                    Conveyor.RootModel.PushMessageToConveyor(new SetStatusMessage(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value));
                }
                return true;
            }

            // #stat 1 message
            // or
            // #stat 1
            match = _regexStatusSimple.Match(commandText);
            if (match.Success)
            {
                if (match.Groups.Count == 3)
                {
                    Conveyor.RootModel.PushMessageToConveyor(new SetStatusMessage(match.Groups[1].Value, match.Groups[2].Value));
                }
                return true;
            }

            return false;
        }
    }
}
