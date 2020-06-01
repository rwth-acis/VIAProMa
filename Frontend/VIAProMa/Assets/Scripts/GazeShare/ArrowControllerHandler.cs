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

    public void Update()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out raycastHit, Mathf.Infinity, LayerMask.GetMask("Pointable")))
        {

            objectBeingHit = raycastHit.collider.gameObject;
            //pointerHitPosition = raycastHit.point;
            //var result = raycastHit.point;
            //pointerHitRotation = Quaternion.LookRotation(result.normalized);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * raycastHit.distance, Color.yellow);
            Debug.Log("Did hit " + objectBeingHit.name);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not hit");
        }
    }

    /// <summary>
    /// Checks if HTC Vive is being used, if so goes trough all the possible
    /// objects that can be clicked, finds the one that has been hit and sets
    /// the public variables pointerHitPosition, pointerHitRotation and objectBeingHit
    /// </summary>
    /// <param name="eventData"> Data from a click Input Event</param>
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (StaticGaze.GetIsUsingVive() == true)
        {
            foreach (GameObject controller in GetAllGameObjectsWithArrowScript())
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

    protected GameObject[] GetAllGameObjectsWithArrowScript()
    {
        GameObject[] arrayAll = GameObject.FindGameObjectsWithTag("showArrow");
        return arrayAll;
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData) { }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }
}