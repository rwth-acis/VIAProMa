using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;

public class VisualizationInstantiator : MonoBehaviour, IMixedRealityPointerHandler
{
    [SerializeField] GameObject visualizationPrefab;

    private void Awake()
    {
        if (visualizationPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(visualizationPrefab));
        }
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        ResourceManager.Instance.NetworkInstantiate(visualizationPrefab, transform.position, transform.rotation);
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
    }
}
