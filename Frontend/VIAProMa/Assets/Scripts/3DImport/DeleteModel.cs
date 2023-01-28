using i5.VIAProMa.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using static SessionBrowserRefresher;
using Photon.Pun;

public class DeleteModel : MonoBehaviour
{
    public GameObject model;

    public void DeleteObject()
    {
        PhotonView viewUI = GameObject.Find("AnchorParent").GetComponentInChildren<ImportManager>().gameObject.GetComponent<PhotonView>();       
        viewUI.RPC("DeleteObjectNetwork", RpcTarget.All, model);       
    }

    
}
