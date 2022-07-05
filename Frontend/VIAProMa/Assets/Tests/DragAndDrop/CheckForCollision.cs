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

// This script needs a rigit body for the Trigger function to work
[RequireComponent(typeof(Rigidbody))]
public class CheckForCollision : MonoBehaviour
{
    [SerializeField] float distance = 0.3f;
    [SerializeField] Vector3 direction = Vector3.forward;
    [SerializeField] Color rayColor = Color.white;

    [SerializeField] bool _isInRayMode = true;

    public bool isInRayMode
    {
        get => _isInRayMode;
        set
        {
            _isInRayMode = value;
            UpdateRayVisual();
        }
    }

    [SerializeField]
    protected LineRenderer raycastLine;

    #region Overlap Events
    [Header("Overlap Events")]
    public ColliderEvent RaycastHitEvent = new ColliderEvent();

    public ColliderEvent RaycastHitStopEvent = new ColliderEvent();
    #endregion Overlap Events

    bool rayHit;
    GameObject currentColliderObject;
    bool lastRaycastResult;


    // Start is called before the first frame update
    void Awake()
    {
        //RaycastHitEvent.AddListener(x => Debug.Log(this.name + " ++ recieved hit from " + x.name));
        //RaycastHitStopEvent.AddListener(x => Debug.Log(this.name + " -- no longer hits " + x.name));
        UpdateRayVisual();

        //Make it so object is not influenced by forces
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;

    }

    void UpdateRayVisual()
    {
        if (raycastLine)
        {
            raycastLine.enabled = _isInRayMode;
        }
        else if (_isInRayMode)
        {
            Debug.LogWarning("The Component " + name + " should have a Line Renderer to see the raycast line.");
        }
    }

    private void FixedUpdate()
    {
        if(_isInRayMode)
        {
            TestForRayHit();
        }
    }
    // Update is called once per frame
    //void Update()
    //{
    //}

    public void OnTriggerEnter(Collider potentialTarget)
    {
        if (!_isInRayMode)
        {
            RaycastHitEvent.Invoke(potentialTarget.gameObject);
        }
    }
    public void OnTriggerExit(Collider potentialTarget)
    {
        if (!_isInRayMode)
        {
            RaycastHitStopEvent.Invoke(potentialTarget.gameObject);
        }
    }

    void TestForRayHit()
    {
        RaycastHit raycastHit;

        // checks if there is an object that is colliding with the ray,
        // if there is it saves that object in "raycastHit"
        if(raycastLine)
        {
            rayHit = Physics.Raycast(transform.position + raycastLine.GetPosition(0), direction, out raycastHit, distance);
        }
        else
        {
            rayHit = Physics.Raycast(transform.position, direction, out raycastHit, distance);
        }


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
            Vector3 worldRayEnd = transform.position + raycastLine.GetPosition(0) + (direction.normalized * distance);//This needs to be in WORLD space for once, and not local space
            raycastLine.SetPosition(1, raycastLine.transform.worldToLocalMatrix * ((Vector4)worldRayEnd + new Vector4(0, 0, 0, 1)));
            raycastLine.startColor = rayColor;
            raycastLine.endColor = rayColor;
        }
        //Debug.DrawRay(transform.position, direction*distance, rayColor);
    }

}
