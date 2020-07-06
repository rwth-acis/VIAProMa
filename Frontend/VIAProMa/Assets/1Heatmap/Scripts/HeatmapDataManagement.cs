using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class HeatmapDataManagement : MonoBehaviourPunCallbacks
{
    [Header("Data information")]
    public int testDataRange = 100;
    public int arraySize = 40;
    [SerializeField]
    public int[,] data;
    public event Action onDataChanged;

    HeatmapVisualizer heatmapVisualizer;
    HeatmapSerializer heatmapSerializer;
    PhotonView photonView;


    private void Awake()
    {
        heatmapSerializer = GetComponent<HeatmapSerializer>();
        heatmapVisualizer = GetComponent<HeatmapVisualizer>();
        data = GenerateTestData(arraySize, testDataRange);
        photonView = PhotonView.Get(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        //DEBUG:
        var position = GameObject.Find("Main Camera").transform.position;
        position.y = 1.8f;
        GameObject.Find("Main Camera").transform.position = position;


        //Update userpositions every second
        InvokeRepeating("UpdateFromUserPositions", 1f, 1f);

    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I am the Master");
        }
        else
        {
            Debug.Log(PhotonNetwork.MasterClient + " is the Master");
        }
    }

    public override void OnLeftRoom()
    {
        HeatmapVisualizer.SetVisible(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New player joined");
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("UpdateData", newPlayer, data);
        }
    }


    [PunRPC]
    void UpdateData(int[,] newData)
    {
        data = newData;
    }



    /// <summary>
    /// Get position of all users and Update Heatmap acordingly
    /// </summary>
    private void UpdateFromUserPositions()
    {
        var position = GameObject.Find("Main Camera").transform.position;
        Debug.Log("Player position is: " + position);

        if (position.x < -heatmapVisualizer.width / 2 || heatmapVisualizer.width / 2 <= position.x) return;
        if (position.z < -heatmapVisualizer.width / 2 || heatmapVisualizer.width / 2 <= position.z) return;

        int x = Mathf.FloorToInt((position.x + heatmapVisualizer.width / 2) * arraySize / heatmapVisualizer.width);
        int z = Mathf.FloorToInt((position.z + heatmapVisualizer.width / 2) * arraySize / heatmapVisualizer.width);

        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("UpdatePosition", RpcTarget.All, x, z);
        }

    }

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

        if (onDataChanged != null) onDataChanged();
    }


    public static int[,] StringToArray(string s)
    {
        string[] lines = s.Split('#');
        int[,] array = new int[lines.Length, lines.Length];
        for (int y = 0; y < array.GetLength(1); y++)
        {
            string[] sNums = lines[y].Split(';');
            for (int x = 0; x < array.GetLength(0); x++)
            {
                int.TryParse(sNums[x], out array[x, y]);
            }
        }
        return array;

    }
    public static string ArrayToString(int[,] array)
    {
        string s = "";
        for (int y = 0; y < array.GetLength(1); y++)
        {
            for (int x = 0; x < array.GetLength(0); x++)
            {
                s += array[x, y];
                if (x < array.GetLength(0) - 1)
                {
                    s += ";";
                }
            }
            if (y < array.GetLength(1) - 1)
            {
                s += "#";
            }
        }

        return s;
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
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                testData[x, z] = (int)(Mathf.PerlinNoise(x / (float)size, z / (float)size) * range);
                //testData[x, z] = 0;
            }
        }
        print(ArrayToString(testData));
        return testData;
    }

}
