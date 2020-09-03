using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GazeShareTester_Evaluation : MonoBehaviour
{
    [SerializeField] private GameObject issueShelfPrefab;
    [SerializeField] private GameObject visualizationShelfPrefab;
    [SerializeField] private GameObject targetPrefab;
    private GameObject issueShelf;
    private GameObject visualizationShelf;
    private GameObject target;
    // Timer
    [SerializeField] private GameObject timerWindowPrefab;
    private GameObject timerWindow;
    private bool timerOn;
    public static Stopwatch timer { get; private set; }
    private TimeSpan time;
    public static string elapsedTime { get; private set; }

    private void Start()
    {
        // Timer
        timerOn = false;
        timer = new Stopwatch();
        time = timer.Elapsed;
        elapsedTime = String.Format("{0:00}:{1:00}.{2:00}", time.Minutes, time.Seconds, time.Milliseconds / 10);
    }

    private void Update()
    {
        // Timer
        time = timer.Elapsed;
        elapsedTime = String.Format("{0:00}:{1:00}.{2:00}", time.Minutes, time.Seconds, time.Milliseconds / 10);

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (PhotonNetwork.IsConnected)
            {
                UnityEngine.Debug.Log("Connecting to room", gameObject);
                PhotonNetwork.JoinOrCreateRoom("GazeEvaluation", null, null);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            if (PhotonNetwork.InRoom)
            {
                UnityEngine.Debug.Log("Toggling voice");
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
            timerWindow = ResourceManager.Instance.NetworkInstantiate(timerWindowPrefab, new Vector3(-1.5f, 0.5f, 3f), Quaternion.identity);
        }   
    }

    public void PrecisionTest()
    {
        //timerWindow = ResourceManager.Instance.NetworkInstantiate(timerWindowPrefab, new Vector3(-1.5f, 0.5f, 3f), Quaternion.identity);
        timer.Start();
        target = ResourceManager.Instance.NetworkInstantiate(targetPrefab, new Vector3(-1.3f, 0.3f, 2.5f), Quaternion.Euler(0, 180, 0));
    }
}
