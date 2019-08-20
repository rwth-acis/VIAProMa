using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Displays an IItem in an item frame for selection
/// Item frames work in combination with an overspanning VariantSelector
/// </summary>
public class ItemFrame : MonoBehaviour
{
    [Tooltip("The sprite renderer which shows the icon")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private VariantSelector selector;

    /// <summary>
    /// The index of this item in the collection where it is from
    /// The items are distributed from a collection to individual item frames
    /// The item frames only store the index and report this back to the selector which is responsible for finding the corresponding item
    /// </summary>
    public int ItemIndex
    {
        get; set;
    }

    /// <summary>
    /// Checks the setup
    /// </summary>
    private void Awake()
    {
        if (spriteRenderer == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(spriteRenderer));
        }
    }

    /// <summary>
    /// Initializes the component by setting the selector
    /// The selector must be set so that the item can be found in its collection
    /// </summary>
    /// <param name="selector"></param>
    public void Setup(VariantSelector selector)
    {
        this.selector = selector;
    }

    /// <summary>
    /// Updates the item frame's display
    /// Fetches the item from the selector collection and displays its icon
    /// </summary>
    public void UpdateDisplay()
    {
        spriteRenderer.sprite = selector.Items[ItemIndex].Sprite;
    }

    /// <summary>
    /// Called if the user selects this item frame
    /// Reports the selection back to the selector
    /// </summary>
    public void Select()
    {
        selector.Select(ItemIndex);
    }
}
