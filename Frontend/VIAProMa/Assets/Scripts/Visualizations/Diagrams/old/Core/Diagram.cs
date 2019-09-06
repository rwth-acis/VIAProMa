using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a diagram GameObject
/// </summary>
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

    /// <summary>
    /// Event which fires if the diagram is updated
    /// </summary>
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

    /// <summary>
    /// The bounds of the data points
    /// </summary>
    /// <value></value>
    public Bounds PointBounds
    {
        get
        {
            return pointBounds;
        }
    }

    /// <summary>
    /// Initializes the component
    /// Checks if it is set up correctly
    /// </summary>
    protected virtual void Awake()
    {
        if (xAxis == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(xAxis));
        }
        if (yAxis == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(yAxis));
        }
        if (zAxis == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(zAxis));
        }
        pointBounds = new Bounds();
    }

    /// <summary>
    /// Calculates the scaling factors by which the data space needs to be multiplied in order to fit it into the size of the diagram
    /// </summary>
    /// <returns>The scaling factors for all three axis</returns>
    protected Vector3 CalcScalingFactors()
    {
        float xFactor = SingleScalingFactor(boxSize.x, xAxis.AxisMax - xAxis.AxisMin);
        float yFactor = SingleScalingFactor(boxSize.y, yAxis.AxisMax - yAxis.AxisMin);
        float zFactor = SingleScalingFactor(boxSize.z, zAxis.AxisMax - zAxis.AxisMin);

        return new Vector3(xFactor, yFactor, zFactor);
    }

    /// <summary>
    /// Calculates a single scaling factor for one axis
    /// </summary>
    /// <param name="worldLength">The length of the diagram along this axis in world units
    /// This is the available space.</param>
    /// <param name="dataRange">The difference between the maximum and minimum in the data range on this axis</param>
    /// <returns></returns>
    protected float SingleScalingFactor(float worldLength, float dataRange)
    {
        // check if the data range is 0, then scaling is irrelevant (and so that we do not divide by 0)
        if (dataRange == 0)
        {
            return 1;
        }
        else
        {
            return worldLength / dataRange;
        }
    }

    /// <summary>
    /// Updates the visual representation of the diagram
    /// </summary>
    protected virtual void UpdateVisuals()
    {
        OnDiagramUpdated(EventArgs.Empty);
    }

    /// <summary>
    /// Invokes the DiagramUpdated event
    /// </summary>
    /// <param name="e">Arguments of th event</param>
    protected virtual void OnDiagramUpdated(EventArgs e)
    {
        DiagramUpdated?.Invoke(this, e);
    }

    protected virtual void ClearContent()
    {

    }

    /// <summary>
    /// Returns the bounds of the given data set
    /// Encapsulates each data point in a axis aligned bounding volume
    /// This way, one can get the minimum and maximum values for each axis and the average point
    /// </summary>
    /// <param name="points">The data points</param>
    /// <returns></returns>
    protected static Bounds GetBoundsOfData(List<DataPoint> points)
    {
        Bounds b = new Bounds(); // bounds can be initialized this way because the origin should always be included in the diagram
        foreach (DataPoint point in points)
        {
            b.Encapsulate(point.position);
        }
        return b;
    }
}
