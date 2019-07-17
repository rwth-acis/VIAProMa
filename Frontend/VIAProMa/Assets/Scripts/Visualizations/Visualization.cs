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
    /// The data content provider for this visualization
    /// </summary>
    private IVisualizationContentProvider visualizationContentProvider;

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
