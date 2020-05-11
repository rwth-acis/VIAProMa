using System.Collections;
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
    [HideInInspector] public Vector3 pointerHitPosition { get; private set; }
    [HideInInspector] public Quaternion pointerHitRotation { get; private set; }
    [HideInInspector] public GameObject objectBeingHit { get; private set; }

    public void Start()
    {
        pointerHitPosition = new Vector3(0f, -10f, 0f);
    }

    /// <summary>
    /// Checks for the input source type of the detected controllers
    /// </summary>
    /// <remarks>
    /// Change InputSourceType.Controller to InputSourceType.Hand for testing in unity editor
    /// In unity editor use gesture hand from input simulation service found in the mixed reality toolkit to test as controller of HTC Vive
    /// </remarks>
    /// <returns>True if user is using HTC Vive and False if using Hololens</returns>
    protected bool GetIsUsingVive()
    {
        isUsingVive = false;
        foreach (IMixedRealityController controller in MixedRealityToolkit.InputSystem.DetectedControllers)
        {
            if (controller.InputSource.SourceType == InputSourceType.Controller)
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
        if (GetIsUsingVive() == true)
        {
            foreach (GameObject controller in GetAllGameObjectsWithArrowScriptTesting2())
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

    protected GameObject[] GetAllGameObjectsWithArrowScriptTesting2()
    {
        GameObject[] arrayAll = GameObject.FindGameObjectsWithTag("showArrow");
        return arrayAll;
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData) {}

    public void OnPointerDragged(MixedRealityPointerEventData eventData) {}

    public void OnPointerUp(MixedRealityPointerEventData eventData) {}
}
