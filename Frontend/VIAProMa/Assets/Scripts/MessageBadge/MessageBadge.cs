using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBadge : MonoBehaviour
{
    [SerializeField] private ProcessingEffect processingEffect;
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private TextMeshPro messageText;
    [SerializeField] private MessagePack messages;

    private MessageContent content;
    private int processing = 0;

    private void Awake()
    {
        if (processingEffect == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(processingEffect));
        }
        if (iconRenderer == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(processingEffect));
        }
        if (messageText == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(processingEffect));
        }
        if (messages == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(messages));
        }
    }

    public void ShowProcessing()
    {
        processing++;
        processingEffect.IsProcessing = true;
    }

    public void DoneProcessing()
    {
        processing--;
        if (processing <= 0)
        {
            processing = 0;
            processingEffect.IsProcessing = false;
        }
    }

    private void SetContent(MessageContent newContent)
    {
        content = newContent;
        iconRenderer.sprite = content.Icon;
        messageText.text = content.Text;
    }

    public void ShowMessage(long messageCode)
    {
        SetContent(messages.GetMessage(messageCode));
    }
}
