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
        Debug.Log("Center: " + center.transform.localPosition);
        Debug.Log("Pointer: " + RaycastVive.pointerHitPosition);
        float distance = Vector2.Distance(centerPosition, RaycastVive.pointerHitPosition)*100f;
        if(distance < 10)
        {
            Debug.Log("Bullseye! ");
        }
        else if (distance < 20)
        {
            Debug.Log("White inner. ");
        }
        else if (distance < 30)
        {
            Debug.Log("Red inner. ");
        }
        else if (distance < 40)
        {
            Debug.Log("White outer... ");
        }
        else if (distance <= 50)
        {
            Debug.Log("Red outer... ");
        }
        //Debug.Log("Time: " + time + "; distance: " + distance.ToString("f"));
        Debug.Log("Timer: " + TimerWindow.elapsedTime + "; distance: " + distance.ToString("f"));
        gameObject.SetActive(false);
        //TimerWindow.timer.Stop();
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
