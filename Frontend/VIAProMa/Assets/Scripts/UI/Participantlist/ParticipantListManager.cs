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
    private GameObject[] ListOfParticipants;
    private bool[] UserExistsInRoom;
    private bool ListInitiated = false;
    private LocationButton ButtonScript;

    public GameObject ItemPrefab;
    public GameObject DummyItem;
    //public Friendlist ListOfFriends; //implement when availabe

    //private bool windowEnabled = true;

    public bool WindowEnabled { get; set; } // not used

    public bool WindowOpen
    {
        get
        {
            return gameObject.activeSelf;
        }
    }

    ////not best option performance wise
    //public void Update()
    //{
    //    //UpdateListView();
    //}


    public event EventHandler WindowClosed;

    private void Awake()
    {
        if (DummyItem == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(DummyItem));
        }
        if (ItemPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(ItemPrefab));
        }
    }

    public void UpdateParticipantList()
    {
        Participants = PhotonNetwork.PlayerList;
        ClearListView();

        ListOfParticipants = new GameObject[Participants.Length + 1];
        ListOfParticipants[0] = DummyItem;

        if (Participants.Length == 0)
        {
            Debug.LogError("Participant List is empty!");
            return;
        }
        UpdateListView();
    }

    private void ClearListView()
    {
        if (!ListInitiated) return;

        int listLenght = ListOfParticipants.Length-1;
        for (int i = 1; i < listLenght; i++)
        {
            Destroy(ListOfParticipants[i]);
        }

        ButtonScript = null;
    }

    private void UpdateListView()
    {
        int listLenght = ListOfParticipants.Length-1;
        if (listLenght == 1)
        {
            Debug.Log("Single Player Mode");
            DummyItem.GetComponentInChildren<TextMeshPro>().text = Participants[0].NickName;
        }
        else
        {
            ListOfParticipants[0].GetComponentInChildren<TextMeshPro>().text = Participants[0].NickName;
            for (int i = 1; i < listLenght; i++)
            {
                ListOfParticipants[i] = Instantiate(ItemPrefab, ListOfParticipants[i - 1].transform, false);
                ListOfParticipants[i].transform.position += Vector3.down * 0.31f;
                ListOfParticipants[i].transform.position += Vector3.forward * 0.04f;
                ListOfParticipants[i].GetComponentInChildren<TextMeshPro>().text = Participants[i].NickName;
                ButtonScript =  ListOfParticipants[i].GetComponent<LocationButton>();
                ButtonScript.ListManager = this;
               
            }
        }
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        ListInitiated = true;
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;

        Participants = PhotonNetwork.PlayerList;

        ListOfParticipants = new GameObject[Participants.Length + 1];
        ListOfParticipants[0] = DummyItem;

        if (Participants.Length == 0)
        {
            Debug.LogError("Participant List is empty!");
            return;
        }
        UpdateListView();
    }

    public void TestCall()
    {
        Participants = PhotonNetwork.PlayerList;
        
        ListOfParticipants = new GameObject[Participants.Length + 1];
        ListOfParticipants[0] = DummyItem;

        if (Participants.Length == 0)
        {
            Debug.LogError("Participant List is empty!");
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
        ClearListView();
    }

    public void AddFriend(string name)
    {
        Debug.Log("Added Friend:"+name);
        foreach (Player user in Participants)
        {
            if (String.Equals(user.NickName, "name"))
            {
                string id = user.UserId;
                //Friendlist.addFriendToList(id);
                break;
            }
        }
    }
}
