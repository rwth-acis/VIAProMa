using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoomMenu : MonoBehaviour, IWindow
{
    [SerializeField] private RoomMenu roomMenu;

    [SerializeField] private InputField roomNameField;
    [SerializeField] private GameObject errorMessage;
    [SerializeField] private Checkbox memberNumberCheckbox;
    [SerializeField] private SliderExtension memberNumberSlider;
    [SerializeField] private Interactable createRoomButton;

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
            createRoomButton.Enabled = false;
        }
    }

    public event EventHandler WindowClosed;

    private void Awake()
    {
        if (roomMenu == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(roomMenu));
        }
        if (roomNameField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(roomNameField));
        }
        if (errorMessage == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(errorMessage));
        }
        if (memberNumberCheckbox == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(memberNumberCheckbox));
        }
        if (memberNumberSlider == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(memberNumberSlider));
        }
        if (createRoomButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(createRoomButton));
        }

        roomNameField.TextChanged += OnInputFieldRoomNameChanged;
        memberNumberCheckbox.OnValueChanged += OnCheckboxMaxNumberOfMembersClicked;

        createRoomButton.Enabled = false;
    }

    private void OnInputFieldRoomNameChanged(object sender, EventArgs e)
    {
        bool roomExists = roomMenu.CheckIfRoomExists(roomNameField.Text);
        errorMessage.SetActive(roomExists);
        createRoomButton.Enabled = !roomExists && roomNameField.Text != "";
    }

    private void OnCheckboxMaxNumberOfMembersClicked(object sender, EventArgs e)
    {
        memberNumberSlider.gameObject.SetActive(memberNumberCheckbox.IsChecked);
    }

    public void OnCreateRoomButtonClicked()
    {
        // do not try to create a room if not connected
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        RoomOptions roomOptions = null; // if null is passed to CreateRoom, the options are ignored
        if (memberNumberCheckbox.IsChecked)
        {
            roomOptions = new RoomOptions { MaxPlayers = (byte)memberNumberSlider.ValueInt };
        }
        PhotonNetwork.CreateRoom(roomNameField.Text, roomOptions);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        WindowClosed?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }
}
