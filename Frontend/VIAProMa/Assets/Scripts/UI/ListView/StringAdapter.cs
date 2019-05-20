using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StringAdapter : ListViewItemAdapter<StringData>
{
    [SerializeField] private TextMeshPro label;

    protected override void UpdateView()
    {
        base.UpdateView();
        label.text = content.text;
    }
}
