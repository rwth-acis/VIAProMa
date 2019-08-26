using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssueShelfSynchronizer : TransformSynchronizer
{
    [PunRPC]
    private void SetActive(bool active)
    {
        Debug.Log("Received SetActive RPC");
        gameObject.SetActive(active);
    }

    private void OnEnable()
    {
        photonView.RPC("SetActive", RpcTarget.Others, true);
    }

    private void OnDisable()
    {
        photonView.RPC("SetActive", RpcTarget.Others, false);
    }
}
