using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnvironmentDataDisplay : DataDisplay<EnvironmentData>
{
    [SerializeField] private TextMeshPro environmentNameLabel;
    [SerializeField] private TextMeshPro previewImage;


    private Interactable button;

    private void Awake()
    {
        if (environmentNameLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(environmentNameLabel));
        }
        if (previewImage == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(previewImage));
        }
    }

    public override void Setup(EnvironmentData content)
    {
        button = GetComponent<Interactable>();
        if (button == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(Interactable), gameObject);
        }
        base.Setup(content);
    }

    public override void UpdateView()
    {
        base.UpdateView();
    }
}
