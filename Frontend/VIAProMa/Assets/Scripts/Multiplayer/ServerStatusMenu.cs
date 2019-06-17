using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServerStatusMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject backendLed;
    [SerializeField] private GameObject sharingLed;
    [SerializeField] private TextMeshPro sharingConnectButtonText;

    [SerializeField] private Color serverOnlineColor = new Color(0f, 135f/255f, 3f/255f); // green
    [SerializeField] private Color serverOfflineColor = new Color(188f/255f, 2f/255f, 0f); // red

    private Renderer backendLedRenderer;
    private Renderer sharingLedRenderer;

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

    private void Start()
    {
        ConnectionManager.Instance.BackendOnlineStatusChanged += BackendOnlineStatusChanged;
        TestBackendConnection();
    }

    private void BackendOnlineStatusChanged(object sender, EventArgs e)
    {
        SetLED(backendLedRenderer, ConnectionManager.Instance.BackendReachable);
    }

    public override void OnConnected()
    {
        SetSharingServerStatus(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetSharingServerStatus(false);
    }

    public async void TestBackendConnection()
    {
        bool res = await BackendConnector.Ping();
        SetLED(backendLedRenderer, res);
    }

    public void SharingConnectDisconnectButtonClicked()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            // settings are set up in the launcher
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void SetSharingServerStatus(bool online)
    {
        SetLED(sharingLedRenderer, online);
        if (online)
        {
            sharingConnectButtonText.text = "Disconnect";
        }
        else
        {
            sharingConnectButtonText.text = "Connect";
        }
    }

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

}
