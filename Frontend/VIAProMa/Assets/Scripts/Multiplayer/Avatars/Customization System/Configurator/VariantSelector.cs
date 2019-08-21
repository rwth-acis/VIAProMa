using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the selection of items from item frames
/// Initializes the item frames and feeds the necessary data to them
/// Item frames will report back the selection which is then handled by this selector
/// </summary>
public class VariantSelector : MonoBehaviour
{
    [Tooltip("The item frames which show the items")]
    [SerializeField] private ItemFrame[] itemFrames;

    [SerializeField] private Interactable pageUpButton;
    [SerializeField] private Interactable pageDownButton;

    private Interactable[] interactables;
    private int page = 0;
    private int selectedIndex;

    private IItem[] items;

    /// <summary>
    /// Invoked if an item was selected
    /// </summary>
    public event EventHandler ItemSelected;

    /// <summary>
    /// Array of items which should be shown for selection in the item frames
    /// </summary>
    public IItem[] Items
    {
        get => items;
        set
        {
            items = value;
            UpdateDisplays();
        }
    }

    /// <summary>
    /// The index of the currently selected option
    /// This index points to an item in the Items array
    /// </summary>
    public int SelectedIndex
    {
        get => selectedIndex;
        set
        {
            selectedIndex = value;
            UpdateDisplays();
        }
    }

    /// <summary>
    /// Initializes the component and setup up the item frames
    /// </summary>
    protected virtual void Awake()
    {
        if (itemFrames.Length == 0)
        {
            SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, nameof(itemFrames));
        }

        // initialize the toggle collection
        interactables = new Interactable[itemFrames.Length];
        for (int i = 0; i < itemFrames.Length; i++)
        {
            // setup item frames
            itemFrames[i].Setup(this);

            // register corresponding interactables
            Interactable interactable = itemFrames[i].gameObject.GetComponent<Interactable>();
            if (interactable == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(Interactable), itemFrames[i].gameObject);
            }
            else
            {
                interactables[i] = interactable;
            }
        }

        if (pageUpButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(pageUpButton));
        }
        else
        {
            pageUpButton.OnClick.AddListener(PageUp);
        }
        if (pageDownButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(pageDownButton));
        }
        else
        {
            pageDownButton.OnClick.AddListener(PageDown);
        }
    }

    /// <summary>
    /// Updates the item frames based on the Items and the current page
    /// </summary>
    private void UpdateDisplays()
    {
        if (Items == null)
        {
            return;
        }

        pageDownButton.Enabled = (page > 0);
        pageUpButton.Enabled = (page < (Items.Length / itemFrames.Length));

        // always go over all itemFrames
        for (int i = 0; i < itemFrames.Length; i++)
        {
            // calculate the index of the frame's element in the Items array
            int index = (page * itemFrames.Length) + i;
            // if the index is in bounds: show the item in the frame
            if (index < Items.Length)
            {
                itemFrames[i].gameObject.SetActive(true);
                itemFrames[i].ItemIndex = index;
                itemFrames[i].UpdateDisplay();

                // check if the frame shows the selected element and set the selection accordingly
                // this also means that we do not need a InteractableToggleCollection; this here does the same but with regard to pages
                if (index == selectedIndex)
                {
                    interactables[i].SetDimensionIndex(1); // select
                }
                else
                {
                    interactables[i].SetDimensionIndex(0); // deselect
                }
            }
            else // out of bounds: deactivate the item frame since we are at the end of the array
            {
                itemFrames[i].ItemIndex = 0;
                itemFrames[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Called by the item frames if they are selected
    /// Passes the selected index and sets it as the current selection
    /// </summary>
    /// <param name="index">The index of the selected element</param>
    public virtual void Select(int index)
    {
        SelectedIndex = index;
        ItemSelected?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Increases the page number and updates the display
    /// </summary>
    public void PageUp()
    {
        page++;
        page = Mathf.Min(page, Items.Length / itemFrames.Length);
        UpdateDisplays();
    }

    /// <summary>
    /// Decreases the page number and updates the display
    /// </summary>
    public void PageDown()
    {
        page--;
        page = Mathf.Max(page, 0);
        UpdateDisplays();
    }
}
