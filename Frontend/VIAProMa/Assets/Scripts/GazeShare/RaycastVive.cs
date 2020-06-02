﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class RaycastVive : MonoBehaviour
{
    protected bool isUsingVive;
    protected Vector3 far = new Vector3(0f, -10f, 0f);
    [HideInInspector] public static Vector3 pointerHitPosition { get; private set; }
    [HideInInspector] public static Quaternion pointerHitRotation { get; private set; }
    [HideInInspector] public static GameObject objectBeingHit { get; private set; }
    // Bit mask for raycasting layers
    private int layerMask;

    // Start is called before the first frame update
    void Start()
    {
        pointerHitPosition = new Vector3(0f, -10f, 0f);
        // Bit shift the index of the layer (8) to get a bit mask. This would cast rays only against colliders in layer 8.
        layerMask = 1 << 8;
    }

    // Update is called once per frame
    void Update()
    {
        if (StaticGaze.GetIsUsingVive() == true) // might not be needed
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out raycastHit, Mathf.Infinity, layerMask))
            {
                objectBeingHit = raycastHit.collider.gameObject;
                pointerHitPosition = raycastHit.point;
                pointerHitRotation = Quaternion.LookRotation(raycastHit.normal);
                Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * raycastHit.distance, Color.yellow);
                Debug.Log("Did hit " + objectBeingHit.name);
            }
            else
            {
                Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 1000, Color.white);
                Debug.Log("Did not hit");
            }
        }
    }
}
