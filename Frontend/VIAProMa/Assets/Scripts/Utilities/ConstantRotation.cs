using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [SerializeField] private Vector3 rotationVector = Vector3.up;

    public Vector3 RotationVector { get => rotationVector; set => rotationVector = value; }

    private void Update()
    {
        transform.localEulerAngles += rotationVector * Time.deltaTime;
    }
}
