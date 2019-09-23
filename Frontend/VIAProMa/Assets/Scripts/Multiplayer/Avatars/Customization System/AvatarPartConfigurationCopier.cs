using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPartConfigurationCopier : MonoBehaviour
{
    [SerializeField] private GameObject observedPart;

    private IConfigurationController observedPartController;

    private IConfigurationController localPartController;


    private void Awake()
    {
        localPartController = GetComponent<IConfigurationController>();

        if (observedPart == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(observedPart));
        }
        else
        {
            observedPartController = observedPart.GetComponent<IConfigurationController>();
            if (observedPartController == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IConfigurationController), observedPart);
            }
            else
            {
                observedPartController.ConfigurationChanged += OnConfigurationChanged;
            }
        }
    }

    private void OnDestroy()
    {
        if (observedPartController != null)
        {
            observedPartController.ConfigurationChanged -= OnConfigurationChanged;
        }
    }

    private void OnConfigurationChanged(object sender, EventArgs e)
    {
        Debug.Log("Observed configuration changed");
        localPartController.AvatarIndex = observedPartController.AvatarIndex;
        localPartController.ModelIndex = observedPartController.ModelIndex;
        localPartController.MaterialIndex = observedPartController.MaterialIndex;
        localPartController.ColorIndex = observedPartController.ColorIndex;
        localPartController.ApplyConfiguration();
    }
}
