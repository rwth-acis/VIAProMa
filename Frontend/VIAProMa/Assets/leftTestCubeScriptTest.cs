using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class leftTestCubeScriptTest : MonoBehaviour, /*IMixedRealityFocusHandler*/ IMixedRealityPointerHandler
{
    public GameObject arrow;
    private Vector3 positionOnCube = new Vector3(0f,0f,0f);

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
        }
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

    /*// Start is called before the first frame update
    void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        if(positionOnCube.x != 0f)
        {
            arrow.transform.position = positionOnCube;
        }
    }
}
