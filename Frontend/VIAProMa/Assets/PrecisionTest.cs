using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecisionTest : MonoBehaviour, IMixedRealityPointerHandler
{
    [SerializeField] private GameObject center;

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        DateTime time = eventData.EventTime;
        IMixedRealityInputSource source = eventData.InputSource;
        Vector3 centerPosition = center.transform.localPosition;
        float distance = Vector3.Distance(centerPosition, RaycastVive.pointerHitPosition);
        Debug.Log("Treffer! Time: " + time + "; source: " + source + "; distance: " + distance.ToString("f3"));
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData) { }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
