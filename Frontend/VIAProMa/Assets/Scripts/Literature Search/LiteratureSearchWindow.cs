using i5.VIAProMa.LiteratureSearch;
using i5.VIAProMa.Multiplayer;
using i5.VIAProMa.UI;
using i5.VIAProMa.UI.InputFields;
using i5.VIAProMa.UI.MessageBadge;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    /// <summary>
    /// Search window for the literature search.
    /// </summary>
    public class LiteratureSearchWindow : MonoBehaviour, IWindow
    {

        [Header("UI Elements")]
        [Tooltip("The button for closing the menu")]
        [SerializeField] private Interactable closeButton;
        [Tooltip("The text field for entering the search query")]
        [SerializeField] private InputField searchField;
        [Tooltip("Checkbox for advanced settings")]
        [SerializeField] private Interactable advancedSettingsCheckbox;
        [Tooltip("The slider for determining the number of members")]
        [SerializeField] private SliderExtension paperCountSlider;
        [Tooltip("The button which confirms the settings and executes the search")]
        [SerializeField] private Interactable executeSearch;
        [Tooltip("Text to display messages")]
        [SerializeField] private TextMeshPro display;
        [Tooltip("The button to show the next page.")]
        [SerializeField] private Interactable next;
        [Tooltip("The button to show the previous page.")]
        [SerializeField] private Interactable previous;
        [Tooltip("GameObject of the loading and retry icon.")]
        [SerializeField] private GameObject loadingIcon;
        [Tooltip("Prefab of the paper group.")]
        [SerializeField] private GameObject groupPrefab;
        [Tooltip("RadioButtons for selection of visualization.")]
        [SerializeField] private InteractableToggleCollection visToggles;
        [Tooltip("Parent for all advanced settings.")]
        [SerializeField] private GameObject advancedSettings;
        [Tooltip("Checkbox to enable the search for a specific DOI.")]
        [SerializeField] private Interactable searchDOICheckbox;

        /// <summary>
        /// Saves if the window is enables.
        /// </summary>
        private bool windowEnabled = true;
        /// <summary>
        /// MessageBadge used to display the loading animation.
        /// </summary>
        private MessageBadge messageBadge;
        public bool WindowEnabled 
        {
            get 
            {
                return windowEnabled;
            }
            set 
            {
                windowEnabled = value;
                closeButton.enabled = value;
                executeSearch.enabled = value;
            }
        }

        public bool WindowOpen { get; private set; }

        private string lastQuery;

        private int currentPage = 0;
        private int CurrentPage 
        { 
            get 
            { 
                return currentPage;  
            } 
            set 
            { 
                currentPage = value; 
                // Makes sure current page cannot be -1
                if(currentPage == 0)
                {
                    previous.IsEnabled = false;
                }
                if(currentPage > 0)
                {
                    previous.IsEnabled = true;

                }
            } 
        } 

        public event EventHandler WindowClosed;

        /// <summary>
        /// Initializes the window and makes sure all UI Elements are referenced and the window is set up correctly.
        /// </summary>
        public void Awake()
        {
            if (closeButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(closeButton));
            }
            if (searchField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(searchField));
            }
            if (advancedSettingsCheckbox == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(advancedSettingsCheckbox));
            }
            if (paperCountSlider == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(paperCountSlider));
            }
            if (executeSearch == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(executeSearch));
            }
            if (display == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(display));
            }
            if (next == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(next));
            }
            if (previous == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(previous));
            }
            if (loadingIcon == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(loadingIcon));
            }
            else 
            {
                messageBadge = loadingIcon.GetComponent<MessageBadge>();
            }
            if (visToggles == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(visToggles));
            }
            if (advancedSettings == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(advancedSettings));
            }
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public void Close()
        {
            PaperController.Instance.ClearResults();

            WindowOpen = false;
            WindowClosed?.Invoke(this, EventArgs.Empty);
            gameObject.SetActive(false);

            LobbyManager.Instance.LobbyJoinStatusChanged -= OnLobbyStatusChanged;
        }

        /// <summary>
        /// Opens the window.
        /// </summary>
        public void Open()
        {
            gameObject.SetActive(true);
            WindowOpen = true;

            LobbyManager.Instance.LobbyJoinStatusChanged += OnLobbyStatusChanged;

            AdjustToRoomStatus();
        }

        /// <summary>
        /// Event call for when the room/lobby status changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLobbyStatusChanged(object sender, EventArgs e)
        {
            AdjustToRoomStatus();
        }

        /// <summary>
        /// Adjusts the window to a changed room status.
        /// </summary>
        private void AdjustToRoomStatus()
        {
            if (PhotonNetwork.InRoom)
            {
                executeSearch.IsEnabled = true;
                display.text = "";
            }
            else
            {
                executeSearch.IsEnabled = false;
                display.text = "Join a room to search for papers.";
            }
        }

        /// <summary>
        /// Opens the window at <paramref name="position"/> with the angles <paramref name="eulerAngles"/>.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="eulerAngles"></param>
        public void Open(Vector3 position, Vector3 eulerAngles)
        {
            Open();
            transform.position = position;
            transform.eulerAngles = eulerAngles;
        }

        /// <summary>
        /// Activates or deactivated the advanced settings.
        /// </summary>
        public void OnAdvanvedSettingsClick()
        {
            advancedSettings.SetActive(advancedSettingsCheckbox.CurrentDimension == 1);
        }
        /// <summary>
        /// Executes a literature search with the input of the search field.
        /// </summary>
        public async void OnSearchClick()
        {
            CrossRefMessage message;
            CurrentPage = 0;
            lastQuery = searchField.Text;
            int maxResults;
            if(advancedSettingsCheckbox.CurrentDimension == 1)
            {
                maxResults = paperCountSlider.ValueInt;
                if (searchDOICheckbox.IsToggled)
                {
                    Paper paper = await Communicator.GetPaper(searchField.Text);
                    if(paper is null)
                    {
                        display.text = $"{searchField.Text} was not found.";
                    }
                    else
                    {
                        PaperController.Instance.ShowResults(new List<Paper>() { paper }, transform);
                    }
                    return;
                }
            }
            else
            {
                maxResults = 5;
            }
            ShowLoading();
            message = await Communicator.APISearch(searchField.Text, maxResults);
            if(message == null)
            {
                next.IsEnabled = false;
                display.text = "There has been an error.";
            }
            else if(message.totalresults == 0){
                next.IsEnabled = false;
                display.text = "No matches were found.";
            }
            else{
                display.text = "Total results: " + message.totalresults;
                next.IsEnabled = !(maxResults >= message.totalresults);
            }
            StopLoading();
            PaperController.Instance.ShowResults(CrossRefPaper.ToPapers(message.items), transform);
        }
        /// <summary>
        /// Button click event for the next page button.
        /// </summary>
        public async void OnNextClick()
        {
            CrossRefMessage message;
            CurrentPage++;
            int maxResults;
            if (advancedSettingsCheckbox.CurrentDimension == 1)
            {
                maxResults = paperCountSlider.ValueInt;
            }
            else
            {
                maxResults = 5;
            }
            ShowLoading();
            message = await Communicator.APISearch(lastQuery, maxResults, CurrentPage);
            StopLoading();
            // Trys to prevent offset bigger then total results. CAN FAIL, if max result options are increased on later pages.
            next.IsEnabled = !(maxResults*currentPage >= message.totalresults);
            PaperController.Instance.ShowResults(CrossRefPaper.ToPapers(message.items), transform);
        }
        /// <summary>
        /// Button click event for the previous page button.
        /// </summary>
        public async void OnPreviousClick()
        {
            CrossRefMessage message;
            CurrentPage--;
            int maxResults;
            if (advancedSettingsCheckbox.CurrentDimension == 1)
            {
                maxResults = paperCountSlider.ValueInt;
            }
            else
            {
                maxResults = 5;
            }
            ShowLoading();
            message = await Communicator.APISearch(lastQuery, maxResults, CurrentPage);
            StopLoading();
            PaperController.Instance.ShowResults(CrossRefPaper.ToPapers(message.items), transform);
        }

        /// <summary>
        /// Sets the message badge gameobject so that it shows loading.
        /// </summary>
        private void ShowLoading()
        {
            executeSearch.IsEnabled = false;
            messageBadge.gameObject.SetActive(true);
            messageBadge.ShowProcessing();
        }

        /// <summary>
        /// Sets the message bade gameobject to done loading.
        /// </summary>
        private void StopLoading()
        {
            executeSearch.IsEnabled = true;
            messageBadge.DoneProcessing();
        }

        /// <summary>
        /// Creates an empty paper group.
        /// </summary>
        public void CreatePaperGroup()
        {
            Instantiate(groupPrefab, transform.position + new Vector3(-.6f,0,-.1f), transform.rotation);
        }

        /// <summary>
        /// Changes the selected visualization according to the radio buttons in the advanced settings.
        /// </summary>
        public void VisualizationSelectionChanged()
        {
            PaperController.Instance.ChangeVisualization(visToggles.CurrentIndex);
        }
    }



}