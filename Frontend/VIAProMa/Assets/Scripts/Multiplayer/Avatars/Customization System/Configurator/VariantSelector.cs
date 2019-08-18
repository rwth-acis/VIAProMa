using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantSelector : MonoBehaviour
{
    [SerializeField] protected InteractableToggleCollection selectionFrameCollection;
    [SerializeField] protected ItemFrame[] itemFrames;

    protected int page = 0;

    public List<IItem> Items { get; set; }

    protected virtual void Awake()
    {
        // initialize the toggle collection
        List<Interactable> interactables = new List<Interactable>();
        foreach (Transform child in selectionFrameCollection.transform)
        {
            Interactable interactable = child.gameObject.GetComponent<Interactable>();
            if (interactable == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(Interactable), child.gameObject);
            }
            else
            {
                interactables.Add(interactable);
            }
        }
        selectionFrameCollection.ToggleList = interactables.ToArray();

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
            if (index < Items.Count)
            {
                itemFrames[i].gameObject.SetActive(true);
                itemFrames[i].ItemIndex = (page * itemFrames.Length) + i;
                itemFrames[i].UpdateDisplay();
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
    }
}
