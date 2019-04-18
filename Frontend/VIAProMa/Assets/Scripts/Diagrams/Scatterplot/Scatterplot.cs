using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scatterplot : Diagram
{
    [Tooltip("Prefab for displaying data points in the scatterplot")]
    [SerializeField] private GameObject pointPrefab;
    [Tooltip("Parent transform which will have all data point transforms as children")]
    [SerializeField] private Transform pointsParent;

    [Tooltip("X-Axis of the diagram")]
    [SerializeField] private Axis xAxis;
    [Tooltip("Y-Axis of the diagram")]
    [SerializeField] private Axis yAxis;
    [Tooltip("Z-Axis of the diagram")]
    [SerializeField] private Axis zAxis;

    private DataSet dataSet;
    /// <summary>
    /// The instantiated representations of data points as gameobjects in the 3D scene
    /// </summary>
    private List<GameObject> pointRepresentations;

    /// <summary>
    /// How much the data points should be scaled on each axis so that the diagram fits into the boxSize
    /// </summary>
    private Vector3 scalingFactors;

    private Vector3 boxSize;

    /// <summary>
    /// Specifies how big in global units the diagram should be
    /// </summary>
    public Vector3 BoxSize
    {
        get { return boxSize; }
        set { boxSize = value; UpdateVisuals(); }
    }

    public DataSet DataSet
    {
        get
        {
            return dataSet;
        }
        set
        {
            dataSet = value;
            GetBoundsOfData(dataSet.Points);
            xAxis.Type = dataSet.XAxisType;
            yAxis.Type = dataSet.YAxisType;
            zAxis.Type = dataSet.ZAxisType;
            UpdateVisuals();
        }
    }

    /// <summary>
    /// The size of the displayed points
    /// </summary>
    public float PointSize { get; set; } = 0.3f;

    /// <summary>
    /// Initializes the required fields, checks if the component was set up correctly
    /// </summary>
    private void Awake()
    {
        if (pointPrefab == null)
        {
            SpecialDebugMessages.LogComponentMissingReferenceError(this, nameof(pointPrefab));
        }
        if (pointsParent == null)
        {
            SpecialDebugMessages.LogComponentMissingReferenceError(this, nameof(pointsParent));
        }
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

        pointRepresentations = new List<GameObject>();
        pointBounds = new Bounds();
    }

    /// <summary>
    /// Updates the visual representation of the diagram
    /// </summary>
    protected override void UpdateVisuals()
    {
        ClearPointRepresentations();
        if (xAxis.Type == AxisType.NUMERIC)
        {
            UpdateAxis(xAxis, boxSize.x, pointBounds.min.x, pointBounds.max.x);
        }
        else
        {
            UpdateAxis(xAxis, boxSize.x, dataSet.XAxisLabels);
        }

        if (yAxis.Type == AxisType.NUMERIC)
        {
            UpdateAxis(yAxis, boxSize.y, pointBounds.min.y, pointBounds.max.y);
        }
        else
        {
            UpdateAxis(yAxis, boxSize.y, dataSet.YAxisLabels);
        }

        if (zAxis.Type == AxisType.NUMERIC)
        {
            UpdateAxis(zAxis, boxSize.z, pointBounds.min.z, pointBounds.max.z);
        }
        else
        {
            UpdateAxis(zAxis, boxSize.z, dataSet.ZAxisLabels);
        }



        //scalingFactors = DetermineScalingFactors(boxSize, pointBounds);
        foreach (DataPoint point in DataSet.Points)
        {
            GameObject instance = Instantiate(pointPrefab, pointsParent);
            instance.transform.localScale = new Vector3(PointSize, PointSize, PointSize);
            instance.transform.localPosition = point.position;
            instance.GetComponent<Renderer>().material.color = point.color;
            pointRepresentations.Add(instance);
        }
        base.UpdateVisuals();
    }

    private static Bounds GetBoundsOfData(List<DataPoint> points)
    {
        Bounds b = new Bounds(); // bounds can be initialized this way because the origin should always be included in the diagram
        foreach (DataPoint point in points)
        {
            b.Encapsulate(point.position);
        }
        return b;
    }

    /// <summary>
    /// Updates a axis
    /// </summary>
    /// <param name="axis">The axis to update</param>
    /// <param name="length">The target length of the axis</param>
    /// <param name="scaleFactor">The scaling factor of the data points along this axis</param>
    private void UpdateAxis(Axis axis, float length, float dataMin, float dataMax)
    {
        if (axis == null)
        {
            return;
        }

        axis.Length = length;
        axis.SetNumbericLabels(dataMin, dataMax, 1f, transform);
    }

    private void UpdateAxis(Axis axis, float length, List<string> labels)
    {
        if (axis == null)
        {
            return;
        }

        axis.Length = length;
        axis.SetStringLabels(labels, transform);
    }

    private void SetScalingFactors()
    {

    }

    private void ClearPointRepresentations()
    {
        for (int i = 0; i < pointRepresentations.Count; i++)
        {
            Destroy(pointRepresentations[i]);
        }
        pointRepresentations.Clear();
    }
}
