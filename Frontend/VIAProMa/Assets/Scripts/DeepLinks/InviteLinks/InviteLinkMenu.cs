using System;
using UnityEngine;
using i5.VIAProMa.UI;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.DeepLinks.InviteLinks;
using Photon.Pun;
using TMPro;
using i5.VIAProMa.Multiplayer;

namespace i5.VIAProMa.DeepLinks.InviteLinks
{
    /// <summary>
    /// Class of the Invite Link Menu / Room Options Menu
    /// </summary>
    public class InviteLinkMenu : MonoBehaviour, IWindow
    {

        [Header("UI Elements")]
        [SerializeField] private TextMeshPro linkTextfield;
        [SerializeField] private TextMeshPro feedbackText;


        public bool WindowEnabled { get; set; } // not used here

        public bool WindowOpen { get; private set; }

        public event EventHandler WindowClosed;

        private string curInviteLink;

        private void Awake()
        {
            if (linkTextfield == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(linkTextfield));
            }
            if (feedbackText == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(feedbackText));
            }
        }

        public void OnEnable()
        {
            curInviteLink = InviteLinksHandler.GenerateInviteLink();
            linkTextfield.text = curInviteLink;
            feedbackText.text = "";

        }

        /// <summary>
        /// Opens the window by making the GameObject active
        /// </summary>
        public void Open()
        {
            LobbyManager.Instance.LobbyJoinStatusChanged += (s, e) =>
            {
                if(PhotonNetwork.CurrentRoom == null)
                {
                    this.Close();
                }
            };
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
        /// Copys the invite link into the clipboard
        /// </summary>
        public void CopyLinkToClipboard() 
        {
            GUIUtility.systemCopyBuffer = curInviteLink;
            feedbackText.text = "Link copied to clipboard!";
        }


        /// <summary>
        /// Opens the users default mail client with a share mail template
        /// </summary>
        public void ShareLinkMail()
        {
            string mailDefaultSubject = "Come join my ViaProMa session!";
            string mailDefaultBody = $"Hello, {Environment.NewLine} I'd like to invite you to my current ViaProMa session! {Environment.NewLine} Just open the link below: {Environment.NewLine}" + curInviteLink + $"{Environment.NewLine} Best regards!";

            string mailUrl = "mailto:?to=&subject=" + EncodeMailtoURL(mailDefaultSubject) +
                "&body=" + EncodeMailtoURL(mailDefaultBody);

            Application.OpenURL(mailUrl);
            feedbackText.text = "Check your e-mail client!";

        }

        /// <summary>
        /// Gets string to be used in mailto and converts it to encoded url string.
        /// </summary>
        /// <param name="url">The text to URL-encode</param>
        /// <returns>A URL-encoded string.</returns>
        public static string EncodeMailtoURL(string url)
        {
            url = System.Net.WebUtility.UrlEncode(url);
            url = url.Replace("+", "%20");
            return url;
        }

        /// <summary>
        /// Leaves the Photonroom.
        /// </summary>
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            this.Close();
        }

        
    }
}


