namespace Adan.Client.Plugins.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Common.Conveyor;
    using Common.Messages;
    using Common.Model;
    using Common.Themes;
    using Common.Utils.PatternMatching;
    using Skills;

    public sealed class StatisticsHolder
    {
        private static readonly RootModel EmptyRootModel = new RootModel();

        private static readonly PatternToken SlavaToken = WildcardParser.ParseWildcardString("^Вы получили %1 единиц славы.", EmptyRootModel);
        private static readonly PatternToken SlavaToken1 = WildcardParser.ParseWildcardString("^Вы получили свою часть славы - %1.", EmptyRootModel);

        private static readonly PatternToken ExpToken = WildcardParser.ParseWildcardString("^Вы получили %1 единиц опыта.", EmptyRootModel);
        private static readonly PatternToken ExpToken1 = WildcardParser.ParseWildcardString("^Вы получили свою часть опыта -- %1.", EmptyRootModel);

        private readonly IList<string> _matchingResults = new List<string>(Enumerable.Repeat(string.Empty, 10));

        private readonly IList<SkillBase> _damageSkills = new List<SkillBase>
        {
            new StabSkill(),
            new TripSkill(),
            new BashSkill(),
            new KickSkill(),
            new ResqueSkill(),
            new DisarmSkill(),
            new ParrySkill(),
            new DodgeSkill(),
            new HeroicStrikeSkill(),
            new AccurateShotSkill(),
            new MagicSkill("брызги", "Ваши разноцветные брызги ударили ", " в лицо -- Ух!!", String.Empty, "вас разноцветными брызгами, и от этого вам стало как-то не по себе!"),
            new MagicSkill("землетряс.", "Когда земля ушла из под ног ", "!", "Когда земля ушла у вас из под ног, вы упали на землю и больно ударились!", String.Empty),
            new MagicSkill("камнепад", "Камни, посыпавшиеся сверху на ", " скорчиться от боли.", string.Empty, "Камни, посыпавшиеся сверху на вас, заставили скорчиться от боли."),
            new MagicSkill("вред", "Вы с мрачным удовлетворением заметили, как ", String.Empty, "Вы скривились в ужасной гримасе боли, когда ", " на вас свой вред."),
            new MagicSkill("сфера", "Ваша сфера холода окутала ", String.Empty, "Сфера холода ", " окутала вас!"),
            new MagicSkill("кислота", "Ваша кислота покрыла ", String.Empty, "Кислота ", " покрыла вас!"),
            new MagicSkill("молния", "Посланная вами молния ударила ", " содрогнуться!", "Молния, посланная ", ", ударила вас в полную силу, заставив содрогнуться!"),
            new MagicSkill("л. прикос.", "Вы заморозили ", "!", "Вы чувствуете, как ваша жизнь съедается холодом, исходящим от ", "!"),
            new MagicSkill("огн. шар", "Ваш огненный шар попал в ", " окружило пламя!", "Вас окружило пламя от огненного шара, пущенного ", "!"),
            new MagicSkill("маг. имп.", "Вы с гордостью увидели, как ваш магический импульс поразил ", "!", "Вы содрогнулись под ударом магического импульса ", "!"),
            new MagicSkill("шок. хват.", "Вы схватили ", " несколько костей!", String.Empty, " вас, ломая pебpа."),
            new MagicSkill("легкий вред", "Ваш вред задел ", "!", "Вред ", " задел вас!"),
            new MagicSkill("горящие руки ", "Вы схватили ", "!", "Вы забились от боли, когда ", " вас горящими руками!"), // текст спелла такой, что поделать... но конфликта с шок хваткой нет, т.к. шок хватка первая

            new MagicSkill("яд", string.Empty, " в судорогах.", "Вы чувствуете жгучий яд в крови и страдаете.", String.Empty),
            new MagicSkill("боль", string.Empty, " от боли.", "Вы содрогнулись от боли.", String.Empty),
            new MagicSkill("коварн.", string.Empty, " немного крови.", "Ваши кровоточащие раны причинили вам боль и страдание.", String.Empty),
            new MagicSkill("огн. щит", "Ваш огненный щит опалил ", "!", "Огненный щит ", " опалил вас!"),

            new HoldSkill("п. персону","придержать персону", "персонам репрехендере", "на месте, не в силах шевельнуться.","Вы замерли на месте, не в силах шевельнуться..."),
            new HoldSkill("п. любого","придержать любого", "рем натам репрехендере", "на месте, не в силах шевельнуться.","Вы замерли на месте, не в силах шевельнуться..."),
            new HoldSkill("паралич", "паралич","паралисис", " сковал паралич.","Вас сковал паралич."),
            new HoldSkill("молчание","молчание", "такитурнитас", "способность разговаривать.","Вам показалось что ваши голосовые связки пропали, и вы немы, как рыба..."),
            new HoldSkill("слепота","слепота", "каекитас", "!","Вы ослепли!"),
            new HoldSkill("к. прокл.","каменное проклятие", "ляпиде нокса", " превращаться в каменную статую.","Вы почувствовали, что начали превращаться в камень."),
            new HoldSkill("страх", "страх","метус", " леденящий душу страх.","Вы почувствовали леденящий душу страх. Больше нет сил терпеть! Бежать! Бежать! Бежааааать!"),
            new HoldSkill("зат. раз.", "затуманивание разума","ментис нубес", " затуманился и поглупел","Вы почувствовали себя намного глупее. Ой."),

        };

        private DamageStats _sessionStats = new DamageStats();
        private DamageStats _fightStats = new DamageStats();
        private DamageStats _zoneStats = new DamageStats();

        public void ProcessMessage(TextMessage currentMessage, TextMessage nextMessage, TextMessage previousMessage)
        {
            int damage = 0;
            bool isCrit = false;
            var textBlock = currentMessage.MessageBlocks.FirstOrDefault();
            if (textBlock == null)
            {
                return;
            }

            var textWithoutDamage = textBlock.Text;
            var nextMessageText = nextMessage?.InnerText ?? string.Empty;
            var previousMessageText = previousMessage?.InnerText ?? string.Empty;
            if (textBlock.Foreground == TextColor.BrightYellow || textBlock.Foreground == TextColor.BrightRed)
            {
                if (textBlock.Text.EndsWith(")"))
                {
                    var openBraketIndex = currentMessage.InnerText.LastIndexOf("(", StringComparison.Ordinal);
                    if (openBraketIndex > 0)
                    {
                        var damageString = currentMessage.InnerText.Substring(openBraketIndex + 1, currentMessage.InnerText.Length - openBraketIndex - 2);
                        if (damageString.EndsWith(" крит!"))
                        {
                            isCrit = true;

                            damageString = damageString.Substring(0, damageString.Length - 6); //" крит!".Length
                        }

                        int.TryParse(damageString, out damage);
                        textWithoutDamage = textWithoutDamage.Substring(0, openBraketIndex - 1);
                    }
                }
            }

            foreach (var damageSkill in _damageSkills)
            {
                if (damageSkill.ProcessMessage(textWithoutDamage, nextMessageText, previousMessageText, damage))
                {
                    break;
                }
            }

            if (textWithoutDamage.StartsWith("Вы произнесли магические слова:", StringComparison.Ordinal))
            {
                _sessionStats.TotalCasts++;
                _fightStats.TotalCasts++;
                _zoneStats.TotalCasts++;
            }

            if (textWithoutDamage.StartsWith("Вам не удалось сконцентрироваться!", StringComparison.Ordinal))
            {
                _sessionStats.FailedCasts++;
                _fightStats.FailedCasts++;
                _zoneStats.FailedCasts++;

                _sessionStats.TotalCasts++;
                _fightStats.TotalCasts++;
                _zoneStats.TotalCasts++;
            }


            if (textBlock.Foreground == TextColor.BrightYellow && damage > 0)
            {
                _sessionStats.TotalDamageMessages++;
                _fightStats.TotalDamageMessages++;
                _zoneStats.TotalDamageMessages++;

                if (isCrit)
                {
                    _sessionStats.TotalCritDamageMessages++;
                    _fightStats.TotalCritDamageMessages++;
                    _zoneStats.TotalCritDamageMessages++;
                }

                _sessionStats.TotalRoundDamage += damage;
                _fightStats.TotalRoundDamage += damage;
                _zoneStats.TotalRoundDamage += damage;
            }

            if (textWithoutDamage.StartsWith("Ваш мощный удар оглушил на некоторое время", StringComparison.Ordinal))
            {
                _sessionStats.StunRounds++;
                _fightStats.StunRounds++;
                _zoneStats.StunRounds++;
            }

            if (currentMessage.MessageBlocks.FirstOrDefault()?.Foreground == TextColor.BrightRed && currentMessage.InnerText.EndsWith(")"))
            {
                _sessionStats.TotalTankRoundDamage += damage;
                _fightStats.TotalTankRoundDamage += damage;
                _zoneStats.TotalTankRoundDamage += damage;
                return;
            }

            _matchingResults[1] = string.Empty;
            if (SlavaToken.Match(currentMessage.InnerText, 0, _matchingResults).IsSuccess)
            {
                int slava;
                if (int.TryParse(_matchingResults[1], out slava))
                {
                    _sessionStats.TotalSlava += slava;
                    _fightStats.TotalSlava += slava;
                    _zoneStats.TotalSlava += slava;
                }
            }

            _matchingResults[1] = string.Empty;
            if (SlavaToken1.Match(currentMessage.InnerText, 0, _matchingResults).IsSuccess)
            {
                int slava;
                if (int.TryParse(_matchingResults[1], out slava))
                {
                    _sessionStats.TotalSlava += slava;
                    _fightStats.TotalSlava += slava;
                    _zoneStats.TotalSlava += slava;
                }
            }

            _matchingResults[1] = string.Empty;
            if (ExpToken.Match(currentMessage.InnerText, 0, _matchingResults).IsSuccess)
            {
                int exp;
                if (int.TryParse(_matchingResults[1], out exp))
                {
                    _sessionStats.TotalExp += exp;
                    _fightStats.TotalExp += exp;
                    _zoneStats.TotalExp += exp;
                }
            }

            _matchingResults[1] = string.Empty;
            if (ExpToken1.Match(currentMessage.InnerText, 0, _matchingResults).IsSuccess)
            {
                int exp;
                if (int.TryParse(_matchingResults[1], out exp))
                {
                    _sessionStats.TotalExp += exp;
                    _fightStats.TotalExp += exp;
                    _zoneStats.TotalExp += exp;
                }
            }
        }

        public void RoundCompleted()
        {
            ProcessRoundCompleted(_sessionStats);
            ProcessRoundCompleted(_fightStats);
            ProcessRoundCompleted(_zoneStats);
        }

        private static void ProcessRoundCompleted(DamageStats stat)
        {
            if (stat.TotalRoundDamage > 0)
            {
                stat.TotalRounds++;
                stat.TotalDamage += stat.TotalRoundDamage;
                if (stat.TotalRoundDamage > stat.MaxRoundDamage)
                {
                    stat.MaxRoundDamage = stat.TotalRoundDamage;
                }

                stat.TotalRoundDamage = 0.0d;
            }

            if (stat.TotalTankRoundDamage > 0)
            {
                stat.TotalTankRounds++;
                stat.TotalTankDamage += stat.TotalTankRoundDamage;
                if (stat.TotalTankRoundDamage > stat.MaxTankRoundDamage)
                {
                    stat.MaxTankRoundDamage = stat.TotalTankRoundDamage;
                }

                stat.TotalTankRoundDamage = 0.0d;
            }
        }

        public void PrintStatistics(MessageConveyor conveyor, StatisticTypes statisticTypeToPrint)
        {
            if (statisticTypeToPrint == StatisticTypes.Session)
            {
                conveyor.PushMessage(new OutputToMainWindowMessage("Статистика за текущую сессию:", TextColor.BrightWhite));
                PrintStatistics(conveyor, _sessionStats, statisticTypeToPrint);
            }

            if (statisticTypeToPrint == StatisticTypes.Zone)
            {
                conveyor.PushMessage(new OutputToMainWindowMessage("Статистика за последнюю зону:", TextColor.BrightWhite));
                PrintStatistics(conveyor, _zoneStats, statisticTypeToPrint);
            }

            if (statisticTypeToPrint == StatisticTypes.Fight)
            {
                conveyor.PushMessage(new OutputToMainWindowMessage("Статистика за последний бой:", TextColor.BrightWhite));
                PrintStatistics(conveyor, _fightStats, statisticTypeToPrint);
            }
        }

        private void PrintStatistics(MessageConveyor conveyor, DamageStats stat, StatisticTypes statisticTypeToPrint)
        {
            conveyor.PushMessage(new OutputToMainWindowMessage("Ваши повреждения:", TextColor.BrightYellow));

            if (stat.TotalRounds > 0)
            {
                conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                {
                    new TextMessageBlock("   Дамаг в раунд средний: ", TextColor.White),
                    new TextMessageBlock((stat.TotalDamage/stat.TotalRounds).ToString("00.00"), TextColor.BrightWhite),
                    new TextMessageBlock("\t Максимальный: ", TextColor.White),
                    new TextMessageBlock(stat.MaxRoundDamage.ToString("00.00"), TextColor.BrightWhite),
                }));

                conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                {
                    new TextMessageBlock("   Дамаг cуммарный: ", TextColor.White),
                    new TextMessageBlock(stat.TotalDamage.ToString(CultureInfo.InvariantCulture), TextColor.BrightWhite),
                }));

            }

            if (stat.TotalDamageMessages > 0)
            {
                conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                {
                    new TextMessageBlock("   Крит шанс: ", TextColor.White),
                    new TextMessageBlock(((double) stat.TotalCritDamageMessages/stat.TotalDamageMessages).ToString("P"),
                        TextColor.BrightWhite),
                }));
            }

            if (stat.TotalCasts > 0)
            {
                conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                {
                    new TextMessageBlock("   Шанс фейл каста: ", TextColor.White),
                    new TextMessageBlock(((double) stat.FailedCasts/stat.TotalCasts).ToString("P"), TextColor.BrightWhite),
                }));
            }

            if (stat.StunRounds > 0 && stat.TotalRounds > 0)
            {
                conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                {
                    new TextMessageBlock("   Шанс оглушить: ", TextColor.White),
                    new TextMessageBlock(((double) stat.StunRounds/stat.TotalRounds).ToString("P"), TextColor.BrightWhite),
                }));
            }

            conveyor.PushMessage(new OutputToMainWindowMessage("Умения/заклинания:", TextColor.BrightYellow));
            foreach (var damageSkill in _damageSkills)
            {
                damageSkill.PrintDamageStatistics(conveyor, statisticTypeToPrint);
            }

            conveyor.PushMessage(new OutputToMainWindowMessage("Повреждения по вам:", TextColor.BrightRed));

            if (stat.TotalTankRounds > 0)
            {
                conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                {
                    new TextMessageBlock("   Дамаг в раунд средний: ", TextColor.White),
                    new TextMessageBlock((stat.TotalTankDamage/stat.TotalTankRounds).ToString("00.00"), TextColor.BrightWhite),
                    new TextMessageBlock("\t Максимальный: ", TextColor.White),
                    new TextMessageBlock(stat.MaxTankRoundDamage.ToString("00.00"), TextColor.BrightWhite),
                }));

                conveyor.PushMessage(new OutputToMainWindowMessage(new[]
                {
                    new TextMessageBlock("   Дамаг cуммарный: ", TextColor.White),
                    new TextMessageBlock(stat.TotalTankDamage.ToString(CultureInfo.InvariantCulture), TextColor.BrightWhite),
                }));
            }

            conveyor.PushMessage(new OutputToMainWindowMessage("Умения/заклинания:", TextColor.BrightRed));
            foreach (var damageSkill in _damageSkills)
            {
                damageSkill.PrintTankStatistics(conveyor, statisticTypeToPrint);
            }

            var expMessages = new List<TextMessageBlock>();
            if (stat.TotalExp > 0)
            {
                expMessages.Add(new TextMessageBlock("Опыт: ", TextColor.White));
                expMessages.Add(new TextMessageBlock(stat.TotalExp.ToString(CultureInfo.InvariantCulture), TextColor.BrightWhite));
                expMessages.Add(new TextMessageBlock("\t", TextColor.White));
            }

            if (stat.TotalSlava > 0)
            {
                expMessages.Add(new TextMessageBlock("Слава: ", TextColor.White));
                expMessages.Add(new TextMessageBlock(stat.TotalSlava.ToString(CultureInfo.InvariantCulture),
                    TextColor.BrightWhite));
            }

            if (expMessages.Any())
            {
                conveyor.PushMessage(new OutputToMainWindowMessage(expMessages));
            }
        }

        public void ResetStats(StatisticTypes statisticTypeToReset)
        {
            if (statisticTypeToReset == StatisticTypes.Session)
            {
                _sessionStats=new DamageStats();
            }
            if (statisticTypeToReset == StatisticTypes.Zone)
            {
                _zoneStats=new DamageStats();
            }
            if (statisticTypeToReset == StatisticTypes.Fight)
            {
                _fightStats = new DamageStats();
            }

            foreach (var damageSkill in _damageSkills)
            {
                damageSkill.Reset(statisticTypeToReset);
            }
        }

        private class DamageStats
        {
            public int TotalSlava;
            public int TotalExp;

            public double TotalRoundDamage;

            public double TotalDamage;
            public double MaxRoundDamage;
            public int TotalDamageMessages;
            public int TotalCritDamageMessages;
            public int TotalRounds;
            public int StunRounds;
            public int TotalCasts;
            public int FailedCasts;
            public double TotalTankRoundDamage;
            public double TotalTankDamage;
            public double MaxTankRoundDamage;
            public int TotalTankRounds;
        }

    }

    public enum StatisticTypes
    {
        Session,
        Zone,
        Fight
    }
}
