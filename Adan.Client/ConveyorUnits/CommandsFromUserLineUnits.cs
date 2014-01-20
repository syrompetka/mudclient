using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adan.Client.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Input;

    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Model;
    using Common.Messages;
    using Common.Themes;
    using Common.Utils;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Adan.Client.Commands;
    using Adan.Client.Model.Actions;
    using Adan.Client.Messages;

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
        private readonly Regex _regexShowme = new Regex(@"#sho?w?m?e?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexTickOn = new Regex(@"#tickon\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexTickOff = new Regex(@"#tickoff?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexTickSize = new Regex(@"#ticksi?z?e?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexZap = new Regex(@"#za?p?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexConnect = new Regex(@"#conn?e?c?t?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexStatus = new Regex(@"#stat?u?s?\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _regexGroup = new Regex(@"#gr?o?u?p?\s*(enable|disable)\s*(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandsFromUserLineUnit"/> class.
        /// </summary>
        public CommandsFromUserLineUnit()
            : base()
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

            string commandText = textCommand.CommandText.Trim();

            if (!commandText.StartsWith(RootModel.CommandChar.ToString()))
                return;

            Match match;

            match = _regexAction.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[1].Success)
                {
                    if(!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Triggers List"), rootModel);
                    return;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 2)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);
                    return;
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
                        return;
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

                group.Triggers.RemoveAll(tr => ((TextTrigger)tr).MatchingPattern == trigger.MatchingPattern);
                group.Triggers.Add(trigger);

                rootModel.RecalculatedEnabledTriggersPriorities();

                if (!isImport) 
                    base.PushMessageToConveyor(new InfoMessage("#Триггер добавлен"), rootModel);

                return;
            }

            match = _regexUnAction.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[1].Success)
                {
                    return;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);
                    return;
                }

                if (args.Length == 2)
                {
                    var group = rootModel.Groups.FirstOrDefault(gr => gr.Name == args[1]);
                    if (group != null)
                    {
                        group.Triggers.RemoveAll(tr => ((TextTrigger)tr).MatchingPattern == args[0]);

                        if (!isImport)
                            base.PushMessageToConveyor(new InfoMessage("#Триггеры удалены."), rootModel);
                    }
                }
                else
                {
                    foreach (var group in rootModel.Groups)
                    {
                        group.Triggers.RemoveAll(tr => ((TextTrigger)tr).MatchingPattern == args[0]);
                        if (!isImport)
                            base.PushMessageToConveyor(new InfoMessage("#Триггеры удалены."), rootModel);
                    }
                }

                rootModel.RecalculatedEnabledTriggersPriorities();

                return;
            }

            match = _regexAlias.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[1].Success)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Alias List"), rootModel);
                    return;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 2)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);
                    return;
                }

                string groupName = "Default";

                if (args.Length == 3)
                    groupName = args[2];

                string comm = Regex.Replace(args[0], "%[0-9]", "").Trim();

                CommandAlias commandAlias = new CommandAlias()
                {
                    Command = comm
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

                group.Aliases.RemoveAll(allias => allias.Command == commandAlias.Command);
                group.Aliases.Add(commandAlias);

                if (!isImport)
                    base.PushMessageToConveyor(new InfoMessage("#Алиас добавлен"), rootModel);

                return;
            }

            match = _regexUnAlias.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[1].Success)
                {
                    return;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);
                    return;
                }

                if (args.Length == 2)
                {
                    var group = rootModel.Groups.FirstOrDefault(gr => gr.Name == args[1]);
                    if (group != null)
                    {
                        group.Aliases.RemoveAll(all => all.Command == args[0]);
                        if (!isImport)
                            base.PushMessageToConveyor(new InfoMessage("#Алиас удален"), rootModel);
                    }
                }
                else
                {
                    foreach (var group in rootModel.Groups)
                    {
                        group.Aliases.RemoveAll(all => all.Command == args[0]);
                    }

                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Алиас удален"), rootModel);
                }

                return;
            }

            match = _regexHighLight.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[1].Success)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Highlight List"), rootModel);
                    return;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 2)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);
                    return;
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
                    return;

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
                    return;
                }

                highlight.ForegroundColor = foregroundColor;
                highlight.BackgroundColor = backgroundColor;

                group.Highlights.RemoveAll(hi => ((Highlight)hi).TextToHighlight == args[2]);
                group.Highlights.Add(highlight);

                if (!isImport)
                    base.PushMessageToConveyor(new InfoMessage("#Подсветка добавлена"), rootModel);

                return;
            }

            match = _regexUnHighLight.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[1].Success)
                {
                    return;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);
                    return;
                }

                if (args.Length == 2)
                {
                    var group = rootModel.Groups.FirstOrDefault(gr => gr.Name == args[1]);
                    if (group != null)
                    {
                        group.Highlights.RemoveAll(hi => ((Highlight)hi).TextToHighlight == args[0]);
                        if (!isImport)
                            base.PushMessageToConveyor(new InfoMessage("#Подсветка удалена"), rootModel);
                    }
                }
                else
                {
                    foreach (var group in rootModel.Groups)
                    {
                        group.Highlights.RemoveAll(hi => ((Highlight)hi).TextToHighlight == args[0]);
                    }

                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Подсветка удалена"), rootModel);
                }

                return;
            }

            match = _regexSubstitution.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[1].Success)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Substitute List"), rootModel);
                    return;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 2)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);
                    return;
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

                group.Substitutions.RemoveAll(sub => sub.Pattern == subtitution.Pattern);
                group.Substitutions.Add(subtitution);

                if (!isImport)
                    base.PushMessageToConveyor(new InfoMessage("#Замена добавлена"), rootModel);

                return;
            }

            match = _regexUnSubstitution.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[1].Success)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Action List"), rootModel);
                    return;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);
                    return;
                }

                if (args.Length == 2)
                {
                    var group = rootModel.Groups.FirstOrDefault(gr => gr.Name == args[1]);
                    if (group != null)
                    {
                        group.Substitutions.RemoveAll(sub => sub.Pattern == args[0]);
                        if (!isImport)
                            base.PushMessageToConveyor(new InfoMessage("#Замена удалена"), rootModel);
                    }
                }
                else
                {
                    foreach (var group in rootModel.Groups)
                    {
                        group.Substitutions.RemoveAll(sub => sub.Pattern == args[0]);
                    }

                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Замена удалена"), rootModel);
                }

                return;
            }

            match = _regexHotkey.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[1].Success)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Action List"), rootModel);
                    return;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 2)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);
                    return;
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

                Hotkey hot = new Hotkey();

                ModifierKeysConverter modifierKeysConverter = new ModifierKeysConverter();
                KeyConverter keyConverter = new KeyConverter();

                foreach (string hotkey in hotkeys)
                {
                    if (modifierKeysConverter.IsValid(hotkey))
                        hot.ModifierKeys |= (ModifierKeys)modifierKeysConverter.ConvertFrom(hotkey);
                    else if (keyConverter.IsValid(hotkey))
                        hot.Key |= (Key)keyConverter.ConvertFrom(hotkey);
                    else
                    {
                        if (!isImport)
                            base.PushMessageToConveyor(new ErrorMessage("#Неверный код хоткея"), rootModel);

                        return;
                    }
                }

                hot.Actions.Add(new SendTextAction()
                {
                    CommandText = args[1]
                });

                group.Hotkeys.RemoveAll(h => h.Key == hot.Key && h.ModifierKeys == hot.ModifierKeys);
                group.Hotkeys.Add(hot);

                if (!isImport)
                    base.PushMessageToConveyor(new InfoMessage("#Хоткей добавлен"), rootModel);

                return;
            }

            match = _regexUnHotkey.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[1].Success)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Action List"), rootModel);
                    return;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);
                    return;
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
                        group.Hotkeys.RemoveAll(hot => hot.Key == key && hot.ModifierKeys == modifierKeys);
                        if (!isImport)
                            base.PushMessageToConveyor(new InfoMessage("#Хоткей удален"), rootModel);
                    }
                }
                else
                {
                    foreach (var group in rootModel.Groups)
                    {
                        group.Hotkeys.RemoveAll(hot => hot.Key == key && hot.ModifierKeys == modifierKeys);
                    }

                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Хоткей удален"), rootModel);
                }

                return;
            }

            match = _regexVariable.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[1].Success)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Action List"), rootModel);

                    return;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 2 || (args.Length > 0 && String.IsNullOrEmpty(args[0])))
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return;
                }

                if(!isImport)
                    rootModel.SetVariableValue(args[0], args[1], false);
                else
                    rootModel.SetVariableValue(args[0], args[1], true);

                if (!isImport)
                    base.PushMessageToConveyor(new InfoMessage("#Переменная добавлена"), rootModel);

                return;
            }

            match = _regexUnVariable.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[1].Success)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Action List"), rootModel);

                    return;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return;
                }

                rootModel.ClearVariableValue(args[0], false);

                if (!isImport)
                    base.PushMessageToConveyor(new InfoMessage("#Переменная удалена"), rootModel);

                return;
            }

            match = _regexLog.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if(!match.Groups[1].Success)
                {                   
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return;
                }

                if (rootModel.IsLogging)
                    rootModel.PushMessageToConveyor(new StopLoggingMessage());
                else
                    rootModel.PushMessageToConveyor(new StartLoggingMessage(CommandLineParser.GetArgs(match.Groups[1].ToString())[0]));

                return;
            }

            match = _regexShowme.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[1].Success)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new InfoMessage("#Action List"), rootModel);

                    return;
                }

                if (!isImport)
                    base.PushMessageToConveyor(new InfoMessage(match.Groups[1].ToString().Replace("{", String.Empty).Replace("}", String.Empty)), rootModel);

                return;
            }

            match = _regexConnect.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[1].Success)
                {
                    return;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[1].ToString());

                if (args.Length < 2)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return;
                }

                int port;
                if (!int.TryParse(args[1], out port))
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return;
                }

                if (!isImport)
                    base.PushCommandToConveyor(new ConnectCommand(args[0], port), rootModel);

                return;
            }

            match = _regexZap.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!isImport)
                    base.PushCommandToConveyor(new DisconnectCommand(), rootModel);

                return;
            }

            //TODO
            match = _regexStatus.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                return;
            }

            match = _regexGroup.Match(commandText);
            if (match.Success)
            {
                command.Handled = true;

                if (!match.Groups[2].Success)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return;
                }

                string[] args = CommandLineParser.GetArgs(match.Groups[2].ToString());

                if (args.Length < 1)
                {
                    if (!isImport)
                        base.PushMessageToConveyor(new ErrorMessage("#Синтаксическая ошибка"), rootModel);

                    return;
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

                return;
            }

        }
    }
}
