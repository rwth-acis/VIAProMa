using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPreviewSquare : MonoBehaviour
{
    [SerializeField] private Renderer previewQuad;

    public Color Color
    {
        get
        {
            return previewQuad.material.color;
        }
        set
        {
            previewQuad.material.color = value;
        }
    }

    public int ColorIndex
    {
        get; set;
    }

    private void Awake()
    {
        if (previewQuad == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(previewQuad));
        }
    }
}
