﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for visualizations
/// </summary>
public class Visualization : MonoBehaviour, IViewContainer
{
    protected IVisualizationVisualController visualController;
    protected IColorChangeable colorChanger;

    /// <summary>
    /// The data content provider for this visualization
    /// </summary>
    private IVisualizationContentProvider visualizationContentProvider;

    /// <summary>
    /// The title of the visualization
    /// </summary>
    public string Title
    {
        get => visualController.Title;
        set
        {
            visualController.Title = value;
        }
    }

    public virtual Color Color
    {
        get
        {
            if (colorChanger == null)
            {
                return Color.white;
            }
            else
            {
                return colorChanger.Color;
            }
        }
        set
        {
            if (colorChanger != null)
            {
                colorChanger.Color = value;
            }
        }
    }

    /// <summary>
    /// Invoked if the visualization was updated
    /// </summary>
    public event EventHandler VisualizationUpdated;

    /// <summary>
    /// Sets or gets the VisualizationContentProvider
    /// </summary>
    public IVisualizationContentProvider ContentProvider
    {
        get => visualizationContentProvider;
        protected set
        {
            if (visualizationContentProvider != null)
            {
                visualizationContentProvider.ContentChanged -= OnContentChanged; // de-register from old content provider
            }
            visualizationContentProvider = value;
            visualizationContentProvider.ContentChanged += OnContentChanged;
            OnContentProviderChanged();
        }
    }

    protected virtual void Awake()
    {
        visualController = GetComponent<IVisualizationVisualController>();
        colorChanger = GetComponent<IColorChangeable>();
    }

    /// <summary>
    /// Called if the content of the content provider was changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnContentChanged(object sender, EventArgs e)
    {
        UpdateView();
    }

    protected virtual void OnContentProviderChanged()
    {
        UpdateView();
    }

    public virtual void UpdateView()
    {
        VisualizationUpdated?.Invoke(this, EventArgs.Empty);
    }
}
