using i5.VIAProMa.IssueSelection;
using i5.VIAProMa.UI;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    /// <summary>
    /// Component on the paper which makes the card selectable
    /// </summary>
    [RequireComponent(typeof(PaperDataDisplay))]
    public class PaperSelector : MonoBehaviour, IViewContainer, IMixedRealityPointerHandler
    {
        [SerializeField] private GameObject selectionIndicator;
        [SerializeField] private Renderer backgroundRenderer;

        private PaperDataDisplay paperDataDisplay;
        private bool selected;
        private Color originalRendererColor;

        public Color selectedColor = new Color(0.1698113f, 0.2845136f, 0.6792453f); // blue

        /// <summary>
        /// True if the paper is currently selected
        /// </summary>
        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                UpdateView();
            }
        }
        

        /// <summary>
        /// Checks the component's setup and fetches necessary references
        /// </summary>
        private void Awake()
        {
            if (selectionIndicator == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(selectionIndicator));
            }
            paperDataDisplay = GetComponent<PaperDataDisplay>();
            if (paperDataDisplay == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(paperDataDisplay), gameObject);
            }
            if (backgroundRenderer == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(backgroundRenderer));
            }
            else
            {
                originalRendererColor = backgroundRenderer.material.color;
            }
        }

        /// <summary>
        /// Called if the GameObject is enabled
        /// Registers for the PaperSelectionManager's events
        /// </summary>
        private void OnEnable()
        {
            PaperSelectionManager.Instance.SelectionModeChanged += ReactToChangedSelectionMode;
            PaperSelectionManager.Instance.PaperSelectionChanged += ReactToPaperSelectionChanged;
        }

        /// <summary>
        /// Called when the GameObject is created.
        /// </summary>
        private void Start()
        {
            // check selection mode in start => all other components which use awake should now be set up
            if (PaperSelectionManager.Instance.SelectionModeActive)
            {
                Selected = PaperSelectionManager.Instance.IsSelected(paperDataDisplay);
                UpdateView();
            }
        }

        /// <summary>
        /// Called if the GameObject is disabled
        /// De-registers from the PaperSelectionManager's events
        /// </summary>
        private void OnDisable()
        {
            if (PaperSelectionManager.Instance != null)
            {
                PaperSelectionManager.Instance.SelectionModeChanged -= ReactToChangedSelectionMode;
                PaperSelectionManager.Instance.PaperSelectionChanged -= ReactToPaperSelectionChanged;
            }
        }

        /// <summary>
        /// Called if the paper selection on the PaperSelectionManager is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReactToPaperSelectionChanged(object sender, SelectionChangedArgs<PaperDataDisplay> e)
        {
            if (paperDataDisplay != null)
            {
                if (e.ChangedItem.Equals(paperDataDisplay))
                {
                    Selected = e.Selected;
                }
            }
        }

        /// <summary>
        /// Called if the selection mode on the PaperSelectionManager is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReactToChangedSelectionMode(object sender, EventArgs e)
        {
            if (PaperSelectionManager.Instance.SelectionModeActive) // selection mode was just activated
            {
                Selected = PaperSelectionManager.Instance.IsSelected(paperDataDisplay);
            }
            else // selection mode has ended
            {
                selectionIndicator.SetActive(false);
                backgroundRenderer.material.color = originalRendererColor;
            }
        }

        /// <summary>
        /// Toggles the selection of the paper of this card
        /// </summary>
        public void ToggleSelection()
        {
            // report selection or deselection to selection manager
            if (Selected)
            {
                PaperSelectionManager.Instance.SetDeselected(paperDataDisplay);
            }
            else
            {
                PaperSelectionManager.Instance.SetSelected(paperDataDisplay);
            }
            // do not update the selection visuals here; they will be updated by the selection manager through its PaperSelectionChanged event
        }

        /// <summary>
        /// Updates the visual selection indiciation on the card
        /// </summary>
        public void UpdateView()
        {
            if (PaperSelectionManager.Instance.SelectionModeActive)
            {
                selectionIndicator.SetActive(Selected);
                if (Selected)
                {
                    backgroundRenderer.material.color = selectedColor;
                }
                else
                {
                    backgroundRenderer.material.color = originalRendererColor;
                }
            }
            else
            {
                selectionIndicator.SetActive(false);
                backgroundRenderer.material.color = originalRendererColor;
            }
        }

        #region Un-used but required by interface
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
        }
        #endregion

        /// <summary>
        /// Called by the Mixed Reality Toolkit if the object was clicked
        /// </summary>
        /// <param name="eventData">The event data of the interaction</param>
        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            if (PaperSelectionManager.Instance.SelectionModeActive)
            {
                ToggleSelection();
                eventData.Use();
            }
        }
    }
}
