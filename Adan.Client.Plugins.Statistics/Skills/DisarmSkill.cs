namespace Adan.Client.Plugins.Statistics.Skills
{
    using System;
    using Common.Conveyor;
    using Common.Messages;
    using Common.Themes;

    public sealed class DisarmSkill : SkillBase
    {
        private DisarmSkillStats _sessionStats = new DisarmSkillStats();
        private DisarmSkillStats _fightStats = new DisarmSkillStats();
        private DisarmSkillStats _zoneStats = new DisarmSkillStats();

        public override bool ProcessMessage(string messageText, string nextMessageText, string previousMessageText, int damage)
        {
            if (messageText.StartsWith("Вы ловко выбили ", StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;

                _sessionStats.SuccessAttempts++;
                _fightStats.SuccessAttempts++;
                _zoneStats.SuccessAttempts++;

                return true;
            }

            if (messageText.StartsWith("Вам не удалось обезоружить ", StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;
                return true;
            }

            if (messageText.EndsWith("обезоружить вас!", StringComparison.Ordinal))
            {
                _sessionStats.TankAttempts++;
                _fightStats.TankAttempts++;
                _zoneStats.TankAttempts++;
                return true;
            }

            if (messageText.EndsWith(" из ваших рук!", StringComparison.Ordinal))
            {
                _sessionStats.TankAttempts++;
                _fightStats.TankAttempts++;
                _zoneStats.TankAttempts++;

                _sessionStats.SuccessTankAttempts++;
                _fightStats.SuccessTankAttempts++;
                _zoneStats.SuccessTankAttempts++;
                return true;
            }

            return false;
        }

        public override void PrintDamageStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
            var stats = SelectStatsToPrint(statisticTypeToPrint);
            if (stats.Attempts == 0)
            {
                return;
            }

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock("   обезоруж.:", TextColor.BrightYellow),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double)stats.SuccessAttempts/stats.Attempts).ToString("P"), TextColor.BrightWhite)
            }));
        }

        public override void PrintTankStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
            var stats = SelectStatsToPrint(statisticTypeToPrint);
            if (stats.TankAttempts == 0)
            {
                return;
            }

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock("   обезоруж.:", TextColor.BrightRed),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double)stats.SuccessTankAttempts/stats.TankAttempts).ToString("P"), TextColor.BrightWhite),
            }));
        }

        public override void Reset(StatisticTypes statisticTypeToReset)
        {
            if (statisticTypeToReset == StatisticTypes.Zone)
            {
                _zoneStats = new DisarmSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Fight)
            {
                _fightStats = new DisarmSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Session)
            {
                _sessionStats = new DisarmSkillStats();
            }
        }

        private DisarmSkillStats SelectStatsToPrint(StatisticTypes statisticTypeToPrint)
        {
            if (statisticTypeToPrint == StatisticTypes.Zone)
            {
                return _zoneStats;
            }
            if (statisticTypeToPrint == StatisticTypes.Fight)
            {
                return _fightStats;
            }
            return _sessionStats;
        }


        private sealed class DisarmSkillStats
        {
            public int Attempts;
            public int SuccessAttempts;

            public int TankAttempts;
            public int SuccessTankAttempts;
        }
    }
}
