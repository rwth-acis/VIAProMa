using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace i5.VIAProMa.DeepLinks.InviteLinks
{
    /// <summary>
    /// Generates an invite link to the currently active session
    /// </summary>
    public class InviteLinkGenerator : MonoBehaviour
    {

        /// TODO Invite URI config, for testing purposed only, will be relocated later
        string inviteURI = "i5://invite";
        string paramName_roomName = "roomName";


        /// <summary>
        /// Generates an invite link for the current session
        /// </summary>
        public string GenerateInviteLink()
        {

            Room currentRoom = PhotonNetwork.CurrentRoom;
            
            if (currentRoom == null)
            {
                Debug.Log("No session is active, Link can not be generated");
                return "Link can not be generated";
            }

            string roomName = currentRoom.Name;
            return inviteURI + "?" + paramName_roomName + "=" + roomName;
        }

        public void TestLinkGeneration()
        {
            Debug.Log("Generated Invite Link: " + GenerateInviteLink());
        }
    }
}
