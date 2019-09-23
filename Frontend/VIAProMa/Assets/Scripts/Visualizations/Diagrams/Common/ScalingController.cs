using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingController : MonoBehaviour
{
    [SerializeField] private i5.ViaProMa.Visualizations.Common.Diagram diagram;
    [SerializeField] private Vector3 minSize = 0.1f * Vector3.one;
    [SerializeField] private MovementSlider xPosSlider;
    [SerializeField] private MovementSlider xNegSlider;
    [SerializeField] private MovementSlider yPosSlider;
    [SerializeField] private MovementSlider yNegSlider;
    [SerializeField] private MovementSlider zPosSlider;
    [SerializeField] private MovementSlider zNegSlider;

    private void Awake()
    {
        if (diagram == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(diagram));
        }
        if (xPosSlider == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(xPosSlider));
        }
        if (xNegSlider == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(xNegSlider));
        }
        if (yPosSlider == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(yPosSlider));
        }
        if (yNegSlider == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(yNegSlider));
        }
        if (zPosSlider == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(zPosSlider));
        }
        if (zNegSlider == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(zNegSlider));
        }
    }

    private void Start()
    {
        xPosSlider.OnInteractionUpdated.AddListener(UpdateSize);
        xNegSlider.OnInteractionUpdated.AddListener(UpdateSize);
        yPosSlider.OnInteractionUpdated.AddListener(UpdateSize);
        yNegSlider.OnInteractionUpdated.AddListener(UpdateSize);
        zPosSlider.OnInteractionUpdated.AddListener(UpdateSize);
        zNegSlider.OnInteractionUpdated.AddListener(UpdateSize);
    }

    public void UpdateSize()
    {
        xPosSlider.transform.localPosition 
    }
}
