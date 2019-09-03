using HoloToolkit.Unity;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationSystem : Singleton<NotificationSystem>
{
    [SerializeField] private GameObject notificationWidget;
    [SerializeField] private TextMeshPro notificationPreviewLabel;

    protected override void Awake()
    {
        base.Awake();
        if (notificationWidget == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(notificationWidget));
        }
        if (notificationPreviewLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(notificationPreviewLabel));
        }
    }

    private void Start()
    {
        ChatManager.Instance.MessageReceived += OnMessageReceived;
        HideMessage();
    }

    protected override void OnDestroy()
    {
        if (ChatManager.Instance != null)
        {
            ChatManager.Instance.MessageReceived -= OnMessageReceived;
        }
        base.OnDestroy();
    }

    public void ShowMessage(string text)
    {
        gameObject.SetActive(true);
        notificationPreviewLabel.text = text;
    }

    public void HideMessage()
    {
        Debug.Log("Message Read");
        notificationPreviewLabel.text = "";
        gameObject.SetActive(false);
    }

    private void OnMessageReceived(object sender, ChatMessageEventArgs e)
    {
        Debug.Log("Message Received: " + e.MessageSender.NickName + ": " + e.Message);
        ShowMessage(e.MessageSender.NickName + ": " + e.Message);
    }
}
