using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scatterplot : Diagram
{
    [Tooltip("Prefab for displaying data points in the scatterplot")]
    [SerializeField] private GameObject pointPrefab;
    [Tooltip("Parent transform which will have all data point transforms as children")]
    [SerializeField] private Transform pointsParent;

    public float pointSize = 0.1f;

    private DataSet dataSet;
    /// <summary>
    /// The instantiated representations of data points as gameobjects in the 3D scene
    /// </summary>
    private List<GameObject> pointRepresentations;

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
    /// Initializes the required fields, checks if the component was set up correctly
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        if (pointPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(pointPrefab));
        }
        if (pointsParent == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(pointsParent));
        }

        pointRepresentations = new List<GameObject>();
    }

    /// <summary>
    /// Updates the visual representation of the diagram
    /// </summary>
    protected override void UpdateVisuals()
    {
        ClearContent();

        if (DataSet == null)
        {
            return;
        }

        xAxis.VisualizeAxis(1f, transform);
        yAxis.VisualizeAxis(1f, transform);
        zAxis.VisualizeAxis(1f, transform);

        scalingFactors = CalcScalingFactors();

        foreach (DataPoint point in DataSet.Points)
        {
            GameObject instance = Instantiate(pointPrefab, pointsParent);
            instance.transform.localScale = new Vector3(pointSize, pointSize, pointSize);
            instance.transform.localPosition = new Vector3(
                point.position.x * scalingFactors.x,
                point.position.y * scalingFactors.y,
                point.position.z * scalingFactors.z);
            instance.GetComponent<Renderer>().material.color = point.color;
            pointRepresentations.Add(instance);
        }
        base.UpdateVisuals();
    }

    /// <summary>
    /// Clears the visual representation of the data points
    /// </summary>
    protected override void ClearContent()
    {
        for (int i = 0; i < pointRepresentations.Count; i++)
        {
            Destroy(pointRepresentations[i]);
        }
        pointRepresentations.Clear();
    }
}
