﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls the ID card on the avatar and fills its displays with data
/// </summary>
public class IdCardController : MonoBehaviour
{
    [Tooltip("The label which displays the user name")]
    [SerializeField] private TextMeshPro nameLabel;
    [Tooltip("The label which displays the role of the user")]
    [SerializeField] private TextMeshPro roleLabel;
    [Tooltip("The renderer for the profile image")]
    [SerializeField] private Renderer profileImageRenderer;

    private PhotonView photonView;

    /// <summary>
    /// Checks the component's setup and initializes it
    /// </summary>
    private void Awake()
    {
        if (nameLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(nameLabel));
        }
        if (roleLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(roleLabel));
        }
        if (profileImageRenderer == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(profileImageRenderer));
        }

        photonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Initializes the text of the labels
    /// </summary>
    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (photonView != null)
            {
                nameLabel.text = photonView.Owner.NickName;
            }
            else
            {
                nameLabel.text = PhotonNetwork.LocalPlayer.NickName;
            }
            roleLabel.text = "";
        }
    }

    [PunRPC]
    private void OnIDCardPropertyUpdated()
    {
        if (photonView != null)
        {
            nameLabel.text = photonView.Owner.NickName;
        }
        else
        {
            nameLabel.text = PhotonNetwork.LocalPlayer.NickName;
        }
        roleLabel.text = "";
    }
}
