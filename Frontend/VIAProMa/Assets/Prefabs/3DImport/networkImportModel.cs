using i5.VIAProMa.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SessionBrowserRefresher;

public class networkImportModel : MonoBehaviour, IPunInstantiateMagicCallback
{
    public string path;
    public ImportedObject model;

    [SerializeField] private Shader GLTFshaderMetallic;
    [SerializeField] private Shader GLTFshaderMetallicTransparent;
    [SerializeField] private Shader GLTFshaderSpecular;
    [SerializeField] private Shader GLTFshaderSpecularTransparent;

    [SerializeField] private Shader shaderMetallic;
    [SerializeField] private Shader shaderMetallicTransparent;
    [SerializeField] private Shader shaderSpecular;
    [SerializeField] private Shader shaderSpecularTransparent;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("Hello! OnPhotonInstantiate");
        GameObject anch = GameObject.Find("AnchorParent");
        SessionBrowserRefresher refresher = anch.GetComponentInChildren<SessionBrowserRefresher>();
        //GameObject modelWrapper = anch.GetComponentInChildren<ImportManager>().modelWrapper;

        model = new ImportedObject();
        model.gameObject = null;

        object[] instantiationData = info.photonView.InstantiationData;
        path = (string)instantiationData[0];
        model.webLink = (string)instantiationData[1];
        model.fileName = (string)instantiationData[2];
        model.dateOfDownload = (string)instantiationData[3];
        model.size = (string)instantiationData[4];
        

        ImportModel impModel = anch.GetComponentInChildren<ImportManager>().gameObject.GetComponent<ImportModel>();

        model.gameObject = impModel.LoadModel(path);
        model.gameObject.name = model.fileName;

        model.gameObject.transform.position = (Vector3)instantiationData[5];
        model.gameObject.transform.rotation = (Quaternion)instantiationData[6];

        refresher.AddItem(model);
    }
}
