using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// event that returns a gameObject if a collider was hit.
/// </summary>
[Serializable]
public class ColliderEvent : UnityEvent<GameObject> { }

public class CheckForCollision : MonoBehaviour
{
    [SerializeField] float distance = 0.3f;
    [SerializeField] Vector3 direction = Vector3.forward;

    public bool isInRayMode = true;

    [SerializeField]
    protected LineRenderer raycastLine;

    [Header("Overlap Start Events")]
    public ColliderEvent RaycastHitEvent = new ColliderEvent();

    [Header("Overlap End Events")]
    public ColliderEvent RaycastHitStopEvent = new ColliderEvent();


    bool rayHit;
    GameObject currentColliderObject;
    bool lastRaycastResult;
    Color rayColor = Color.white;


    // Start is called before the first frame update
    void Start()
    {
        //RaycastHitEvent.AddListener(x => Debug.Log(this.name + " ++ recieved hit from " + x.name));
        //RaycastHitStopEvent.AddListener(x => Debug.Log(this.name + " -- no longer hits " + x.name));

        if(raycastLine)
        {
            raycastLine.enabled = isInRayMode;
        }
        else if(isInRayMode)
        {
            Debug.LogWarning("The Component " + name + " should probably have a Line Renderer to see the raycast line.");
        }
    }

    private void FixedUpdate()
    {
        if(isInRayMode)
        {
            TestForRayHit();
        }
    }
    // Update is called once per frame
    //void Update()
    //{
    //}

    private void OnTriggerEnter(Collider potentialTarget)
    {
        if (!isInRayMode)
        {
            RaycastHitEvent.Invoke(potentialTarget.gameObject);
        }
    }
    private void OnTriggerExit(Collider potentialTarget)
    {
        if (!isInRayMode)
        {
            RaycastHitStopEvent.Invoke(potentialTarget.gameObject);
        }
    }

    void TestForRayHit()
    {
        RaycastHit raycastHit;

        // checks if there is an object that is colliding with the ray,
        // if there is it saves that object in "raycastHit"
        rayHit = Physics.Raycast(transform.position, direction, out raycastHit, distance);

        if(lastRaycastResult != rayHit)
        {
            lastRaycastResult = rayHit;
            // here we need to test if the object the ray is colliding with is an object
            // on which we can use drag and drop on
            if (rayHit)
            {
                rayColor = Color.green;
                currentColliderObject = raycastHit.collider.gameObject;
                RaycastHitEvent.Invoke(currentColliderObject);
            }
            else
            {
                rayColor = Color.red;
                RaycastHitStopEvent.Invoke(currentColliderObject);
                currentColliderObject = null;
            }
        }

        //Show Ray
        if (raycastLine != null)
        {
            Vector3 worldHitLocation = transform.position + raycastLine.GetPosition(0) + (direction.normalized * distance);//This needs to be in WORLD space for once, and not local space
            raycastLine.SetPosition(1, raycastLine.transform.worldToLocalMatrix * ((Vector4)worldHitLocation + new Vector4(0, 0, 0, 1)));
            raycastLine.startColor = rayColor;
            raycastLine.endColor = rayColor;
        }
        //Debug.DrawRay(transform.position, direction*distance, rayColor);
    }

}
