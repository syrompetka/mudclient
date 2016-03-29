// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AffectViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the AffectViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget.ViewModel
{
    using System;

    using Common.Model;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A view model to display single affect
    /// </summary>
    public class AffectViewModel : ViewModelBase
    {
        private readonly int _priority;
        private string _displayIcon;
        private float _lastServerDuration;
        private bool _isBlinking;

        private float _secondsLeft;
        private bool _secondsLeftVisible;

        private int _roundsLeft;
        private bool _roundsLeftVisible;
        private string _realAffectName;
        private DateTime _lastTimerUpdate = DateTime.Now;

        /// <summary>
        /// Initializes a new instance of the <see cref="AffectViewModel"/> class.
        /// </summary>
        public AffectViewModel([NotNull] AffectDescription affectDescription, int priority)
        {
            Assert.ArgumentNotNull(affectDescription, "affectDescription");
            _priority = priority;
            AffectDescription = affectDescription;
        }

        /// <summary>
        /// Gets the affect description.
        /// </summary>
        [NotNull]
        public AffectDescription AffectDescription
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of icon to display.
        /// </summary>
        [CanBeNull]
        public string DisplayIcon
        {
            get
            {
                return _displayIcon;
            }

            private set
            {
                if (value != _displayIcon)
                {
                    _displayIcon = value;
                    OnPropertyChanged("DisplayIcon");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether to display seconds left or not.
        /// </summary>
        public bool SecondsLeftVisible
        {
            get
            {
                return _secondsLeftVisible;
            }

            private set
            {
                if (value != _secondsLeftVisible)
                {
                    _secondsLeftVisible = value;
                    OnPropertyChanged("SecondsLeftVisible");
                }
            }
        }

        /// <summary>
        /// Gets the numer of  rounds left for this affect to disapear.
        /// </summary>
        public int RoundsLeft
        {
            get
            {
                return _roundsLeft;
            }

            private set
            {
                if (value != _roundsLeft)
                {
                    _roundsLeft = value;
                    OnPropertyChanged("RoundsLeft");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this rounds left indicator should be displayed.
        /// </summary>
        public bool RoundsLeftVisible
        {
            get
            {
                return _roundsLeftVisible;
            }

            private set
            {
                if (value != _roundsLeftVisible)
                {
                    _roundsLeftVisible = value;
                    OnPropertyChanged("RoundsLeftVisible");
                }
            }
        }

        /// <summary>
        /// Gets the number seconds left for this affect to disapear.
        /// </summary>
        public float SecondsLeft
        {
            get
            {
                return _secondsLeft;
            }

            private set
            {
                if (value != _secondsLeft)
                {
                    _secondsLeft = value;
                    OnPropertyChanged("SecondsLeft");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this affect is blinking.
        /// </summary>
        /// <value>
        /// <c>true</c> if this affect is blinking; otherwise, <c>false</c>.
        /// </value>
        public bool IsBlinking
        {
            get
            {
                return _isBlinking;
            }

            private set
            {
                if (value != _isBlinking)
                {
                    _isBlinking = value;
                    OnPropertyChanged("IsBlinking");
                }
            }
        }

        /// <summary>
        /// Gets the name of affect as it comes from the server.
        /// </summary>
        [CanBeNull]
        public string RealAffectName
        {
            get
            {
                return _realAffectName;
            }

            private set
            {
                if (value != _realAffectName)
                {
                    _realAffectName = value;
                    OnPropertyChanged("RealAffectName");
                }
            }
        }

        public int Priority
        {
            get
            {
                if (string.IsNullOrEmpty(DisplayIcon))
                {
                    return int.MaxValue;
                }

                return _priority;
            }
        }

        /// <summary>
        /// Updates from view model from model.
        /// </summary>
        /// <param name="affect">The affect.</param>
        public void UpdateFromModel([NotNull] Affect affect)
        {
            Assert.ArgumentNotNull(affect, "affect");

            if (!AffectDescription.IsRoundBased)
            {
                if (_lastServerDuration != affect.Duration)
                {
                    _lastServerDuration = affect.Duration == -1.0f ? float.PositiveInfinity : affect.Duration;
                    SecondsLeft = _lastServerDuration;
                    _lastTimerUpdate = DateTime.Now;
                    SecondsLeftVisible = SecondsLeft <= 9.0f;
                    IsBlinking = SecondsLeft <= 5.0f;
                    RoundsLeftVisible = false;
                    RoundsLeft = 0;
                }
            }
            else
            {
                RoundsLeft = affect.Rounds <= 0 ? int.MaxValue : affect.Rounds;
                RoundsLeftVisible = RoundsLeft <= 6;
                IsBlinking = RoundsLeft <= 2;
                SecondsLeftVisible = false;
                SecondsLeft = 0;
            }

            var iconIndex = AffectDescription.AffectNames.IndexOf(affect.Name);
            var oldDisplayIcon = DisplayIcon;
            DisplayIcon = iconIndex >= 0 ? AffectDescription.Icons[iconIndex] : string.Empty;
            if (string.IsNullOrEmpty(oldDisplayIcon) && !string.IsNullOrEmpty(DisplayIcon))
            {
                OnPropertyChanged("Priority");
            }

            RealAffectName = affect.Name;
        }

        /// <summary>
        /// Called when affect is removed.
        /// </summary>
        public void OnAffectRemoved()
        {
            DisplayIcon = string.Empty;
            SecondsLeftVisible = false;
            SecondsLeft = 0;
            RoundsLeft = 0;
            RoundsLeftVisible = false;
            RealAffectName = string.Empty;
            IsBlinking = false;
            OnPropertyChanged("Priority");
        }

        /// <summary>
        /// Updates the timings.
        /// </summary>
        public void UpdateTimings()
        {
            if (!AffectDescription.IsRoundBased)
            {
                if (SecondsLeft > 0)
                {
                    SecondsLeft -= (float)(DateTime.Now - _lastTimerUpdate).TotalSeconds;
                    if (SecondsLeft < 0)
                    {
                        DisplayIcon = string.Empty;
                        SecondsLeftVisible = false;
                        SecondsLeft = 0;
                    }
                    else
                    {
                        SecondsLeftVisible = SecondsLeft < 10.0f;
                        IsBlinking = SecondsLeft <= 5.0f;
                    }

                    _lastTimerUpdate = DateTime.Now;
                }
            }
        }
    }
}
