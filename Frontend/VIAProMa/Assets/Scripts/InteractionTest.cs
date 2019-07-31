using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTest : MonoBehaviour, IMixedRealityPointerHandler
{
    [SerializeField] GameObject prefab;

    private ManipulationHandler handler;

    private void Awake()
    {
        if (prefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(prefab));
        }
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        GameObject instance = ResourceManager.Instance.NetworkInstantiate(prefab, transform.position, transform.rotation);
        handler = instance.GetComponentInChildren<ManipulationHandler>();
        handler.OnPointerDown(eventData);
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        handler.OnPointerDragged(eventData);
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        handler.OnPointerUp(eventData);
    }
}
