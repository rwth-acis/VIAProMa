using i5.ViaProMa.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls the UI for the server status menu
/// This menu indicates whether the backend servers are online
/// </summary>
public class ServerStatusMenu : MonoBehaviourPunCallbacks, IWindow
{
    [Header("UI Elements")]
    [SerializeField] private GameObject backendLed;
    [SerializeField] private GameObject sharingLed;
    [SerializeField] private TextMeshPro sharingConnectButtonText;
    [SerializeField] private GameObject backendSettingsMenu;
    [SerializeField] private InputField backendAddressField;

    [Header("Values")]
    [SerializeField] private Color serverOnlineColor = new Color(0f, 135f/255f, 3f/255f); // green
    [SerializeField] private Color serverOfflineColor = new Color(188f/255f, 2f/255f, 0f); // red

    private Renderer backendLedRenderer;
    private Renderer sharingLedRenderer;

    public bool WindowEnabled { get; set; } // not used here

    public event EventHandler WindowClosed;

    /// <summary>
    /// Initializes the component and makes sure that it is set up correctly
    /// </summary>
    private void Awake()
    {
        if (backendLed == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(backendLed));
        }
        if (sharingLed == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(sharingLed));
        }
        if (sharingConnectButtonText == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(sharingConnectButtonText));
        }
        if (backendSettingsMenu == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(backendSettingsMenu));
        }
        else
        {
            backendSettingsMenu.SetActive(false);
        }
        if (backendAddressField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(backendAddressField));
        }
        else
        {
            backendAddressField.TextChanged += BackendAddressChanged;
        }

        backendLedRenderer = backendLed?.GetComponent<Renderer>();
        sharingLedRenderer = sharingLed?.GetComponent<Renderer>();

        if (backendLedRenderer == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(Renderer), backendLed);
        }
        if (sharingLedRenderer == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(Renderer), sharingLed);
        }

        SetLED(backendLedRenderer, false);
        SetSharingServerStatus(false);
    }

    private void BackendAddressChanged(object sender, EventArgs e)
    {
        ConnectionManager.Instance.BackendAddress = backendAddressField.Text;
    }

    public void OpenBackendSettings()
    {
        backendSettingsMenu.SetActive(true);
    }

    public void CloseBackendSettings()
    {
        backendSettingsMenu.SetActive(false);
    }

    /// <summary>
    /// Check the connection
    /// Subscribe for the backend status changed event
    /// </summary>
    private void Start()
    {
        ConnectionManager.Instance.BackendOnlineStatusChanged += BackendOnlineStatusChanged;
        backendAddressField.Text = ConnectionManager.Instance.BackendAddress;
        TestBackendConnection();
    }

    /// <summary>
    /// Called if the client finds out that the backend changed its status from online to offline or vice versa
    /// Changes the LED for the backend based on the online/offline status
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">Generic event arguments</param>
    private void BackendOnlineStatusChanged(object sender, EventArgs e)
    {
        SetLED(backendLedRenderer, ConnectionManager.Instance.BackendReachable);
    }

    /// <summary>
    /// Called by Photon if the client connects to the networking server
    /// Shows the networking server as connected
    /// </summary>
    public override void OnConnected()
    {
        SetSharingServerStatus(true);
    }

    /// <summary>
    /// Called by Photon if the client disconnects from the networking server
    /// Shows teh networking server as disconnected
    /// </summary>
    /// <param name="cause">Contains information about the cause for the disconnect</param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        SetSharingServerStatus(false);
    }

    /// <summary>
    /// Tests the backend connection by sending a "ping" message to the server
    /// </summary>
    /// <returns>async operation</returns>
    public async void TestBackendConnection()
    {
        bool res = await BackendConnector.Ping();
        SetLED(backendLedRenderer, res);
    }

    /// <summary>
    /// Called if the user clicks the connect/disconnect button for the networking server
    /// Depending on the current conneciton status, it either connects or disconnects to/from the server
    /// </summary>
    public void SharingConnectDisconnectButtonClicked()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            // settings are set up in the launcher on application startup
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /// <summary>
    /// Sets the connection status of the sharing server
    /// </summary>
    /// <param name="online">True, if the sharing server is online</param>
    private void SetSharingServerStatus(bool online)
    {
        // indicate the status
        SetLED(sharingLedRenderer, online);
        // change the text on the connect/disconnect button to the operation which will happen when pushing the button
        // if connected, the button will show "Disconnect" and vice versa
        if (online)
        {
            sharingConnectButtonText.text = "Disconnect";
        }
        else
        {
            sharingConnectButtonText.text = "Connect";
        }
    }

    /// <summary>
    /// Sets the emission color of the given LED renderer based on the online status of the corresponding server
    /// </summary>
    /// <param name="ledRenderer">The renderer of the LED to change</param>
    /// <param name="online">True, if the corresponding server is online; otherwise false</param>
    private void SetLED(Renderer ledRenderer, bool online)
    {
        if (online)
        {
            ledRenderer.material.SetColor("_EmissiveColor", serverOnlineColor);
        }
        else
        {
            ledRenderer.material.SetColor("_EmissiveColor", serverOfflineColor);
        }
    }

    /// <summary>
    /// Opens the window by making the GameObject active
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Closes the window and raises the WindowClosed event
    /// Deactivates the GameObject (so the window still exists but is invisible)
    /// </summary>
    public void Close()
    {
        WindowClosed?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }
}
