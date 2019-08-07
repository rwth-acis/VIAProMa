using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CompetenceBarController : MonoBehaviour
{
    [SerializeField] private Transform bar;
    [SerializeField] private TextMeshPro[] textMeshes;

    private float length = 1f;
    private float thickness = 0.12f;
    private string text = "";
    private Color color = Color.white;

    private Renderer rend;

    public float Length
    {
        get => length;
        set
        {
            length = value;
            UpdateVisuals();
        }
    }

    public float Thickness
    {
        get => thickness;
        set
        {
            thickness = value;
            UpdateVisuals();
        }
    }

    public float Height
    {
        get { return Mathf.Sqrt(3) * thickness / 2f; }
    }

    public string Text
    {
        get => text;
        set
        {
            text = value;
            UpdateVisuals();
        }
    }

    public Color Color
    {
        get => color;
        set
        {
            color = value;
            UpdateVisuals();
        }
    }

    private void Awake()
    {
        rend = bar.gameObject.GetComponent<Renderer>();
        if (bar == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(bar));
        }
        if (textMeshes.Length == 0)
        {
            SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, nameof(textMeshes));
        }
        for (int i = 0; i < textMeshes.Length; i++)
        {
            if (textMeshes[i] == null)
            {
                SpecialDebugMessages.LogArrayMissingReferenceError(this, nameof(textMeshes), i);
            }
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        bar.localScale = new Vector3(
                thickness,
                thickness,
                length
                );
        for (int i = 0; i < textMeshes.Length; i++)
        {
            textMeshes[i].rectTransform.sizeDelta = new Vector2(length, thickness);
            // cache height so that it does not need to be re-calculated
            float height = Height;
            // determine the side
            float side = Mathf.Sign(textMeshes[i].rectTransform.localPosition.x);

            // get the coordinates of the face middle
            Vector2 bottomRightCorner = new Vector2(thickness / 2f, -height / 2f);
            Vector2 topCorner = new Vector2(0f, height / 2f);
            Vector2 faceMiddle2D = Vector2.Lerp(bottomRightCorner, topCorner, 0.5f);
            // also calculate normal
            Vector2 bottomToTopCorner = topCorner - bottomRightCorner;
            //bottomToTopCorner.x *= -side;
            Vector3 normal = Vector3.Cross(Vector3.back, bottomToTopCorner).normalized;

            // position text
            textMeshes[i].rectTransform.localPosition = new Vector3(side * faceMiddle2D.x, faceMiddle2D.y, length / 2f);
            // apply text
            textMeshes[i].text = text;
            // automatically change text color for dark and light backgrounds
            if (color.grayscale < 0.5f)
            {
                textMeshes[i].color = Color.white;
            }
            else
            {
                textMeshes[i].color = Color.black;
            }
        }
        rend.material.color = color;
    }
}
