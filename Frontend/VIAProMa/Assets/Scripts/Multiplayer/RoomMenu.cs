using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMenu : MonoBehaviour, ILobbyCallbacks
{
    [SerializeField] private NetworkRoomListView roomListView;

    [SerializeField] private Interactable createRoomButton;
    [SerializeField] private Interactable pageUpButton;
    [SerializeField] private Interactable pageDownButton;

    public int entriesPerPage = 5;

    private List<NetworkRoomData> rooms = new List<NetworkRoomData>();

    private int page = 0;

    private void Awake()
    {
        if (createRoomButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(createRoomButton));
        }
        if (pageUpButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(pageUpButton));
        }
        if (pageDownButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(pageDownButton));
        }

        roomListView.ItemSelected += OnRoomSelected;
    }

    private void OnEnable()
    {
        // register to get callbacks
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        // unregister from callbacks
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    public void OnLeftLobby()
    {
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
        UpdateRoomDisplay();
        SetPageButtonStates();
    }

    private void OnRoomSelected(object sender, ListViewItemSelectedArgs e)
    {
        PhotonNetwork.JoinRoom(roomListView.SeletedItem.RoomInfo.Name);
    }

    public void PageUp()
    {
        page = Mathf.Max(0, page - 1);
        SetPageButtonStates();
        UpdateRoomDisplay();
    }

    public void PageDown()
    {
        page = Mathf.Min(page + 1, ((rooms.Count - 1) / entriesPerPage));
        SetPageButtonStates();
        UpdateRoomDisplay();
    }

    public bool CheckIfRoomExists(string roomName)
    {
        for (int i=0;i<rooms.Count;i++)
        {
            if (roomName == rooms[i].RoomInfo.Name)
            {
                return true;
            }
        }
        return false;
    }

    private void SetPageButtonStates()
    {
        if (page == 0) // first page
        {
            pageUpButton.Enabled = false;
        }
        else
        {
            pageUpButton.Enabled = true;
        }

        if (page == ((rooms.Count - 1) / entriesPerPage)) // last page
        {
            pageDownButton.Enabled = false;
        }
        else
        {
            pageDownButton.Enabled = true;
        }
    }

    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        rooms.Clear();
        foreach (RoomInfo roomInfo in roomList)
        {
            rooms.Add(new NetworkRoomData(roomInfo));
        }
    }

    private void UpdateRoomDisplay()
    {
        if (rooms.Count > 0)
        {
            int startIndex = Mathf.Min(page * entriesPerPage, rooms.Count - 1);
            int length = Mathf.Min(rooms.Count - startIndex, entriesPerPage);
            roomListView.Items = rooms.GetRange(startIndex, length);
        }
        else
        {
            roomListView.Items = new List<NetworkRoomData>();
        }
    }
}
