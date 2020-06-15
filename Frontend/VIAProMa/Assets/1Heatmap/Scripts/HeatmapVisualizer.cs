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

    }

    public void UpdateData(int[,] data)
    {
        this.data = data;

        min = FindMin(data);
        max = FindMax(data);

        string s = "";
        // Update Data on all points
        for (int x = 0; x < arraySize; x++)
        {
            for (int z = 0; z < arraySize; z++)
            {
                points[x, z].UpdateData(data[x, z]);
                s += data[x, z];
            }
            s += "\n";
        }
        print(s);
    }


    private void UpdateFromUserPositions()
    {
   /*     foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            //TODO sync positions and run UpdateDataPoint
        }
        */
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
    public Color GetColor(int value)
    {
        return colorGradient.Evaluate(Value2Range(value));
    }
    public float GetHeight(int value)
    {
        return Value2Range(value) * height;
    }
    public float GetSize(int value)
    {
        return Value2Range(value) * pointSize;
    }

    public float Value2Range(int value)
    {
        return (value - min) / (float)(max-min);
    }

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
