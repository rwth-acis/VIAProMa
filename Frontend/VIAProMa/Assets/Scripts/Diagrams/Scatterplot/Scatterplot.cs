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
    [SerializeField] private AxisController xAxis;
    [Tooltip("Y-Axis of the diagram")]
    [SerializeField] private AxisController yAxis;
    [Tooltip("Z-Axis of the diagram")]
    [SerializeField] private AxisController zAxis;

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
        set
        {
            boxSize = value;
            xAxis.Length = boxSize.x;
            yAxis.Length = boxSize.y;
            zAxis.Length = boxSize.z;
            UpdateVisuals();
        }
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
            pointBounds = GetBoundsOfData(dataSet.Points);
            dataSet.XAxis.DataMin = pointBounds.min.x;
            dataSet.YAxis.DataMin = pointBounds.min.y;
            dataSet.ZAxis.DataMin = pointBounds.min.z;
            dataSet.XAxis.DataMax = pointBounds.max.x;
            dataSet.YAxis.DataMax = pointBounds.max.y;
            dataSet.ZAxis.DataMax = pointBounds.max.z;
            xAxis.Axis = dataSet.XAxis;
            yAxis.Axis = dataSet.YAxis;
            zAxis.Axis = dataSet.ZAxis;
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

        if (DataSet == null)
        {
            return;
        }

        xAxis.VisualizeAxis(1f, transform);
        yAxis.VisualizeAxis(1f, transform);
        zAxis.VisualizeAxis(1f, transform);

        scalingFactors = CalcScalingFactors();
        Debug.Log(scalingFactors);

        foreach (DataPoint point in DataSet.Points)
        {
            GameObject instance = Instantiate(pointPrefab, pointsParent);
            instance.transform.localScale = new Vector3(PointSize, PointSize, PointSize);
            instance.transform.localPosition = new Vector3(
                point.position.x * scalingFactors.x,
                point.position.y * scalingFactors.y,
                point.position.z * scalingFactors.z);
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

    private Vector3 CalcScalingFactors()
    {
        float xFactor = SingleScalingFactor(boxSize.x, xAxis.AxisMax - xAxis.AxisMin);
        float yFactor = SingleScalingFactor(boxSize.y, yAxis.AxisMax - yAxis.AxisMin);
        float zFactor = SingleScalingFactor(boxSize.z, zAxis.AxisMax - zAxis.AxisMin);

        return new Vector3(xFactor, yFactor, zFactor);
    }

    private float SingleScalingFactor(float worldLength, float dataRange)
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

    private void ClearPointRepresentations()
    {
        for (int i = 0; i < pointRepresentations.Count; i++)
        {
            Destroy(pointRepresentations[i]);
        }
        pointRepresentations.Clear();
    }
}
