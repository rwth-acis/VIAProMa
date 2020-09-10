using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deleteThis : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Vector3.SignedAngle(Vector3.forward, Vector3.right,Vector3.up));
        Debug.Log(Vector3.SignedAngle(Vector3.right, Vector3.forward, Vector3.up));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
