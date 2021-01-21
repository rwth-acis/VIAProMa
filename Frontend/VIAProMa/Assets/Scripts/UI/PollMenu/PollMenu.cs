using Photon.Pun;
using HoloToolkit.Unity;
using i5.VIAProMa.UI;
using i5.VIAProMa.Utilities;
using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using i5.VIAProMa.Multiplayer.Poll;
using i5.VIAProMa.UI.InputFields;

namespace i5.VIAProMa.UI.Poll
{
    public class PollMenu : MonoBehaviour, IWindow
    {
        [Header("Poll Creation UI")]
        [SerializeField] private GameObject pollCreationPanel;
        [SerializeField] private InputField questionInput;
        [SerializeField] private List<InputField> answerInputs;

        [Header("Poll Options UI")]
        [SerializeField] private GameObject pollOptionsPanel;
        [SerializeField] private InputField timerInputMinutes;
        [SerializeField] private InputField timerInputSeconds;

        [Header("Poll Selection UI")]
        [SerializeField] private GameObject pollSelectionPanel;
        [SerializeField] private TextMeshPro questionLabel;
        [SerializeField] private List<GameObject> answerButtons;
//        [SerializeField] private List<GameObject> answerToggles; // Later

        public bool EnterIncomingPolls { get; set; } = true;

        public bool WindowEnabled // not used
        {
            get; set;
        }

        public bool WindowOpen
        {
            get
            {
                return gameObject.activeSelf;
            }
        }

        public event EventHandler WindowClosed;


        // State
        private bool started = false;
        private bool responded = false;
        // Currently running poll
        private PollStartEventArgs pollArgs;
        // Current selection from UI
        private bool[] curSelection;
        // Currently running created poll
        private i5.VIAProMa.Multiplayer.Poll.Poll createdPoll;

        protected void Awake()
        {
/* TODO
            if (notificationWidget == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(notificationWidget));
            }
*/

            started = responded = false;

            pollCreationPanel.SetActive(false);
            pollOptionsPanel.SetActive(false);
            pollSelectionPanel.SetActive(false);

			// Setup own watchdog even if disabled
            PollHandler.Instance.PollStarted += OnPollStarted;
            PollHandler.Instance.PollEnded += OnPollEnded;
        }
	

        protected void OnDestroy()
        {
            if (PollHandler.Instance != null)
            {
                PollHandler.Instance.PollStarted -= OnPollStarted;
                PollHandler.Instance.PollEnded -= OnPollEnded;
            }
        }

        public void Open()
        {
            ShowCreationInterface();
        }

        public void Open(Vector3 position, Vector3 eulerAngles)
        {
            Open();
            transform.position = position;
            transform.eulerAngles = eulerAngles;
        }

        public void Close()
        {
			PollClear();
            HidePollInterface();
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }

        public void ShowCreationInterface()
        {
            // Update interface
            questionInput.Text = "";
            for (int i = 0; i < answerButtons.Count; i++)
                answerInputs[i].Text = "";
			timerInputMinutes.Text = "0";
            timerInputSeconds.Text = "0";
            // Show interface
            gameObject.SetActive(true);
            pollCreationPanel.SetActive(true);
            pollOptionsPanel.SetActive(false);
            pollSelectionPanel.SetActive(false);
        }

        public void ShowPollInterface(PollStartEventArgs poll)
        {
            // Update interface
            questionLabel.text = poll.Question;
            for (int i = 0; i < answerButtons.Count && i < poll.Answers.Length; i++)
            { // Setup answers
                answerButtons[i].SetActive(true);
                answerButtons[i].GetComponentInChildren<TextMesh>().text = poll.Answers[i]; // Don't mind the GetComponent there
            }
            for (int i = poll.Answers.Length; i < answerButtons.Count; i++)
                answerButtons[i].SetActive(false);
            // Show interface
            gameObject.SetActive(true);
            pollCreationPanel.SetActive(false);
            pollOptionsPanel.SetActive(false);
            pollSelectionPanel.SetActive(true);
        }

        public void HidePollInterface()
        {
            gameObject.SetActive(false);
            pollCreationPanel.SetActive(false);
            pollOptionsPanel.SetActive(false);
            pollSelectionPanel.SetActive(false);
        }

        private void SendCreationRequest()
        {
            int minutes, seconds;
            if (!Int32.TryParse(timerInputMinutes.Text, out minutes))
            {
                Debug.LogError("Failed to parse countdown minutes input!");
				return;
			}
            if (!Int32.TryParse(timerInputSeconds.Text, out seconds))
            {
                Debug.LogError("Failed to parse countdown seconds input!");
				return;
			}
			createdPoll = new i5.VIAProMa.Multiplayer.Poll.Poll(questionInput.Text, answerInputs.Where(i => !String.IsNullOrEmpty(i.Text)).Select(i => i.Text).ToArray(), PollOptions.None, DateTime.Now.AddMinutes(minutes).AddSeconds(seconds));
        }

        private void SendResponse()
        {
            PollHandler.Instance?.RespondPoll(curSelection, pollArgs.MessageSender);
        }

		private void PollClear()
		{
			if (started && !responded)
				PollHandler.Instance?.SendNAK(pollArgs.MessageSender);
            started = responded = false;
			createdPoll = null;
			pollArgs = null;
		}

        /**
         * Event called from buttons/toggles 
         */
        public void OnSelectOption(int option)
        {
            curSelection[option] = !curSelection[option];
        }

        /**
         * Event called from poll creation to advance to poll options
         */
        public void OnPollNext()
        {
            pollCreationPanel.SetActive(false);
            pollOptionsPanel.SetActive(true);
        }

        /**
         * Event called from poll options to return to poll creation
         */
        public void OnPollPrevious()
        {
            pollCreationPanel.SetActive(true);
            pollOptionsPanel.SetActive(false);
        }

        /**
         * Event called from menu to create the new poll
         */
        public void OnPollCreate()
        {
			SendCreationRequest();
			HidePollInterface();
        }

        /**
         * Event called from menu to submit the poll selection
         */
        public void OnPollSubmit()
        {
			SendResponse();
			HidePollInterface();
        }

        private void OnPollStarted(object sender, PollStartEventArgs e)
        {
            if (!EnterIncomingPolls) return;
            if (createdPoll != null && e.MessageSender != PhotonNetwork.LocalPlayer)
            {
                Debug.LogError("Crashing my party, aren't ya? Not with me!");
				return;
            }
			Debug.Log("Opening Poll!");
            started = true;
            responded = false;
            curSelection = new bool[e.Answers.Length];
            pollArgs = e;
            ShowPollInterface(e);
        }

        private void OnPollEnded(object sender, PollEndEventArgs e)
        {
            if (e.MessageSender != pollArgs.MessageSender)
            {
                Debug.LogError("Received Poll End request from somebody other than initiator!");
                return;
            }
			Debug.Log("Close Poll!");
            PollClear();
            HidePollInterface();
        }
    }
}
