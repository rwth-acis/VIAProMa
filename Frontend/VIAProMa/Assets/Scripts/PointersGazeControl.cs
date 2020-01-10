using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class PointersGazeControl : MonoBehaviour, /*IMixedRealityFocusHandler*/ IMixedRealityPointerHandler
{
    public GameObject arrow;
    private Transform t;
    private Vector3 positionOnCube = new Vector3(0f, 0f, 0f);
    /*public void OnFocusEnter(FocusEventData eventData)
    {
        var result = eventData.Pointer.Result;
        if (result != null)
        {
            positionOnCube = result.Details.Point;
        }
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        arrow.transform.position = new Vector3(0f, 10f, 0f);
    }*/

    // Could be a feature, that you can choose if you want your gaze to be always on or click to show current point
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        var result = eventData.Pointer.Result;
        if (result != null)
        {
            positionOnCube = result.Details.Point;
        } else { positionOnCube = new Vector3(0f, 0f, 0f); }
        Debug.Log("Click");
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        Debug.Log("Down");
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        Debug.Log("Dragged");
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        Debug.Log("Up");
    }

    void Start()
    {
        t = arrow.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (positionOnCube.x != 0f && positionOnCube.y != 0f)
        {
            t.position = positionOnCube;
        }
    }
}
