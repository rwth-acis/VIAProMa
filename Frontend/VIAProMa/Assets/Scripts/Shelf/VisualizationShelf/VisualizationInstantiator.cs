using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.UI;

public class VisualizationInstantiator : MonoBehaviour, IMixedRealityPointerHandler
{
    [SerializeField] private GameObject visualizationPrefab;

    private BoundingBoxStateController boxStateController;
    private ManipulationHandler handler;

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
        ResourceManager.Instance.SceneNetworkInstantiate(visualizationPrefab, transform.position, transform.rotation, (instance) =>
        {
            boxStateController = instance.GetComponentInChildren<BoundingBoxStateController>();
            if (boxStateController == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoundingBoxStateController), instance);
            }
            boxStateController.BoundingBoxActive = true;
            handler = instance.GetComponentInChildren<ManipulationHandler>();
            handler.OnPointerDown(eventData);
        });
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        handler.OnPointerDragged(eventData);
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        handler.OnPointerUp(eventData);
        boxStateController.BoundingBoxActive = false;
    }
}
