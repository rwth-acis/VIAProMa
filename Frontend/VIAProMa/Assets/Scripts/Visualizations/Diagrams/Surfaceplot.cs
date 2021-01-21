using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations.Common.Grid;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Common
{
    public class Surfaceplot : Diagram
    {
        [SerializeField] private Material surfaceMaterial;

        private MeshFilter surfaceMeshFilter;
        private MeshRenderer surfaceMeshRenderer;
        private Vector3[] verticesInUnitSpace;
        private Vector3[] vertices;
        private int[] triangles;
        private Vector2Int gridSize;
        private GridController gridController;

        private float[,] gridHeight;

        protected override void Awake()
        {
            base.Awake();

            if (surfaceMaterial == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(surfaceMaterial));
            }

            surfaceMeshFilter = contentParent.GetComponent<MeshFilter>();
            if (surfaceMeshFilter == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(MeshFilter), contentParent.gameObject);
            }
            surfaceMeshRenderer = contentParent.GetComponent<MeshRenderer>();
            if (surfaceMeshRenderer == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(MeshRenderer), contentParent.gameObject);
            }
            gridController = contentParent.GetComponent<GridController>();
            if (gridController == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(GridController), contentParent.gameObject);
            }

            surfaceMeshRenderer.material = Instantiate(surfaceMaterial);
        }

        public override void UpdateDiagram()
        {
            base.UpdateDiagram();
            // axes and grid are already set up at this point

            ClearContent();

            if (DataSet.DataColumns.Count < 3)
            {
                Debug.LogError("Cannot visualize data set with less than 3 columns");
                return;
            }

            ConstructMesh();
        }

        protected override void ClearContent()
        {
            surfaceMeshFilter.mesh = null;
        }

        private void ConstructMesh()
        {
            Mesh mesh = new Mesh();
            mesh.name = "Grid Surface Plot";
            gridSize = new Vector2Int(
                Mathf.CeilToInt(xAxisController.NumericAxisMax - xAxisController.NumericAxisMin),
                Mathf.CeilToInt(zAxisController.NumericAxisMax - zAxisController.NumericAxisMin));
            CalculateHeightField();
            CalculateVertexPositions();
            mesh.vertices = verticesInUnitSpace;
            FormTriangles();
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.uv = ExtractUVs();
            surfaceMeshFilter.mesh = mesh;
            contentParent.localScale = new Vector3(Size.x, 1, Size.z);
            gridController.CellSize = new Vector2(xAxisController.Length / gridSize.x, zAxisController.Length / gridSize.y);
            gridController.UpdateGrid();
        }

        private void CalculateHeightField()
        {
            gridHeight = new float[gridSize.x + 1, gridSize.y + 1];
            // go over all data points and accumulate them in the height field
            for (int i = 0; i < Mathf.Min(DataSet.DataColumns[0].ValueCount, DataSet.DataColumns[1].ValueCount, DataSet.DataColumns[2].ValueCount); i++)
            {
                float xValue = DataSet.DataColumns[0].GetFloatValue(i);
                int xFloor = Mathf.FloorToInt(xValue);
                int xCeil = Mathf.CeilToInt(xValue);
                float zValue = DataSet.DataColumns[2].GetFloatValue(i);
                int zFloor = Mathf.FloorToInt(zValue);
                int zCeil = Mathf.CeilToInt(zValue);

                float xFraction = xValue % 1;
                float zFraction = zValue % 1;
                float invXFraction = 1 - xFraction;
                float invZFraction = 1 - zFraction;

                float yValue = DataSet.DataColumns[1].GetFloatValue(i);

                if (xFloor >= 0)
                {
                    if (zFloor >= 0)
                    {
                        gridHeight[xFloor, zFloor] += invXFraction * invZFraction * yValue;
                    }
                    if (zCeil <= gridSize.y)
                    {
                        gridHeight[xFloor, zCeil] += invXFraction * zFraction * yValue;
                    }
                }
                if (xCeil <= gridSize.x)
                {
                    if (zFloor >= 0)
                    {
                        gridHeight[xCeil, zFloor] += xFraction * invZFraction * yValue;
                    }
                    if (zCeil <= gridSize.y)
                    {
                        gridHeight[xCeil, zCeil] += xFraction * zFraction * yValue;
                    }
                }
            }
        }

        private void CalculateVertexPositions()
        {
            vertices = new Vector3[(gridSize.x + 1) * (gridSize.y + 1)];
            verticesInUnitSpace = new Vector3[(gridSize.x + 1) * (gridSize.y + 1)];
            int i = 0;
            for (int y = 0; y < (gridSize.y + 1); y++)
            {
                for (int x = 0; x < (gridSize.x + 1); x++)
                {
                    verticesInUnitSpace[i] = new Vector3(
                        (float)x / gridSize.x,
                        gridHeight[x, y] / yAxisController.NumericAxisMax,
                        (float)y / gridSize.y);

                    vertices[i] = Vector3.Scale(Size, verticesInUnitSpace[i]);
                    i++;
                }
            }
        }

        private void FormTriangles()
        {
            triangles = new int[gridSize.x * gridSize.y * 6];

            int triangleIndex = 0;
            int vertexIndex = 0;

            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    // create a quad
                    // first triangle
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + gridSize.x + 1;
                    triangles[triangleIndex + 2] = vertexIndex + 1;
                    // second triangle
                    triangles[triangleIndex + 3] = vertexIndex + 1;
                    triangles[triangleIndex + 4] = vertexIndex + gridSize.x + 1;
                    triangles[triangleIndex + 5] = vertexIndex + gridSize.x + 2;
                    // move triangle index forward for next quad
                    triangleIndex += 6;
                    vertexIndex++;
                }
                vertexIndex++;
            }
        }

        private Vector2[] ExtractUVs()
        {
            Vector2[] res = new Vector2[verticesInUnitSpace.Length];
            for (int i = 0; i < verticesInUnitSpace.Length; i++)
            {
                res[i] = new Vector2(verticesInUnitSpace[i].x, verticesInUnitSpace[i].z);
            }
            return res;
        }
    }
}