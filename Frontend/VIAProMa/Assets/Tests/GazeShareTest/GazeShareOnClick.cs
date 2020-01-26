using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class GazeShareOnClick : MonoBehaviour, IMixedRealityPointerHandler
{
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to room", gameObject);
            PhotonNetwork.JoinOrCreateRoom("GazeTest", null, null);
        }
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
