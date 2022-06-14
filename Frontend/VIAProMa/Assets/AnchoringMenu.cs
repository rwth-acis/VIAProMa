using System;
using UnityEngine;
using i5.VIAProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using i5.Toolkit.Core.Utilities;

/// <summary>
/// Controls the menu which allows a user to enable and disable the anchoring system as well as the corresponding settings
/// </summary>
public class AnchoringMenu : MonoBehaviour, IWindow
{
    private bool windowEnabled = true;
    [SerializeField] private Interactable useAnchoringCheckbox;
    [SerializeField] private Interactable moveAnchorAloneCheckbox;
    [SerializeField] private Interactable anchorLockedButton;

    [SerializeField] private GameObject extendedSettings;
    [SerializeField] private SpriteRenderer lockedIconSpriteRenderer;
    [SerializeField] private TextMeshPro lockButtonText;

    [SerializeField] private Sprite unlockedIcon;
    [SerializeField] private Sprite lockedIcon;

    /// <summary>
    /// The anchor in the scene as well as the 3D anchor model as a child
    /// </summary>
    private GameObject anchorParent;
    private GameObject anchorObject;

    /// <summary>
    /// Whether the locking of the anchor is currently enable or disabled
    /// </summary>
    private bool locked;

    public void Awake()
    {
        anchorParent = this.transform.parent.parent.parent.gameObject;
        anchorObject = anchorParent.transform.Find("AnchorObject").gameObject;
        anchorObject.SetActive(false);
        locked = false;
        DisableAnchorLock();
    }

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
        moveAnchorAloneCheckbox.IsEnabled = !moveAnchorAloneCheckbox.IsEnabled;
        anchorLockedButton.IsEnabled = !anchorLockedButton.IsEnabled;
        anchorObject.SetActive(!anchorObject.activeSelf);
        extendedSettings.SetActive(!extendedSettings.activeSelf);
        if (extendedSettings.activeSelf)
        {
            Debug.Log("Anchoring has been activated.");
        }
        else
        {
            Debug.Log("Anchoring has been deactivated.");
        }
    }

    /// <summary>
    /// Called when the user activates or deactivates the function to move the anchor object without independently using the corresponding checkbox
    /// When activated only the 3D model is moved upon manipulation, otherwise the entire anchor parent is transformed
    /// </summary>
    public void OnCheckboxMoveAnchorClicked()
    {
        if (anchorObject.GetComponent<ObjectManipulator>().HostTransform == anchorObject.transform)
        {
            anchorObject.GetComponent<ObjectManipulator>().HostTransform = anchorParent.transform;
            Debug.Log("Individual moving mode disabled.");
        }
        else
        {
            anchorObject.GetComponent<ObjectManipulator>().HostTransform = anchorObject.transform;
            Debug.Log("Individual moving mode enabled.");
        }       
    }

    /// <summary>
    /// Called when the user locks or unlocks the anchor
    /// The status of the object manipulator component is updated together with the button icon and corresponding status text
    /// </summary>
    public void OnLockButtonClicked()
    {
        if (!locked)
        {
            EnableAnchorLock();
            locked = true;
            Debug.Log("The anchor is now locked.");

        }
        else
        {
            DisableAnchorLock();
            locked = false;
            Debug.Log("The anchor is now unlocked.");
        }
    }

    /// <summary>
    /// Enables the anchor lock and updates the corresponding components
    /// </summary>
    public void EnableAnchorLock()
    {
        anchorObject.GetComponent<ObjectManipulator>().enabled = false;
        lockedIconSpriteRenderer.sprite = lockedIcon;
        lockButtonText.text = "Anchor is locked";
    }

    /// <summary>
    /// Disables the anchor lock and updates the corresponding components
    /// </summary>
    public void DisableAnchorLock()
    {
        anchorObject.GetComponent<ObjectManipulator>().enabled = true;
        lockedIconSpriteRenderer.sprite = unlockedIcon;
        lockButtonText.text = "Anchor is unlocked";
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

        if (locked)
        {
            EnableAnchorLock();

        }
        else
        {
            DisableAnchorLock();
        }
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
    }

}
