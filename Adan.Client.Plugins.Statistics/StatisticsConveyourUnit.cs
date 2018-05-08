namespace Adan.Client.Plugins.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;
    using Common.Themes;
    using GroupWidget;
    using GroupWidget.Messages;
    using Map.Messages;

    public sealed class StatisticsConveyourUnit : ConveyorUnit
    {
        private bool _isFighting;
        private int _lastZoneId;
        private int _currentZoneId;
        private readonly Stack<TextMessage> _messageStack = new Stack<TextMessage>();
        private readonly StatisticsHolder _statisticsHolder = new StatisticsHolder();
        private readonly Queue<TextMessage> _previousTextMessageWindowQueue = new Queue<TextMessage>(15);

        private bool _isDisabled;

        public StatisticsConveyourUnit(MessageConveyor conveyor) : base(conveyor)
        {
        }

        public override IEnumerable<int> HandledMessageTypes => new[] { BuiltInMessageTypes.TextMessage, Constants.GroupStatusMessageType, Constants.RoomMonstersMessage, Map.Constants.CurrentRoomMessageType, BuiltInMessageTypes.ConnectionMessages };
        public override IEnumerable<int> HandledCommandTypes => new[] { BuiltInCommandTypes.TextCommand };

        public override void HandleMessage(Message message)
        {
            var connMessage = message as ConnectedMessage;
            if (connMessage != null)
            {
                if (Conveyor.RootModel.GetVariableValue("StatisticsPluginEnabled") != "true")
                {
                    Conveyor.PushMessage(new OutputToMainWindowMessage("Модуль сбора статистики отключен.", TextColor.BrightWhite));
                    _isDisabled = true;
                }
                else
                {
                    _isDisabled = false;
                    Conveyor.PushMessage(new OutputToMainWindowMessage("Модуль сбора статистики включен.", TextColor.BrightWhite));
                }

                Conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                {
                    new TextMessageBlock("Наберите ", TextColor.BrightWhite),
                    new TextMessageBlock("\"стат справка\"", TextColor.BrightYellow),
                    new TextMessageBlock(" для подробностей.", TextColor.BrightWhite),
                }));

                return;
            }

            if (_isDisabled)
            {
                return;
            }

            bool checkForRound = false;
            if (message.MessageType == Map.Constants.CurrentRoomMessageType)
            {
                var currentRoomMessage = message as CurrentRoomMessage;
                if (currentRoomMessage != null)
                {
                    _currentZoneId = currentRoomMessage.ZoneId;
                }
            }

            if (message.MessageType == Constants.GroupStatusMessageType)
            {
                var groupMessage = message as GroupStatusMessage;

                if (groupMessage != null)
                {
                    var newIsFighting = groupMessage.GroupMates.Any(g => g.IsAttacked);
                    if (newIsFighting && !_isFighting)
                    {
                        _statisticsHolder.ResetStats(StatisticTypes.Fight);
                        while (_previousTextMessageWindowQueue.Count > 0)
                        {
                            _messageStack.Push(_previousTextMessageWindowQueue.Dequeue());
                        }
                    }

                    _isFighting = newIsFighting;
                }

                //processor.UpdateIsFighting(_isFighting);
            }

            if (message.MessageType == Constants.RoomMonstersMessage)
            {
                var monstersMessage = message as RoomMonstersMessage;
                if (monstersMessage != null)
                {
                    checkForRound = monstersMessage.IsRound;
                }
            }

            if (message.MessageType == BuiltInMessageTypes.TextMessage)
            {
                var textMessage = message as TextMessage;
                if (textMessage != null)
                {
                    if (_isFighting)
                    {
                        _messageStack.Push(textMessage.Clone());
                    }

                    _previousTextMessageWindowQueue.Enqueue(textMessage.Clone());
                    if (_previousTextMessageWindowQueue.Count >= 15)
                    {
                        _previousTextMessageWindowQueue.Dequeue();
                    }

                }
            }

            if (checkForRound || !_isFighting)
            {
                TextMessage nextMessage = null;
                bool messagesProcessed = false;
                while (_messageStack.Count > 0)
                {
                    if (_currentZoneId != _lastZoneId)
                    {
                        _statisticsHolder.ResetStats(StatisticTypes.Zone);
                        _lastZoneId = _currentZoneId;
                    }

                    var currentMessage = _messageStack.Pop();

                    _statisticsHolder.ProcessMessage(currentMessage, nextMessage, _messageStack.Count > 0 ? _messageStack.Peek() : null);
                    nextMessage = currentMessage;
                    messagesProcessed = true;
                }

                if (messagesProcessed)
                {
                    _statisticsHolder.RoundCompleted();
                }
            }
        }

        public override void HandleCommand(Command command, bool isImport = false)
        {
            if (isImport)
            {
                return;
            }

            var textCommand = command as TextCommand;
            if (textCommand == null)
            {
                return;
            }

            if (textCommand.CommandText.TrimStart().StartsWith("стат"))
            {
                command.Handled = true;
                var commandArgs = textCommand.CommandText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (commandArgs.Length == 2 && string.Equals(commandArgs[1], "вкл", StringComparison.OrdinalIgnoreCase))
                {
                    Conveyor.PushMessage(new OutputToMainWindowMessage("Модуль сбора статистики включен.", TextColor.BrightWhite));
                    Conveyor.RootModel.SetVariableValue("StatisticsPluginEnabled", "true", true);
                    _isDisabled = false;
                }
                else if (commandArgs.Length == 2 && string.Equals(commandArgs[1], "откл", StringComparison.OrdinalIgnoreCase))
                {
                    Conveyor.PushMessage(new OutputToMainWindowMessage("Модуль сбора статистики отключен.", TextColor.BrightWhite));
                    Conveyor.RootModel.SetVariableValue("StatisticsPluginEnabled", "false", true);
                    _isDisabled = true;
                }
                else if (commandArgs.Length == 2 &&
                    (string.Equals(commandArgs[1], "справка", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(commandArgs[1], "?", StringComparison.OrdinalIgnoreCase)))
                {
                    Conveyor.PushMessage(new OutputToMainWindowMessage("Модуль статистики. Возможные команды:", TextColor.BrightWhite));

                    Conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                    {
                        new TextMessageBlock("\t\"стат вкл|откл\"", TextColor.BrightYellow),
                        new TextMessageBlock(" - включение/отключение модуля статистики.", TextColor.BrightWhite),
                    }));

                    Conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                    {
                        new TextMessageBlock("\t\"стат сброс зона|сессия|все\"", TextColor.BrightYellow),
                        new TextMessageBlock(" - сброс статистики последней зоны/текущей сессии.", TextColor.BrightWhite),
                    }));

                    Conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                    {
                        new TextMessageBlock("\t\"стат зона|бой|сесия\"", TextColor.BrightYellow),
                        new TextMessageBlock(" - вывод статистики последней зоны/боя/текущей сессии.", TextColor.BrightWhite),
                    }));

                    Conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                    {
                        new TextMessageBlock("\t\"стат\"", TextColor.BrightYellow),
                        new TextMessageBlock(" - вывод статистики текущей сессии.", TextColor.BrightWhite),
                    }));

                    Conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                    {
                        new TextMessageBlock("\t\"стат справка|?\"", TextColor.BrightYellow),
                        new TextMessageBlock(" - вывод данной справки.", TextColor.BrightWhite),
                    }));


                    Conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                    {
                        new TextMessageBlock("Внимание!!!", TextColor.BrightYellow),
                        new TextMessageBlock(" Для корректной работы модуля статистики необходимо включить ", TextColor.BrightWhite),
                        new TextMessageBlock("\"реж детальный\"", TextColor.BrightYellow),
                        new TextMessageBlock(" а также ", TextColor.BrightWhite),
                        new TextMessageBlock("\"реж цвет полный\".", TextColor.BrightYellow),

                    }));
                }
                else if(_isDisabled)
                {
                    Conveyor.PushMessage(new OutputToMainWindowMessage("Модуль сбора статистики отключен.", TextColor.BrightWhite));
                    Conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                    {
                        new TextMessageBlock("Наберите ", TextColor.BrightWhite),
                        new TextMessageBlock("\"стат справка\"", TextColor.BrightYellow),
                        new TextMessageBlock(" для подробностей.", TextColor.BrightWhite),
                    }));

                }
                else if (commandArgs.Length == 2 && string.Equals(commandArgs[1], "сброс", StringComparison.OrdinalIgnoreCase))
                {
                    Conveyor.PushMessage(new OutputToMainWindowMessage("Что именно вы хотите сбросить?", TextColor.BrightWhite));
                    Conveyor.PushMessage(new OutputToMainWindowMessage("стат сброс зона|сессия|все", TextColor.BrightWhite));
                }
                else if (commandArgs.Length == 3 &&
                    string.Equals(commandArgs[1], "сброс", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(commandArgs[2], "зона", StringComparison.OrdinalIgnoreCase))
                {
                    _statisticsHolder.ResetStats(StatisticTypes.Zone);
                    Conveyor.PushMessage(new OutputToMainWindowMessage("Статистика последней зоны сброшена.", TextColor.BrightWhite));
                }
                else if (commandArgs.Length == 3 &&
                   string.Equals(commandArgs[1], "сброс", StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(commandArgs[2], "сессия", StringComparison.OrdinalIgnoreCase))
                {
                    _statisticsHolder.ResetStats(StatisticTypes.Session);
                    Conveyor.PushMessage(new OutputToMainWindowMessage("Статистика текущей сессии сброшена.", TextColor.BrightWhite));
                }
                else if (commandArgs.Length == 3 &&
                         string.Equals(commandArgs[1], "сброс", StringComparison.OrdinalIgnoreCase) &&
                         string.Equals(commandArgs[2], "все", StringComparison.OrdinalIgnoreCase))
                {
                    _statisticsHolder.ResetStats(StatisticTypes.Zone);
                    _statisticsHolder.ResetStats(StatisticTypes.Session);
                    Conveyor.PushMessage(new OutputToMainWindowMessage("Статистика последней зоны сброшена.", TextColor.BrightWhite));
                    Conveyor.PushMessage(new OutputToMainWindowMessage("Статистика текущей сессии сброшена.", TextColor.BrightWhite));
                }
                else if (commandArgs.Length == 2 && string.Equals(commandArgs[1], "зона", StringComparison.OrdinalIgnoreCase))
                {
                    _statisticsHolder.PrintStatistics(Conveyor, StatisticTypes.Zone);
                }
                else if (commandArgs.Length == 2 && string.Equals(commandArgs[1], "бой", StringComparison.OrdinalIgnoreCase))
                {
                    _statisticsHolder.PrintStatistics(Conveyor, StatisticTypes.Fight);
                }
                else if (commandArgs.Length == 2 && string.Equals(commandArgs[1], "сессия", StringComparison.OrdinalIgnoreCase))
                {
                    _statisticsHolder.PrintStatistics(Conveyor, StatisticTypes.Session);
                }
                else if (commandArgs.Length == 1)
                {
                    _statisticsHolder.PrintStatistics(Conveyor, StatisticTypes.Session);
                }
                else
                {
                    Conveyor.PushMessage(new OutputToMainWindowMessage("Непонятно.", TextColor.BrightRed));

                    Conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                    {
                        new TextMessageBlock("Наберите ", TextColor.BrightWhite),
                        new TextMessageBlock("\"стат справка\"", TextColor.BrightYellow),
                        new TextMessageBlock(" для подробностей.", TextColor.BrightWhite),
                    }));
                }
            }
        }
    }
}
