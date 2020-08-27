using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;

public class EvaluationTestscript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ConnectToEvalRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async void ConnectToEvalRoom()
    {
        while (!PhotonNetwork.IsConnected || !PhotonNetwork.InLobby)
        {
            await Task.Yield();
        }
        Debug.Log("Connecting to room");
        PhotonNetwork.JoinOrCreateRoom("SebEvalRoom", null, null);
    }

}
