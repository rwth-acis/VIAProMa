using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class FriendListManager : MonoBehaviour, IWindow
{
    //Variablen erstellen
    //private static Player[] Friend;
    //private object friendList;
    private List<string> FriendIds;
    private GameObject[] ListOfFriends;
    public GameObject ItemPrefab;
    public GameObject DummyItem;
    private bool ListInitiated = false;

    public bool WindowEnabled { get; set; } // not used
    public event EventHandler WindowClosed;

    //private LocationButton ButtonScript;
    //private bool[] UserExistsInRoom;

    // public Recievermanager buttonManager; 



    //funktion die Liste abdatetet
    // add friendtoList übergabewert user id 


    //---------------------------------------------------I Window functionallity-----------------------------------------//
    public bool WindowOpen
    {
        get
        {
            return gameObject.activeSelf;
        }
    }

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

    /// <summary>
    /// Opens the window.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="eulerAngles"></param>
    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
       //ListInitiated = true;
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;

        LoadFreindsfromFile();

        ListOfFriends = new GameObject[FriendIds.Count + 1];
        ListOfFriends[0] = DummyItem;

        if (FriendIds.Count == 0)
        {
            Debug.LogError("Friend List is empty!");
            return;
        }
        UpdateListView();
        ListInitiated = true;
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        WindowClosed?.Invoke(this, EventArgs.Empty);
        SaveFriendsToFile();
    }

    private void UpdateListView()
    {
        if (ListInitiated)
        {
            ClearListView();
        }
        //if friend is offline display id if not name
        int listLenght = ListOfFriends.Length - 1;
        if (listLenght == 1)
        {
            Debug.Log("Only one friend");
            DummyItem.GetComponentInChildren<TextMeshPro>().text = FriendIds[0];
        }
        else
        {
            ListOfFriends[0].GetComponentInChildren<TextMeshPro>().text = FriendIds[0];
            for (int i = 1; i < listLenght; i++)
            {
                ListOfFriends[i] = Instantiate(ItemPrefab, ListOfFriends[i - 1].transform, false);
                ListOfFriends[i].transform.position += Vector3.down * 0.31f;
                ListOfFriends[i].transform.position += Vector3.forward * 0.04f;
                ListOfFriends[i].GetComponentInChildren<TextMeshPro>().text = FriendIds[i];
                //ButtonScript = ListOfFriends[i].GetComponent<LocationButton>();
                //ButtonScript.ListManager = this;
            }
        }

    }

    public void AddButtonClicked()
    {
        //implement
    }

    public void RemoveButtonClicked ()
    { 
        //implement
    }

    private void LoadFreindsfromFile()
    {
        System.IO.File.ReadAllLines(@"friends.txt");
    }

    private void SaveFriendsToFile()
    {
        //implement
    }

    public void AddFriendById(string userId)
    {
        FriendIds.Add(userId);
        UpdateListView();
    }
    
    private void ClearListView()
    {
        if (!ListInitiated) return;

        int listLenght = ListOfFriends.Length - 1;
        for (int i = 1; i < listLenght; i++)
        {
            Destroy(ListOfFriends[i]);
        }
        // ButtonScript = null;
    }
}
