using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class ArrowControllerHandler : MonoBehaviour, IMixedRealityPointerHandler
{
    protected bool isUsingVive;
    protected Vector3 far = new Vector3(0f, -10f, 0f);
    [HideInInspector] public Vector3 pointerHitPosition;
    [HideInInspector] public GameObject objectBeingHit;

    public void Start()
    {
        pointerHitPosition = new Vector3(0f, -10f, 0f);
    }

    protected bool getIsUsingVive2()
    {
        isUsingVive = false;
        foreach (IMixedRealityController controller in MixedRealityToolkit.InputSystem.DetectedControllers)
        {
            if (controller.InputSource.SourceType == InputSourceType.Hand)
            {
                isUsingVive = true;
            }
        }
        return isUsingVive;
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (getIsUsingVive2() == true)
        {
            foreach (GameObject controller in getAllGameObjectsWithArrowScriptTesting2())
            {
                //Debug.Log("Name " + controller.name + "Position" + controller.GetComponent<ArrowControllerHandler>().pointerHitPosition);
                if (controller.name != gameObject.name)
                {
                    controller.GetComponent<ArrowControllerHandler>().pointerHitPosition = far;
                }
                else
                {
                    pointerHitPosition = eventData.Pointer.Result.Details.Point;
                    objectBeingHit = eventData.Pointer.Result.Details.Object;
                }
            }
        }
    }

    protected GameObject[] getAllGameObjectsWithArrowScriptTesting2()
    {
        GameObject[] arrayAll = GameObject.FindGameObjectsWithTag("showArrow");
        return arrayAll;
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
