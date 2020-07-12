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

    private static Player[] Friend;
    private object friendList;
    private float[] Friends; // float ist Platzsparender 

    private GameObject[] ListOfFriends;
    public GameObject ItemPrefab;
    public GameObject DummyItem;
    private LocationButton ButtonScript;
    private bool[] UserExistsInRoom;
    private bool ListInitiated = false;

    // public Recievermanager buttonManager; 

    public GameObject vorlage;
    public GameObject platzhaltzer;

  

    //funktion die Liste abdatetet
    //button delete and add friendyname Methode
    // add friendtoList übergabewert user id 


    private object GetFriendList()
    {
        return friendList;
    }

    private void SetFriendList(object value)
    {
        friendList = value;
    }

    public bool WindowEnabled {
        get; set; } // not used

    public event EventHandler WindowClosed;

    public bool WindowOpen
    {
        get
        {
            return gameObject.activeSelf;
        }
    }

    public object Friendlist { get; private set; }

    public void Close()
    {
        gameObject.SetActive(false);
        WindowClosed?.Invoke(this, EventArgs.Empty);
        
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }


    public void Open(Vector3 position, Vector3 eulerAngles)
    {

        Open();
        ListInitiated = true;
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;

        Friend= PhotonNetwork.PlayerList;

        //ListOfFriends = new GameObject[Friend().Length + 1];
        ListOfFriends[0] = DummyItem;

        //if (Friend().Length == 0)
        //{
        //    Debug.LogError("Friends List is empty!");
        //    return;
        //}
        
    }



    public void OnUpdatedFriendsList()
    {
        Debug.Log("Updated: " + PhotonNetwork.Friend.Count.ToString());
    }






    // Start is called before the first frame update
    public void AddFriendname(string name)
    {
        Debug.Log("Added Friend:" + name);
        foreach (Player user in Friend)
        {
            if (String.Equals(user.NickName, "name"))
            {
            string id = user.UserId;
            break;
            }
        }
    }

     void AddFriendToList(string id)
        {
        // Friendlist.addFriendToList(id);
        
    }

    void RemoveFriend (string id) {

     
        //FriendList.RemoveFriendToList(id);
    }
    
}
