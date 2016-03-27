// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the HighlightViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel
{
    using System.Collections.Generic;

    using Common.Model;
    using Common.Themes;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model;

    /// <summary>
    /// View model for highlight editor.
    /// </summary>
    public class HighlightViewModel : ViewModelBase
    {
        private GroupViewModel _highlightGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightViewModel"/> class.
        /// </summary>
        /// <param name="allGroups">All groups.</param>
        /// <param name="highlightGroup">The highlight group.</param>
        /// <param name="highlight">The highlight.</param>
        public HighlightViewModel([NotNull] IEnumerable<GroupViewModel> allGroups, [NotNull] GroupViewModel highlightGroup, [NotNull] Highlight highlight)
        {
            Assert.ArgumentNotNull(allGroups, "allGroups");
            Assert.ArgumentNotNull(highlightGroup, "highlightGroup");
            Assert.ArgumentNotNull(highlight, "highlight");

            _highlightGroup = highlightGroup;
            AllGroups = allGroups;
            Highlight = highlight;
        }

        /// <summary>
        /// Gets or sets the highlight group.
        /// </summary>
        /// <value>
        /// The highlight group.
        /// </value>
        [NotNull]
        public GroupViewModel HighlightGroup
        {
            get
            {
                return _highlightGroup;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _highlightGroup = value;
                OnPropertyChanged("HighlightGroup");
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
        /// 
        /// </summary>
        [NotNull]
        public bool IsRegExp
        {
            get
            {
                return Highlight.IsRegExp;
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");

                Highlight.IsRegExp = value;
                OnPropertyChanged("IsRegExp");
            }
        }

        /// <summary>
        /// Gets the highlight.
        /// </summary>
        /// <value>
        /// The highlight.
        /// </value>
        [NotNull]
        public Highlight Highlight
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the text to high light.
        /// </summary>
        /// <value>
        /// The text to high light.
        /// </value>
        [NotNull]
        public string TextToHighlight
        {
            get
            {
                return Highlight.TextToHighlight;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                Highlight.TextToHighlight = value;
                OnPropertyChanged("TextToHighlight");
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>
        /// The color of the text.
        /// </value>
        public TextColor TextColor
        {
            get
            {
                return Highlight.ForegroundColor;
            }

            set
            {
                Highlight.ForegroundColor = value;
                OnPropertyChanged("TextColor");
            }
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        public TextColor BackgroundColor
        {
            get
            {
                return Highlight.BackgroundColor;
            }

            set
            {
                Highlight.BackgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        [NotNull]
        public HighlightViewModel Clone()
        {
            return new HighlightViewModel(AllGroups, HighlightGroup, new Highlight())
                       {
                           TextToHighlight = TextToHighlight,
                           TextColor = TextColor,
                           BackgroundColor = BackgroundColor,
                           IsRegExp = IsRegExp
                       };
        }
    }
}
