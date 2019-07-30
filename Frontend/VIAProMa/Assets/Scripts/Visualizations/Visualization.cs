using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for visualizations
/// </summary>
public class Visualization : MonoBehaviour, IViewContainer
{
    /// <summary>
    /// The visual controller of the visualization
    /// It determins its visual appearance
    /// </summary>
    protected IVisualizationVisualController visualController;
    /// <summary>
    /// A color changer script which is responsible for exchanging colors on elements of the visualization
    /// </summary>
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

    /// <summary>
    /// The primary color for the visualization
    /// </summary>
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

    /// <summary>
    /// Gets the components
    /// </summary>
    protected virtual void Awake()
    {
        visualController = GetComponent<IVisualizationVisualController>();
        // color changer is optional, so no check if it was found
        // just try to fetch it
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

    /// <summary>
    /// Called if the content provider was changed
    /// </summary>
    protected virtual void OnContentProviderChanged()
    {
        UpdateView();
    }

    /// <summary>
    /// Updates the view of the visualization
    /// Invokes the VisualizationUpdated event
    /// Overwrite this method for custom behaviour on the update view
    /// </summary>
    public virtual void UpdateView()
    {
        VisualizationUpdated?.Invoke(this, EventArgs.Empty);
    }
}
