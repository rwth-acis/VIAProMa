using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class TopBarHighlight : MonoBehaviour , IMixedRealityFocusHandler
{
    private Renderer backgroundRenderer;

    private Color defaultColor;
    [SerializeField] private Color highlightColor;

    private void Awake()
    {
        backgroundRenderer = gameObject.GetComponent<Renderer>();
        defaultColor = backgroundRenderer.material.color;

    }


    public void OnFocusEnter(FocusEventData eventData)
    {
        backgroundRenderer.material.color = highlightColor;
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        backgroundRenderer.material.color = defaultColor;
    }

    // Use this for initialization
    void Start()
    {
        backgroundRenderer.material.color = defaultColor;
    }
}
