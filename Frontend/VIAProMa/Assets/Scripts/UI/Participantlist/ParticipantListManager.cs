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
    //private RoomInfo RoomInfo;
    private GameObject[] ListOfParticipants;
    public GameObject ItemPrefab;
    public GameObject DummyItem;

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
        if (DummyItem == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(DummyItem));
        }
    }

    public void UpdateListView()
    {
        //GameObject participantListItem = ListView.GetComponentInChildren<GameObject>();
        //participantListItem.GetComponent<TextMeshPro>().text = Participants[0].NickName;

        int listLenght = Participants.Length;
        if (listLenght == 1)
        {
            Debug.Log("in if");
            DummyItem.GetComponentInChildren<TextMeshPro>().text = Participants[0].NickName;
        }
        else
        {
            ListOfParticipants[0].GetComponentInChildren<TextMeshPro>().text = Participants[0].NickName;
            for (int i = 1; i < listLenght; i++)
            {
                Debug.Log(i);
                ListOfParticipants[i] = Instantiate(ItemPrefab, ListOfParticipants[i - 1].transform, false);
                ListOfParticipants[i].GetComponentInChildren<TextMeshPro>().text = Participants[i - 1].NickName;
            }
        }
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;

        Participants = PhotonNetwork.PlayerList;

        ListOfParticipants = new GameObject[Participants.Length + 1];
        ListOfParticipants[0] = DummyItem;

        if (Participants == null)
        {
            Debug.Log("Participant List is empty!");
            return;
        }
        UpdateListView();
    }

    public void TestCall()
    {
        Participants = PhotonNetwork.PlayerList;

        ListOfParticipants = new GameObject[Participants.Length + 1];
        ListOfParticipants[0] = DummyItem;

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
