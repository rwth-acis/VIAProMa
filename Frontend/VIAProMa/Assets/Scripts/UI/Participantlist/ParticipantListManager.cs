using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;

public class ParticipantListManager : MonoBehaviour, IWindow
{
    private static Player[] PARTICIPANTS;
    private RoomInfo ROOMINFO;
    public GameObject LISTVIEW;

    private bool windowEnabled = true;


    public bool WindowEnabled
    {
        get
        {
            return windowEnabled;
        }
        set
        {
            windowEnabled = value;
        }
    }


    public bool WindowOpen
    {
        get; private set;
    }


    public event EventHandler WindowClosed;


    public void UpdateListView() {
        GameObject participantListItem = LISTVIEW.GetComponentInChildren<GameObject>();
        participantListItem.GetComponent<TextMeshPro>().text = PARTICIPANTS[0].NickName;

        int listLenght = PARTICIPANTS.Length;
        for (int i = 1; i < listLenght-1; i++)
        {
            GameObject newListItem = Instantiate(participantListItem, LISTVIEW.transform, false);
            newListItem.GetComponentInChildren<TextMeshPro>().text = PARTICIPANTS[i].NickName;
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
        WindowOpen = true;
        PARTICIPANTS = PhotonNetwork.PlayerList;
        if (PARTICIPANTS == null)
        {
            Debug.Log("Participant List is empty!");
            return;
        }
        UpdateListView();
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
        PARTICIPANTS = PhotonNetwork.PlayerList;
        if (PARTICIPANTS == null)
        {
            Debug.Log("Participant List is empty!");
            return;
        }
        UpdateListView();
    }

    public void Close()
    {
        WindowOpen = false;
        WindowClosed?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }
}
