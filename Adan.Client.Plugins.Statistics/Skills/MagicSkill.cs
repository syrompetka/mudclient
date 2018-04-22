
namespace Adan.Client.Plugins.Statistics.Skills
{
    using System;
    using System.Runtime.CompilerServices;
    using Common.Conveyor;
    using Common.Messages;
    using Common.Themes;

    public sealed class MagicSkill : SkillBase
    {
        private readonly string _skillName;
        private readonly string _attackPrefix;
        private readonly string _attackSuffix;
        private readonly string _attackedPrefix;
        private readonly string _attackedSuffix;

        private MagicSkillStats _sessionStats = new MagicSkillStats();
        private MagicSkillStats _fightStats = new MagicSkillStats();
        private MagicSkillStats _zoneStats = new MagicSkillStats();

        public MagicSkill(string skillName, string attackPrefix, string attackSuffix, string attackedPrefix, string attackedSuffix)
        {
            _skillName = skillName;
            _attackPrefix = attackPrefix;
            _attackSuffix = attackSuffix;
            _attackedPrefix = attackedPrefix;
            _attackedSuffix = attackedSuffix;
        }

        public override bool ProcessMessage(string messageText, string nextMessageText, string previousMessageText, int damage)
        {
            if (damage == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(_attackedPrefix) || messageText.StartsWith(_attackedPrefix, StringComparison.Ordinal))
            {
                if (string.IsNullOrEmpty(_attackedSuffix) || messageText.EndsWith(_attackedSuffix, StringComparison.Ordinal))
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

                    return true;
                }
            }


            if (string.IsNullOrEmpty(_attackPrefix) || messageText.StartsWith(_attackPrefix, StringComparison.Ordinal))
            {
                if (string.IsNullOrEmpty(_attackSuffix) || messageText.EndsWith(_attackSuffix, StringComparison.Ordinal))
                {
                    _sessionStats.TotalAttempts++;
                    _fightStats.TotalAttempts++;
                    _zoneStats.TotalAttempts++;

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

                    return true;
                }
            }

            return false;
        }

        public override void PrintDamageStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
            var stat = SelectStatsToPrint(statisticTypeToPrint);
            if (stat.TotalAttempts == 0)
            {
                return;
            }

            //for nice printing
            if (_skillName.Length < 4)
            {
                conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                {
                    new TextMessageBlock($"   {_skillName}:   ", TextColor.BrightYellow),
                    new TextMessageBlock("\t Средний: ", TextColor.White),
                    new TextMessageBlock(((double) stat.TotalDamage/stat.TotalAttempts).ToString("00.00"), TextColor.BrightWhite),
                    new TextMessageBlock("\t Максимальный: ", TextColor.White),
                    new TextMessageBlock(stat.MaxDamage.ToString(), TextColor.BrightWhite),
                }));
            }
            else
            {
                conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                {
                    new TextMessageBlock($"   {_skillName}:", TextColor.BrightYellow),
                    new TextMessageBlock("\t Средний: ", TextColor.White),
                    new TextMessageBlock(((double) stat.TotalDamage/stat.TotalAttempts).ToString("00.00"), TextColor.BrightWhite),
                    new TextMessageBlock("\t Максимальный: ", TextColor.White),
                    new TextMessageBlock(stat.MaxDamage.ToString(), TextColor.BrightWhite),
                }));
            }

        }

        public override void PrintTankStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
            var stat = SelectStatsToPrint(statisticTypeToPrint);
            if (stat.TotalTankAttempts == 0)
            {
                return;
            }

            //for nice printing
            if (_skillName.Length < 4)
            {
                conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                {
                    new TextMessageBlock($"   {_skillName}:   ", TextColor.BrightRed),
                    new TextMessageBlock("\t Средний: ", TextColor.White),
                    new TextMessageBlock(((double) stat.TotalTankDamage/stat.TotalTankAttempts).ToString("00.00"), TextColor.BrightWhite),
                    new TextMessageBlock("\t Максимальный: ", TextColor.White),
                    new TextMessageBlock(stat.MaxTankDamage.ToString(), TextColor.BrightWhite),
                }));

            }
            else
            {
                conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                {
                    new TextMessageBlock($"   {_skillName}:", TextColor.BrightRed),
                    new TextMessageBlock("\t Средний: ", TextColor.White),
                    new TextMessageBlock(((double) stat.TotalTankDamage/stat.TotalTankAttempts).ToString("00.00"), TextColor.BrightWhite),
                    new TextMessageBlock("\t Максимальный: ", TextColor.White),
                    new TextMessageBlock(stat.MaxTankDamage.ToString(), TextColor.BrightWhite),
                }));
            }
        }

        public override void Reset(StatisticTypes statisticTypeToReset)
        {
            if (statisticTypeToReset == StatisticTypes.Zone)
            {
                _zoneStats = new MagicSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Fight)
            {
                _fightStats = new MagicSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Session)
            {
                _sessionStats = new MagicSkillStats();
            }
        }

        private MagicSkillStats SelectStatsToPrint(StatisticTypes statisticTypeToPrint)
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

        private sealed class MagicSkillStats
        {
            public int TotalAttempts;
            public int TotalDamage;
            public int MaxDamage;

            public int TotalTankAttempts;
            public int TotalTankDamage;
            public int MaxTankDamage;
        }

    }
}
