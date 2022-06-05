using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckObjectInDirection : MonoBehaviour
{
    private bool inRay;
    [SerializeField] float distance = 0.7f;
    [SerializeField] Vector3 direction = Vector3.forward;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        InRay();
    }
    // Update is called once per frame
    //void Update()
    //{
    //}

    void InRay()
    {
        Color rayColor;
        RaycastHit raycastHit;

        // checks if there is an object that is colliding with the ray,
        // if there is it saves that object in "raycastHit"
        inRay = Physics.Raycast(transform.position, direction, out raycastHit, distance);

        // here we need to test if the object the ray is colliding with is an object
        // on which we can use drag and drop on
        if (inRay)
        {
            rayColor = Color.green;
            inRay = true;
            Debug.Log(raycastHit.collider.ToString());
        } else {
            rayColor = Color.red;
            inRay = false;
        }
        Debug.DrawRay(transform.position, direction*distance, rayColor);
    }
}
