using Photon.Pun;
using Photon.Realtime;
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
    /// <summary>
	/// Local Poll UI menu, listens to Poll events and calls into PollHandler for user interaction
    /// Stores the state of the currently running poll as well as whether it has been started by this user   
	/// </summary>
    public class PollMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject pollMasterControlPanel;
        [SerializeField] private GameObject pollWaitingPanel;

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
        [SerializeField] private Interactable publicToggle;
        [SerializeField] private Interactable realtimeVizToggle;

        [Header("Poll Selection UI")]
        [SerializeField] private GameObject pollSelectionPanel;
        [SerializeField] private TextMeshPro questionLabel;
        [SerializeField] private TextMeshPro countdownLabel;
        [SerializeField] private GameObject multipleChoicePanel;
        [SerializeField] private List<Interactable> answerToggles;
        [SerializeField] private GameObject singleChoicePanel;
        [SerializeField] private List<Interactable> answerButtons;
        [SerializeField] private GameObject publicIcon;
        [SerializeField] private GameObject saveIcon;

        // IWindow implementation
        public bool WindowEnabled { get; set; } // not used here
        public bool WindowOpen { get { return gameObject.activeSelf; }}
        public event EventHandler WindowClosed;

        /* State */

        // Retains whether a poll is currently active
        private bool started = false;
        // Retains whether the active poll has been responded (either with a NAK, e.g. not participating, or an actual response)
        private bool responded = false;
        // Stores the circumstances by which the active poll has been started
        private PollStartEventArgs pollArgs;
        // Current selection from UI
        private bool[] curSelection;
        // Retains whether a poll has been created (might be on before the active poll has been set)
        private bool createdPoll;
        // Local Countdown for the active poll, just to update UI, not to end the poll
        private IEnumerator countdown;

        private void Awake()
        {
            if (pollMasterControlPanel == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(pollMasterControlPanel));
            if (pollWaitingPanel == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(pollWaitingPanel));
            if (pollCreationPanel == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(pollCreationPanel));
            if (questionInput == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(questionInput));
            if (answerInputs.Count == 0 || answerInputs.Any(a => a == null)) SpecialDebugMessages.LogMissingReferenceError(this, nameof(answerInputs));
            if (pollOptionsPanel == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(pollOptionsPanel));
            if (countdownToggle == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(countdownToggle));
            if (timerInputMinutes == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(timerInputMinutes));
            if (timerInputSeconds == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(timerInputSeconds));
            if (multipleChoiceToggle == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(multipleChoiceToggle));
            if (saveResultToggle == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(saveResultToggle));
            if (publicToggle == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(publicToggle));
            if (realtimeVizToggle == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(realtimeVizToggle));
            if (pollSelectionPanel == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(pollSelectionPanel));
            if (questionLabel == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(questionLabel));
            if (countdownLabel == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(countdownLabel));
            if (multipleChoicePanel == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(multipleChoicePanel));
            if (answerToggles.Count == 0 || answerToggles.Any(a => a == null)) SpecialDebugMessages.LogMissingReferenceError(this, nameof(answerToggles));
            if (singleChoicePanel == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(singleChoicePanel));
            if (answerButtons.Count == 0 || answerButtons.Any(a => a == null)) SpecialDebugMessages.LogMissingReferenceError(this, nameof(answerButtons));
            if (publicIcon == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(publicIcon));
            if (saveIcon == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(saveIcon));

            // Reset interface
            PollClear();
            HidePollInterface();

            // Setup watchdog to react to poll events
            PollHandler.Instance.PollStarted += OnPollStarted;
            PollHandler.Instance.PollEnded += OnPollEnded;
            PollHandler.Instance.PollDiscardByPlayer += OnPollDiscardByPlayer;
        }

        private void OnDestroy()
        {
            if (PollHandler.Instance != null)
            {
                PollHandler.Instance.PollStarted -= OnPollStarted;
                PollHandler.Instance.PollEnded -= OnPollEnded;
                PollHandler.Instance.PollDiscardByPlayer -= OnPollDiscardByPlayer;
            }
        }

        /// <summary>
        /// Open the Poll window for Poll creation
        /// </summary>
        public void Open()
        {
            if (!started)
                ShowCreationInterface();
        }

        /// <summary>
        /// Open the Poll window for Poll creation with the specified transform
        /// </summary>
        public void Open(Vector3 position, Vector3 eulerAngles)
        {
            Open();
            transform.position = position;
            transform.eulerAngles = eulerAngles;
        }

        /// <summary>
        /// Close the Poll window and stops participating in the active poll if no response has been send yet
        /// </summary>
        public void Close()
        {
            HidePollInterface();
            WindowClosed?.Invoke(this, EventArgs.Empty);
            if (started && !responded)
            { // Leave Poll
                PollHandler.Instance?.SendStatus(false, pollArgs.MessageSender);
                responded = true;
            }
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
            pollMasterControlPanel.SetActive(false);
        }

        private void ShowPollInterface()
        {
            // Update interface
            questionLabel.text = pollArgs.Question;
            publicIcon?.SetActive(pollArgs.Flags.HasFlag(PollOptions.Public));
            saveIcon?.SetActive(pollArgs.Flags.HasFlag(PollOptions.SaveResults));

            // Set up answers
            if (pollArgs.Flags.HasFlag(PollOptions.MultipleChoice))
            {
                multipleChoicePanel.SetActive(true);
                singleChoicePanel.SetActive(false);
                for (int i = 0; i < answerToggles.Count && i < pollArgs.Answers.Length; i++)
                { // Setup answers
                    answerToggles[i].gameObject.SetActive(true);
                    answerToggles[i].GetComponentInChildren<TextMesh>().text = pollArgs.Answers[i]; // Don't mind the GetComponent there
                    answerToggles[i].IsToggled = false;
                }
                for (int i = pollArgs.Answers.Length; i < answerToggles.Count; i++)
                    answerToggles[i].gameObject.SetActive(false);
                for (int i = 0; i < answerButtons.Count; i++)
                    answerButtons[i].gameObject.SetActive(false);
            }
            else
            {
                multipleChoicePanel.SetActive(false);
                singleChoicePanel.SetActive(true);
                for (int i = 0; i < answerButtons.Count && i < pollArgs.Answers.Length; i++)
                { // Setup answers
                    answerButtons[i].gameObject.SetActive(true);
                    answerButtons[i].GetComponentInChildren<TextMesh>().text = pollArgs.Answers[i]; // Don't mind the GetComponent there
                }
                for (int i = pollArgs.Answers.Length; i < answerButtons.Count; i++)
                    answerButtons[i].gameObject.SetActive(false);
                for (int i = 0; i < answerToggles.Count; i++)
                    answerToggles[i].gameObject.SetActive(false);
            }

            // Show interface
            gameObject.SetActive(true);
            pollCreationPanel.SetActive(false);
            pollOptionsPanel.SetActive(false);
            pollSelectionPanel.SetActive(true);

            // Update countdown (after gameObject.SetActive to allow Coroutine)
            if (pollArgs.Flags.HasFlag(PollOptions.Countdown))
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
            pollCreationPanel.SetActive(false);
            pollOptionsPanel.SetActive(false);
            pollSelectionPanel.SetActive(false);
            if (countdown != null)
                StopCoroutine(countdown);
        }

        private void SendCreationRequest()
        { // Reads current creation UI state and requests for poll to be created
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
                options |= PollOptions.SaveResults;
            if (publicToggle.IsToggled)
                options |= PollOptions.Public;
            if (realtimeVizToggle.IsToggled)
                options |= PollOptions.RealtimeViz;
            createdPoll = true;
            PollHandler.Instance?.StartPoll(questionInput.Text, answerInputs.Where(i => !String.IsNullOrEmpty(i.Text)).Select(i => i.Text).ToArray(), options, endTime);
        }

        private void SendResponse()
        { // Sends the current selection
            responded = true;
            PollHandler.Instance?.RespondPoll(curSelection, pollArgs.MessageSender);
        }

        private void PollClear()
        { // Reset active Poll state
            started = responded = false;
            createdPoll = false;
            pollArgs = null;
            gameObject.SetActive(false);
            pollMasterControlPanel.SetActive(false);
            pollWaitingPanel.SetActive(false);
        }

        private IEnumerator CountdownUpdater()
        { // Update countdown of the current poll
            while (started)
            {
                TimeSpan span = pollArgs.End - DateTime.Now;
                countdownLabel.text = span.Minutes + ":" + span.Seconds.ToString("D2");
                yield return new WaitForSecondsRealtime(span.Milliseconds / 1000.0f);
            }
        }

        /// <summary>
        /// Event called from poll creation to advance to poll options
        /// </summary>
        public void OnPollNext()
        {
            pollCreationPanel.SetActive(false);
            pollOptionsPanel.SetActive(true);
        }

        /// <summary>
        /// Event called from poll options to return to poll creation
        /// </summary>
        public void OnPollPrevious()
        {
            pollCreationPanel.SetActive(true);
            pollOptionsPanel.SetActive(false);
        }

        /// <summary>
        /// Event called from menu to create the new poll
        /// </summary>
        public void OnPollCreate()
        {
            if (answerInputs.All(i => String.IsNullOrEmpty(i.Text)))
                return; // No answers written
            HidePollInterface();
            SendCreationRequest();
        }

        /// <summary>
        /// Event called when an answer button has been selected (Single-Choice only)
        /// </summary>
        /// <param name="option">Index of answer that has been selected</param>
        public void OnSelectOption(int option)
        {
            for (int i = 0; i < curSelection.Length; i++)
                curSelection[i] = false;
            curSelection[option] = true;
            SendResponse();
            HidePollInterface();
            pollWaitingPanel.SetActive(true);
        }

        /// <summary>
        /// Event called from menu to submit the poll selection (Multi-Choice only)
        /// </summary>
        public void OnPollSubmit()
        {
            for (int i = 0; i < curSelection.Length && i < answerToggles.Count; i++)
                curSelection[i] = answerToggles[i].IsToggled;
            SendResponse();
            HidePollInterface();
            pollWaitingPanel.SetActive(true);
        }

        /// <summary>
        /// Event called from master control panel to stop the poll prematurely
        /// </summary>
        public void OnStopButton()
        {
            if (createdPoll)
            { // Just double check, but on the receiving end clients check anyway if the host is correct
                PollHandler.Instance.EndPoll();
            }
        }

        /// <summary>
        /// Event called from creation panel to load shelf with stored polls
        /// </summary>
        public void OnLoadPolls()
        {
            PollHandler.Instance.PollShelfDisplay();
            Close();
        }

        private void OnPollStarted(object sender, PollStartEventArgs e)
        { // Event from PollHandler to set up a new poll
            if (started)
            { // Can happen when two polls are started within margin of ping
                Debug.LogError("Already participating in a poll, not joining new poll!");
                return;
            }
            if (createdPoll)
            {
                if (e.MessageSender != PhotonNetwork.LocalPlayer)
                { // Can happen when two polls are started within margin of ping, but should already be filtered out above
                    Debug.LogError("Crashing my party, aren't ya? Not with me!");
                    return;
                }
                // This interface created that poll
                pollMasterControlPanel.SetActive(true);
            }

            // Init state
            started = true;
            responded = false;
            curSelection = new bool[e.Answers.Length];
            pollArgs = e;
            // Show interface
            ShowPollInterface();
            // Notify poll master we are participating in his poll
            PollHandler.Instance?.SendStatus(true, pollArgs.MessageSender);
        }

        private void OnPollEnded(object sender, PollEndEventArgs e)
        { // Somebody sent a poll ended event
            if (!started)
                return; // Should only happen if we never received the poll start or we received two and second was ignored
            if (e.MessageSender != pollArgs.MessageSender)
            { // Should only happen in case we had two polls simulatenously, when both got started within margin of ping
                Debug.LogWarning("Received Poll End request from somebody other than initiator!");
                return;
            }
            // If not responded, send a NAK
            if (started && !responded)
                PollHandler.Instance?.SendStatus(false, pollArgs.MessageSender);
            // Clear local state
            PollClear();
            HidePollInterface();
        }

        private void OnPollDiscardByPlayer(object sender, Player player)
        {
            if (started && pollArgs.MessageSender == player)
            { // Poll host left room or discarded poll, clear local state
                PollClear();
                HidePollInterface();
            }
        }
    }
}
