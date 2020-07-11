using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;


/// <summary>
/// This class implements the functionallity of the window that displays all users currently present in one room.
/// The prefab item for the list entry and the dummy entry must be set in the inspector.
/// </summary>
public class ParticipantListManager : MonoBehaviour, IWindow
{
    //Private Variables 
    private static Player[] Participants;
    private GameObject[] ListOfParticipants;
    private bool[] UserExistsInRoom;
    private bool ListInitiated = false;
    private LocationButton ButtonScript;

    //Public Varibales
    public GameObject ItemPrefab;
    public GameObject DummyItem;
    //public Friendlist ListOfFriends; //implement when availabe

    public bool WindowEnabled { get; set; } // not used
    public event EventHandler WindowClosed;


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
    /// Opens the window and saves current useres in room in variable. Also invokes the update of the list view.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="eulerAngles"></param>
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



//------------------------------------------List Functionality---------------------------------------//
    
    /// <summary>
    /// Update method that gets called everytime a user joins the room or leaves the room and handels 
    /// the update functionality
    /// </summary>
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

    /// <summary>
    /// Rememoves all list entries by deleting the game objects
    /// </summary>
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

    /// <summary>
    /// Creates a new game object for every user in the room and displays their nickname on the button.
    /// </summary>
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


    /// <summary>
    /// Only for testing purpose. Obsolete in final version.
    /// </summary>
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


    /// <summary>
    /// Gets called when the button of a participant is clicked and envokes the friendslist add functionality
    /// </summary>
    /// <param name="name">unsername of the person that should be added as friend.</param>
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
