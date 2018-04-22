namespace Adan.Client.Plugins.Statistics.Skills
{
    using System;
    using Common.Conveyor;
    using Common.Messages;
    using Common.Themes;

    public sealed class ResqueSkill : SkillBase
    {
        private ResqueSkillStats _sessionStats = new ResqueSkillStats();
        private ResqueSkillStats _fightStats = new ResqueSkillStats();
        private ResqueSkillStats _zoneStats = new ResqueSkillStats();

        public override bool ProcessMessage(string messageText, string nextMessageText, string previousMessageText, int damage)
        {
            if (messageText.StartsWith("Банзай! Вам удалось! Вы героически спасли", StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;

                _sessionStats.SuccessAttempts++;
                _fightStats.SuccessAttempts++;
                _zoneStats.SuccessAttempts++;
                return true;
            }

            if (messageText.StartsWith("Вам не удалось этого сделать.", StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;
                return true;
            }

            return false;
        }

        public override void PrintDamageStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
            var stats = SelectStatsToPrint(statisticTypeToPrint);

            if (stats.Attempts== 0)
            {
                return;
            }

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock("   спасти:", TextColor.BrightYellow),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double)stats.SuccessAttempts/stats.Attempts).ToString("P"), TextColor.BrightWhite)
            }));
        }

        public override void PrintTankStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
        }

        public override void Reset(StatisticTypes statisticTypeToReset)
        {
            if (statisticTypeToReset == StatisticTypes.Zone)
            {
                _zoneStats = new ResqueSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Fight)
            {
                _fightStats = new ResqueSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Session)
            {
                _sessionStats = new ResqueSkillStats();
            }
        }

        private ResqueSkillStats SelectStatsToPrint(StatisticTypes statisticTypeToPrint)
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

        private sealed class ResqueSkillStats
        {
            public int Attempts;
            public int SuccessAttempts;
        }
    }
}
