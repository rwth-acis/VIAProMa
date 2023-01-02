using i5.VIAProMa.UI.InputFields;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Voice.PUN.UtilityScripts;
using System;
using UnityEngine;

namespace i5.VIAProMa.UI
{
    public class ImportManager : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject searchMenu;
        [SerializeField] private GameObject sessionMenu;
        [SerializeField] private GameObject harddriveMenu;

        [SerializeField] private GameObject tabs;
        [SerializeField] private Interactable searchTabButton;
        [SerializeField] private Interactable sessionTabButton;
        [SerializeField] private Interactable harddriveTabButton;
        [SerializeField] private string currActiveWindow;

        [SerializeField] private InputField searchField;
        [SerializeField] private Interactable loginButton;
        [SerializeField] private Interactable searchUpButton;
        [SerializeField] private Interactable searchDownButton;

        [SerializeField] private Interactable sessionUpButton;
        [SerializeField] private Interactable sessionDownButton;

        public GameObject modelWrapper;


        public bool WindowEnabled { get; set; }

        public bool WindowOpen
        {
            get => gameObject.activeSelf;
        }

        public event EventHandler WindowClosed;

        private void Awake()
        {
            if (searchMenu == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(searchMenu));
            }
            if (sessionMenu == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(sessionMenu));
            }
            if (harddriveMenu == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(harddriveMenu));
            }
            if (searchTabButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(searchTabButton));
            }
            if (sessionTabButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(sessionTabButton));
            }
            if (harddriveTabButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(harddriveTabButton));
            }
            if (searchField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(searchField));
            }
            else
            {
                searchField.TextChanged += OnQueryChanged;
            }
            if (loginButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(loginButton));
            }
            if (searchUpButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(searchUpButton));
            }
            if (searchDownButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(searchDownButton));
            }
            if (sessionUpButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(sessionUpButton));
            }
            if (sessionDownButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(sessionDownButton));
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            modelWrapper = new GameObject();
            modelWrapper.name = "3Dmodels";
            modelWrapper.transform.position = Vector3.zero;
            modelWrapper.transform.parent = GameObject.Find("AnchorParent").transform;

            // Search Menu as default menu
            SetMenuTo("search");
        }

        private void OnQueryChanged(object sender, EventArgs e)
        {
            //Search field changed
            this.gameObject.GetComponent<SearchBrowserRefresher>().SearchChanged(searchField.Text);
            //Debug.Log("Import Menu Search Field: " + searchField.Text);
        }

        // Changes the shown menu and de/-activates tab buttons accordingly
        public void SetMenuTo(String menuName)
        {
            //set positions of windows
            switch (currActiveWindow)
            {
                case "search":
                    sessionMenu.transform.GetChild(0).transform.position = searchMenu.transform.GetChild(0).transform.position;
                    sessionMenu.transform.GetChild(0).transform.rotation = searchMenu.transform.GetChild(0).transform.rotation;
                    sessionMenu.transform.GetChild(0).transform.localScale = searchMenu.transform.GetChild(0).transform.localScale;
                    harddriveMenu.transform.GetChild(0).transform.position = searchMenu.transform.GetChild(0).transform.position;
                    harddriveMenu.transform.GetChild(0).transform.rotation = searchMenu.transform.GetChild(0).transform.rotation;
                    harddriveMenu.transform.GetChild(0).transform.localScale = searchMenu.transform.GetChild(0).transform.localScale;
                    break;
                case "session":
                    searchMenu.transform.GetChild(0).transform.position = sessionMenu.transform.GetChild(0).transform.position;
                    searchMenu.transform.GetChild(0).transform.rotation = sessionMenu.transform.GetChild(0).transform.rotation;
                    searchMenu.transform.GetChild(0).transform.localScale = sessionMenu.transform.GetChild(0).transform.localScale;
                    harddriveMenu.transform.GetChild(0).transform.position = sessionMenu.transform.GetChild(0).transform.position;
                    harddriveMenu.transform.GetChild(0).transform.rotation = sessionMenu.transform.GetChild(0).transform.rotation;
                    harddriveMenu.transform.GetChild(0).transform.localScale = sessionMenu.transform.GetChild(0).transform.localScale;
                    break;
                case "harddrive":
                    sessionMenu.transform.GetChild(0).transform.position = harddriveMenu.transform.GetChild(0).transform.position;
                    sessionMenu.transform.GetChild(0).transform.rotation = harddriveMenu.transform.GetChild(0).transform.rotation;
                    sessionMenu.transform.GetChild(0).transform.localScale = harddriveMenu.transform.GetChild(0).transform.localScale;
                    searchMenu.transform.GetChild(0).transform.position = harddriveMenu.transform.GetChild(0).transform.position;
                    searchMenu.transform.GetChild(0).transform.rotation = harddriveMenu.transform.GetChild(0).transform.rotation;
                    searchMenu.transform.GetChild(0).transform.localScale = harddriveMenu.transform.GetChild(0).transform.localScale;
                    break;
            }
            
            
            //enable/disable windows and buttons
            switch (menuName)
            {
                case "search":
                    searchMenu.SetActive(true);
                    searchTabButton.IsEnabled = false;
                    sessionMenu.SetActive(false);
                    sessionTabButton.IsEnabled = true;
                    harddriveMenu.SetActive(false);
                    harddriveTabButton.IsEnabled = true;
                    tabs.transform.parent = searchMenu.transform.GetChild(0).transform;
                    currActiveWindow = "search";
                    break;
                case "session":
                    searchMenu.SetActive(false);
                    searchTabButton.IsEnabled = true;
                    sessionMenu.SetActive(true);
                    sessionTabButton.IsEnabled = false;
                    harddriveMenu.SetActive(false);
                    harddriveTabButton.IsEnabled = true;
                    tabs.transform.parent = sessionMenu.transform.GetChild(0).transform;
                    currActiveWindow = "session";
                    break;
                case "harddrive":
                    searchMenu.SetActive(false);
                    searchTabButton.IsEnabled = true;
                    sessionMenu.SetActive(false);
                    sessionTabButton.IsEnabled = true;
                    harddriveMenu.SetActive(true);
                    harddriveTabButton.IsEnabled = false;
                    tabs.transform.parent = harddriveMenu.transform.GetChild(0).transform;
                    currActiveWindow = "harddrive";
                    break;
            }

        }

        public void Close()
        {
            gameObject.SetActive(false);
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Open(Vector3 position, Vector3 eulerAngles)
        {
            Open();
            transform.localPosition = position;
            transform.localEulerAngles = eulerAngles;
        }
    }
}
