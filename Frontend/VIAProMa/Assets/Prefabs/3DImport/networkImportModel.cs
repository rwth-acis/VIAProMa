using i5.VIAProMa.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SessionBrowserRefresher;

public class networkImportModel : MonoBehaviour
{
    public GameObject obj;
    public ImportedObject model;
    public int networkID;

    // Start is called before the first frame update
    void Start()
    {
        SessionBrowserRefresher refresher = GameObject.Find("AnchorParent").GetComponentInChildren<SessionBrowserRefresher>();

        GameObject modelWrapper = GameObject.Find("AnchorParent").GetComponentInChildren<ImportManager>().modelWrapper;

        GameObject instantiatedObj = Instantiate(obj);
        instantiatedObj.transform.SetParent(modelWrapper.transform);

        model.gameObject = instantiatedObj;
        refresher.AddItem(model);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        obj = (GameObject)instantiationData[0];
        model = (ImportedObject)instantiationData[1];
    }
}
