namespace Adan.Client.Plugins.Statistics.Skills
{
    using System;
    using Common.Conveyor;
    using Common.Messages;
    using Common.Themes;

    public sealed class DodgeSkill : SkillBase
    {
        private DodgeSkillStats _sessionStats = new DodgeSkillStats();
        private DodgeSkillStats _fightStats = new DodgeSkillStats();
        private DodgeSkillStats _zoneStats = new DodgeSkillStats();

        public override bool ProcessMessage(string messageText, string nextMessageText, string previousMessageText, int damage)
        {
            if (messageText.StartsWith("Вы полностью уклонились от выпада ", StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;

                _sessionStats.SuccessAttempts++;
                _fightStats.SuccessAttempts++;
                _zoneStats.SuccessAttempts++;
                
                return true;
            }

            if (messageText.StartsWith("Вы не смогли полностью уклониться от выпада ", StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;

                _sessionStats.PartialAttempts++;
                _fightStats.PartialAttempts++;
                _zoneStats.PartialAttempts++;
                return true;
            }

            if (messageText.StartsWith("Вы не смогли уклониться от выпада ", StringComparison.Ordinal))
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
                new TextMessageBlock("   уклон:", TextColor.BrightYellow),
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
                _zoneStats = new DodgeSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Fight)
            {
                _fightStats = new DodgeSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Session)
            {
                _sessionStats = new DodgeSkillStats();
            }
        }

        private DodgeSkillStats SelectStatsToPrint(StatisticTypes statisticTypeToPrint)
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

        private sealed class DodgeSkillStats
        {
            public int Attempts;
            public int PartialAttempts;
            public int SuccessAttempts;
        }
    }
}
