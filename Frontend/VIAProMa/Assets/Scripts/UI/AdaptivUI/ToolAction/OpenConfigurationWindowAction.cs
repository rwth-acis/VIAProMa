using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class OpenConfigurationWindowAction : ActionHelperFunctions
{
    public void OpenConfigurationWindow(BaseInputEventData eventData)
    {
        GameObject target = GetVisualisationFromGameObject(eventData.InputSource.Pointers[0].Result.CurrentPointerTarget, new Type[] { typeof(ConfigurationWindow) }, true, false);
        if (target != null)
        {
            ConfigurationWindow configurationWindow = target.transform.GetComponentInChildren<ConfigurationWindow>(true);
            if (configurationWindow != null)
            {
                configurationWindow.Open();
                RotateToCameraOnXZPlane(configurationWindow.gameObject, eventData.InputSource.Pointers[0].Position);
            }
        }
    }
}
