using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatMenu : MonoBehaviour, IWindow
{
    [SerializeField] private TextMeshPro chatHistory;
    [SerializeField] private InputField chatInputField;
    [SerializeField] private InputField chatReceiverInputField; //here
    [SerializeField] private Interactable sendButton;
    [SerializeField] private Interactable pageUpButton;
    [SerializeField] private Interactable pageDownButton;

    public bool WindowEnabled // not used
    {
        get; set;
    }

    public bool WindowOpen
    {
        get
        {
            return gameObject.activeSelf;
        }
    }

    public event EventHandler WindowClosed;

    private void Awake()
    {
        if (chatHistory == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(chatHistory));
        }
        if (chatInputField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(chatInputField));
        }
        if (chatReceiverInputField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(chatInputField));
        }
        if (sendButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(sendButton));
        }
        if (pageUpButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(pageUpButton));
        }
        if (pageDownButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(pageDownButton));
        }
    }

    private void Start()
    {
        chatHistory.text = "";
        ChatManager.Instance.MessageReceived += OnMessageReceived;
        // catch up with messages which have already been collected
        foreach (ChatMessageEventArgs msg in ChatManager.Instance.ChatMessages)
        {
            chatHistory.text += CreateMessageString(msg);
        }
        ChatManager.Instance.ChatMessages.Clear();
        ChatManager.Instance.RecordMessages = false; // no need to record messages anymore
        chatInputField.TextChanged += OnMessageTextChanged;
        chatReceiverInputField.TextChanged += OnMessageTextChanged; //here
        sendButton.Enabled = !string.IsNullOrEmpty(chatInputField.Text);
        sendButton.Enabled = !string.IsNullOrEmpty(chatReceiverInputField.Text); //IsNullOrEmpty kann keine zwei Argumente annehmen
        CheckPageButtons();
    }

    private void OnMessageTextChanged(object sender, EventArgs e)
    {
        sendButton.Enabled = !string.IsNullOrEmpty(chatInputField.Text);
    }

    private void OnDestroy()
    {
        if (ChatManager.Instance != null)
        {
            ChatManager.Instance.MessageReceived -= OnMessageReceived;
        }
    }

    private void OnMessageReceived(object sender, ChatMessageEventArgs e)
    {
        chatHistory.text += CreateMessageString(e);
        chatHistory.ForceMeshUpdate();
        chatHistory.pageToDisplay = chatHistory.textInfo.pageCount; // go to the last page to show the new text
        CheckPageButtons();
    }

    public void Send()
    {
        if (!string.IsNullOrEmpty(chatInputField.Text))
        {
            ChatManager.Instance.SendChatMessage(chatInputField.Text);
            chatInputField.Text = "";
        }
    }

    public void PageUp()
    {
        chatHistory.pageToDisplay = Mathf.Max(chatHistory.pageToDisplay - 1, 0);
        CheckPageButtons();
    }

    public void PageDown()
    {
        chatHistory.pageToDisplay = Mathf.Min(chatHistory.pageToDisplay + 1, chatHistory.textInfo.pageCount);
        CheckPageButtons();
    }

    private void CheckPageButtons()
    {
        Debug.Log("Page to display: " + chatHistory.pageToDisplay);
        Debug.Log("Page count: " + chatHistory.textInfo.pageCount);
        pageUpButton.Enabled = chatHistory.pageToDisplay > 1;
        pageDownButton.Enabled = chatHistory.pageToDisplay < chatHistory.textInfo.pageCount;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        NotificationSystem.Instance.CanShowMessages = false;
        NotificationSystem.Instance.HideMessage();
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.position = position;
        transform.eulerAngles = eulerAngles;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if (NotificationSystem.Instance != null)
        {
            NotificationSystem.Instance.CanShowMessages = true;
        }
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }

    private string CreateMessageString(ChatMessageEventArgs msg)
    {
        string res = "";
        // go to a new line if there is already content in the chat
        if (!string.IsNullOrEmpty(chatHistory.text))
        {
            res += "\n";
        }
        if (msg.MessageSender == null) // local message
        {
            res += "<i>" + msg.Message + "</i>"; // local messages are in italics
        }
        else
        {
            res += "<b>" + msg.MessageSender.NickName + "</b>: " + msg.Message;
        }
        return res;
    }


    public void getUsername(string username)
    {
        string setUsername = username;
    }

}
