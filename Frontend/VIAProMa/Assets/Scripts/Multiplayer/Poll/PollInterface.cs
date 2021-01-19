using Photon.Pun;
using HoloToolkit.Unity;
using i5.VIAProMa.UI;
using i5.VIAProMa.Utilities;
using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace i5.VIAProMa.Multiplayer.Poll
{
    public class PollInterface : Singleton<PollInterface>
    {
		[Header("Poll Creation UI")]
        [SerializeField] private GameObject pollCreationPanel;
        [SerializeField] private TextMeshPro questionInput;
        [SerializeField] private List<TextMeshPro> answerInputs;
        [SerializeField] private TextMeshPro timerInput;
		
		[Header("Poll Selection UI")]
        [SerializeField] private GameObject pollSelectionPanel;
        [SerializeField] private TextMeshPro questionLabel;
        [SerializeField] private List<GameObject> answerButtons;
//        [SerializeField] private List<GameObject> answerToggles; // Later

        public bool EnterIncomingPolls { get; set; } = true;

		// State
		private bool started = false;
		private bool responded = false;
		// Currently running poll
		private PollStartEventArgs pollArgs;
		// Current selection from UI
		private bool[] curSelection;
		// Currently running created poll
		private Poll createdPoll;

        protected override void Awake()
        {
            base.Awake();
/* TODO
            if (notificationWidget == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(notificationWidget));
            }
*/
			pollCreationPanel.SetActive(false);
			pollSelectionPanel.SetActive(false);
        }

        private void Start()
        {
            PollHandler.Instance.PollStarted += OnPollStarted;
            PollHandler.Instance.PollEnded += OnPollEnded;
            HidePollInterface();
			started = responded = false;
        }

        protected override void OnDestroy()
        {
            if (PollHandler.Instance != null)
            {
                PollHandler.Instance.PollStarted -= OnPollStarted;
                PollHandler.Instance.PollEnded -= OnPollEnded;
            }
            base.OnDestroy();
        }

        public void ShowCreationInterface()
        {
			// Update interface
			questionInput.text = "";
			for (int i = 0; i < answerButtons.Count && i < pollArgs.Answers.Length; i++)
				answerInputs[i].text = "";
			// Show interface
			gameObject.SetActive(true);
			pollCreationPanel.SetActive(true);
			pollSelectionPanel.SetActive(false);
        }

		private void SendCreationRequest()
		{
			int seconds;
			if (Int32.TryParse(timerInput.text, out seconds))
			{			
				createdPoll = new Poll(questionInput.text, answerInputs.Where(i => !String.IsNullOrEmpty(i.text)).Select(i => i.text).ToArray(), PollOptions.None, DateTime.Now.AddSeconds(seconds));
			}
			else 
			{
				Debug.LogError("Failed to parse timer input!");

			}
		}

        public void ShowPollInterface(PollStartEventArgs poll)
        {
			// Update interface
			questionLabel.text = poll.Question;
			for (int i = 0; i < answerButtons.Count && i < poll.Answers.Length; i++)
			{ // Setup answers
				answerButtons[i].SetActive(true);
				answerButtons[i].GetComponent<TextMeshPro>().text = poll.Answers[i]; // Don't mind the GetComponent there
			}
			for (int i = poll.Answers.Length; i < answerButtons.Count; i++)
				answerButtons[i].SetActive(false);
			// Show interface
			gameObject.SetActive(true);
			pollCreationPanel.SetActive(false);
			pollSelectionPanel.SetActive(true);
        }

        public void HidePollInterface()
        {
			questionLabel.text = "";
            gameObject.SetActive(false);
        }

		private void SendResponse()
		{
			PollHandler.Instance.RespondPoll(curSelection, pollArgs.MessageSender);
		}

		/**
		 * Event called from buttons/toggles 
		 */
		public void OnSelectOption(int option)
		{
			curSelection[option] = !curSelection[option];
		}

		/**
		 * Event called from menu to create a new poll 
		 */
		public void OnCreatePoll()
		{
			if (started)
			{
				// Nope
			}
			else 
			{
				ShowCreationInterface();
			}
		}

		/**
		 * Event called from menu to submit the newly created poll
		 */
		public void OnSubmitPoll()
		{
			// TODO: read in 
		}

        private void OnPollStarted(object sender, PollStartEventArgs e)
        {
			if (!EnterIncomingPolls) return;
			if (createdPoll != null && e.MessageSender != PhotonNetwork.LocalPlayer)
			{
				
			}
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
			createdPoll = null;
			if (started && !responded)
				SendResponse();
			started = responded = false;
			HidePollInterface();
        }
    }
}
