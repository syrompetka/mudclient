namespace Adan.Client.Plugins.Statistics.Skills
{
    using System;
    using Common.Conveyor;
    using Common.Messages;
    using Common.Themes;

    public sealed class StabSkill : SkillBase
    {
        private StabSkillStats _sessionStats = new StabSkillStats();
        private StabSkillStats _fightStats = new StabSkillStats();
        private StabSkillStats _zoneStats = new StabSkillStats();

        public override bool ProcessMessage(string messageText, string nextMessageText, string previousMessageText, int damage)
        {
            if (!messageText.EndsWith("!"))
            {
                return false;
            }
            if (messageText.EndsWith("в его спину!", StringComparison.Ordinal) ||
                messageText.EndsWith("в ee спину!", StringComparison.Ordinal) ||
                messageText.EndsWith("в их спину!", StringComparison.Ordinal))
            {
                 _sessionStats.TotalAttempts++;
                _fightStats.TotalAttempts++;
                _zoneStats.TotalAttempts++;

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
                if (nextMessageText.EndsWith("отравлен!", StringComparison.Ordinal) ||
                    nextMessageText.EndsWith("отравлена!", StringComparison.Ordinal) ||
                    nextMessageText.EndsWith("отравлено!", StringComparison.Ordinal))
                {
                    _sessionStats.PoisonAttempts++;
                    _fightStats.PoisonAttempts++;
                    _zoneStats.PoisonAttempts++;
                }

                return true;
            }

            if (messageText.EndsWith(", и вы чуть не отрезали себе палец!", StringComparison.Ordinal))
            {
                _sessionStats.TotalAttempts++;
                _fightStats.TotalAttempts++;
                _zoneStats.TotalAttempts++;

                return true;
            }

            if (messageText.EndsWith(" вас в спину!", StringComparison.Ordinal))
            {

                _sessionStats.TotalTankAttempts++;
                _fightStats.TotalTankAttempts++;
                _zoneStats.TotalTankAttempts++;

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

                return true;
            }

            if (messageText.EndsWith("заколоть вас в спину, но вам удалось избежать его удара!", StringComparison.Ordinal) ||
                messageText.EndsWith("заколоть вас в спину, но вам удалось избежать ee удара!", StringComparison.Ordinal) ||
                messageText.EndsWith("заколоть вас в спину, но вам удалось избежать их удара!", StringComparison.Ordinal))
            {
                _sessionStats.TotalTankAttempts++;
                _fightStats.TotalTankAttempts++;
                _zoneStats.TotalTankAttempts++;

                return true;
            }

            return false;
        }

        public override void PrintDamageStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
            var stats = SelectStatsToPrint(statisticTypeToPrint);
            if (stats.TotalAttempts== 0 || stats.SuccessAttempts == 0)
            {
                return;
            }

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock("   заколоть:", TextColor.BrightYellow),
                new TextMessageBlock("\t Средний: ", TextColor.White),
                new TextMessageBlock(((double) stats.TotalDamage/stats.SuccessAttempts).ToString("00.00"), TextColor.BrightWhite),
                new TextMessageBlock("\t Максимальный: ", TextColor.White),
                new TextMessageBlock(stats.MaxDamage.ToString(), TextColor.BrightWhite),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double) stats.SuccessAttempts/stats.TotalAttempts).ToString("P"), TextColor.BrightWhite),
            }));

            conveyor.PushMessage(new OutputToMainWindowMessage(new []
            {
                new TextMessageBlock("\t\t Шанс отравить: ", TextColor.White),
                new TextMessageBlock(((double)stats.PoisonAttempts/stats.SuccessAttempts).ToString("P"), TextColor.BrightWhite),
            }));
        }

        public override void PrintTankStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
            var stats = SelectStatsToPrint(statisticTypeToPrint);
            if (stats.TotalTankAttempts == 0 || stats.SuccessTankAttempts == 0)
            {
                return;
            }

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock("   заколоть:", TextColor.BrightRed),
                new TextMessageBlock("\t Средний: ", TextColor.White),
                new TextMessageBlock(((double)stats.TotalTankDamage/stats.SuccessTankAttempts).ToString("00.00"), TextColor.BrightWhite),
                new TextMessageBlock("\t Максимальный: ", TextColor.White),
                new TextMessageBlock(stats.MaxTankDamage.ToString(), TextColor.BrightWhite),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double)stats.SuccessTankAttempts/stats.TotalTankAttempts).ToString("P"), TextColor.BrightWhite),
            }));
        }

        public override void Reset(StatisticTypes statisticTypeToReset)
        {
            if (statisticTypeToReset == StatisticTypes.Zone)
            {
                _zoneStats = new StabSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Fight)
            {
                _fightStats = new StabSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Session)
            {
                _sessionStats = new StabSkillStats();
            }
        }

        private StabSkillStats SelectStatsToPrint(StatisticTypes statisticTypeToPrint)
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

        private sealed class StabSkillStats
        {
            public int TotalAttempts;
            public int SuccessAttempts;
            public int PoisonAttempts;
            public int TotalDamage;
            public int MaxDamage;

            public int TotalTankAttempts;
            public int SuccessTankAttempts;
            public int TotalTankDamage;
            public int MaxTankDamage;

        }
    }
}
