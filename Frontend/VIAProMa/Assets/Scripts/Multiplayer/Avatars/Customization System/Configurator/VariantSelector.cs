using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantSelector : MonoBehaviour
{
    [SerializeField] private InteractableToggleCollection selectionFrameCollection;
    [SerializeField] private ItemFrame[] itemFrames;

    private Interactable[] interactables;
    private int page = 0;
    private int selectedIndex;

    public event EventHandler ItemSelected;

    public IItem[] Items { get; set; }

    public int SelectedIndex
    {
        get => selectedIndex;
        set
        {
            selectedIndex = value;
        }
    }

    protected virtual void Awake()
    {
        // initialize the toggle collection
        interactables = new Interactable[itemFrames.Length];
        for (int i = 0; i < itemFrames.Length; i++)
        {
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
        selectionFrameCollection.ToggleList = interactables;

        if (itemFrames.Length == 0)
        {
            SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, nameof(itemFrames));
        }

        for (int i = 0; i < itemFrames.Length; i++)
        {
            itemFrames[i].Setup(this);
        }
    }

    private void UpdateDisplays()
    {
        if (Items == null)
        {
            return;
        }

        for (int i = 0; i < itemFrames.Length; i++)
        {
            int index = (page * itemFrames.Length) + i;
            if (index < Items.Length)
            {
                itemFrames[i].gameObject.SetActive(true);
                itemFrames[i].ItemIndex = index;
                itemFrames[i].UpdateDisplay();

                if (index == selectedIndex)
                {
                    interactables[i].SetDimensionIndex(1); // select
                }
                else
                {
                    interactables[i].SetDimensionIndex(0); // deselect
                }
            }
            else
            {
                itemFrames[i].ItemIndex = 0;
                itemFrames[i].gameObject.SetActive(false);
            }
        }
    }

    public virtual void Select(int index)
    {
        SelectedIndex = index;
        ItemSelected?.Invoke(this, EventArgs.Empty);
    }

    public void PageUp()
    {
        page++;
        page = Mathf.Min(page, Items.Length / itemFrames.Length);
        UpdateDisplays();
    }

    public void PageDown()
    {
        page--;
        page = Mathf.Max(page, 0);
        UpdateDisplays();
    }
}
