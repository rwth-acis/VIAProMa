using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Dark box around camera to reduce colors of items not related to heatmap
/// </summary>
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
