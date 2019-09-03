using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationSystem : MonoBehaviour
{
    [SerializeField] private GameObject notificationWidget;
    [SerializeField] private Animator notificationSphereAnimationController;
    [SerializeField] private Renderer notificationSphereRenderer;
    [SerializeField] private TextMeshPro notificationPreviewLabel;

    private const string emissionColorShaderProperty = "_EmissiveColor";

    private Color origEmissionColor;

    public bool NotificationMessageShown { get; private set; }

    private void Awake()
    {
        if (notificationWidget == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(notificationWidget));
        }
        if (notificationSphereAnimationController == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(notificationSphereAnimationController));
        }
        if (notificationSphereRenderer == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(notificationSphereRenderer));
        }
        else
        {
            origEmissionColor = notificationSphereRenderer.material.GetColor(emissionColorShaderProperty);
        }
        if (notificationPreviewLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(notificationPreviewLabel));
        }
    }

    private void OnEnable()
    {
        ChatManager.Instance.MessageReceived += OnMessageReceived;
    }

    private void OnDisable()
    {
        ChatManager.Instance.MessageReceived -= OnMessageReceived;
    }

    public void MessageRead()
    {
        notificationPreviewLabel.text = "";
        notificationWidget.SetActive(false);
        NotificationMessageShown = false;
    }

    private void OnMessageReceived(object sender, ChatMessageEventArgs e)
    {
        NotificationMessageShown = true;
        notificationWidget.SetActive(true);
        notificationPreviewLabel.text = e.MessageSender.NickName + ": " + e.Message;
    }
}
