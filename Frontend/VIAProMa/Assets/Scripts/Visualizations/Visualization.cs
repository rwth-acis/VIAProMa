﻿using i5.VIAProMa.UI;
using System;
using UnityEngine;

namespace i5.VIAProMa.Visualizations
{
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
                TitleChanged?.Invoke(this, EventArgs.Empty);
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
                    ColorChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Invoked if the visualization was updated
        /// </summary>
        public event EventHandler VisualizationUpdated;

        /// <summary>
        /// Invoked if the visualization title was changed
        /// </summary>
        public event EventHandler TitleChanged;

        /// <summary>
        /// Invoked if the color of this visualization was changed
        /// </summary>
        public event EventHandler ColorChanged;

        /// <summary>
        /// Sets or gets the VisualizationContentProvider
        /// </summary>
        public IVisualizationContentProvider ContentProvider
        {
            get => visualizationContentProvider;
            set
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
}