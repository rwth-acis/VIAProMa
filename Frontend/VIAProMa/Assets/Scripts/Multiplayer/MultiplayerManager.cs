﻿using i5.VIAProMa.Multiplayer.Chat;
using i5.VIAProMa.ResourceManagagement;
using i5.VIAProMa.Utilities;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer
{
    /// <summary>
    /// Handles the multiplayer networking logic
    /// Focuses on the logic inside a room
    /// </summary>
    public class MultiplayerManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject playerAvatarPrefab;

        /// <summary>
        /// Checks if the component is set up correctly
        /// </summary>
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
            ResourceManager.Instance.NetworkInstantiate(playerAvatarPrefab, Vector3.zero, Quaternion.identity);
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

        /// <summary>
        /// Called by Photon if another person enters the room that the client is currently in
        /// </summary>
        /// <param name="newPlayer"></param>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log(newPlayer.NickName + " joined");
            ChatManager.Instance.AddLocalMessage(newPlayer.NickName + " joined the room.");
        }

        /// <summary>
        /// Called by Photon if another person leaves the room that the client is currently in
        /// </summary>
        /// <param name="otherPlayer"></param>
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log(otherPlayer.NickName + " left");
            ChatManager.Instance.AddLocalMessage(otherPlayer.NickName + " left the room.");
        }
    }
}