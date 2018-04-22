namespace Adan.Client.Plugins.Statistics.Skills
{
    using Common.Conveyor;

    public abstract class SkillBase
    {
        public abstract bool ProcessMessage(string  messageText, string nextMessageText, string previousMessageText, int damage);
        public abstract void PrintDamageStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint);
        public abstract void PrintTankStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint);

        public abstract void Reset(StatisticTypes statisticTypeToReset);
    }
}
