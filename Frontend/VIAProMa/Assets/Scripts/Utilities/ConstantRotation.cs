using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [SerializeField] private Vector3 rotationVector = Vector3.up;

    public Vector3 RotationVector { get => rotationVector; set => rotationVector = value; }

    private void Update()
    { 
        Vector3 eulerAngles = transform.localEulerAngles + rotationVector * Time.deltaTime;
        transform.localEulerAngles = new Vector3(
            eulerAngles.x % 360,
            eulerAngles.y % 360,
            eulerAngles.z % 360
            );
    }
}
