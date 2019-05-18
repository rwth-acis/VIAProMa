using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectArray : MonoBehaviour
{
    public Vector3 offset;

    private void Update()
    {
        for (int i=0;i<transform.childCount; i++)
        {
            transform.GetChild(i).localPosition = offset * i;
        }
    }

    private void OnValidate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localPosition = offset * i;
        }
    }
}