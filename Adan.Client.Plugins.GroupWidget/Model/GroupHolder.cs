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
    public class GroupHolder
    {
        private readonly GroupManager _groupManager;
        private readonly RootModel _rootModel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupManager"></param>
        /// <param name="rootModel"></param>
        /// <param name="uid"></param>
        public GroupHolder([NotNull] GroupManager groupManager, [NotNull] RootModel rootModel, string uid)
        {
            Assert.ArgumentNotNull(groupManager, "groupManager");
            Assert.ArgumentNotNull(rootModel, "rootModel");

            _groupManager = groupManager;
            _rootModel = rootModel;
            Uid = uid;
            Characters = new List<CharacterStatus>();

            rootModel.MessageConveyor.MessageReceived += MessageConveyor_MessageReceived;
            rootModel.MessageConveyor.OnDisconnected += MessageConveyor_OnDisconnected;
        }

        private void MessageConveyor_OnDisconnected(object sender, EventArgs e)
        {
            Characters = new List<CharacterStatus>();
            _groupManager.UpdateGroup(this);
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
        public List<CharacterStatus> Characters
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
            if (e.Message.MessageType == Constants.GroupStatusMessageType)
            {
                var groupMessage = e.Message as GroupStatusMessage;

                Characters = groupMessage.GroupMates;
                _groupManager.UpdateGroup(this);
            }
        }
    }
}
