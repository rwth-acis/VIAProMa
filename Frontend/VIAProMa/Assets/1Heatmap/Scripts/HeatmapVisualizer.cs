using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HeatmapVisualizer : MonoBehaviour
{
    public static HeatmapVisualizer instance;
    [SerializeField]
    public int[,] data;
    int min, max;
    [Header("Visualization")]
    public float width = 10;
    public float height = 3;
    public float pointSize = 0.5f;
    public Gradient colorGradient;
    public GameObject spherePrefab;

    HeatmapDataManagement heatmapDataManagement;

    //Rest
    HeatmapPoint[,] points;

    private void Awake()
    {
        heatmapDataManagement = GetComponent<HeatmapDataManagement>();
        heatmapDataManagement.onDataChanged += OnUpdateData;
        instance = this;
        min = 0;
        max = 100;
    }

    private void Start()
    {
        Setup();

        data = heatmapDataManagement.data;

        UpdateData(data);
    }

    public void Setup()
    {
        int arraySize = heatmapDataManagement.arraySize;
        // Initialize Dots in child object.
        // Position of this transform is also middle of the Heatmap
        points = new HeatmapPoint[arraySize, arraySize];
        float stepSize = width / arraySize;
        Vector3 bottomLeft = transform.position - new Vector3((width - stepSize) / 2, 0, (width - stepSize) / 2);
        for (int x = 0; x < arraySize; x++)
        {
            for (int z = 0; z < arraySize; z++)
            {
                Vector3 position = bottomLeft + new Vector3(stepSize * x, 0, stepSize * z);
                GameObject point = Instantiate(spherePrefab, position, Quaternion.Euler(Vector3.zero), transform);
                points[x, z] = point.GetComponent<HeatmapPoint>();
                points[x, z].UpdateData(0);
            }
        }

    }


    public void OnUpdateData()
    {
        UpdateData(heatmapDataManagement.data);
    }

    /// <summary>
    /// Replaces the saved Heatmapdata with the given data and Updates the visualization according to the new Min and Max Values
    /// </summary>
    /// <param name="data">New Heatmapdata</param>
    public void UpdateData(int[,] data)
    {
        this.data = data;
        min = FindMin(data);
        max = FindMax(data);
        // Update Data on all points
        for (int x = 0; x < data.GetLength(0); x++)
        {
            for (int z = 0; z < data.GetLength(1); z++)
            {
                points[x, z].UpdateData(data[x, z]);
            }
        }
    }

    //
    // Visualization
    //

    /// <summary>
    /// Get color form the gradient for the given value in comparison to the max and min values
    /// </summary>
    /// <param name="value">The value of the sought color</param>
    /// <returns></returns>
    public Color GetColor(int value)
    {
        return colorGradient.Evaluate(Value2Range(value));
    }

    /// <summary>
    /// Get height for the given value in comparison to the max and min values
    /// </summary>
    /// <param name="value">The value of the sought height</param>
    /// <returns></returns>
    public float GetHeight(int value)
    {
        return Value2Range(value) * height;
    }

    /// <summary>
    /// Get size for the given value in comparison to the max and min values
    /// </summary>
    /// <param name="value">The value of the sought size</param>
    /// <returns></returns>
    public float GetSize(int value)
    {
        return Value2Range(value) * pointSize;
    }

    /// <summary>
    /// Returns the linear mapping of the value from (min, max) to (0,1)
    /// </summary>
    /// <param name="value">The mapped value</param>
    /// <returns></returns>
    public float Value2Range(int value)
    {
        if (max - min == 0)
            return 0;
        else
            return (value - min) / (float)(max-min);
    }

    /// <summary>
    /// Returns the minimum value in a given array
    /// </summary>
    /// <param name="array">The searched array</param>
    /// <returns></returns>
    public int FindMin(int[,] array)
    {
        int min = 100000;
        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int z = 0; z < array.GetLength(1); z++)
            {
                min = Mathf.Min(min, array[x, z]);
            }
        }
        return min;
    }

    /// <summary>
    /// Returns the maximum value in a given array
    /// </summary>
    /// <param name="array">The searched array</param>
    /// <returns></returns>
    public int FindMax(int[,] array)
    {
        int max = 0;
        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int z = 0; z < array.GetLength(1); z++)
            {
                max = Mathf.Max(max, array[x, z]);
            }
        }
        return max;
    }

    //
    // Debug
    //


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position + Vector3.up * (height / 2), new Vector3(width, height, width));
    }

}
