using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfacePlot : Diagram
{
    [SerializeField] private MeshRenderer surfaceRenderer;

    private DataSet dataSet;

    private Mesh constructedMesh;
    private Vector3[] vertices;


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

    public Vector2Int GridSize { get; set; }

    protected override void Awake()
    {
        base.Awake();
        if (surfaceRenderer == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(surfaceRenderer));
        }
    }

    protected override void UpdateVisuals()
    {
        // first clear the existing bars
        ClearContent();

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

        ConstructMesh();

        base.UpdateVisuals();
    }

    protected override void ClearContent()
    {
        base.ClearContent();
    }

    private void ConstructMesh()
    {
        CalculateVertexPositions();
    }

    private void CalculateVertexPositions()
    {
        vertices = new Vector3[(GridSize.x + 1) * (GridSize.y + 1)];
        int i = 0;
        for (int y = 0; y < (GridSize.y + 1); y++)
        {
            for (int x = 0; x < (GridSize.x + 1); x++)
            {
                vertices[i] = new Vector3((float) x / GridSize.x, 0, (float)y / GridSize.y);
                i++;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
