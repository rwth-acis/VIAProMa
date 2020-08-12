using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


/// <summary>
/// Manager for heatmap data
/// </summary>
public class HeatmapDataManagement : MonoBehaviourPunCallbacks
{
    [Header("Data information")]
    public int testDataRange = 0;
    public int arraySize = 40;
    [Tooltip("Update intervall of the heatmap data in seconds")]
    public float updateIntervall = 1f;


    public int[,] data;
    public event Action OnDataChanged;
    HeatmapVisualizer heatmapVisualizer;


    /// <summary>
    /// Instanciates variables
    /// </summary>
    private void Awake()
    {
        heatmapVisualizer = GetComponent<HeatmapVisualizer>();
        if (data == null)   data = GenerateTestData(arraySize, testDataRange);
    }


    /// <summary>
    /// Start invoke to update user positions every updateIntervall
    /// </summary>
    void Start()
    {
        InvokeRepeating("UpdateFromUserPositions", updateIntervall, updateIntervall);
    }


    /// <summary>
    /// Turn heatmap off when returning to the lobby
    /// </summary>
    public override void OnLeftRoom()
    {
        HeatmapVisualizer.SetVisible(false);
    }


    /// <summary>
    /// Send new players the current data as masterclient
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("UpdateData", newPlayer, data);
        }
    }


    /// <summary>
    /// PunRPC to recieve current Data from master client on room entrie
    /// </summary>
    /// <param name="newData"></param>
    [PunRPC]
    void UpdateData(int[,] newData)
    {
        data = newData;
    }


    /// <summary>
    /// Get own position and send the corresponding array position to all players inside the room
    /// </summary>
    private void UpdateFromUserPositions()
    {
        var position = Camera.main.transform.position;

        if (position.x < -heatmapVisualizer.width / 2 || heatmapVisualizer.width / 2 <= position.x) return;
        if (position.z < -heatmapVisualizer.width / 2 || heatmapVisualizer.width / 2 <= position.z) return;

        int x = Mathf.FloorToInt((position.x + heatmapVisualizer.width / 2) * arraySize / heatmapVisualizer.width);
        int z = Mathf.FloorToInt((position.z + heatmapVisualizer.width / 2) * arraySize / heatmapVisualizer.width);

        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("UpdatePosition", RpcTarget.All, x, z);
        }
    }


    /// <summary>
    /// PunRPC to recieve current position of other players
    /// </summary>
    /// <param name="x">First array position </param>
    /// <param name="z">Second array position</param>
    [PunRPC]
    void UpdatePosition(int x, int z)
    {
        IncreaseDataPoint(x, z);
    }

    
    /// <summary>
    /// Increase the data of the heatmap at global position x,z and update the visualization
    /// </summary>
    /// <param name="x"> -width/2 <= x < width/2 </param>
    /// <param name="z"> -width/2 <= z < width/2 </param>
    /// <param name="value"> 0 <= value < size(int) </param>
    public void IncreaseDataPoint(int x, int z)
    {
        int maxDistance = 2;
        for (int i = -maxDistance; i <= maxDistance; i++)
        {
            for (int j = -maxDistance; j <= maxDistance; j++)
            {
                if(x + i>=0 && x + i<arraySize && z + j>=0&& z + j < arraySize)
                {
                    data[x + i, z + j] += 2 * maxDistance - Mathf.Abs(i) - Mathf.Abs(j);
                }
            }
        }
        OnDataChanged?.Invoke();
    }

    
    /// <summary>
    /// Debugfunction that creates an array with testdata for a given size and range from Perlin Noise.
    /// If range == 0, it is used to initilize the data array
    /// </summary>
    /// <param name="size">The height and width of the array</param>
    /// <param name="range">The maximum of the values inside the array</param>
    /// <returns></returns>
    int[,] GenerateTestData(int size, int range)
    {
        int[,] testData = new int[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                testData[x, z] = (int)(Mathf.PerlinNoise(x / (float)size, z / (float)size) * range);
            }
        }
        return testData;
    }
}
