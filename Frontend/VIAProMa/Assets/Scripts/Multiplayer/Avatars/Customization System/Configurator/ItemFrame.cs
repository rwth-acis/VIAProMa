using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFrame : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private VariantSelector selector;


    public int ItemIndex
    {
        get; set;
    }

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(spriteRenderer));
        }
    }

    public void Setup(VariantSelector selector)
    {
        this.selector = selector;
    }

    public void UpdateDisplay()
    {
        spriteRenderer.sprite = selector.Items[ItemIndex].Sprite;
    }

    public void Select()
    {
        selector.Select(ItemIndex);
    }
}
