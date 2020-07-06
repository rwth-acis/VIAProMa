using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShowcaseHeatmap : MonoBehaviour
{
    private void Start()
    {
        Invoke("joinLobby", 3f);

        var position = GameObject.Find("Main Camera").transform.position;
        position.y = 1.8f;
        GameObject.Find("Main Camera").transform.position = position;
    }
    public void ToggleHeatmap()
    {
        if (PhotonNetwork.InRoom)
        {
            HeatmapVisualizer.Toggle();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleHeatmap();
        }
    }

    public void joinLobby()
    {
        PhotonNetwork.JoinLobby();
        Invoke("createRoom", 0.5f);
    }

    public void createRoom()
    {
        PhotonNetwork.CreateRoom("heatmapDemo");
        Invoke("joinRoom", 0.5f);
    }

    public void joinRoom()
    {
        PhotonNetwork.JoinRoom("heatmapDemo");
    }
}
