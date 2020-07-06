using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;

public class ParticipantListManager : MonoBehaviour, IWindow
{
    private static Player[] Participants;
    private RoomInfo RoomInfo;
    public GameObject ListView;

    //private bool windowEnabled = true;


    public bool WindowEnabled // not used
    {
        get; set;
    }


    public bool WindowOpen
    {
        get
        {
            return gameObject.activeSelf;
        }
    }


    public event EventHandler WindowClosed;

    private void Awake()
    {
        if (ListView == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(ListView));
        }
    }

    public void UpdateListView() {
        GameObject participantListItem = ListView.GetComponentInChildren<GameObject>();
        participantListItem.GetComponent<TextMeshPro>().text = Participants[0].NickName;

        int listLenght = Participants.Length;
        for (int i = 1; i < listLenght-1; i++)
        {
            GameObject newListItem = Instantiate(participantListItem, ListView.transform, false);
            newListItem.GetComponentInChildren<TextMeshPro>().text = Participants[i].NickName;
        }
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
        Participants = PhotonNetwork.PlayerList;
        Debug.Log(Participants[0].NickName);
        if (Participants == null)
        {
            Debug.Log("Participant List is empty!");
            return;
        }
        UpdateListView();
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }
}
