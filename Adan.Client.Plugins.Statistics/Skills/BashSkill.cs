namespace Adan.Client.Plugins.Statistics.Skills
{
    using System;
    using Common.Conveyor;
    using Common.Messages;
    using Common.Themes;

    public sealed class BashSkill : SkillBase
    {
        private BashSkillStats _sessionStats = new BashSkillStats();
        private BashSkillStats _fightStats = new BashSkillStats();
        private BashSkillStats _zoneStats = new BashSkillStats();

        public override bool ProcessMessage(string messageText, string nextMessageText, string previousMessageText, int damage)
        {
            if (messageText.EndsWith("на землю своим сокрушающим ударом!", StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;

                _sessionStats.SuccessAttempts++;
                _fightStats.SuccessAttempts++;
                _zoneStats.SuccessAttempts++;

                return true;
            }

            if (messageText.EndsWith("от вашего удара, вы не удержали равновесия и упали!", StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;
                return true;
            }

            if (messageText.StartsWith("Вы полетели на землю от мощного удара", StringComparison.Ordinal))
            {
                _sessionStats.TankAttempts++;
                _fightStats.TankAttempts++;
                _zoneStats.TankAttempts++;

                _sessionStats.SuccessTankAttempts++;
                _fightStats.SuccessTankAttempts++;
                _zoneStats.SuccessTankAttempts++;
                return true;
            }

            if (messageText.EndsWith(", попытавшись завалить вас!", StringComparison.Ordinal))
            {
                _sessionStats.TankAttempts++;
                _fightStats.TankAttempts++;
                _zoneStats.TankAttempts++;
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
                new TextMessageBlock("   сбить:", TextColor.BrightYellow),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double)stats.SuccessAttempts/stats.Attempts).ToString("P"), TextColor.BrightWhite)
            }));
        }

        public override void PrintTankStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
            var stat = SelectStatsToPrint(statisticTypeToPrint);
            if (stat.TankAttempts== 0)
            {
                return;
            }

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock("   сбить:", TextColor.BrightRed),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double)stat.SuccessTankAttempts/stat.TankAttempts).ToString("P"), TextColor.BrightWhite),
            }));
        }

        public override void Reset(StatisticTypes statisticTypeToReset)
        {
            if (statisticTypeToReset == StatisticTypes.Zone)
            {
                _zoneStats = new BashSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Fight)
            {
                _fightStats = new BashSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Session)
            {
                _sessionStats = new BashSkillStats();
            }
        }

        private BashSkillStats SelectStatsToPrint(StatisticTypes statisticTypeToPrint)
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

        private sealed class BashSkillStats
        {
            public int Attempts;
            public int SuccessAttempts;

            public int TankAttempts;
            public int SuccessTankAttempts;
        }
    }
}
