using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected override void UpdateVisuals()
    {
        ClearBarRepresentations();

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

    private void ClearBarRepresentations()
    {
        for (int i = 0; i < instantiatedBars.Count; i++)
        {
            Destroy(instantiatedBars[i]);
        }

        instantiatedBars.Clear();
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
}
