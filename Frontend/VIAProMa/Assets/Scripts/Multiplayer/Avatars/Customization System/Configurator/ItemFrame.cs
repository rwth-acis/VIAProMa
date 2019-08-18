using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFrame : MonoBehaviour
{
    private IVariantSelector selector;

    private IVariantDisplay display;


    public int ItemIndex
    {
        get; set;
    }

    private void Awake()
    {
        display = GetComponent<IVariantDisplay>();
    }

    public void Setup(IVariantSelector selector)
    {
        this.selector = selector;
    }

    public void UpdateDisplay()
    {
        display.UpdateDisplay();
    }

    public void Select()
    {
        selector.Select(ItemIndex);
    }
}
