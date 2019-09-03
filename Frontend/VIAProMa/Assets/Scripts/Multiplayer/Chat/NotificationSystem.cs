using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationSystem : MonoBehaviour
{
    [SerializeField] private GameObject notificationWidget;
    [SerializeField] private TextMeshPro notificationPreviewLabel;

    private void Awake()
    {
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
        MessageRead();
    }

    private void OnDestroy()
    {
        if (ChatManager.Instance != null)
        {
            ChatManager.Instance.MessageReceived -= OnMessageReceived;
        }
    }

    public void MessageRead()
    {
        notificationPreviewLabel.text = "";
        gameObject.SetActive(false);
    }

    private void OnMessageReceived(object sender, ChatMessageEventArgs e)
    {
        gameObject.SetActive(true);
        notificationPreviewLabel.text = e.MessageSender.NickName + ": " + e.Message;
    }
}
