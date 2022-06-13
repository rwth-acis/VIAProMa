using System;
using UnityEngine;
using i5.VIAProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;

public class AnchoringMenu : MonoBehaviour, IWindow
{
    private bool windowEnabled = true;
    [SerializeField] private Interactable useAnchoringCheckbox;
    [SerializeField] private Interactable moveAnchorAloneCheckbox;
    [SerializeField] private Interactable anchorLockedButton;

    [SerializeField] private SpriteRenderer lockedIconSpriteRenderer;

    [SerializeField] private Sprite unlockedIcon;
    [SerializeField] private Sprite lockedIcon;

    private GameObject anchorParent;
    private GameObject anchorObject;

    private bool locked = false;

    public void Awake()
    {
        anchorParent = this.transform.parent.parent.parent.gameObject;
        anchorObject = anchorParent.transform.Find("AnchorObject").gameObject;
        anchorObject.SetActive(false);
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

    public void OnCheckboxUseAnchoringClicked()
    {
        moveAnchorAloneCheckbox.IsEnabled = !moveAnchorAloneCheckbox.IsEnabled;
        anchorLockedButton.IsEnabled = !anchorLockedButton.IsEnabled;
        anchorObject.SetActive(!anchorObject.activeSelf);
    }

    public void OnCheckboxMoveAnchorClicked()
    {
        if (anchorObject.GetComponent<ObjectManipulator>().HostTransform == anchorObject.transform)
            anchorObject.GetComponent<ObjectManipulator>().HostTransform = anchorParent.transform;
        else
            anchorObject.GetComponent<ObjectManipulator>().HostTransform = anchorObject.transform;
    }

    public void OnLockButtonClicked()
    {
        if (locked)
        {
            anchorObject.GetComponent<ObjectManipulator>().enabled = false;
            lockedIconSpriteRenderer.sprite = lockedIcon;
        }
        else
        {
            anchorObject.GetComponent<ObjectManipulator>().enabled = true;
            lockedIconSpriteRenderer.sprite = unlockedIcon;
        }
        locked = !locked;
    }

    public void Close()
    {
        gameObject.SetActive(false);
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
