﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

/// <summary>
/// Handles the controllers/pointers interaction when using the HTC Vive
/// </summary>
public class ArrowControllerHandler : MonoBehaviour, IMixedRealityPointerHandler
{
    protected bool isUsingVive;
    protected Vector3 far = new Vector3(0f, -10f, 0f);
    [HideInInspector] public Vector3 pointerHitPosition;
    [HideInInspector] public Quaternion pointerHitRotation;
    [HideInInspector] public GameObject objectBeingHit;

    public void Start()
    {
        pointerHitPosition = new Vector3(0f, -10f, 0f);
    }

    /// <summary>
    /// Checks for the input source type of the detected controllers
    /// </summary>
    /// <returns>True if user is using HTC Vive and False if using Hololens</returns>
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

    /// <summary>
    /// Checks if HTC Vive is being used, if so goes trough all the possible
    /// objects that can be clicked, finds the one that has been hit and sets
    /// the public variables pointerHitPosition, pointerHitRotation and objectBeingHit
    /// </summary>
    /// <param name="eventData"> Data from a click Input Event</param>
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (getIsUsingVive2() == true)
        {
            foreach (GameObject controller in getAllGameObjectsWithArrowScriptTesting2())
            {
                if (controller.name != gameObject.name)
                {
                    controller.GetComponent<ArrowControllerHandler>().pointerHitPosition = far;
                }
                else
                {
                    pointerHitPosition = eventData.Pointer.Result.Details.Point;
                    objectBeingHit = eventData.Pointer.Result.Details.Object;
                    var result = eventData.Pointer.Result;
                    pointerHitRotation = Quaternion.LookRotation(result.Details.Normal);
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
