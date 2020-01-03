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

    private LineDrawLogic linedrawscript;

    private void Awake()
    {
        linedrawscript = GameObject.FindGameObjectWithTag("LineDraw").GetComponent<LineDrawLogic>();
        if (visualizationPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(visualizationPrefab));
        }
    }

    /// <summary>
    /// Called by the Mixed Reality Toolkit if the object was clicked
    /// If the LineDraw mode is active, the transform component of the object is saved as either start or destination.
    /// </summary>
    /// <param name="eventData">The event data of the interaction</param>
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (linedrawscript.isLineModeActivated)
        {
            if (!linedrawscript.oneSelected)
            {
                linedrawscript.start = transform;
                linedrawscript.oneSelected = true;
            }
            else
            {
                linedrawscript.destination = transform;
                linedrawscript.oneSelected = false;
            }
        }
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
