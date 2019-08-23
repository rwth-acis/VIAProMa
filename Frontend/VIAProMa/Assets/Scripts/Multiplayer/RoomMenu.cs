using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the menu which allows a user to select existing rooms (or navigate to the menu where a new room can be created)
/// </summary>
public class RoomMenu : MonoBehaviour, IWindow
{
    [SerializeField] private NetworkRoomListView roomListView;

    [SerializeField] private Interactable createRoomButton;
    [SerializeField] private Interactable pageUpButton;
    [SerializeField] private Interactable pageDownButton;

    [SerializeField] private CreateRoomMenu createRoomSubMenu;

    /// <summary>
    /// The number of room entries which are shown on one page
    /// </summary>
    public int entriesPerPage = 5;

    private List<NetworkRoomData> rooms = new List<NetworkRoomData>();

    private int page = 0;
    private bool windowEnabled = true;

    /// <summary>
    /// States whether the window is enabled
    /// If set to false, the window will remain visible but all interactable controls are disabled
    /// </summary>
    /// <value></value>
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

    public bool WindowOpen
    {
        get; private set;
    }

    /// <summary>
    /// Event which is invoked if the window is closed
    /// </summary>
    public event EventHandler WindowClosed;

    /// <summary>
    /// Initializes the component, makes sure that it is set up correctly
    /// </summary>
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

        // subscribe to the necessary events
        roomListView.ItemSelected += OnRoomSelected;
        createRoomSubMenu.WindowClosed += CreateRoomMenuClosed;

        LobbyManager.Instance.RoomListChanged += UpdateRoomList;
        LobbyManager.Instance.LobbyJoinStatusChanged += OnLobbyStatusChanged;

        Close();
    }

    /// <summary>
    /// Called if the client joined or left the lobby
    /// Handles the visibility of the menu: it is only visible if the client is in the lobby
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">Generic event arguments</param>
    private void OnLobbyStatusChanged(object sender, EventArgs e)
    {
        // close the window if we are not in a lobby anymore
        if (WindowOpen && !PhotonNetwork.InLobby)
        {
            Close();
        }
    }

    /// <summary>
    /// Makes sure that if the menu becomes visible, it will start in its original state
    /// </summary>
    private void OnEnable()
    {
        createRoomSubMenu.Close();
    }

    /// <summary>
    /// Called if the sub menu for creating a room is closed
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">Generic event arguments</param>
    private void CreateRoomMenuClosed(object sender, EventArgs e)
    {
        WindowEnabled = true;
    }

    /// <summary>
    /// Called if a element of the room list view was selected by the user
    /// Makes sure that the client joins the selected room
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">Arguments about the list view selection event</param>
    private void OnRoomSelected(object sender, ListViewItemSelectedArgs e)
    {
        if (windowEnabled)
        {
            PhotonNetwork.JoinRoom(roomListView.SeletedItem.RoomInfo.Name);
        }
    }

    /// <summary>
    /// Called if the user pushes the page up button
    /// Swiches to the previous page
    /// </summary>
    public void PageUp()
    {
        page = Mathf.Max(0, page - 1);
        SetPageButtonStates();
        UpdateRoomDisplay();
    }

    /// <summary>
    /// Called if the user pages the page down button
    /// Switches to the next page
    /// </summary>
    public void PageDown()
    {
        page = Mathf.Min(page + 1, ((rooms.Count - 1) / entriesPerPage));
        SetPageButtonStates();
        UpdateRoomDisplay();
    }

    /// <summary>
    /// Called if the user pushes the "create room" menu
    /// Opens the sub menu for creating a room
    /// </summary>
    public void OpenCreateRoomMenu()
    {
        createRoomSubMenu.Open();
        WindowEnabled = false;
    }

    /// <summary>
    /// Checks if the given roomName already exists
    /// </summary>
    /// <param name="roomName">Name for a room</param>
    /// <returns>True if a room with this name already exists, else false</returns>
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

    /// <summary>
    /// Adapts the button states of the page up and page down buttons
    /// If the first page is shown, the up button is disabled and if the last page is shown, the down button is disabled
    /// </summary>
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

    /// <summary>
    /// Called if the room list is updated
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Generic event arguments</param>
    private void UpdateRoomList(object sender, EventArgs e)
    {
        rooms = LobbyManager.Instance.Rooms;
        UpdateRoomDisplay();
        SetPageButtonStates();
    }

    /// <summary>
    /// Updates the list view showing the room lists (on the current page)
    /// </summary>
    private void UpdateRoomDisplay()
    {
        if (rooms.Count > 0)
        {
            // get the start index and length of the sub array to display
            // make sure that it stays within the bounds of the room list
            int startIndex = Mathf.Min(page * entriesPerPage, rooms.Count - 1);
            int length = Mathf.Min(rooms.Count - startIndex, entriesPerPage);
            roomListView.Items = rooms.GetRange(startIndex, length);
        }
        else
        {
            roomListView.Items = new List<NetworkRoomData>();
        }
    }

    /// <summary>
    /// Opens the window
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);
        WindowOpen = true;
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
    }

    /// <summary>
    /// Closes the window
    /// </summary>
    public void Close()
    {
        WindowOpen = false;
        WindowClosed?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }
}
