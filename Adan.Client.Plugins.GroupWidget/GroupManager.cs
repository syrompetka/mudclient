using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Adan.Client.Common.Model;
using Adan.Client.Plugins.GroupWidget.Model;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.Plugins.GroupWidget
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupManager
    {
        private readonly GroupWidgetControl _groupWidget;
        private readonly Dictionary<string, GroupHolder> _groupHolders;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupWidgetControl"></param>
        public GroupManager([NotNull] GroupWidgetControl groupWidgetControl)
        {
            Assert.ArgumentNotNull(groupWidgetControl, "groupWidgetControl");

            _groupWidget = groupWidgetControl;
            _groupHolders = new Dictionary<string, GroupHolder>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        /// <param name="uid"></param>
        public void OutputWindowCreated([NotNull] RootModel rootModel, [NotNull] string uid)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");
            Assert.ArgumentNotNullOrWhiteSpace(uid, "uid");

            _groupHolders.Add(uid, new GroupHolder(this, rootModel, uid));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        public void OutputWindowClosed([NotNull] string uid)
        {
            Assert.ArgumentNotNullOrWhiteSpace(uid, "uid");

            if (_groupHolders.ContainsKey(uid))
                _groupHolders.Remove(uid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        public void OutputWindowChanged([NotNull] string uid)
        {
            Assert.ArgumentNotNullOrWhiteSpace(uid, "uid");

            if (_groupHolders.ContainsKey(uid))
            {
                _groupWidget.ViewModelUid = uid;
                _groupWidget.UpdateModel(_groupHolders[uid].RootModel, _groupHolders[uid].Characters);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupHolder"></param>
        public void UpdateGroup([NotNull] GroupHolder groupHolder)
        {
            Assert.ArgumentNotNull(groupHolder, "groupHolder");

            if (_groupWidget.ViewModelUid == groupHolder.Uid)
            {
                _groupWidget.UpdateModel(groupHolder.RootModel, groupHolder.Characters);
            }
        }
    }
}
