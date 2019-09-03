using i5.ViaProMa.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatMenu : MonoBehaviour, IWindow
{
    [SerializeField] private TextMeshPro chatHistory;
    [SerializeField] private InputField chatInputField;

    public bool WindowEnabled
    {
        get;set;
    }

    public bool WindowOpen
    {
        get
        {
            return gameObject.activeSelf;
        }
    }

    public event EventHandler WindowClosed;

    private void Start()
    {
        ChatManager.Instance.MessageReceived += OnMessageReceived;
    }

    private void OnDestroy()
    {
        ChatManager.Instance.MessageReceived -= OnMessageReceived;
    }

    private void OnMessageReceived(object sender, ChatMessageEventArgs e)
    {
        if (e.MessageSender == null) // local message
        {
            chatHistory.text += "\n" + e.Message;
        }
        else
        {
            chatHistory.text += "\n" + e.MessageSender + ": " + e.Message;
        }
    }

    public void SendMessage()
    {
        if (!string.IsNullOrEmpty(chatInputField.Text))
        {
            ChatManager.Instance.SendChatMessage(chatInputField.Text);
            chatInputField.Text = "";
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.position = position,
        transform.eulerAngles = eulerAngles,
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
