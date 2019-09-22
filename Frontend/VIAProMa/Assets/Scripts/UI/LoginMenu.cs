using ExitGames.Client.Photon;
using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginMenu : MonoBehaviour, IWindow
{
    [SerializeField] private InputField nameInputField;
    [SerializeField] private Interactable doneButton;

    public bool WindowEnabled { get; set; }

    public bool WindowOpen => gameObject.activeSelf;

    public event EventHandler WindowClosed;

    private void Awake()
    {
        if (nameInputField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(nameInputField));
        }
        else
        {
            nameInputField.TextChanged += NameInputChanged;
        }
        if (doneButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(doneButton));
        }
    }

    private void NameInputChanged(object sender, EventArgs e)
    {
        doneButton.Enabled = !string.IsNullOrWhiteSpace(nameInputField.Text);
    }

    private void OnEnable()
    {
        nameInputField.Text = PhotonNetwork.NickName;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
    }

    public void SetName()
    {
        if (!string.IsNullOrWhiteSpace(nameInputField.Text))
        {
            PhotonNetwork.NickName = nameInputField.Text;
            RaiseNameChangedEvent();
        }
        Close();
    }

    private void RaiseNameChangedEvent()
    {
        if (PhotonNetwork.InRoom)
        {
            byte eventCode = 1;
            byte content = 0;
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, sendOptions);
        }
    }
}
