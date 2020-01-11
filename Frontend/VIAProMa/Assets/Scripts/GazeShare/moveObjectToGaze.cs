using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class MoveObjectToGaze : MonoBehaviour, IMixedRealityPointerHandler
{
    private Vector3 up = new Vector3(0f,0.1f,0f);
    private Vector3 far = new Vector3(0f,-10f,0f);
    private bool isSharing = true;
    Material mat;
    public Text txt;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        gameObject.transform.position = far;
    }

    void Update()
    {
        //Debug.Log("Current Pointer something : " + MixedRealityPointerProfile);
        if (giveGaze().GazeTarget && isSharing == true)
        {
            Vector3 currentHitPosition = giveGaze().HitPosition;
            gameObject.transform.position = currentHitPosition + up;
            Vector3 angle = new Vector3(gameObject.transform.eulerAngles.x, giveGaze().GazeDirection.x * 90, gameObject.transform.eulerAngles.z);
            gameObject.transform.eulerAngles = angle;
            txt.text = giveGaze().GazeTarget.name;
            isSharing = true;

        } else
        {
            gameObject.transform.position = far;
        }
    }

    public void toggleSharing()
    {
        if(isSharing == true)
        {
            isSharing = false;
        } else { isSharing = true; }
    }

    public void toggleColor()
    {
        mat.color = Random.ColorHSV();
    }

    public IMixedRealityGazeProvider giveGaze()
    {
        IMixedRealityGazeProvider currentGaze = MixedRealityToolkit.InputSystem.GazeProvider;
        return currentGaze;
        
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        Debug.Log("Testing 1");
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        Debug.Log("Testing 2");
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        /*var result = eventData.Pointer.Result;
        if (result != null)
        {
            gameObject.transform.position = result.Details.Point;
        }*/
        Debug.Log("Testing 3");
        //throw new System.NotImplementedException();
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        Debug.Log("Testing 4");
        //throw new System.NotImplementedException();
    }
}