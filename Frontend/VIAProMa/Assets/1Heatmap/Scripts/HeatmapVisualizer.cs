using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HeatmapVisualizer : MonoBehaviour
{
    public static HeatmapVisualizer instance;
    [Header("Data information")]
    public int valueRange = 100;
    public int arraySize = 40;
    [SerializeField]
    public int[,] data;
    int min, max;
    [Header("Visualization")]
    public float width = 10;
    public float height = 3;
    public float pointSize = 0.5f;
    public Gradient colorGradient;
    public GameObject spherePrefab;

    //Rest
    HeatmapPoint[,] points;

    private void Awake()
    {
        instance = this;
        min = 0;
        max = 100;
    }

    private void Start()
    {
        Setup();

        data = GenerateTestData(arraySize, 100);

        UpdateData(data);
    }

    public void Setup()
    {
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
        //Update userpositions every second
        InvokeRepeating("UpdateFromUserPositions", 1f, 1f);
        //Update Visualization for all points with new min max values every 5 seconds
        InvokeRepeating("UpdateDataInvoke", 5f, 5f);

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
        for (int x = 0; x < arraySize; x++)
        {
            for (int z = 0; z < arraySize; z++)
            {
                points[x, z].UpdateData(data[x, z]);
            }
        }
    }

    /// <summary>
    /// Call UpdataData as an Invoke every x seconds
    /// </summary>
    private void UpdateDataInvoke()
    {
        UpdateData(data);
    }

    /// <summary>
    /// Get position of all users and Update Heatmap acordingly
    /// </summary>
    private void UpdateFromUserPositions()
    {
        if (PhotonNetwork.InRoom)
        {
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                //TODO sync positions and run UpdateDataPoint
                Debug.Log(player.Value.UserId);
            }

        }

        //Workaround for own position
        var position = GameObject.Find("Main Camera").transform.position;
        Debug.Log("Player position is: " +position);
        IncreaseDataPoint(position.x, position.z-2.0f);
    }

    /// <summary>
    /// Increase the data of the heatmap at global position x,z and update the visualization
    /// </summary>
    /// <param name="x"> -width/2 <= x < width/2 </param>
    /// <param name="z"> -width/2 <= z < width/2 </param>
    /// <param name="value"> 0 <= value < size(int) </param>
    public void IncreaseDataPoint(float x, float z)
    {
        if (x < -width / 2 || width / 2 <= x) return;
        if (z < -width / 2 || width / 2 <= z) return;

        int positionX = Mathf.FloorToInt((x + width / 2) * arraySize / width);
        int positionZ = Mathf.FloorToInt((z + width / 2) * arraySize / width);
        data[positionX, positionZ]++;
        points[positionX, positionZ].UpdateData(data[positionX, positionZ]);
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

    /// <summary>
    /// Debugfunction that creates an array with testdata for a given size and range from Perlin Noise
    /// </summary>
    /// <param name="size">The height and width of the array</param>
    /// <param name="range">The maximum of the values inside the array</param>
    /// <returns></returns>
    int[,] GenerateTestData(int size, int range)
    {
        int[,] testData = new int[size,size];
        string s = "";
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                testData[x, z] = (int)(Mathf.PerlinNoise(x/(float)size, z/(float)size) * range);
                s += testData[x, z];
            }
            s += "\n";
        }
        print(s);
        return testData;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position + Vector3.up * (height / 2), new Vector3(width, height, width));
    }

}
