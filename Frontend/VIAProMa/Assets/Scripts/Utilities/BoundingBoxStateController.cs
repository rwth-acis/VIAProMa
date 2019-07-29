using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundingBox))]
public class BoundingBoxStateController : MonoBehaviour
{
    private BoundingBox boundingBox;
    private BoxCollider boxCollider;
    private ManipulationHandler manipulationHandler;
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
        boundingBox = GetComponent<BoundingBox>();
        if (boundingBox == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoundingBox), gameObject);
        }
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(boxCollider), gameObject);
        }
        manipulationHandler = GetComponent<ManipulationHandler>();
        // manipulation handler is optional, so no check here
    }

    private void Start()
    {
        BoundingBoxActive = false;
    }

    private void SetBoundingBoxState()
    {
        boxCollider.enabled = boundingBoxActive;
        boundingBox.Active = boundingBoxActive;
        boundingBox.Refresh();
        if (manipulationHandler != null)
        {
            manipulationHandler.enabled = boundingBoxActive;
        }
    }
}
