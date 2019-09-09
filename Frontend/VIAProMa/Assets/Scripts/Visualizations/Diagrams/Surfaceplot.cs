using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surfaceplot : i5.ViaProMa.Visualizations.Common.Diagram
{
    private MeshFilter surfaceMeshFilter;
    private Vector3[] vertices;
    private int[] triangles;

    protected override void Awake()
    {
        base.Awake();
        surfaceMeshFilter = contentParent.GetComponent<MeshFilter>();
        if (surfaceMeshFilter == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(MeshFilter), surfaceMeshFilter.gameObject);
        }
    }

    public override void UpdateDiagram()
    {
        base.UpdateDiagram();
        // axes and grid are already set up at this point

        ClearContent();

        if (DataSet.DataColumns.Count == 0)
        {
            Debug.LogError("Cannot visualize empty data set");
            return;
        }
        int minColumnLength = Mathf.Min(DataSet.DataColumns[0].ValueCount, DataSet.DataColumns[1].ValueCount, DataSet.DataColumns[2].ValueCount);

        for (int i = 0; i < minColumnLength; i++)
        {
            float xInUnitSpace = FractionInUnitSpace(DataSet.DataColumns[0].GetFloatValue(i), xAxisController);
            float yInUnitSpace = FractionInUnitSpace(DataSet.DataColumns[1].GetFloatValue(i), yAxisController);
            float zInUnitSpace = FractionInUnitSpace(DataSet.DataColumns[2].GetFloatValue(i), zAxisController);

            Vector3 vertexPosition = Vector3.Scale(Size, new Vector3(xInUnitSpace, yInUnitSpace, zInUnitSpace));
        }
    }

    protected override void ClearContent()
    {
        surfaceMeshFilter.mesh = null;
    }

    //private void ConstructMesh()
    //{
    //    Mesh mesh = new Mesh();
    //    mesh.name = "Grid Surface Plot";
    //    CalculateVertexPositions();
    //    mesh.vertices = vertices;
    //    FormTriangles();
    //    mesh.triangles = triangles;
    //    mesh.RecalculateNormals();
    //    surfaceMeshFilter.mesh = mesh;
    //}

    //private void CalculateVertexPositions()
    //{
    //    vertices = new Vector3[xAxisController.LabelCount + zAxisController.LabelCount];
    //    int i = 0;
    //    for (int y = 0; y < (zAxisController.LabelCount); y++)
    //    {
    //        for (int x = 0; x < (xAxisController.LabelCount); x++)
    //        {
    //            vertices[i] = new Vector3(
    //                (float)x / GridSize.x,
    //                heightData[x, y],
    //                (float)y / GridSize.y);
    //            i++;
    //        }
    //    }
    //}

    //private void FormTriangles()
    //{
    //    triangles = new int[GridSize.x * GridSize.y * 6];

    //    int triangleIndex = 0;
    //    int vertexIndex = 0;

    //    for (int y = 0; y < GridSize.y; y++)
    //    {
    //        for (int x = 0; x < GridSize.x; x++)
    //        {
    //            // create a quad
    //            // first triangle
    //            triangles[triangleIndex] = vertexIndex;
    //            triangles[triangleIndex + 1] = vertexIndex + GridSize.x + 1;
    //            triangles[triangleIndex + 2] = vertexIndex + 1;
    //            // second triangle
    //            triangles[triangleIndex + 3] = vertexIndex + 1;
    //            triangles[triangleIndex + 4] = vertexIndex + GridSize.x + 1;
    //            triangles[triangleIndex + 5] = vertexIndex + GridSize.x + 2;
    //            // move triangle index forward for next quad
    //            triangleIndex += 6;
    //            vertexIndex++;
    //        }
    //        vertexIndex++;
    //    }

    //    triangles[0] = 0;
    //    triangles[1] = GridSize.x + 1;
    //    triangles[2] = 1;
    //    triangles[3] = 1;
    //    triangles[4] = GridSize.x + 1;
    //    triangles[5] = GridSize.x + 2;
    //}
}
