namespace Adan.Client.Plugins.Statistics.Skills
{
    using System;
    using Common.Conveyor;
    using Common.Messages;
    using Common.Themes;

    public sealed class TripSkill : SkillBase
    {
        private TripSkillStats _sessionStats = new TripSkillStats();
        private TripSkillStats _fightStats = new TripSkillStats();
        private TripSkillStats  _zoneStats = new TripSkillStats();

        public override bool ProcessMessage(string messageText, string nextMessageText, string previousMessageText, int damage)
        {
            if (messageText.StartsWith("Вы ловко поставили подножку", StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;

                _sessionStats.SuccessAttempts++;
                _fightStats.SuccessAttempts++;
                _zoneStats.SuccessAttempts++;
                return true;
            }

            if (messageText.StartsWith("Вы попытались поставить подножку", StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;
                return true;
            }

            if (messageText.EndsWith("поставить вам подножку, но упал сам.", StringComparison.Ordinal) ||
                messageText.EndsWith("поставить вам подножку, но упала сама.", StringComparison.Ordinal) ||
                messageText.EndsWith("поставить вам подножку, но упало само.", StringComparison.Ordinal) ||
                messageText.EndsWith("поставить вам подножку, но упали сами.", StringComparison.Ordinal))
            {
                _sessionStats.TankAttempts++;
                _fightStats.TankAttempts++;
                _zoneStats.TankAttempts++;
                return true;
            }

            if (messageText.EndsWith("вам подножку, сбив вас с ног.", StringComparison.Ordinal))
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
                new TextMessageBlock("   подножка:", TextColor.BrightYellow),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double)stats.SuccessAttempts/stats.Attempts).ToString("P"), TextColor.BrightWhite)
            }));
        }

        public override void PrintTankStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
            var stats = SelectStatsToPrint(statisticTypeToPrint);
            if (stats.TankAttempts== 0)
            {
                return;
            }

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock("   подножка:", TextColor.BrightRed),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double)stats.SuccessTankAttempts/stats.TankAttempts).ToString("P"), TextColor.BrightWhite),
            }));
        }

        public override void Reset(StatisticTypes statisticTypeToReset)
        {
            if (statisticTypeToReset == StatisticTypes.Zone)
            {
                _zoneStats = new TripSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Fight)
            {
                _fightStats = new TripSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Session)
            {
                _sessionStats = new TripSkillStats();
            }
        }

        private TripSkillStats SelectStatsToPrint(StatisticTypes statisticTypeToPrint)
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

        private sealed class TripSkillStats
        {
            public int Attempts;
            public int SuccessAttempts;
            public int TankAttempts;
            public int SuccessTankAttempts;
        }
    }
}
