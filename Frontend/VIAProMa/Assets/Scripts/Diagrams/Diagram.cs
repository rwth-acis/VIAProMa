using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Diagram : MonoBehaviour
{

    [Tooltip("X-Axis of the diagram")]
    [SerializeField] protected AxisController xAxis;
    [Tooltip("Y-Axis of the diagram")]
    [SerializeField] protected AxisController yAxis;
    [Tooltip("Z-Axis of the diagram")]
    [SerializeField] protected AxisController zAxis;

    /// <summary>
    /// How much the data points should be scaled on each axis so that the diagram fits into the boxSize
    /// </summary>
    protected Vector3 scalingFactors;

    protected Bounds pointBounds; // field is required because otherwise Bounds.Encapsulate does not work as intended

    public event EventHandler DiagramUpdated;

    protected Vector3 boxSize;

    /// <summary>
    /// Specifies how big in global units the diagram should be
    /// </summary>
    public Vector3 BoxSize
    {
        get { return boxSize; }
        set
        {
            boxSize = value;
            xAxis.Length = boxSize.x;
            yAxis.Length = boxSize.y;
            zAxis.Length = boxSize.z;
            UpdateVisuals();
        }
    }


    public Bounds PointBounds
    {
        get
        {
            return pointBounds;
        }
    }

    protected virtual void Awake()
    {
        if (xAxis == null)
        {
            SpecialDebugMessages.LogComponentMissingReferenceError(this, nameof(xAxis));
        }
        if (yAxis == null)
        {
            SpecialDebugMessages.LogComponentMissingReferenceError(this, nameof(yAxis));
        }
        if (zAxis == null)
        {
            SpecialDebugMessages.LogComponentMissingReferenceError(this, nameof(zAxis));
        }
        pointBounds = new Bounds();
    }

    protected Vector3 CalcScalingFactors()
    {
        float xFactor = SingleScalingFactor(boxSize.x, xAxis.AxisMax - xAxis.AxisMin);
        float yFactor = SingleScalingFactor(boxSize.y, yAxis.AxisMax - yAxis.AxisMin);
        float zFactor = SingleScalingFactor(boxSize.z, zAxis.AxisMax - zAxis.AxisMin);

        return new Vector3(xFactor, yFactor, zFactor);
    }

    protected float SingleScalingFactor(float worldLength, float dataRange)
    {
        if (dataRange == 0)
        {
            return 1;
        }
        else
        {
            return worldLength / dataRange;
        }
    }

    protected virtual void UpdateVisuals()
    {
        OnDiagramUpdated(EventArgs.Empty);
    }

    protected virtual void OnDiagramUpdated(EventArgs e)
    {
        DiagramUpdated?.Invoke(this, e);
    }
}
