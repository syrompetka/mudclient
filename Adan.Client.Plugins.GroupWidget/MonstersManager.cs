using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adan.Client.Common.Model;
using Adan.Client.Plugins.GroupWidget.Model;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.Plugins.GroupWidget
{
    /// <summary>
    /// 
    /// </summary>
    public class MonstersManager
    {
        private readonly MonstersWidgetControl _monstersWidget;
        private readonly Dictionary<string, MonsterHolder> _monsterHolders;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="monstersWidget"></param>
        public MonstersManager([NotNull] MonstersWidgetControl monstersWidget)
        {
            Assert.ArgumentNotNull(monstersWidget, "monstersWidget");

            _monstersWidget = monstersWidget;
            _monsterHolders = new Dictionary<string, MonsterHolder>();
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

            _monsterHolders.Add(uid, new MonsterHolder(this, rootModel, uid));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        public void OutputWindowClosed([NotNull] string uid)
        {
            Assert.ArgumentNotNullOrWhiteSpace(uid, "uid");

            if (_monsterHolders.ContainsKey(uid))
                _monsterHolders.Remove(uid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        public void OutputWindowChanged([NotNull] string uid)
        {
            Assert.ArgumentNotNullOrWhiteSpace(uid, "uid");

            if (_monsterHolders.ContainsKey(uid))
            {
                _monstersWidget.ViewModelUid = uid;
                _monstersWidget.UpdateModel(_monsterHolders[uid].RootModel, _monsterHolders[uid].Characters);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="monsterHolder"></param>
        public void UpdateMonsters([NotNull] MonsterHolder monsterHolder)
        {
            Assert.ArgumentNotNull(monsterHolder, "monsterHolder");

            if (_monstersWidget.ViewModelUid == monsterHolder.Uid)
            {
                _monstersWidget.UpdateModel(monsterHolder.RootModel, monsterHolder.Characters);
            }
        }
    }
}
