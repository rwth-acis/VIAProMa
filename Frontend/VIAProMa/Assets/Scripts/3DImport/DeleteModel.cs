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
        PhotonView viewButton = GetComponent<PhotonView>();
        viewButton.RPC("DeleteObjectNetwork", RpcTarget.All);       
    }

    [PunRPC]
    public void DeleteObjectNetwork()
    {
        //Refresh session browser correctly and delete actual GameObject
        SessionBrowserRefresher refresher = GetComponentInParent<SessionBrowserRefresher>();
        ImportedObject deleteThisItem = refresher.importedObjects.Find(e => e.gameObject == model);
        Destroy(model.transform.parent.gameObject);
        refresher.importedObjects.Remove(deleteThisItem);

        //was FILE deleted?
        //if (!System.IO.File.Exists(Path.Combine(Application.persistentDataPath, GetComponentInParent<ImportManager>().folderName, deleteThisItem.fileName + ".glb")))
        //{
        //    GetComponentInParent<SearchBrowserRefresher>().SearchChanged(deleteThisItem.webLink);
        //    GetComponentInParent<SearchBrowserRefresher>().searchBarText.GetComponent<TextMeshPro>().text = deleteThisItem.webLink;
        //    return;
        //}

        refresher.Refresh(refresher.head);
    }
}
