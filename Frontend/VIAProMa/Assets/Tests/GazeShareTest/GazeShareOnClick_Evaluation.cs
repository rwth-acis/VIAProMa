using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class GazeShareOnClick_Evaluation : MonoBehaviour, IMixedRealityPointerHandler
{
    private bool firstclick;

    // Start is called before the first frame update
    void Start()
    {
        firstclick = true;
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (firstclick == true)
        {
            firstclick = false;
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("Connecting to room", gameObject);
                PhotonNetwork.JoinOrCreateRoom("GazeEvaluation", null, null);
            }
        }
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData) { }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }

    // Update is called once per frame
    void Update() { }
}
