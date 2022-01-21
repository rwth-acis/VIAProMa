using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;
using System.Xml;

namespace i5.VIAProMa.DeepLinks.InviteLinks
{
    /// <summary>
    /// Invite link manager which handles everything related to invite links at runtime
    /// </summary>
    public class InviteLinkManager : Singleton<InviteLinkManager>
    {
        /// <summary>
        /// Method to read setting from AppSettings
        /// <param name="key">Key to read.</param>
        /// <returns>Corresponding value or error string.</returns>
        /// </summary>
        public static string ReadSetting(string key)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("./Assets/Scripts/DeepLinks/App.config");
            XmlNode node = doc.SelectSingleNode("configuration/configSections/"+key);
            return node.InnerText;
        }

        /// <summary>
        /// Generates an invite link for the current session
        /// <returns>Invite link.</returns>
        /// </summary>
        public static string GenerateInviteLink()
        {
            string inviteURL = ReadSetting("inviteURL");
            string paramName_roomName = ReadSetting("paramName_roomName");


            Room currentRoom = PhotonNetwork.CurrentRoom;

            if (currentRoom == null)
            {
                Debug.Log("No session is active, Link can not be generated");
                return "Link can not be generated";
            }

            string roomName = currentRoom.Name;
            Regex rgx = new Regex("[^a-zA-Z0-9_]");
            roomName = rgx.Replace(roomName, "");




            return inviteURL + "?" + paramName_roomName + "=" + roomName;
        }
    }

    
}