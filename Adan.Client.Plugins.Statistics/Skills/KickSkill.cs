namespace Adan.Client.Plugins.Statistics.Skills
{
    using System;
    using Common.Conveyor;
    using Common.Messages;
    using Common.Themes;

    public sealed class KickSkill : SkillBase
    {
        private KickSkillStats _sessionStats = new KickSkillStats();
        private KickSkillStats _fightStats = new KickSkillStats();
        private KickSkillStats _zoneStats = new KickSkillStats();

        public override bool ProcessMessage(string messageText, string nextMessageText, string previousMessageText, int damage)
        {
            if (messageText.StartsWith("Вы ударили", StringComparison.Ordinal) &&
                messageText.EndsWith("кости...", StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;

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

                if (nextMessageText.StartsWith("Своим мощным ударом, вы завалили "))
                {
                    _sessionStats.BashAttempts++;
                    _fightStats.BashAttempts++;
                    _zoneStats.BashAttempts++;
                }

                return true;
            }

            if (messageText.StartsWith("Вы попытались ударить ", StringComparison.Ordinal) &&
                messageText.EndsWith("ногой, но промахнулись.", StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;


                if (nextMessageText.Equals("Вы не удержали равновесия и упали.", StringComparison.Ordinal))
                {
                    _sessionStats.FallAttempts++;
                    _fightStats.FallAttempts++;
                    _zoneStats.FallAttempts++;
                }

                return true;
            }

            if (messageText.StartsWith("вас ногой, ломая кости!", StringComparison.Ordinal))
            {
                _sessionStats.TankAttempts++;
                _fightStats.TankAttempts++;
                _zoneStats.TankAttempts++;

                _sessionStats.SuccessTankAttempts++;
                _fightStats.SuccessTankAttempts++;
                _zoneStats.SuccessTankAttempts++;

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

                if (nextMessageText.StartsWith("Своим мощным ударом,", StringComparison.Ordinal))
                {
                    _sessionStats.BashTankAttempts++;
                    _fightStats.BashTankAttempts++;
                    _zoneStats.BashTankAttempts++;
                }
                return true;
            }

            if (messageText.EndsWith("ударить вас ногой, но промахнулся.", StringComparison.Ordinal) ||
                messageText.EndsWith("ударить вас ногой, но промахнулась.", StringComparison.Ordinal) ||
                messageText.EndsWith("ударить вас ногой, но промахнулось.", StringComparison.Ordinal) ||
                messageText.EndsWith("ударить вас ногой, но промахнулись.", StringComparison.Ordinal))
            {
                _sessionStats.TankAttempts++;
                _fightStats.TankAttempts++;
                _zoneStats.TankAttempts++;

                if (nextMessageText.EndsWith("не удержал равновесия и упал.", StringComparison.Ordinal) ||
                    nextMessageText.EndsWith("не удержала равновесия и упала.", StringComparison.Ordinal) ||
                    nextMessageText.EndsWith("не удержало равновесия и упало.", StringComparison.Ordinal) ||
                    nextMessageText.EndsWith("не удержали равновесия и упали.", StringComparison.Ordinal))
                {
                    _sessionStats.FallTankAttempts++;
                    _fightStats.FallTankAttempts++;
                    _zoneStats.FallTankAttempts++;
                }

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
                new TextMessageBlock("   пнуть:", TextColor.BrightYellow),
                new TextMessageBlock("\t Средний: ", TextColor.White),
                new TextMessageBlock(((double)stats.TotalDamage/(stats.SuccessAttempts>0?stats.SuccessAttempts:1)).ToString("00.00"), TextColor.BrightWhite),
                new TextMessageBlock("\t Максимальный: ", TextColor.White),
                new TextMessageBlock(stats.MaxDamage.ToString(), TextColor.BrightWhite),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double)stats.SuccessAttempts/stats.Attempts).ToString("P"), TextColor.BrightWhite),
            }));

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock("\t\t Шанс сбить: ", TextColor.White),
                new TextMessageBlock(((double)stats.BashAttempts/(stats.SuccessAttempts>0?stats.SuccessAttempts:1)).ToString("P"), TextColor.BrightWhite),
                new TextMessageBlock("\t Шанс упасть: ", TextColor.White),
                new TextMessageBlock(((double)stats.FallAttempts/(stats.Attempts-stats.SuccessAttempts>0?stats.Attempts-stats.SuccessAttempts:1)).ToString("P"), TextColor.BrightWhite),

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
                new TextMessageBlock("   пнуть:", TextColor.BrightRed),
                new TextMessageBlock("\t Средний: ", TextColor.White),
                new TextMessageBlock(((double)stats.TotalTankDamage/(stats.SuccessTankAttempts>0?stats.SuccessTankAttempts:1)).ToString("P"), TextColor.BrightWhite),
                new TextMessageBlock("\t Максимальный: ", TextColor.White),
                new TextMessageBlock(stats.MaxTankDamage.ToString(), TextColor.BrightWhite),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double)stats.SuccessTankAttempts/stats.TankAttempts).ToString("P"), TextColor.BrightWhite),
            }));

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock("\t\t Шанс сбить: ", TextColor.White),
                new TextMessageBlock(((double) stats.BashTankAttempts/(stats.SuccessTankAttempts> 0 ? stats.SuccessTankAttempts: 1)).ToString("P"), TextColor.BrightWhite),
                new TextMessageBlock("\t Шанс упасть: ", TextColor.White),
                new TextMessageBlock(((double) stats.FallTankAttempts/(stats.TankAttempts- stats.SuccessTankAttempts> 0 ? stats.TankAttempts- stats.SuccessTankAttempts: 1)).ToString("P"), TextColor.BrightWhite),

            }));

        }

        public override void Reset(StatisticTypes statisticTypeToReset)
        {
            if (statisticTypeToReset == StatisticTypes.Zone)
            {
                _zoneStats = new KickSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Fight)
            {
                _fightStats = new KickSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Session)
            {
                _sessionStats = new KickSkillStats();
            }
        }

        private KickSkillStats SelectStatsToPrint(StatisticTypes statisticTypeToPrint)
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

        private sealed class KickSkillStats
        {
            public int Attempts;
            public int FallAttempts;
            public int BashAttempts;
            public int SuccessAttempts;
            public int TotalDamage;
            public int MaxDamage;

            public int TankAttempts;
            public int SuccessTankAttempts;
            public int TotalTankDamage;
            public int MaxTankDamage;
            public int FallTankAttempts;
            public int BashTankAttempts;
        }
    }
}
