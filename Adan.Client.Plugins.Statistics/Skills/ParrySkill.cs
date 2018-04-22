namespace Adan.Client.Plugins.Statistics.Skills
{
    using System;
    using Common.Conveyor;
    using Common.Messages;
    using Common.Themes;

    public sealed class ParrySkill : SkillBase
    {
        private ParrySkillStats _sessionStats = new ParrySkillStats();
        private ParrySkillStats _fightStats = new ParrySkillStats();
        private ParrySkillStats  _zoneStats = new ParrySkillStats();

        public override bool ProcessMessage(string messageText, string nextMessageText, string previousMessageText, int damage)
        {
            if (messageText.StartsWith("Вы полностью парировали выпад", StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;

                _sessionStats.SuccessAttempts++;
                _fightStats.SuccessAttempts++;
                _zoneStats.SuccessAttempts++;
                return true;
            }

            if (messageText.StartsWith("Вы не смогли полностью парировать выпад", StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;

                _sessionStats.PartialAttempts++;
                _fightStats.PartialAttempts++;
                _zoneStats.PartialAttempts++;

                return true;
            }

            if (messageText.StartsWith("Вы не смогли парировать выпад", StringComparison.Ordinal))
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
            if (stats.Attempts == 0)
            {
                return;
            }

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock("   пари:", TextColor.BrightYellow),
                new TextMessageBlock("\t Шанс полн.: ", TextColor.White),
                new TextMessageBlock(((double)stats.SuccessAttempts/stats.Attempts).ToString("P"), TextColor.BrightWhite),
                new TextMessageBlock("\t Шанс частич.: ", TextColor.White),
                new TextMessageBlock(((double)stats.PartialAttempts/stats.Attempts).ToString("P"), TextColor.BrightWhite)
            }));
        }

        public override void PrintTankStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
        }

        public override void Reset(StatisticTypes statisticTypeToReset)
        {
            if (statisticTypeToReset == StatisticTypes.Zone)
            {
                _zoneStats = new ParrySkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Fight)
            {
                _fightStats = new ParrySkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Session)
            {
                _sessionStats = new ParrySkillStats();
            }
        }

        private ParrySkillStats SelectStatsToPrint(StatisticTypes statisticTypeToPrint)
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

        private sealed class ParrySkillStats
        {
            public int Attempts;
            public int PartialAttempts;
            public int SuccessAttempts;
        }
    }
}
