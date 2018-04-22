namespace Adan.Client.Plugins.Statistics.Skills
{
    using System;
    using Common.Conveyor;
    using Common.Messages;
    using Common.Themes;

    public sealed class AccurateShotSkill : SkillBase
    {
        private AccurateShotSkillStats _sessionStats = new AccurateShotSkillStats();
        private AccurateShotSkillStats _fightStats = new AccurateShotSkillStats();
        private AccurateShotSkillStats _zoneStats = new AccurateShotSkillStats();

        public override bool ProcessMessage(string messageText, string nextMessageText, string previousMessageText, int damage)
        {
            if (previousMessageText.StartsWith("Вы точно прицелились в ", StringComparison.Ordinal) &&
                previousMessageText.EndsWith(" и метко выстрелили.", StringComparison.Ordinal))
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


            if (messageText.EndsWith(" точно прицелился в вас и метко выстрелил.", StringComparison.Ordinal) ||
                messageText.EndsWith(" точно прицелилась в вас и метко выстрелила.", StringComparison.Ordinal) ||
                messageText.EndsWith(" точно прицелилось в вас и метко выстрелило.", StringComparison.Ordinal) ||
                messageText.EndsWith(" точно прицелились в вас и метко выстрелили.", StringComparison.Ordinal))
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
            var stat = SelectStatsToPrint(statisticTypeToPrint);
            if (stat.TotalAttempts == 0 || stat.SuccessAttempts == 0)
            {
                return;
            }

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock("   меткий:", TextColor.BrightYellow),
                new TextMessageBlock("\t Средний: ", TextColor.White),
                new TextMessageBlock(((double) stat.TotalDamage/stat.SuccessAttempts).ToString("00.00"), TextColor.BrightWhite),
                new TextMessageBlock("\t Максимальный: ", TextColor.White),
                new TextMessageBlock(stat.MaxDamage.ToString(), TextColor.BrightWhite),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double) stat.SuccessAttempts/stat.TotalAttempts).ToString("P"), TextColor.BrightWhite),
            }));
        }

        public override void PrintTankStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
            var stat = SelectStatsToPrint(statisticTypeToPrint);
            if (stat.TotalTankAttempts == 0)
            {
                return;
            }

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock("   меткий:", TextColor.BrightRed),
                new TextMessageBlock("\t Средний: ", TextColor.White),
                new TextMessageBlock(((double)stat.TotalTankDamage/stat.TotalTankAttempts).ToString("P"), TextColor.BrightWhite),
                new TextMessageBlock("\t Максимальный: ", TextColor.White),
                new TextMessageBlock(stat.MaxTankDamage.ToString(), TextColor.BrightWhite),
            }));
        }

        public override void Reset(StatisticTypes statisticTypeToReset)
        {
            if (statisticTypeToReset == StatisticTypes.Zone)
            {
                _zoneStats = new AccurateShotSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Fight)
            {
                _fightStats = new AccurateShotSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Session)
            {
                _sessionStats = new AccurateShotSkillStats();
            }
        }

        private AccurateShotSkillStats SelectStatsToPrint(StatisticTypes statisticTypeToPrint)
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

        private sealed class AccurateShotSkillStats
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
