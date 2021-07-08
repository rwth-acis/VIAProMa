using i5.VIAProMa.Multiplayer.Chat;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer
{
    /// <summary>
    /// The launcher's task is to connect to the Photon services and to join a lobby
    /// This script focuses on monitoring the connection and joining the lobby if necessary
    /// Lobby Management can be found in the LobbyManager class and network logic inside a room is handled by MultiplayerManager
    /// </summary>
    public class Launcher : MonoBehaviourPunCallbacks
    {
        string gameVersion = "1"; // this will be updated if breaking changes are made to the network behavior of the client

        public static Launcher Instance { get; private set; } // this is a singleton, but we cannot use the singleton class since it also inherits from PunCallbacks

        public event EventHandler ConnectionStatusChanged;

        /// <summary>
        /// Handles the initialization
        /// </summary>
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Multiple instances of " + nameof(Launcher));
            }
            Instance = this;
        }

        /// <summary>
        /// Initiates a connection attempt to the server
        /// </summary>
        private void Start()
        {
            Connect(); // try to connect to the server
        }

        /// <summary>
        /// Connects to the server
        /// If the connection already exists, we proceed and go to the lobby
        /// </summary>
        private void Connect()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
            {
                PhotonNetwork.JoinLobby();
            }
            else
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
                Debug.Log("now the offline mode is " + PhotonNetwork.OfflineMode);
                PhotonNetwork.NickName = "Guest" + UnityEngine.Random.Range(0, 1000);
            }
        }

        /// <summary>
        /// Called when the client has connected to the server
        /// Client can now proceed and join a lobby
        /// </summary>
        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster()");
            ConnectionStatusChanged?.Invoke(this, EventArgs.Empty);
            if (!PhotonNetwork.OfflineMode)
            {
                PhotonNetwork.JoinLobby();
            }
            else
            {
                PhotonNetwork.JoinRoom("offline");
            }
        }

        /// <summary>
        /// Called when the client disconnects from the server
        /// </summary>
        /// <param name="cause">The cause for the disconnect</param>
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("OnDisconnected()" + cause.ToString());
            ConnectionStatusChanged?.Invoke(this, EventArgs.Empty);
            if (cause != DisconnectCause.DisconnectByClientLogic)
            {
                ChatManager.Instance.AddLocalMessage("Lost connection (" + cause.ToString() + ")");
            }
        }

        /// <summary>
        /// Called when the client joins the lobby
        /// </summary>
        public override void OnJoinedLobby()
        {
            Debug.Log("Client is now in the lobby");
        }

        /// <summary>
        /// Called when the client leaves the lobby
        /// </summary>
        public override void OnLeftLobby()
        {
            Debug.Log("Client left the lobby");
        }
    }
}