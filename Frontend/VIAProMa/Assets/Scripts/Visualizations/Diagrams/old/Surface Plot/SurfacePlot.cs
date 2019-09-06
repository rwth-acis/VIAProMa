using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfacePlot : Diagram
{
    [SerializeField] private MeshFilter surfaceMeshFilter;

    private DataSet dataSet;

    private Mesh constructedMesh;
    private Vector3[] vertices;
    private int[] triangles;

    private float[,] heightData;
    private Vector2Int gridSize;

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
            dataSet.XAxis.DataMin = 0;
            dataSet.YAxis.DataMin = 0;
            dataSet.ZAxis.DataMin = 0;
            dataSet.XAxis.DataMax = gridSize.x;
            dataSet.YAxis.DataMax = 1;
            dataSet.ZAxis.DataMax = gridSize.y;
            xAxis.Axis = dataSet.XAxis;
            yAxis.Axis = dataSet.YAxis;
            zAxis.Axis = dataSet.ZAxis;

            // Calculate height data from data set


            UpdateVisuals();
        }
    }

    public float[,] HeightData
    {
        get => heightData;
        set
        {
            heightData = value;
            UpdateVisuals();
        }
    }

    public Vector2Int GridSize
    {
        get => gridSize;
        set
        {
            gridSize = value;
            heightData = new float[gridSize.x + 1, gridSize.y + 1];
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (surfaceMeshFilter == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(surfaceMeshFilter));
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
        Mesh mesh = new Mesh();
        mesh.name = "Grid Surface Plot";
        CalculateVertexPositions();
        mesh.vertices = vertices;
        FormTriangles();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        surfaceMeshFilter.mesh = mesh;
    }

    private void CalculateVertexPositions()
    {
        vertices = new Vector3[(GridSize.x + 1) * (GridSize.y + 1)];
        int i = 0;
        for (int y = 0; y < (GridSize.y + 1); y++)
        {
            for (int x = 0; x < (GridSize.x + 1); x++)
            {
                vertices[i] = new Vector3(
                    (float) x / GridSize.x,
                    heightData[x, y],
                    (float)y / GridSize.y);
                i++;
            }
        }
    }

    private void FormTriangles()
    {
        triangles = new int[GridSize.x * GridSize.y * 6];

        int triangleIndex = 0;
        int vertexIndex = 0;

        for (int y = 0; y < GridSize.y; y++)
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                // create a quad
                // first triangle
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + GridSize.x + 1;
                triangles[triangleIndex + 2] = vertexIndex + 1;
                // second triangle
                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + GridSize.x + 1;
                triangles[triangleIndex + 5] = vertexIndex + GridSize.x + 2;
                // move triangle index forward for next quad
                triangleIndex += 6;
                vertexIndex++;
            }
            vertexIndex++;
        }

        triangles[0] = 0;
        triangles[1] = GridSize.x + 1;
        triangles[2] = 1;
        triangles[3] = 1;
        triangles[4] = GridSize.x + 1;
        triangles[5] = GridSize.x + 2;
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
