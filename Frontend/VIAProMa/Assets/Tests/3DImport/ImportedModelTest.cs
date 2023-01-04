using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ImportedModelTest : MonoBehaviourPunCallbacks
{
	public override void OnJoinedLobby()
	{
        PhotonNetwork.CreateRoom("", new Photon.Realtime.RoomOptions{ MaxPlayers = 1 });
    }

	public override void OnJoinedRoom()
    {
		object[] InstantiationData = new object[] {(object)"http://www.nikita-zaloga.de/undertale-_low_poly.glb"};
		PhotonNetwork.Instantiate("ImportedModel", new Vector3(0, 0, 0), Quaternion.identity, 0, InstantiationData);
    }
}
