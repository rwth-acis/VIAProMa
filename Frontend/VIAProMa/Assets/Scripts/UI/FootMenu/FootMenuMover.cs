using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootMenuMover : MonoBehaviour
{
    public float maxHeight = 1.5f;

    private void Update()
    {
        float heightRatio = 1f - Mathf.Clamp01(transform.parent.position.y / maxHeight);
        float tiltAngle = heightRatio * 90f;

        transform.localEulerAngles = new Vector3(
            tiltAngle,
            transform.localEulerAngles.y,
            0f
            );
    }
}
