using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the multiplayer networking logic
/// Focuses on the logic inside a room
/// </summary>
public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerAvatarPrefab;

    private void Awake()
    {
        if (playerAvatarPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(playerAvatarPrefab));
        }
    }

    /// <summary>
    /// Called when the client joins a room
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("Client is now in the room " + PhotonNetwork.CurrentRoom.Name);
        PrefabResourceCollection.NetworkInstantiate(playerAvatarPrefab, Vector3.zero, Quaternion.identity);
    }

    /// <summary>
    /// Called when the client 
    /// s to join a room
    /// </summary>
    /// <param name="returnCode">The code of the error message</param>
    /// <param name="message">The error message</param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room join failed\n" + returnCode + ": " + message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " left");
    }
}
