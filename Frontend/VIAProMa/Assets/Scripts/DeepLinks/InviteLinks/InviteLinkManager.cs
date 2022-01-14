using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;

namespace i5.VIAProMa.DeepLinks.InviteLinks
{
    /// <summary>
    /// Invite link manager which handles everything related to invite links at runtime
    /// </summary>
    public class InviteLinkManager : Singleton<InviteLinkManager>
    {
        /// TODO Invite URI config, for testing purposed only, will be relocated later
        static string inviteURI = "i5://invite";
        static string paramName_roomName = "roomName";


        /// <summary>
        /// Generates an invite link for the current session
        /// </summary>
        public static string GenerateInviteLink()
        {

            Room currentRoom = PhotonNetwork.CurrentRoom;

            if (currentRoom == null)
            {
                Debug.Log("No session is active, Link can not be generated");
                return "Link can not be generated";
            }

            string roomName = currentRoom.Name;
            Regex rgx = new Regex("[^a-zA-Z0-9_]");
            roomName = rgx.Replace(roomName, "");




            return inviteURI + "?" + paramName_roomName + "=" + roomName;
        }
    }

    
}

