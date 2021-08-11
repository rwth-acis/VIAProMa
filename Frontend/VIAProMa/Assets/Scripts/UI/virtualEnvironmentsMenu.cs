using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the menu which allows a user to select existing rooms (or navigate to the menu where a new room can be created)
/// </summary>
public class virtualEnvironmentsMenu : MonoBehaviour, IWindow
{
    [SerializeField] private EnvironmentListView environmentListView;
    /// <summary>
    /// The number of environment entries which are shown on one page
    /// </summary>
    [SerializeField] private int entriesPerPage;
    [SerializeField] private Interactable pageUpButton;
    [SerializeField] private Interactable pageDownButton;
    [SerializeField] private Material defaultSkybox;
    [SerializeField] private Sprite defaultpreviewImage;
    [SerializeField] private string defaultCredits;

    private Material[] environmentSkyboxes;
    private Sprite[] previewImages;
    [SerializeField] private string[] environmentNames;
    private GameObject[] environmentPrefabs;
    private string[] environmentCredits;
    [SerializeField] private string[] environmentURLs;

    private Material currentSkybox;
    private GameObject currentPrefab;
    private bool coroutinesFinished = false;
    private string assetBundlesURL;

    private List<EnvironmentData> environments = new List<EnvironmentData>();

    private int page = 0;
    private bool windowEnabled = true;
    private GameObject currentEnvironmentInstance;

    /// <summary>
    /// States whether the window is enabled
    /// If set to false, the window will remain visible but all interactable controls are disabled
    /// </summary>
    /// <value></value>
    public bool WindowEnabled
    {
        get
        {
            return windowEnabled;
        }
        set
        {
            windowEnabled = value;
            pageUpButton.Enabled = value;
            pageDownButton.Enabled = value;
        }
    }

    public bool WindowOpen
    {
        get; private set;
    }

    /// <summary>
    /// Event which is invoked if the window is closed
    /// </summary>
    public event EventHandler WindowClosed;

    /// <summary>
    /// Initializes the component, makes sure that it is set up correctly
    /// </summary>
    private void Awake()
    {
        if (pageUpButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(pageUpButton));
        }
        if (pageDownButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(pageDownButton));
        }

        environmentPrefabs = new GameObject[environmentURLs.Length];
        previewImages = new Sprite[environmentURLs.Length];
        environmentSkyboxes = new Material[environmentURLs.Length];
        environmentCredits = new string[environmentURLs.Length];

        previewImages[0] = defaultpreviewImage;
        environmentSkyboxes[0] = defaultSkybox;
        environmentCredits[0] = defaultCredits;

        assetBundlesURL = "file:///" + Application.dataPath + "/AssetBundles/";
        if(UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
            StartCoroutine(GetAssetBundleObjects());

        environmentListView.ItemSelected += OnEnvironmentSelected;

        Close();
    }

    /// <summary>
    /// Called if a element of the room list view was selected by the user
    /// </summary>
    /// <param name="skybox">The selected skybox</param>
    /// <param name="e">Arguments about the list view selection event</param>
    private void OnEnvironmentSelected(object sender, ListViewItemSelectedArgs e)
    {
        if ((environmentListView.SeletedItem != null) && windowEnabled)
        {
            if (environmentListView.SeletedItem.EnvironmentBackground != null)
            {
                RenderSettings.skybox = environmentListView.SeletedItem.EnvironmentBackground;
            }
            if(currentEnvironmentInstance != null)
            {
                Destroy(currentEnvironmentInstance);
            }
            if (environmentListView.SeletedItem.EnvironmentPrefab != null)
            {
                currentEnvironmentInstance = Instantiate(environmentListView.SeletedItem.EnvironmentPrefab, environmentListView.SeletedItem.EnvironmentPrefab.transform.position, environmentListView.SeletedItem.EnvironmentPrefab.transform.rotation);            
            }
        }
    }

    /// <summary>
    /// Called if the user pushes the page up button
    /// Swiches to the previous page
    /// </summary>
    public void PageUp()
    {
        page = Mathf.Max(0, page - 1);
        SetPageButtonStates();
        UpdateEnvironmentDisplay();
    }

    /// <summary>
    /// Called if the user pages the page down button
    /// Switches to the next page
    /// </summary>
    public void PageDown()
    {
        page = Mathf.Min(page + 1, ((environments.Count - 1) / entriesPerPage));
        SetPageButtonStates();
        UpdateEnvironmentDisplay();
    }


    /// <summary>
    /// Adapts the button states of the page up and page down buttons
    /// If the first page is shown, the up button is disabled and if the last page is shown, the down button is disabled
    /// </summary>
    private void SetPageButtonStates()
    {
        if (page == 0) // first page
        {
            pageUpButton.Enabled = false;
        }
        else
        {
            pageUpButton.Enabled = true;
        }

        if (page == ((environments.Count - 1) / entriesPerPage)) // last page
        {
            pageDownButton.Enabled = false;
        }
        else
        {
            pageDownButton.Enabled = true;
        }
    }


    /// <summary>
    /// Updates the list view showing the environment lists (on the current page)
    /// </summary>
    private void UpdateEnvironmentDisplay()
    {
        if (environments.Count > 0)
        {
            // get the start index and length of the sub array to display
            // make sure that it stays within the bounds of the room list
            int startIndex = Mathf.Min(page * entriesPerPage, environments.Count - 1);
            int length = Mathf.Min(environments.Count - startIndex, entriesPerPage);
            environmentListView.Items = environments.GetRange(startIndex, length);
        }
        else
        {
            environmentListView.Items = new List<EnvironmentData>();
        }
    }

    /// <summary>
    /// Opens the window
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);
        WindowOpen = true;
        UpdateEnvironmentDisplay();
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
    }

    /// <summary>
    /// Closes the window
    /// </summary>
    public void Close()
    {
        WindowOpen = false;
        WindowClosed?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }

    IEnumerator GetAssetBundleObjects()
    {
        for (int arrayIndex = 0; arrayIndex < environmentURLs.Length; arrayIndex++)
        {
            if (arrayIndex != 0)
            {
                currentSkybox = null;
                currentPrefab = null;
                string url = assetBundlesURL + environmentURLs[arrayIndex];
                var request = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
                AsyncOperation sentRequest = request.SendWebRequest();
                while (!sentRequest.isDone)
                { }

                if (UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request) != null)
                {
                    AssetBundle bundle = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);
                    if (bundle != null)
                    {
                        environmentSkyboxes[arrayIndex] = bundle.LoadAllAssets<Material>()[0];
                        if (bundle.LoadAllAssets<GameObject>().Length != 0)
                            environmentPrefabs[arrayIndex] = bundle.LoadAllAssets<GameObject>()[0];
                        previewImages[arrayIndex] = bundle.LoadAllAssets<Sprite>()[0];
                        environmentCredits[arrayIndex] = bundle.LoadAllAssets<TextAsset>()[0].text;
                    }
                }
            }

            if (previewImages[arrayIndex] != null && environmentSkyboxes[arrayIndex] != null)
            {
                environments.Add(new EnvironmentData(environmentNames[arrayIndex], previewImages[arrayIndex], environmentSkyboxes[arrayIndex], environmentPrefabs[arrayIndex], environmentCredits[arrayIndex], environmentURLs[arrayIndex]));
            }
        }
        yield return null;
    }
}
