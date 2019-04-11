using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scatterplot : MonoBehaviour
{
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Transform pointsParent;

    private List<Vector3> points;
    private List<GameObject> pointRepresentations;

    List<Vector3> Points { get { return points; } set { points = value; UpdateVisuals(); } }

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
    }

    private void UpdateVisuals()
    {
        ClearPointRepresentations();
        foreach (Vector3 point in Points)
        {
            GameObject instance = Instantiate(pointPrefab, pointsParent);
            instance.transform.localPosition = point;
            pointRepresentations.Add(instance);
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
