using System;
using UnityEngine;
using i5.VIAProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using i5.Toolkit.Core.Utilities;
using Photon.Pun;
using i5.VIAProMa.Multiplayer;
using i5.VIAProMa.UI.MainMenuCube;
using i5.VIAProMa.Anchoring;

/// <summary>
/// Controls the menu which allows a user to enable and disable the anchoring system as well as the corresponding settings
/// </summary>
public class AnchoringMenu : MonoBehaviour, IWindow
{
    private bool windowEnabled = true;

    /// <summary>
    /// UI Elements
    /// </summary>
    [SerializeField] private Interactable useAnchoringCheckbox;
    [SerializeField] private Interactable moveAnchorAloneCheckbox;

    [SerializeField] private GameObject extendedSettings;
    [SerializeField] private SpriteRenderer lockedIconSpriteRenderer;
    [SerializeField] private TextMeshPro lockButtonText;

    [SerializeField] private Sprite unlockedIcon;
    [SerializeField] private Sprite lockedIcon;

    /// <summary>
    /// The anchor in the scene as well as the 3D anchor model as a child and the corresponding manager
    /// </summary>
    private GameObject anchorParent;
    private GameObject anchorObject;
    private AnchorManager anchorManager;


    public void Awake()
    {
        anchorManager = AnchorManager.Instance;
        anchorObject = AnchorManager.Instance.AnchorObject;
        anchorParent = AnchorManager.Instance.AnchorParent;
        anchorObject.SetActive(false);
        DisableAnchorLock();
        DisableMoveAnchorAlone();
    }

    /// <summary>
    /// States whether the window is enabled
    /// If set to false, the window will remain visible but all interactable controls are disabled
    /// </summary>
    public bool WindowEnabled
    {
        get
        {
            return windowEnabled;
        }
        set
        {
            windowEnabled = value;
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
    /// Called when the user activates or deactivates the anchoring system using the corresponding checkbox
    /// Based on the current state the extended settings are enabled or disabled, along with the corresponding buttons
    /// </summary>
    public void OnCheckboxUseAnchoringClicked()
    {
        if (anchorManager.anchoringEnabled)
        {          
            DisableAnchoring();
        }
        else
        {
            EnableAnchoring();
        }
    }

    /// <summary>
    /// Called when the user activates or deactivates the function to move the anchor object without independently using the corresponding checkbox
    /// When activated only the 3D model is moved upon manipulation, otherwise the entire anchor parent is transformed
    /// </summary>
    public void OnCheckboxMoveAnchorClicked()
    {
        if (anchorManager.moveAnchorAloneEnabled)
        {
            DisableMoveAnchorAlone();
        }
        else
        {
            EnableMoveAnchorAlone();
        }       
    }

    /// <summary>
    /// Called when the user locks or unlocks the anchor
    /// The status of the object manipulator component is updated together with the button icon and corresponding status text
    /// </summary>
    public void OnLockButtonClicked()
    {
        if (!anchorManager.anchorLocked)
        {
            EnableAnchorLock();

        }
        else
        {
            DisableAnchorLock();
        }
    }

    /// <summary>
    /// Enables the anchor lock and updates the corresponding components
    /// </summary>
    public void EnableAnchorLock()
    {
        anchorManager.anchorLocked = true;
        anchorObject.GetComponent<ObjectManipulator>().enabled = false;
        lockedIconSpriteRenderer.sprite = lockedIcon;
        lockButtonText.text = "Anchor is locked";
        Debug.Log("The anchor is now locked.");
    }

    /// <summary>
    /// Disables the anchor lock and updates the corresponding components
    /// </summary>
    public void DisableAnchorLock()
    {
        anchorManager.anchorLocked = false;
        anchorObject.GetComponent<ObjectManipulator>().enabled = true;
        lockedIconSpriteRenderer.sprite = unlockedIcon;
        lockButtonText.text = "Anchor is unlocked";
        Debug.Log("The anchor is now unlocked.");
    }

    /// <summary>
    /// Disables anchoring and all corresponding UI components.
    /// </summary>
    public void DisableAnchoring()
    {
        anchorManager.anchoringEnabled = false;
        useAnchoringCheckbox.IsToggled = false;
        anchorObject.SetActive(false);
        extendedSettings.SetActive(false);
        Debug.Log("Anchoring has been deactivated.");
    }

    /// <summary>
    /// Enables anchoring and all corresponding UI components.
    /// </summary>
    public void EnableAnchoring()
    {
        anchorManager.anchoringEnabled = true;
        anchorObject.SetActive(true);
        extendedSettings.SetActive(true);
        Debug.Log("Anchoring has been activated.");
    }

    /// <summary>
    /// Enables the functionality to move the anchor independently of the scene
    /// </summary>
    public void EnableMoveAnchorAlone()
    {
        anchorManager.moveAnchorAloneEnabled = true;
        moveAnchorAloneCheckbox.IsToggled = true;
        anchorObject.GetComponent<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>().HostTransform = anchorObject.transform;
        Debug.Log("Individual moving mode enabled.");
    }

    /// <summary>
    /// Disables the functionality to move the anchor independently of the scene
    /// </summary>
    public void DisableMoveAnchorAlone()
    {
        anchorManager.moveAnchorAloneEnabled = false;
        moveAnchorAloneCheckbox.IsToggled = false;
        anchorObject.GetComponent<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>().HostTransform = anchorParent.transform;
        Debug.Log("Individual moving mode disabled.");
    
    }


    /// <summary>
    /// Closes the window
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Opens the window and sets the components values for the locking button
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);

        if (anchorManager.anchorLocked)
        {
            EnableAnchorLock();

        }
        else
        {
            DisableAnchorLock();
        }

        if (anchorManager.anchoringEnabled)
        {
            EnableAnchoring();

        }
        else
        {
            DisableAnchoring();
        }

        if (anchorManager.moveAnchorAloneEnabled)
        {
            EnableMoveAnchorAlone();

        }
        else
        {
            DisableMoveAnchorAlone();
        }
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
    }

}
