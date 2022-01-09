using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using HoloToolkit.Unity;

namespace i5.VIAProMa.DeepLinks
{
    /// <summary>
    /// Invite Link Handler which handles the reception of Invite Links at runtime
    /// </summary>
    public class InviteLinksHandler : Singleton<InviteLinksHandler>
    { 
        /// <summary>
        /// Gets room name from deep link parameters.
        /// </summary>
        /// <param name="parameters">Deep link parameters.</param>
        /// <returns></returns>
        private string DecodeLink(Dictionary<string, string> parameters)
        {
            Debug.Log("The deep link does" + (parameters.ContainsKey("roomName") ? "" : " not") + " contain a room name.");
            if (!parameters.ContainsKey("roomName"))
            {
                Debug.Log("Deep link does not contain valid room name.");
                return null;
            }
            else
            {
                return parameters["roomName"];
            }

        }

        /// <summary>
        /// Joins a room given in deep link parameters.
        /// </summary>
        /// <param name="parameters">Deep link parameters.</param>
        public void JoinByDeepLink(Dictionary<string, string> parameters)
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.Log("PhotonNetwork is not connected. Cannot process DeepLink right now.");
                return; 
            }
            string roomname = DecodeLink(parameters);
            if(roomname == null)
            {
                return;
            }
            Debug.Log("The room name contained in the invite link is: " + roomname);
            if(PhotonNetwork.CurrentRoom != null)
            {
                Debug.Log("The user was already in a room and will automatically leave the room now.");
                // TODO: Add pop-up with join-option here.
                PhotonNetwork.LeaveRoom();
            }
            bool wasJoined = PhotonNetwork.JoinOrCreateRoom(roomname, null, null);
            Debug.Log("The room was" + (wasJoined ? "" : " not") + " joined." + (wasJoined ? "" :
                "\n This may indicate that there was no such room."));
        }
    }
}
