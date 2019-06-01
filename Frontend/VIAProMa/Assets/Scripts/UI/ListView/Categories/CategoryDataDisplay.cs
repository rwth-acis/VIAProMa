using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CategoryDataDisplay : DataDisplay<Category>
{
    [SerializeField] private TextMeshPro label;

    public override void UpdateView()
    {
        base.UpdateView();
        label.text = content.name;
    }
}
