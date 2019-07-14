using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the object follow the given target transform (position only)
/// </summary>
public class FollowObject : MonoBehaviour
{
    public Transform target;

    /// <summary>
    /// Copies the position of a target transform to this transform
    /// </summary>
    void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
        }
    }
}
