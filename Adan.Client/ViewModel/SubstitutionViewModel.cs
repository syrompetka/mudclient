// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubstitutionViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the SubstitutionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel
{
    using System.Collections.Generic;

    using Common.Model;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// View model for substitution editor.
    /// </summary>
    public class SubstitutionViewModel : ViewModelBase
    {
        private GroupViewModel _substitutionGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubstitutionViewModel"/> class.
        /// </summary>
        /// <param name="allGroups">All groups.</param>
        /// <param name="substitutionGroup">The substitution group.</param>
        /// <param name="substitution">The substitution.</param>
        public SubstitutionViewModel([NotNull] IEnumerable<GroupViewModel> allGroups, [NotNull] GroupViewModel substitutionGroup, [NotNull] Substitution substitution)
        {
            Assert.ArgumentNotNull(allGroups, "allGroups");
            Assert.ArgumentNotNull(substitutionGroup, "substitutionGroup");
            Assert.ArgumentNotNull(substitution, "substitution");

            _substitutionGroup = substitutionGroup;
            AllGroups = allGroups;
            Substitution = substitution;
        }

        /// <summary>
        /// Gets or sets the substitution group.
        /// </summary>
        /// <value>
        /// The substitution group.
        /// </value>
        [NotNull]
        public GroupViewModel SubstitutionGroup
        {
            get
            {
                return _substitutionGroup;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _substitutionGroup = value;
                OnPropertyChanged("substitutionGroup");
            }
        }

        /// <summary>
        /// Gets all groups.
        /// </summary>
        [NotNull]
        public IEnumerable<GroupViewModel> AllGroups
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the substitution.
        /// </summary>
        [NotNull]
        public Substitution Substitution
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>
        /// The pattern.
        /// </value>
        [NotNull]
        public string Pattern
        {
            get
            {
                return Substitution.Pattern;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                Substitution.Pattern = value;
                OnPropertyChanged("Pattern");
            }
        }

        /// <summary>
        /// Gets or sets the substitute with.
        /// </summary>
        /// <value>
        /// The substitute with.
        /// </value>
        [NotNull]
        public string SubstituteWith
        {
            get
            {
                return Substitution.SubstituteWith;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                Substitution.SubstituteWith = value;
                OnPropertyChanged("SubstituteWith");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public bool IsRegExp
        {
            get
            {
                return Substitution.IsRegExp;
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");

                Substitution.IsRegExp = value;
                OnPropertyChanged("IsRegExp");
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        [NotNull]
        public SubstitutionViewModel Clone()
        {
            return new SubstitutionViewModel(AllGroups, SubstitutionGroup, new Substitution())
                       {
                           Pattern = Pattern,
                           SubstituteWith = SubstituteWith
                       };
        }
    }
}
