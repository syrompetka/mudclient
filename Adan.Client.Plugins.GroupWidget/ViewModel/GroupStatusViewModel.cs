// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupStatusViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the GroupStatusViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Threading;

    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Properties;

    /// <summary>
    /// A view model for group status widget.
    /// </summary>
    public class GroupStatusViewModel : ViewModelBase
    {
        private readonly DispatcherTimer _tickingTimer = new DispatcherTimer();
        private IList<string> _displayedAffectNames = new List<string>(Settings.Default.GroupWidgetAffects);

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupStatusViewModel"/> class.
        /// </summary>
        public GroupStatusViewModel()
        {
            GroupMates = new ObservableCollection<GroupMateViewModel>();
            _tickingTimer.Interval = TimeSpan.FromSeconds(1);
            _tickingTimer.Tick += (o, e) => UpdateTimings();
            _tickingTimer.Start();
        }

        /// <summary>
        /// Gets the group mates of current player.
        /// </summary>
        [NotNull]
        public ObservableCollection<GroupMateViewModel> GroupMates
        {
            get;
            private set;
        }

        /// <summary>
        /// Updates the model.
        /// </summary>
        /// <param name="groupMates">The group mates as they come from server.</param>
        public void UpdateModel([NotNull] IEnumerable<CharacterStatus> groupMates)
        {
            Assert.ArgumentNotNull(groupMates, "groupMates");

            int position = 0;
            foreach (var characterStatus in groupMates)
            {
                if (position < GroupMates.Count && GroupMates[position].Name == characterStatus.Name)
                {
                    GroupMates[position].UpdateFromModel(characterStatus);
                }
                else
                {
                    var affectsList = _displayedAffectNames.Select(af => Constants.AllAffects.First(a => a.Name == af));
                    GroupMates.Insert(position, new GroupMateViewModel(characterStatus, affectsList));
                }

                position++;
            }

            for (int i = position; i < GroupMates.Count; i++)
            {
                if (!GroupMates[i].IsDeleting)
                {
                    var groupMateToRemove = GroupMates[i];
                    groupMateToRemove.IsDeleting = true;

                    var timer = new DispatcherTimer(DispatcherPriority.SystemIdle);

                    timer.Tick += (o, e) =>
                                  {
                                      timer.Stop();
                                      GroupMates.Remove(groupMateToRemove);
                                  };
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Start();
                }
            }
        }

        /// <summary>
        /// Reloads the displayed affects.
        /// </summary>
        public void ReloadDisplayedAffects()
        {
            GroupMates.Clear();
            _displayedAffectNames = new List<string>(Settings.Default.GroupWidgetAffects);
        }

        private void UpdateTimings()
        {
            foreach (var groupMate in GroupMates)
            {
                groupMate.UpdateTimings();
            }
        }
    }
}

