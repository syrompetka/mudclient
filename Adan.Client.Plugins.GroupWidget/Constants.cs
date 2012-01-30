// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Constants type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget
{
    using System.Collections.Generic;

    using CSLib.Net.Annotations;

    using Properties;

    using ViewModel;

    /// <summary>
    /// Class to store misc constants of this plugin.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Type of "Group status" custom message.
        /// </summary>
        public const int GroupStatusMessageType = 12;

        /// <summary>
        /// Type of "Room monsters" custom message;
        /// </summary>
        public const int RoomMonstersMessage = 13;

        private static readonly List<AffectDescription> _allAffects = new List<AffectDescription>
                              {
                                  new AffectDescription("Invisibility", Resources.Invisibility, "невидимость", "Invisibility"),
                                  new AffectDescription("Blessing", Resources.Blessing, new List<string> { "благословение", "точность" }, new List<string> { "Blessing", "Accuracy" }),
                                  new AffectDescription("Poison", Resources.Poison, "яд", "Poison") { IsRoundBased = true },
                                  new AffectDescription("Protection", Resources.Protection, "защита", "Protection"),
                                  new AffectDescription("Flight", Resources.Flight, "полет", "Flying"),
                                  new AffectDescription("Acceleration", Resources.Acceleration, "ускорение", "Acceleration") { IsRoundBased = true },
                                  new AffectDescription("Blindness", Resources.Blindness, "слепота", "Blindness") { IsRoundBased = true },
                                  new AffectDescription("Healings", Resources.Healings, new List<string> { "легкое заживление", "серьезное заживление", "критическое заживление", "заживление" }, new List<string> { "LightHealing", "SeriousHealing", "CriticalHealing", "Healing" }) { IsRoundBased = true },
                                  new AffectDescription("Renewals", Resources.Renewals, new List<string> { "легкое обновление", "серьезное обновление", "критическое обновление" }, new List<string> { "LightRenewal", "SeriousRenewal", "CriticalRenewal" }) { IsRoundBased = true },
                                  new AffectDescription("Curse", Resources.Curse, "проклятие", "Curse"),
                                  new AffectDescription("ArmorCurse", Resources.ArmorCurse, "проклятие защиты", "ArmorCurse"),
                                  new AffectDescription("Fear", Resources.Fear, "страх", "Fear") { IsRoundBased = true },
                                  new AffectDescription("Power", Resources.Power, "сила", "Power"),
                                  new AffectDescription("Fresh", Resources.Fresh, "восстановление", "Fresh"),
                                  new AffectDescription("Hold", Resources.Hold, new List<string> { "придержать персону", "придержать любого" }, new List<string> { "Hold", "Hold" }) { IsRoundBased = true },
                                  new AffectDescription("Disease", Resources.Disease, "болезнь", "Disease"),
                                  new AffectDescription("Paralysis", Resources.Paralysis, "паралич", "Paralysis") { IsRoundBased = true },
                                  new AffectDescription("Shock", Resources.Shock, "шок", "Shock") { IsRoundBased = true },
                                  new AffectDescription("Silence", Resources.Silence, "молчание", "Silence") { IsRoundBased = true },
                                  new AffectDescription("Slowness", Resources.Slowness, "замедление", "Slowness") { IsRoundBased = true },
                                  new AffectDescription("StoneCurse", Resources.StoneCurse, "каменное проклятие", "StoneCurse"),
                                  new AffectDescription("EntsPower", Resources.EntsPower, "сила энтов", "EntsPower") { IsRoundBased = true },
                                  new AffectDescription("Stun", Resources.Stun, "оглушение", "Stun") { IsRoundBased = true },
                                  new AffectDescription("Weakness", Resources.Weakness, "слабость", "Weakness"),
                              };

        /// <summary>
        /// Gets all supported affects.
        /// </summary>
        [NotNull]
        public static IEnumerable<AffectDescription> AllAffects
        {
            get
            {
                return _allAffects;
            }
        }
    }
}
