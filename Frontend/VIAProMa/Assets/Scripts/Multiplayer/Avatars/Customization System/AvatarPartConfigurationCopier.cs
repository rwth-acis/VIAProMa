using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AvatarPartConfigurationController))]
public class AvatarPartConfigurationCopier : MonoBehaviour
{
    [SerializeField] private AvatarPartConfigurationController observedPartController;

    private AvatarPartConfigurationController localPartController;


    private void Awake()
    {
        localPartController = GetComponent<AvatarPartConfigurationController>();
        observedPartController.ConfigurationChanged += OnConfigurationChanged;
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
