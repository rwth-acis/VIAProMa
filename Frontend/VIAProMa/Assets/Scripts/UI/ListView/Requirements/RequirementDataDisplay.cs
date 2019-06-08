using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RequirementDataDisplay : DataDisplay<Requirement>
{
    [SerializeField] private TextMeshPro titleField;
    [SerializeField] private TextMeshPro descriptionField;

    public override void UpdateView()
    {
        base.UpdateView();
        titleField.text = content.name;
        descriptionField.text = content.description;
    }
}
