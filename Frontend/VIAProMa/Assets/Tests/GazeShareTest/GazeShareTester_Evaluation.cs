using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeShareTester_Evaluation : MonoBehaviour
{
    [SerializeField] private GameObject issueShelfPrefab;
    [SerializeField] private GameObject visualizationShelfPrefab;
    [SerializeField] private GameObject timerPrefab;
    private GameObject issueShelf;
    private GameObject visualizationShelf;
    private GameObject timer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("Connecting to room", gameObject);
                PhotonNetwork.JoinOrCreateRoom("GazeEvaluation", null, null);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            if (PhotonNetwork.InRoom)
            {
                Debug.Log("Toggling voice");
                PhotonVoiceNetwork.Instance.PrimaryRecorder.TransmitEnabled = !PhotonVoiceNetwork.Instance.PrimaryRecorder.TransmitEnabled;
            }
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            RaycastVive.ToggleLaser();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            //PhotonNetwork.Instantiate("Progress Bar", new Vector3(-1.726f, 0.51f, -1.229f), Quaternion.Euler(0,60,0), 0);
            //PhotonNetwork.Instantiate("IssueShelf", new Vector3(-1.726f, 0.51f, -1.229f), Quaternion.Euler(0, 60, 0), 0);

            //issueShelf = ResourceManager.Instance.NetworkInstantiate(issueShelfPrefab, new Vector3(-1f, -1f, 2f), Quaternion.identity);
            //visualizationShelf = ResourceManager.Instance.NetworkInstantiate(visualizationShelfPrefab, new Vector3(1f, -1f, 2f), Quaternion.identity);
            timer = ResourceManager.Instance.NetworkInstantiate(timerPrefab, new Vector3(-1.5f, 0.5f, 3f), Quaternion.identity);
        }   
    }
}
