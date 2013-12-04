using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adan.Client.Common.Model;
using Adan.Client.Map.Messages;
using CSLib.Net.Annotations;

namespace Adan.Client.Map.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class ZoneHolder
    {
        private RootModel _rootModel;
        private ZoneManager _zoneManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoneManager"></param>
        /// <param name="rootModel"></param>
        /// <param name="uid"></param>
        public ZoneHolder(ZoneManager zoneManager, [NotNull] RootModel rootModel, string uid)
        {
            _zoneManager = zoneManager;
            _rootModel = rootModel;
            Uid = uid;
            ZoneId = -1;
            RoomId = -1;

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
        public int RoomId
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ZoneId
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

        private void MessageConveyor_MessageReceived(object sender, Common.Conveyor.MessageReceivedEventArgs e)
        {
            if (e.Message.MessageType == Constants.CurrentRoomMessageType)
            {
                var mapMessage = e.Message as CurrentRoomMessage;
                RoomId = mapMessage.RoomId;
                ZoneId = mapMessage.ZoneId;

                _zoneManager.UpdateControl(this);
            }
        }
    }
}
