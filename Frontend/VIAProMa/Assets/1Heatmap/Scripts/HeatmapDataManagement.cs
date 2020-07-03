using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;

public class HeatmapDataManagement : MonoBehaviour
{
    [Header("Data information")]
    public int valueRange = 100;
    public int arraySize = 40;
    [SerializeField]
    public int[,] data;
    public event Action onDataChanged;

    HeatmapVisualizer heatmapVisualizer;


    private void Awake()
    {
        heatmapVisualizer = GetComponent<HeatmapVisualizer>();
        data = GenerateTestData(arraySize, 100);

    }

    // Start is called before the first frame update
    void Start()
    {
        //Update userpositions every second
        InvokeRepeating("UpdateFromUserPositions", 1f, 1f);

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
        Debug.Log("Player position is: " + position);

        IncreaseDataPoint(position.x, position.z);

    }


    /// <summary>
    /// Increase the data of the heatmap at global position x,z and update the visualization
    /// </summary>
    /// <param name="x"> -width/2 <= x < width/2 </param>
    /// <param name="z"> -width/2 <= z < width/2 </param>
    /// <param name="value"> 0 <= value < size(int) </param>
    public void IncreaseDataPoint(float x, float z)
    {
        if (x < -heatmapVisualizer.width / 2 || heatmapVisualizer.width / 2 <= x) return;
        if (z < -heatmapVisualizer.width / 2 || heatmapVisualizer.width / 2 <= z) return;

        int positionX = Mathf.FloorToInt((x + heatmapVisualizer.width / 2) * arraySize / heatmapVisualizer.width);
        int positionZ = Mathf.FloorToInt((z + heatmapVisualizer.width / 2) * arraySize / heatmapVisualizer.width);
        data[positionX, positionZ]++;

        if (onDataChanged != null) onDataChanged();
    }


    /// <summary>
    /// Debugfunction that creates an array with testdata for a given size and range from Perlin Noise
    /// </summary>
    /// <param name="size">The height and width of the array</param>
    /// <param name="range">The maximum of the values inside the array</param>
    /// <returns></returns>
    int[,] GenerateTestData(int size, int range)
    {
        int[,] testData = new int[size, size];
        string s = "";
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                testData[x, z] = (int)(Mathf.PerlinNoise(x / (float)size, z / (float)size) * range);
                s += testData[x, z];
            }
            s += "\n";
        }
        print(s);
        return testData;
    }

}
