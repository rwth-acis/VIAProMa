using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextSize : Singleton<TextSize>
{
    private TextMesh textMesh;

    private Renderer textMeshRenderer;

    protected override void Awake()
    {
        base.Awake();

        textMesh = GetComponent<TextMesh>();

        // the textMesh should not be visible in the scene
        textMesh.gameObject.SetActive(false);

        if (textMesh == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(TextMesh), gameObject);
            return;
        }
        textMesh.transform.position = Vector3.zero;
        textMesh.transform.rotation = Quaternion.identity;

        textMeshRenderer = textMesh.gameObject.GetComponent<Renderer>();
    }

    public Vector2 GetTextSize(string text, int fontSize)
    {
        textMesh.fontSize = fontSize;
        textMesh.text = text;
        return textMeshRenderer.bounds.size;
    }
}
