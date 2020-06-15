using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the UI menu for creating a new network room
/// </summary>
public class CreateRoomMenu : MonoBehaviour, IWindow
{
    [Header("References")]
    [Tooltip("Reference to the room menu script")]
    [SerializeField] private RoomMenu roomMenu;

    [Header("UI Elements")]
    [Tooltip("The button for closing the menu")]
    [SerializeField] private Interactable closeButton;
    [Tooltip("The text field for entering the room name")]
    [SerializeField] private InputField roomNameField;
    [Tooltip("An error message which can be displayed")]
    [SerializeField] private GameObject errorMessage;
    [Tooltip("Checkbox for advanced settings")]
    [SerializeField] private Interactable advancedSettingsCheckbox;
    [Tooltip("The slider for determining the number of members")]
    [SerializeField] private SliderExtension memberNumberSlider;
    [Tooltip("The button which confirms the settings and creates the room")]
    [SerializeField] private Interactable createRoomButton;

    private GameObject PARTICIPANTLIST;
    private bool windowEnabled = true;

    /// <summary>
    /// States whether the window is enabled
    /// This is different from the window's visibility
    /// If disabled, all controls will be disabled but the window will remain visible
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
            closeButton.Enabled = value;
        }
    }

    public bool WindowOpen { get; private set; }

    /// <summary>
    /// Event which is invoked if the window is closed
    /// </summary>
    public event EventHandler WindowClosed;

    /// <summary>
    /// Initializes the window
    /// Makes sure that all UI elements are referenced and that the component is set up correctly
    /// </summary>
    private void Awake()
    {
        if (closeButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(closeButton));
        }
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
        if (advancedSettingsCheckbox == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(advancedSettingsCheckbox));
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

        createRoomButton.Enabled = false;
        memberNumberSlider.gameObject.SetActive(advancedSettingsCheckbox.GetDimensionIndex() == 1);
    }

    /// <summary>
    /// Called if the text in the input field for the room name is changed
    /// Checks if the entered room name already exists; if yes, an error message is displayed and the room creation cannot be finished
    /// </summary>
    /// <param name="sender">The sender of this event</param>
    /// <param name="e">Generic event arguments</param>
    private void OnInputFieldRoomNameChanged(object sender, EventArgs e)
    {
        bool roomExists = roomMenu.CheckIfRoomExists(roomNameField.Text);
        errorMessage.SetActive(roomExists);
        createRoomButton.Enabled = !roomExists && roomNameField.Text != "";
    }

    /// <summary>
    /// Called if the checkbox is clicked which enables the limiation of members
    /// If it is checked, it displays the slider to specify the maximum number of members
    /// </summary>
    public void OnCheckboxAdvancedSettingsClicked()
    {
        memberNumberSlider.gameObject.SetActive(advancedSettingsCheckbox.GetDimensionIndex() == 1);
    }

    /// <summary>
    /// Called if the create room button is clicked
    /// Invokes the room creation process
    /// </summary>
    public void OnCreateRoomButtonClicked()
    {
        // do not try to create a room if not connected
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        RoomOptions roomOptions = null; // if null is passed to CreateRoom, the options are ignored
        // use the maximum member settings if the checkbox for this is checked
        // otherwise, roomOptions will remain null and so Photon will use default options
        if (advancedSettingsCheckbox.GetDimensionIndex() == 1)
        {
            roomOptions = new RoomOptions { MaxPlayers = (byte)memberNumberSlider.ValueInt };
        }
        PhotonNetwork.CreateRoom(roomNameField.Text, roomOptions);

        WindowManager.Instance.ParticipantList.Open(transform.forward + new Vector3(0.6f, 0.5540501f, 1), transform.localEulerAngles);
    }

    /// <summary>
    /// Opens the window, i.e. makes it visible
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);
        WindowOpen = false;
    }

    /// <summary>
    /// Opens the window, i.e. makes it visible and opens the list of participants
    /// </summary>
    public void Open(GameObject participantList)
    {
        PARTICIPANTLIST = participantList;
        PARTICIPANTLIST.SetActive(true);
        gameObject.SetActive(true);
        WindowOpen = false;
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
    }

    /// <summary>
    /// Hides the window
    /// Invokes the WindowClosed event
    /// </summary>
    public void Close()
    {
        WindowOpen = false;
        WindowClosed?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }
}
