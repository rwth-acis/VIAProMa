﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class TopBarHighlight : MonoBehaviour , IMixedRealityFocusHandler
{
    [SerializeField] Renderer backgroundRenderer;

    private Color defaultColor;
    private Color highlightColor = Color.red;

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
