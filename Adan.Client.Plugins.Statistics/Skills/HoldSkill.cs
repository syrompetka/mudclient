
namespace Adan.Client.Plugins.Statistics.Skills
{
    using System;
    using Common.Conveyor;
    using Common.Messages;
    using Common.Themes;

    public sealed class HoldSkill : SkillBase
    {
        private readonly string _skillAttack;
        private readonly string _skillName;
        private readonly string _successAtackSuffix;
        private readonly string _successAtacked;
        private readonly string _attackedSuffix;
        private readonly string _attackedSuffix2;

        private HoldSkillStats _sessionStats = new HoldSkillStats();
        private HoldSkillStats _fightStats = new HoldSkillStats();
        private HoldSkillStats _zoneStats = new HoldSkillStats();

        public HoldSkill(string skillName, string skillFullName, string skillLatinName, string successAtackSuffix, string successAtacked)
        {
            _skillAttack = $"Вы произнесли магические слова: '{skillFullName}'.";


            _attackedSuffix = $" магические слова: '{skillLatinName}'.";
            _attackedSuffix2 = $" магические слова: '{skillFullName}'.";
            _skillName = skillName;
            _successAtackSuffix = successAtackSuffix;
            _successAtacked = successAtacked;
        }

        public override bool ProcessMessage(string messageText, string nextMessageText, string previousMessageText, int damage)
        {
            if (messageText.Equals(_skillAttack, StringComparison.Ordinal))
            {
                _sessionStats.Attempts++;
                _fightStats.Attempts++;
                _zoneStats.Attempts++;
                if (nextMessageText.EndsWith(_successAtackSuffix, StringComparison.Ordinal))
                {
                    _sessionStats.SuccessAttempts++;
                    _fightStats.SuccessAttempts++;
                    _zoneStats.SuccessAttempts++;
                }

                return true;
            }

            if (messageText.Contains(" на вас ") &&
                (messageText.EndsWith(_attackedSuffix, StringComparison.Ordinal) ||
                 messageText.EndsWith(_attackedSuffix2, StringComparison.Ordinal)))
            {
                _sessionStats.TankAttempts++;
                _fightStats.TankAttempts++;
                _zoneStats.TankAttempts++;

                if (nextMessageText.Equals(_successAtacked, StringComparison.Ordinal))
                {
                    _sessionStats.SuccessTankAttempts++;
                    _fightStats.SuccessTankAttempts++;
                    _zoneStats.SuccessTankAttempts++;
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
                new TextMessageBlock($"   {_skillName}:", TextColor.BrightYellow),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double)stats.SuccessAttempts/stats.Attempts).ToString("P"), TextColor.BrightWhite)
            }));
        }

        public override void PrintTankStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
            var stat = SelectStatsToPrint(statisticTypeToPrint);
            if (stat.TankAttempts == 0)
            {
                return;
            }

            conveyor.PushMessage(new OutputToMainWindowMessage(new[]
            {
                new TextMessageBlock($"   {_skillName}:", TextColor.BrightRed),
                new TextMessageBlock("\t Шанс: ", TextColor.White),
                new TextMessageBlock(((double)stat.SuccessTankAttempts/stat.TankAttempts).ToString("P"), TextColor.BrightWhite),
            }));
        }

        public override void Reset(StatisticTypes statisticTypeToReset)
        {
            if (statisticTypeToReset == StatisticTypes.Zone)
            {
                _zoneStats = new HoldSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Fight)
            {
                _fightStats = new HoldSkillStats();
            }

            if (statisticTypeToReset == StatisticTypes.Session)
            {
                _sessionStats = new HoldSkillStats();
            }
        }

        private HoldSkillStats SelectStatsToPrint(StatisticTypes statisticTypeToPrint)
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

        private sealed class HoldSkillStats
        {
            public int Attempts;
            public int SuccessAttempts;

            public int TankAttempts;
            public int SuccessTankAttempts;
        }
    }
}
