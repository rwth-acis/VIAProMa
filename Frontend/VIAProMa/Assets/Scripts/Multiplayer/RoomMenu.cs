using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMenu : MonoBehaviour, IWindow
{
    [SerializeField] private NetworkRoomListView roomListView;

    [SerializeField] private Interactable createRoomButton;
    [SerializeField] private Interactable pageUpButton;
    [SerializeField] private Interactable pageDownButton;

    [SerializeField] private CreateRoomMenu createRoomSubMenu;

    public int entriesPerPage = 5;

    private List<NetworkRoomData> rooms = new List<NetworkRoomData>();

    private int page = 0;
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
            createRoomButton.Enabled = value;
            pageUpButton.Enabled = value;
            pageDownButton.Enabled = value;
        }
    }

    public event EventHandler WindowClosed;

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
        if (createRoomSubMenu == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(createRoomSubMenu));
        }

        roomListView.ItemSelected += OnRoomSelected;
        createRoomSubMenu.WindowClosed += CreateRoomMenuClosed;

        LobbyManager.Instance.RoomListChanged += UpdateRoomList;
        LobbyManager.Instance.LobbyJoinStatusChanged += OnLobbyStatusChanged;

        Close();
    }

    private void OnLobbyStatusChanged(object sender, EventArgs e)
    {
        if (LobbyManager.Instance.InLobby)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    private void OnEnable()
    {
        createRoomSubMenu.Close();
    }

    private void CreateRoomMenuClosed(object sender, EventArgs e)
    {
        WindowEnabled = true;
    }

    private void OnRoomSelected(object sender, ListViewItemSelectedArgs e)
    {
        if (windowEnabled)
        {
            PhotonNetwork.JoinRoom(roomListView.SeletedItem.RoomInfo.Name);
        }
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

    public void OpenCreateRoomMenu()
    {
        createRoomSubMenu.Open();
        WindowEnabled = false;
    }

    public bool CheckIfRoomExists(string roomName)
    {
        for (int i = 0; i < rooms.Count; i++)
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

    private void UpdateRoomList(object sender, EventArgs e)
    {
        rooms = LobbyManager.Instance.Rooms;
        UpdateRoomDisplay();
        SetPageButtonStates();
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

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
