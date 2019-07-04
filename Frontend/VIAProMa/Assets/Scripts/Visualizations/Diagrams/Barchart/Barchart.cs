using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Administers the logic ofr the bar chart diagram type
/// </summary>
public class Barchart : Diagram
{
    [SerializeField]
    private GameObject barPrefab;

    [SerializeField]
    private Transform barsParent;

    [Range(0, 1)]
    public float relativeBarThickness = 1f;

    private DataSet dataSet;

    private List<GameObject> instantiatedBars;

    /// <summary>
    /// The dataset which is supplied to the bar chart
    /// Setting the data set triggers an update of the diagram visuals
    /// </summary>
    /// <value></value>
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
    /// Initializes the component
    /// Checks if it is set up correctly
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        if (barPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(barPrefab));
        }
        if (barsParent == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(barsParent));
        }
        instantiatedBars = new List<GameObject>();
    }

    /// <summary>
    /// Updates the visual representation of the diagram
    /// Based on the data set, the bars are created and the axis labeling is updated
    /// </summary>
    protected override void UpdateVisuals()
    {
        // first clear the existing bars
        ClearBarRepresentations();

        // if there is no data set, there is nothing to display
        if (DataSet == null)
        {
            return;
        }

        xAxis.VisualizeAxis(1f, transform);
        yAxis.VisualizeAxis(1f, transform);
        zAxis.VisualizeAxis(1f, transform);

        // get the scaling factors so that we know how to scale the bars
        scalingFactors = CalcScalingFactors();

        // each data point represents a bar
        foreach (DataPoint point in DataSet.Points)
        {
            GameObject instance = Instantiate(barPrefab, barsParent);
            instance.transform.localScale = new Vector3(
                Mathf.Max(relativeBarThickness * scalingFactors.x, 0.001f),
                Mathf.Max(point.position.y * scalingFactors.y, 0.001f),
                Mathf.Max(relativeBarThickness * scalingFactors.z, 0.001f)
                );
            instance.transform.localPosition = new Vector3(
                point.position.x * scalingFactors.x,
                0,
                point.position.z * scalingFactors.z);
            instance.GetComponent<Renderer>().material.color = point.color;
            instantiatedBars.Add(instance);
        }
        base.UpdateVisuals();
    }

    /// <summary>
    /// Removes existing bar representations
    /// </summary>
    private void ClearBarRepresentations()
    {
        for (int i = 0; i < instantiatedBars.Count; i++)
        {
            Destroy(instantiatedBars[i]);
        }

        instantiatedBars.Clear();
    }

    /// <summary>
    /// Returns the bounds of the given data set
    /// Encapsulates each data point in a axis aligned bounding volume
    /// This way, one can get the minimum and maximum values for each axis and the average point
    /// </summary>
    /// <param name="points">The data points</param>
    /// <returns></returns>
    private static Bounds GetBoundsOfData(List<DataPoint> points)
    {
        Bounds b = new Bounds(); // bounds can be initialized this way because the origin should always be included in the diagram
        foreach (DataPoint point in points)
        {
            b.Encapsulate(point.position);
        }
        return b;
    }
}
