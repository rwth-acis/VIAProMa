using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

/// <summary>
/// Component on the issue card which makes the card selectable
/// </summary>
[RequireComponent(typeof(IssueDataDisplay))]
public class IssueSelector : MonoBehaviour, IViewContainer, IMixedRealityPointerHandler
{
    [SerializeField] private GameObject selectionIndicator;
    [SerializeField] private Renderer backgroundRenderer;

    private IssueDataDisplay issueDataDisplay;
    private bool selected;
    private Color originalRendererColor;

    public Color selectedColor = new Color(0.1698113f, 0.2845136f, 0.6792453f); // blue

    public LineDrawLogic linedrawscript;


    /// <summary>
    /// True if the issue is currently selected
    /// </summary>
    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            UpdateView();
        }
    }

    /// <summary>
    /// Checks the component's setup and fetches necessary references
    /// </summary>
    private void Awake()
    {
        if (selectionIndicator == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(selectionIndicator));
        }
        issueDataDisplay = GetComponent<IssueDataDisplay>();
        if (issueDataDisplay == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IssueDataDisplay), gameObject);
        }
        if (backgroundRenderer == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(backgroundRenderer));
        }
        else
        {
            originalRendererColor = backgroundRenderer.material.color;
        }
    }

    /// <summary>
    /// Called if the GameObject is enabled
    /// Registers for the IssueSelectionManager's events
    /// </summary>
    private void OnEnable()
    {
        IssueSelectionManager.Instance.SelectionModeChanged += ReactToChangedSelectionMode;
        IssueSelectionManager.Instance.IssueSelectionChanged += ReactToIssueSelectionChanged;
    }

    private void Start()
    {
        // check selection mode in start => all other components which use awake should now be set up
        if (IssueSelectionManager.Instance.SelectionModeActive)
        {
            Selected = IssueSelectionManager.Instance.IsSelected(issueDataDisplay.Content);
            UpdateView();
        }
        linedrawscript = GameObject.FindGameObjectWithTag("LineDraw").GetComponent<LineDrawLogic>();
        
    }

    /// <summary>
    /// Called if the GameObject is disabled
    /// De-registers from the IssueSelectionManager's events
    /// </summary>
    private void OnDisable()
    {
        if (IssueSelectionManager.Instance != null)
        {
            IssueSelectionManager.Instance.SelectionModeChanged -= ReactToChangedSelectionMode;
            IssueSelectionManager.Instance.IssueSelectionChanged -= ReactToIssueSelectionChanged;
        }
    }

    /// <summary>
    /// Called if the issue selection on the IssueSelectionManager is changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ReactToIssueSelectionChanged(object sender, IssueSelectionChangedArgs e)
    {
        if (issueDataDisplay != null)
        {
            if (e.ChangedIssue.Equals(issueDataDisplay.Content))
            {
                Selected = e.Selected;
            }
        }
    }

    /// <summary>
    /// Called if the selection mode on the IssueSelectionManager is changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ReactToChangedSelectionMode(object sender, EventArgs e)
    {
        if (IssueSelectionManager.Instance.SelectionModeActive) // selection mode was just activated
        {
            Selected = IssueSelectionManager.Instance.IsSelected(issueDataDisplay.Content);
        }
        else // selection mode has ended
        {
            selectionIndicator.SetActive(false);
            backgroundRenderer.material.color = originalRendererColor;
        }
    }

    /// <summary>
    /// Toggles the selection of the issue of this card
    /// </summary>
    public void ToggleSelection()
    {
        // report selection or deselection to selection manager
        if (Selected)
        {
            IssueSelectionManager.Instance.SetDeselected(issueDataDisplay.Content);
        }
        else
        {
            IssueSelectionManager.Instance.SetSelected(issueDataDisplay.Content);
        }
        // do not update the selection visuals here; they will be updated by the selection manager through its IssueSelectionChanged event
    }

    /// <summary>
    /// Updates the visual selection indiciation on the card
    /// </summary>
    public void UpdateView()
    {
        if (IssueSelectionManager.Instance.SelectionModeActive)
        {
            selectionIndicator.SetActive(Selected);
            if (Selected)
            {
                backgroundRenderer.material.color = selectedColor;
            }
            else
            {
                backgroundRenderer.material.color = originalRendererColor;
            }
        }
        else
        {
            selectionIndicator.SetActive(false);
            backgroundRenderer.material.color = originalRendererColor;
        }
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
    }

    /// <summary>
    /// Called by the Mixed Reality Toolkit if the object was clicked
    /// If the LineDraw mode is active, the object is saved as either start or destination.
    /// </summary>
    /// <param name="eventData">The event data of the interaction</param>
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (IssueSelectionManager.Instance.SelectionModeActive)
        {
            ToggleSelection();
            eventData.Use();
        }
        if (linedrawscript.isLineModeActivated)
        {
            if (!linedrawscript.oneSelected)
            {
                Debug.Log("First Object selected");
                linedrawscript.start = gameObject;
                linedrawscript.oneSelected = true;
            }
            else
            {
                Debug.Log("Second Object selected");
                linedrawscript.destination = gameObject;
                linedrawscript.oneSelected = false;
            }
        }
    }
}
