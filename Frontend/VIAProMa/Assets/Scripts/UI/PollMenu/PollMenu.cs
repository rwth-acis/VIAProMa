using Photon.Pun;
using HoloToolkit.Unity;
using i5.VIAProMa.UI;
using i5.VIAProMa.Utilities;
using TMPro;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using i5.VIAProMa.Multiplayer.Poll;
using i5.VIAProMa.UI.InputFields;
using Microsoft.MixedReality.Toolkit.UI;

namespace i5.VIAProMa.UI.Poll
{
    /**
     * Local Poll UI menu, listens to Poll events and calls into PollHandler for user interaction
     */
    public class PollMenu : MonoBehaviour, IWindow
    {
        [Header("Poll Creation UI")]
        [SerializeField] private GameObject pollCreationPanel;
        [SerializeField] private InputField questionInput;
        [SerializeField] private List<InputField> answerInputs;

        [Header("Poll Options UI")]
        [SerializeField] private GameObject pollOptionsPanel;
        [SerializeField] private Interactable countdownToggle;
        [SerializeField] private InputField timerInputMinutes;
        [SerializeField] private InputField timerInputSeconds;
        [SerializeField] private Interactable multipleChoiceToggle;
        [SerializeField] private Interactable saveResultToggle;

        [Header("Poll Selection UI")]
        [SerializeField] private GameObject pollSelectionPanel;
        [SerializeField] private TextMeshPro questionLabel;
        [SerializeField] private TextMeshPro countdownLabel;
        [SerializeField] private GameObject multipleChoicePanel;
        [SerializeField] private List<Interactable> answerToggles;
        [SerializeField] private GameObject singleChoicePanel;
        [SerializeField] private List<Interactable> answerButtons;
        [Header("Poll Loading Shelf")]
        [SerializeField] private GameObject pollShelfPrefab;

        public bool EnterIncomingPolls { get; set; } = true;

        public bool WindowEnabled { get; set; }

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
        private bool createdPoll;
        // Local Countdown
        private IEnumerator countdown;

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

        private void ShowCreationInterface()
        {
            // Update interface
            questionInput.Text = "";
            for (int i = 0; i < answerInputs.Count; i++)
                answerInputs[i].Text = "";
            timerInputMinutes.Text = "1";
            timerInputSeconds.Text = "0";
            // Show interface
            gameObject.SetActive(true);
            pollCreationPanel.SetActive(true);
            pollOptionsPanel.SetActive(false);
            pollSelectionPanel.SetActive(false);
        }

        private void ShowPollInterface(PollStartEventArgs poll)
        {
            // Update interface
            questionLabel.text = poll.Question;
            
            if (poll.Flags.HasFlag(PollOptions.MultipleChoice))
            {
                multipleChoicePanel.SetActive(true);
                singleChoicePanel.SetActive(false);
                for (int i = 0; i < answerToggles.Count && i < poll.Answers.Length; i++)
                { // Setup answers
                    answerToggles[i].gameObject.SetActive(true);
                    answerToggles[i].GetComponentInChildren<TextMesh>().text = poll.Answers[i]; // Don't mind the GetComponent there
                    answerToggles[i].IsToggled = false;
                }
                for (int i = poll.Answers.Length; i < answerToggles.Count; i++)
                    answerToggles[i].gameObject.SetActive(false);
                for (int i = 0; i < answerButtons.Count; i++)
                    answerButtons[i].gameObject.SetActive(false);
            }
            else 
            {
                multipleChoicePanel.SetActive(false);
                singleChoicePanel.SetActive(true);
                for (int i = 0; i < answerButtons.Count && i < poll.Answers.Length; i++)
                { // Setup answers
                    answerButtons[i].gameObject.SetActive(true);
                    answerButtons[i].GetComponentInChildren<TextMesh>().text = poll.Answers[i]; // Don't mind the GetComponent there
                }
                for (int i = poll.Answers.Length; i < answerButtons.Count; i++)
                    answerButtons[i].gameObject.SetActive(false);
                for (int i = 0; i < answerToggles.Count; i++)
                    answerToggles[i].gameObject.SetActive(false);
            }

            // Show interface
            gameObject.SetActive(true);
            pollCreationPanel.SetActive(false);
            pollOptionsPanel.SetActive(false);
            pollSelectionPanel.SetActive(true);
            
            // Update countdown
            if (poll.Flags.HasFlag(PollOptions.Countdown))
            {
                countdownLabel.enabled = true;
                countdown = CountdownUpdater();
                StartCoroutine(countdown);
            }
            else
            {
                countdownLabel.enabled = false;
                countdown = null;
            }
        }

        private void HidePollInterface()
        {
            gameObject.SetActive(false);
            pollCreationPanel.SetActive(false);
            pollOptionsPanel.SetActive(false);
            pollSelectionPanel.SetActive(false);
            if (!(countdown is null))
                StopCoroutine(countdown);
        }

        private void SendCreationRequest()
        {
            PollOptions options = PollOptions.None;
            DateTime endTime = DateTime.Now;
            if (countdownToggle.IsToggled)
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
                endTime = endTime.AddMinutes(minutes).AddSeconds(seconds);
                options |= PollOptions.Countdown;
            }
            if (multipleChoiceToggle.IsToggled)
                options |= PollOptions.MultipleChoice;
            if (saveResultToggle.IsToggled)
            {
                options |= PollOptions.SaveResults;
            }
            PollHandler.Instance?.StartPoll(questionInput.Text, answerInputs.Where(i => !String.IsNullOrEmpty(i.Text)).Select(i => i.Text).ToArray(), options, endTime);
            createdPoll = true;
        }

        private void SendResponse()
        {
            responded = true;
            PollHandler.Instance?.RespondPoll(curSelection, pollArgs.MessageSender);
        }

        private void PollClear()
        {
            if (started && !responded)
                PollHandler.Instance?.SendNAK(pollArgs.MessageSender);
            started = responded = false;
            createdPoll = false;
            pollArgs = null;
        }

        private IEnumerator CountdownUpdater() 
        {
            while (pollArgs != null) 
            {
                TimeSpan span = pollArgs.End - DateTime.Now;
                countdownLabel.text = span.Minutes + ":" + 	span.Seconds.ToString("D2");
                yield return new WaitForSecondsRealtime(span.Milliseconds/1000.0f);
            }
        }

        /**
         * Event called from buttons/toggles 
         */
        public void OnSelectOption(int option)
        {
            for (int i = 0; i < curSelection.Length; i++)
                curSelection[i] = false;
            curSelection[option] = true;
            SendResponse();
            HidePollInterface();
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
            HidePollInterface();
            SendCreationRequest();
        }

        /**
         * Event called from menu to submit the poll selection
         */
        public void OnPollSubmit()
        {
            for (int i = 0; i < curSelection.Length && i < answerToggles.Count; i++)
                curSelection[i] = answerToggles[i].IsToggled;
            SendResponse();
            HidePollInterface();
        }

        private void OnPollStarted(object sender, PollStartEventArgs e)
        {
            if (!EnterIncomingPolls) return;
            if (createdPoll && e.MessageSender != PhotonNetwork.LocalPlayer)
            {
                Debug.LogError("Crashing my party, aren't ya? Not with me!");
                return;
            }
            Debug.Log("Opening Poll! Options: " + e.Flags);
            started = true;
            responded = false;
            curSelection = new bool[e.Answers.Length];
            pollArgs = e;
            // TODO: local countdown closing window and showing remaining time
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

        public void OnLoadPolls()
        {
            GameObject pollShelf = Instantiate(pollShelfPrefab);
            Close();
        }
    }
}
