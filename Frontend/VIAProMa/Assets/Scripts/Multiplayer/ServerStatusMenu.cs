using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerStatusMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject backendLed;
    [SerializeField] private GameObject sharingLed;

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
        SetLED(sharingLedRenderer, false);
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
        SetLED(sharingLedRenderer, true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetLED(sharingLedRenderer, false);
    }

    private async void TestBackendConnection()
    {
        bool res = await BackendConnector.Ping();
        SetLED(backendLedRenderer, res);
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
