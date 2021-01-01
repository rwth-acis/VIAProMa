using i5.VIAProMa.UI.ListView.Core;
using Photon.Realtime;

namespace i5.VIAProMa.UI.ListView.NetworkRooms
{
    public class NetworkRoomData : IListViewItemData
    {
        public RoomInfo RoomInfo { get; private set; }

        public bool IsFull { get { return RoomInfo.MaxPlayers != 0 && RoomInfo.PlayerCount == RoomInfo.MaxPlayers; } }

        public NetworkRoomData(RoomInfo roomInfo)
        {
            RoomInfo = roomInfo;
        }
    }
}