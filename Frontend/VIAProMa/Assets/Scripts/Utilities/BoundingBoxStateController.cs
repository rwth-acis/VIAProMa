using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System;
using UnityEngine;

namespace i5.VIAProMa.Utilities
{
    [RequireComponent(typeof(BoundsControl))]
    public class BoundingBoxStateController : MonoBehaviour
    {
        private BoundsControl boundingBox;
        private BoxCollider boxCollider;
        private ObjectManipulator manipulationHandler;
        private bool boundingBoxActive;

        public event EventHandler BoundingBoxStateChanged;

        public bool BoundingBoxActive
        {
            get => boundingBoxActive;
            set
            {
                boundingBoxActive = value;
                SetBoundingBoxState();
                BoundingBoxStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Awake()
        {
            boundingBox = GetComponent<BoundsControl>();
            if (boundingBox == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoundingBox), gameObject);
            }
            boxCollider = GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoxCollider), gameObject);
            }
            manipulationHandler = GetComponent<ObjectManipulator>();
            // manipulation handler is optional, so no check here
        }

        private void Start()
        {
            if (boundingBoxActive == false) // if the variable is already true, this means that another script set the property
            {
                BoundingBoxActive = false;
            }
        }

        private void SetBoundingBoxState()
        {
            if (boxCollider != null)
            {
                boxCollider.enabled = boundingBoxActive;
            }
            boundingBox.Active = boundingBoxActive;
            if (manipulationHandler != null)
            {
                manipulationHandler.enabled = boundingBoxActive;
            }
        }
    }
}