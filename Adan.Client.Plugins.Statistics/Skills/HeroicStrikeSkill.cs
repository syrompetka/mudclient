namespace Adan.Client.Plugins.Statistics.Skills
{
    using System;
    using Common.Conveyor;
    using Common.Messages;
    using Common.Themes;

    public sealed class HeroicStrikeSkill : SkillBase
    {
        private HeroicStrickeSkillStats _sessionStats = new HeroicStrickeSkillStats();
        private HeroicStrickeSkillStats _fightStats = new HeroicStrickeSkillStats();
        private HeroicStrickeSkillStats _zoneStats = new HeroicStrickeSkillStats();

        public override bool ProcessMessage(string messageText, string nextMessageText, string previousMessageText, int damage)
        {
            if (previousMessageText.Equals("Вы попытались нанести героический удар.", StringComparison.Ordinal))
            {
                _sessionStats.TotalAttempts++;
                _fightStats.TotalAttempts++;
                _zoneStats.TotalAttempts++;

                if (damage > 0)
                {
                    _sessionStats.SuccessAttempts++;
                    _fightStats.SuccessAttempts++;
                    _zoneStats.SuccessAttempts++;

                    _sessionStats.TotalDamage += damage;
                    _fightStats.TotalDamage += damage;
                    _zoneStats.TotalDamage += damage;

                    if (damage > _sessionStats.MaxDamage)
                    {
                        _sessionStats.MaxDamage = damage;
                    }
                    if (damage > _fightStats.MaxDamage)
                    {
                        _fightStats.MaxDamage = damage;
                    }
                    if (damage > _zoneStats.MaxDamage)
                    {
                        _zoneStats.MaxDamage = damage;
                    }
                }

                return true;
            }


            if (messageText.EndsWith(" нанести героический удар.", StringComparison.Ordinal))
            {
                if (damage > 0)
                {
                    _sessionStats.TotalTankAttempts++;
                    _fightStats.TotalTankAttempts++;
                    _zoneStats.TotalTankAttempts++;

                    _sessionStats.TotalTankDamage += damage;
                    _fightStats.TotalTankDamage += damage;
                    _zoneStats.TotalTankDamage += damage;

                    if (damage > _sessionStats.MaxTankDamage)
                    {
                        _sessionStats.MaxTankDamage = damage;
                    }
                    if (damage > _fightStats.MaxTankDamage)
                    {
                        _fightStats.MaxTankDamage = damage;
                    }
                    if (damage > _zoneStats.MaxTankDamage)
                    {
                        _zoneStats.MaxTankDamage = damage;
                    }
                }

                return true;
            }

            return false;
        }

        public override void PrintDamageStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
            var stats = SelectStatsToPrint(statisticTypeToPrint);

            if (stats.TotalAttempts == 0 || stats.SuccessAttempts == 0)
            {
                return;
            }

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock("   героич.:", TextColor.BrightYellow),
                new TextMessageBlock("\t Средний: ", TextColor.White),
                new TextMessageBlock(((double) stats.TotalDamage/stats.SuccessAttempts).ToString("00.00"), TextColor.BrightWhite),
                new TextMessageBlock("\t Максимальный: ", TextColor.White),
                new TextMessageBlock(stats.MaxDamage.ToString(), TextColor.BrightWhite),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double) stats.SuccessAttempts/stats.TotalAttempts).ToString("P"), TextColor.BrightWhite),
            }));
        }

        public override void PrintTankStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
            var stats = SelectStatsToPrint(statisticTypeToPrint);

            if (stats.TotalTankAttempts == 0)
            {
                return;
            }

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock("   героич.:", TextColor.BrightRed),
                new TextMessageBlock("\t Средний: ", TextColor.White),
                new TextMessageBlock(((double)stats.TotalTankDamage/stats.TotalTankAttempts).ToString("P"), TextColor.BrightWhite),
                new TextMessageBlock("\t Максимальный: ", TextColor.White),
                new TextMessageBlock(stats.MaxTankDamage.ToString(), TextColor.BrightWhite),
            }));
        }

        public override void Reset(StatisticTypes statisticTypeToReset)
        {
            if (statisticTypeToReset == StatisticTypes.Zone)
            {
                _zoneStats = new HeroicStrickeSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Fight)
            {
                _fightStats = new HeroicStrickeSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Session)
            {
                _sessionStats = new HeroicStrickeSkillStats();
            }
        }

        private HeroicStrickeSkillStats SelectStatsToPrint(StatisticTypes statisticTypeToPrint)
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

        private sealed class HeroicStrickeSkillStats
        {
            public int TotalAttempts;
            public int SuccessAttempts;
            public int TotalDamage;
            public int MaxDamage;

            public int TotalTankAttempts;
            public int TotalTankDamage;
            public int MaxTankDamage;
        }
    }
}
