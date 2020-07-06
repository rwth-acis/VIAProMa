using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreyBox : MonoBehaviour
{
    MeshRenderer meshRenderer;
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetVisible(bool value)
    {
        meshRenderer.enabled = value;
    }
}
