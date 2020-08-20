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
        //Debug.Log("Center: " + center.transform.localPosition);
        //Debug.Log("Pointer: " + RaycastVive.pointerHitPosition);
        float distance = Vector2.Distance(centerPosition, RaycastVive.pointerHitPosition)*100f;
        if(distance < 10)
        {
            Debug.Log("Bullseye! ");
        }
        else if (distance < 20)
        {
            Debug.Log("White 1! ");
        }
        else if (distance < 30)
        {
            Debug.Log("Red 1! ");
        }
        else if (distance < 40)
        {
            Debug.Log("White 2! ");
        }
        else if (distance <= 50)
        {
            Debug.Log("Red 2! ");
        }
        Debug.Log("Time: " + time + "; distance: " + distance.ToString("f"));
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
