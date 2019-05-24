using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StringDataDisplay : DataDisplay<StringData>
{
    [SerializeField] private TextMeshPro label;

    public override void UpdateView()
    {
        base.UpdateView();
        label.text = content.text;
    }
}
