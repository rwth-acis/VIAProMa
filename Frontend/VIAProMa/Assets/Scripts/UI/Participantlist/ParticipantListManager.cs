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
    public GameObject ItemPrefab;
    public GameObject DummyItem;
    //public Friendlist ListOfFriends; //implement when availabe

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
        int listLenght = Participants.Length;
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
    }

    public void OnFriendButtonClicked(UnityEngine.Object source)
    {
        string friendNickname = "not set";
        string objectName = source.name;
        Debug.Log(objectName);
        //friendNickname = source.GetComponentInParent<TextMeshPro>().text;
        AddFriend(friendNickname);
        Debug.Log("On Button Friend Clicked");
    }

    private void AddFriend(string name)
    {
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
