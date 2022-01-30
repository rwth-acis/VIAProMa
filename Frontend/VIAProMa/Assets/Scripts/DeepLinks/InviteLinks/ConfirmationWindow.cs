using i5.VIAProMa.Multiplayer;
using i5.VIAProMa.UI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.DeepLinks.InviteLinks
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfirmationWindow : MonoBehaviourPunCallbacks, IWindow
    {
        public string RoomName { get; private set; }
        public bool WindowEnabled { get; set; } // not used here

        public bool WindowOpen { get; private set; }

        public event EventHandler WindowClosed;


        private void Awake()
        {

        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        /// <summary>
        /// Opens the window by making the GameObject active
        /// </summary>
        public void Open()
        {
            gameObject.SetActive(true);
            WindowOpen = true;
        }

        /// <summary>
        /// Opens the window at <paramref name="position"/> with an angle of <paramref name="eulerAngles"/>.
        /// </summary>
        /// <param name="position">Position of the window.</param>
        /// <param name="eulerAngles">Angle of the window.</param>
        public void Open(Vector3 position, Vector3 eulerAngles)
        {
            Open();
            transform.localPosition = position;
            transform.localEulerAngles = eulerAngles;
        }
        /// <summary>
        /// Opens the window at <paramref name="position"/> with an angle of <paramref name="eulerAngles"/>
        /// to confirm if room <paramref name="roomName"/> should be joined.
        /// </summary>
        /// <param name="position">Position of the window.</param>
        /// <param name="eulerAngles">Angle of the window.</param>
        /// <param name="roomName">Name of room to join</param>
        public void Open(Vector3 position, Vector3 eulerAngles, string roomName)
        {
            RoomName = roomName;
            Open();
            transform.localPosition = position;
            transform.localEulerAngles = eulerAngles;
        }

        /// <summary>
        /// Closes the window and raises the WindowClosed event
        /// Deactivates the GameObject (so the window still exists but is invisible)
        /// </summary>
        public void Close()
        {
            WindowOpen = false;
            WindowClosed?.Invoke(this, EventArgs.Empty);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Joins the room to confirm
        /// </summary>
        public void JoinRoom()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.Log("Photon is disconnected.");
                return;
            }
            if (RoomName != null)
            {
                if(PhotonNetwork.CurrentRoom == null)
                {

                    PhotonNetwork.JoinOrCreateRoom(RoomName, null, null);
                }
                else
                {
                    LobbyManager.Instance.LobbyJoinStatusChanged += RoomLeftAfterWait;
                    PhotonNetwork.LeaveRoom();
                }
            }
            else
            {
                Debug.Log("Roomname was null, when confirming");
            }
            this.Close();
        }

        /// <summary>
        /// Method that subscribes to changes in Room, to join the room when the previous is left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RoomLeftAfterWait(object sender, EventArgs e)
        {
            PhotonNetwork.JoinOrCreateRoom(RoomName, null, null);
            LobbyManager.Instance.LobbyJoinStatusChanged -= RoomLeftAfterWait;
        }
    }
}
