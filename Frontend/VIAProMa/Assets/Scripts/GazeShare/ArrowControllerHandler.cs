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
    protected Vector3 up = new Vector3(0f, 0.1f, 0f);
    [HideInInspector] public Vector3 pointerHitPosition = new Vector3(0f, -10f, 0f);

    protected bool getIsUsingVive2()
    {
        isUsingVive = false;
        foreach (IMixedRealityController controller in MixedRealityToolkit.InputSystem.DetectedControllers)
        {
            //Debug.Log("Controller is : " + controller.InputSource.SourceType);
            if (controller.InputSource.SourceType == InputSourceType.Controller)
            {
                isUsingVive = true;
            }
        }
        return isUsingVive;
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        //Debug.Log("5");
        //Debug.Log("isUsing :" + getIsUsingVive());
        if (getIsUsingVive2() == true)
        {
            var result = eventData.Pointer.Result;
            //Debug.Log("result :" + result.Details.Point);
            //Debug.Log("name : " + gameObject.name);
            if (result != null)
            {
                //Debug.Log("Test");
                pointerHitPosition = result.Details.Point + up;
            }
            else { pointerHitPosition = far; }
        }
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
