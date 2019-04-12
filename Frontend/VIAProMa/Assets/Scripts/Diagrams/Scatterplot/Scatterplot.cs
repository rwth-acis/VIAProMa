using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scatterplot : Diagram
{
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Transform pointsParent;

    [SerializeField] private Axis xAxis;
    [SerializeField] private Axis yAxis;
    [SerializeField] private Axis zAxis;

    private List<DataPoint> points;
    private List<GameObject> pointRepresentations;

    public List<DataPoint> Points { get { return points; } set { points = value; UpdateVisuals(); } }

    public float PointSize { get; set; } = 0.3f;

    private void Awake()
    {
        if (pointPrefab == null)
        {
            SpecialDebugMessages.LogComponentMissingReferenceError("Scatterplot", "pointPrefab", this);
        }
        if (pointsParent == null)
        {
            SpecialDebugMessages.LogComponentMissingReferenceError("Scatterplot", "pointsParent", this);
        }

        pointRepresentations = new List<GameObject>();
        pointBounds = new Bounds();
    }

    protected override void UpdateVisuals()
    {
        ClearPointRepresentations();
        pointBounds = new Bounds();
        foreach (DataPoint point in Points)
        {
            GameObject instance = Instantiate(pointPrefab, pointsParent);
            instance.transform.localScale = new Vector3(PointSize, PointSize, PointSize);
            instance.transform.localPosition = point.position;
            instance.GetComponent<Renderer>().material.color = point.color;
            pointRepresentations.Add(instance);
            pointBounds.Encapsulate(point.position);
        }
        UpdateAxis(xAxis, pointBounds.max.x);
        UpdateAxis(yAxis, PointBounds.max.y);
        UpdateAxis(zAxis, PointBounds.max.z);
        base.UpdateVisuals();
    }

    private void UpdateAxis(Axis axis, float length)
    {
        if (axis == null)
        {
            return;
        }

        axis.Length = length;
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
