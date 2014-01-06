using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adan.Client.Common.Conveyor;
using Adan.Client.Common.Model;
using Adan.Client.Plugins.GroupWidget.Messages;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.Plugins.GroupWidget.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class MonsterHolder
    {
        private readonly MonstersManager _monsterManager;
        private readonly RootModel _rootModel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="monsterManager"></param>
        /// <param name="rootModel"></param>
        public MonsterHolder([NotNull] MonstersManager monsterManager, [NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(monsterManager, "monsterManager");
            Assert.ArgumentNotNull(rootModel, "rootModel");

            _monsterManager = monsterManager;
            _rootModel = rootModel;
            Uid = _rootModel.Uid;
            Characters = new List<MonsterStatus>();

            rootModel.MessageConveyor.MessageReceived += MessageConveyor_MessageReceived;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Uid
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<MonsterStatus> Characters
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public RootModel RootModel
        {
            get
            {
                return _rootModel;
            }
        }

        private void MessageConveyor_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Message.MessageType == Constants.RoomMonstersMessage)
            {
                var monsterMessage = e.Message as RoomMonstersMessage;

                Characters = monsterMessage.Monsters;
                _monsterManager.UpdateMonsters(this);
            }
        }
    }
}
