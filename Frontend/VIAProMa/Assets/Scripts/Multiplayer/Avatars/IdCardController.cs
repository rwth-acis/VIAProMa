using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IdCardController : MonoBehaviour
{
    [SerializeField] private TextMeshPro nameLabel;
    [SerializeField] private TextMeshPro roleLabel;
    [SerializeField] private Renderer profileImageRenderer;

    private PhotonView photonView;

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
