using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IdCardController : MonoBehaviourPun
{
    [SerializeField] private TextMeshPro nameLabel;
    [SerializeField] private TextMeshPro roleLabel;
    [SerializeField] private Renderer profileImageRenderer;

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
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            nameLabel.text = photonView.Owner.NickName;
            roleLabel.text = "";
        }
    }

    [PunRPC]
    private void OnIDCardPropertyUpdated()
    {
        nameLabel.text = photonView.Owner.NickName;
        roleLabel.text = "";
    }
}
