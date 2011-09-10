// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoreMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the LoreMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Serialization;

    using Common.Messages;
    using Common.Themes;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Properties;

    /// <summary>
    /// Message containing stats of some object.
    /// </summary>
    [Serializable]
    public class LoreMessage : Message
    {
        private readonly List<string> _wearSlots;
        private readonly List<string> _objectAffects;
        private readonly List<string> _affects;
        private readonly List<string> _flags;
        private readonly List<string> _restrictionFlags;
        private readonly List<string> _noFlags;
        private readonly List<ScrollOrPotionSpell> _scrollOrPotionSpells;
        private readonly List<AppliedAffect> _appliedAffects;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoreMessage"/> class.
        /// </summary>
        public LoreMessage()
        {
            ObjectName = string.Empty;
            Comments = string.Empty;
            Material = string.Empty;
            ObjectType = string.Empty;
            _wearSlots = new List<string>();
            _objectAffects = new List<string>();
            _affects = new List<string>();
            _flags = new List<string>();
            _restrictionFlags = new List<string>();
            _noFlags = new List<string>();
            _scrollOrPotionSpells = new List<ScrollOrPotionSpell>();
            _appliedAffects = new List<AppliedAffect>();
        }

        /// <summary>
        /// Gets the type of this message.
        /// </summary>
        /// <value>
        /// The type of this message.
        /// </value>
        public override int MessageType
        {
            get
            {
                return Constants.LoreMessageType;
            }
        }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>
        /// The name of the object.
        /// </value>
        [NotNull]
        [XmlAttribute("Name")]
        public string ObjectName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the object.
        /// </summary>
        /// <value>
        /// The type of the object.
        /// </value>
        [NotNull]
        [XmlAttribute("Type")]
        public string ObjectType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the weight of the object.
        /// </summary>
        /// <value>
        /// The weight of the object.
        /// </value>
        [XmlAttribute]
        public float Weight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the price of the object.
        /// </summary>
        /// <value>
        /// The price of the object.
        /// </value>
        [XmlAttribute]
        public int Price
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the rent price of the object.
        /// </summary>
        /// <value>
        /// The rent price of the object.
        /// </value>
        [XmlAttribute(AttributeName = "Rent")]
        public int RentPrice
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the rent price of the equipped object.
        /// </summary>
        /// <value>
        /// The rent price of the equipped object.
        /// </value>
        [XmlAttribute(AttributeName = "RentEquipped")]
        public int RentPriceEquipped
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the timer of the object.
        /// </summary>
        /// <value>
        /// The timer of the object.
        /// </value>
        [XmlAttribute]
        public int Timer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the material of the object.
        /// </summary>
        /// <value>
        /// The material of the object.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Material
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum level required to use the object.
        /// </summary>
        /// <value>
        /// The minimum level required.
        /// </value>
        [XmlAttribute]
        public int MinLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this message contains full description of object or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if this message contains full description of object; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool IsFull
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a list of slots this object can be weared to.
        /// </summary>
        [NotNull]
        [XmlArray(ElementName = "Wear")]
        [XmlArrayItem(ElementName = "WearSlot")]
        public List<string> WearSlots
        {
            get
            {
                return _wearSlots;
            }
        }

        /// <summary>
        /// Gets the object affects.
        /// </summary>
        [NotNull]
        [XmlArrayItem(ElementName = "ObjectAffect")]
        public List<string> ObjectAffects
        {
            get
            {
                return _objectAffects;
            }
        }

        /// <summary>
        /// Gets the flags of the object.
        /// </summary>
        [NotNull]
        [XmlArray(ElementName = "Flags")]
        [XmlArrayItem(ElementName = "Flag")]
        public List<string> Properties
        {
            get
            {
                return _flags;
            }
        }

        /// <summary>
        /// Gets the restriction flags of this object.
        /// </summary>
        [NotNull]
        [XmlArray(ElementName = "RestrictionFlags")]
        [XmlArrayItem(ElementName = "RestrictionFlag")]
        public List<string> Restrictions
        {
            get
            {
                return _restrictionFlags;
            }
        }

        /// <summary>
        /// Gets the "No" flags of this object.
        /// </summary>
        [NotNull]
        [XmlArray(ElementName = "NoFlags")]
        [XmlArrayItem(ElementName = "NoFlag")]
        public List<string> BannedFor
        {
            get
            {
                return _noFlags;
            }
        }

        /// <summary>
        /// Gets the affects of this object.
        /// </summary>
        [NotNull]
        [XmlArrayItem(ElementName = "Affect")]
        public List<string> Affects
        {
            get
            {
                return _affects;
            }
        }

        /// <summary>
        /// Gets or sets the affect that is applied to player character when some object is weared.
        /// </summary>
        /// <value>
        /// <see cref="WearingAffect"/> instance describing applied affect or <c>null</c> if there is no such one.
        /// </value>
        [CanBeNull]
        public WearingAffect WearingAffect
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the scroll or potion spells casted when this object is used.
        /// </summary>
        [NotNull]
        [XmlArrayItem(ElementName = "Spell")]
        public List<ScrollOrPotionSpell> ScrollOrPotionSpells
        {
            get
            {
                return _scrollOrPotionSpells;
            }
        }

        /// <summary>
        /// Gets or sets the spell description casted when this object is used.
        /// </summary>
        /// <value>
        /// The wand or staff spell description.
        /// </value>
        [CanBeNull]
        public WandOrStaffSpell WandOrStaffSpell
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the armor stats.
        /// </summary>
        /// <value>
        /// The armor stats.
        /// </value>
        [CanBeNull]
        public ArmorStats ArmorStats
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the weapon details.
        /// </summary>
        /// <value>
        /// <see cref="WeaponStats"/> instance of <c>null</c> if the object is not weapon. 
        /// </value>
        [CanBeNull]
        public WeaponStats WeaponStats
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the spell book details.
        /// </summary>
        /// <value>
        /// <see cref="SpellBook"/> instance or <c>null</c> if the object is not spell book.
        /// </value>
        [CanBeNull]
        public SpellBook SpellBook
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ingredient details.
        /// </summary>
        /// <value>
        /// <see cref="StuffDatabase.Ingredient"/> instance or <c>null</c> if the object is not ingredient.
        /// </value>
        [CanBeNull]
        public Ingredient Ingredient
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the craft book details.
        /// </summary>
        /// <value>
        /// <see cref="CraftBook"/> instance or <c>null</c> if the object is not craft book.
        /// </value>
        [CanBeNull]
        [XmlElement(ElementName = "Recipe")]
        public CraftBook CraftBook
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the affects that are applied to a character wearing the object.
        /// </summary>
        [XmlArrayItem(typeof(SkillResist))]
        [XmlArrayItem(typeof(SkillEnhance))]
        [XmlArrayItem(typeof(MagicArrows))]
        [XmlArrayItem(typeof(Envenom))]
        [XmlArrayItem(typeof(Enhance))]
        [NotNull]
        public List<AppliedAffect> AppliedAffects
        {
            get
            {
                return _appliedAffects;
            }
        }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>
        /// The comments.
        /// </value>
        [NotNull]
        public string Comments
        {
            get;
            set;
        }

        /// <summary>
        /// Converts this message to a set of <see cref="InfoMessage"/> instance to display to user.
        /// </summary>
        /// <returns>A set of <see cref="InfoMessage"/> instances.</returns>
        [NotNull]
        public IEnumerable<InfoMessage> ConvertToMessages()
        {
            var result = new List<InfoMessage>();

            result.Add(
                new InfoMessage(
                    new[]
                        {
                            new TextMessageBlock(Resources.Object + " ", TextColor.BrightWhite),
                            new TextMessageBlock("'" + ObjectName + "'", TextColor.BrightCyan),
                            new TextMessageBlock(", " + Resources.ObjectType + ": " + ObjectType, TextColor.BrightWhite),
                        }));
            if (IsFull)
            {
                int maxLength = 0;
                maxLength = Math.Max(maxLength, Resources.CanBeWeared.Length);
                maxLength = Math.Max(maxLength, Resources.ObjectAffects.Length);
                maxLength = Math.Max(maxLength, Resources.ObjectFlags.Length);
                maxLength = Math.Max(maxLength, Resources.RestrictionFlags.Length);
                maxLength = Math.Max(maxLength, Resources.NoFlags.Length);
                maxLength = Math.Max(maxLength, Resources.Affects.Length);
                maxLength++;
                if (ObjectAffects.Any())
                {
                    result.Add(FormatFlags(Resources.ObjectAffects, ObjectAffects, maxLength));
                }

                result.Add(FormatFlags(Resources.CanBeWeared, WearSlots.Select(slot => Resources.ResourceManager.GetString("Wear" + slot, CultureInfo.CurrentUICulture)), maxLength));
                result.Add(FormatFlags(Resources.ObjectFlags, Properties, maxLength));
                result.Add(FormatFlags(Resources.RestrictionFlags, Restrictions, maxLength));
                result.Add(FormatFlags(Resources.NoFlags, BannedFor, maxLength));
                result.Add(FormatFlags(Resources.Affects, Affects, maxLength));
            }

            result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.CommonStats, Weight, Price, RentPrice, RentPriceEquipped, Timer, Material)));
            if (MinLevel > 1)
            {
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.MinLevel, MinLevel)));
            }

            if (WearingAffect != null)
            {
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.WearingAffect, WearingAffect.AffectName, WearingAffect.Level, WearingAffect.ResetTimeout)));
            }

            if (ScrollOrPotionSpells.Any())
            {
                result.Add(
                    new InfoMessage(
                        new[]
                            {
                                new TextMessageBlock(Resources.Spells + ": ", TextColor.BrightWhite),
                                new TextMessageBlock(string.Join(", ", ScrollOrPotionSpells.Select(sp => sp.SpellName)), TextColor.Green),
                            }));
            }

            if (WandOrStaffSpell != null)
            {
                result.Add(
                new InfoMessage(
                    new[]
                            {
                                new TextMessageBlock(Resources.Spells + ": ", TextColor.BrightWhite),
                                new TextMessageBlock(WandOrStaffSpell.SpellName, TextColor.Green),
                            }));
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.StaffCharges, WandOrStaffSpell.TotalCharges, WandOrStaffSpell.ChargesLeft)));
            }

            if (WeaponStats != null)
            {
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.WeaponPower, WeaponStats.DiceSides, WeaponStats.DiceCount, WeaponStats.AverageDamage)));
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.WeaponSkill, WeaponStats.RequiredSkill)));
            }

            if (ArmorStats != null)
            {
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.ArmorClass, ArmorStats.ArmorClass)));
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.Armor, ArmorStats.Armor)));
            }

            if (SpellBook != null)
            {
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.SpellProfession, SpellBook.Profession)));
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.SpellMinLevel, SpellBook.LearnLevel)));
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.SpellBookSpell, SpellBook.SpellName)));
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.SpellCastCount, SpellBook.CastCount > 0 ? SpellBook.CastCount.ToString(CultureInfo.InvariantCulture) : Resources.SpellCastCountInfinity)));
            }

            if (Ingredient != null)
            {
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.IngredientColor, Ingredient.Color)));
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.IngredientPower, Ingredient.Power)));
            }

            if (CraftBook != null)
            {
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.CraftBookRecipeName, CraftBook.Name)));
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.CraftBookRecipeDescription, CraftBook.Description)));
                if (CraftBook.MinLevel > 1)
                {
                    result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.CraftBookMinLevel, CraftBook.MinLevel)));
                }

                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentUICulture, Resources.CraftBookMinSkillLevel, CraftBook.MinSkillLevel)));
            }

            if (AppliedAffects.Any())
            {
                result.Add(new InfoMessage(Resources.AppliedAffects));
                result.AddRange(AppliedAffects.Select(appliedAffect => appliedAffect.ConvertToInfoMessage()));
            }

            if (!string.IsNullOrEmpty(Comments))
            {
                result.Add(new InfoMessage(string.Format(CultureInfo.CurrentCulture, Resources.Comments, Comments)));
            }

            return result;
        }

        [NotNull]
        private static InfoMessage FormatFlags([NotNull] string header, [NotNull] IEnumerable<string> flags, int headerLength)
        {
            Assert.ArgumentNotNullOrWhiteSpace(header, "header");
            Assert.ArgumentNotNull(flags, "flags");

            var headerBlock = new TextMessageBlock(header.PadRight(headerLength) + ": ", TextColor.BrightWhite);
            TextMessageBlock contentsBlock;
            if (flags.Any())
            {
                contentsBlock = new TextMessageBlock(string.Join(", ", flags), TextColor.Cyan);
            }
            else
            {
                contentsBlock = new TextMessageBlock("NONE", TextColor.Cyan);
            }

            return new InfoMessage(new[] { headerBlock, contentsBlock });
        }
    }
}
