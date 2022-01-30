using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using HoloToolkit.Unity;
using i5.VIAProMa.UI;
using System.Text.RegularExpressions;
using ConfigLoadData;
//using System.Xml;

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
        /// <returns>Room name.</returns>
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
                WindowManager.Instance.ConfirmationMenu.Open(Camera.main.transform.position + 0.6f * Camera.main.transform.forward, Camera.main.transform.eulerAngles, roomname);
            }
            else
            {
                bool wasJoined = PhotonNetwork.JoinOrCreateRoom(roomname, null, null);
                Debug.Log("The room was" + (wasJoined ? "" : " not") + " joined." + (wasJoined ? "" :
                    "\n This may indicate that there was no such room."));
            }
        }

        /// <summary>
        /// Method to read setting from AppSettings
        /// <param name="key">Key to read.</param>
        /// <returns>Corresponding value or error string.</returns>
        /// </summary>
        /*public static string ReadSetting(string key)
        {
            XmlDocument config = new XmlDocument();
            config.Load("./Assets/Scripts/DeepLinks/DeepLinkConfig.config");
            XmlNode node = config.SelectSingleNode("configuration/configSections/" + key);
            return node.InnerText;
        }
        */
        /// <summary>
        /// Generates an invite link for the current session
        /// <returns>Invite link.</returns>
        /// </summary>
        public static string GenerateInviteLink()
        {

            string inviteURI = LoadData.LoadConfig().scheme + "://" + LoadData.LoadConfig().invitePath;
            string paramName_roomName = LoadData.LoadConfig().paramName_roomName;


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