using i5.VIAProMa.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static SessionBrowserRefresher;

public class networkImportModel : MonoBehaviour, IPunInstantiateMagicCallback
{
    public string path;
    public ImportedObject model;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("Hello! OnPhotonInstantiate");
        GameObject anch = GameObject.Find("AnchorParent");
        SessionBrowserRefresher refresher = anch.GetComponentInChildren<SessionBrowserRefresher>();
        //GameObject modelWrapper = anch.GetComponentInChildren<ImportManager>().modelWrapper;

        model = new ImportedObject();
        model.gameObject = null;

        object[] instantiationData = info.photonView.InstantiationData;
        model.webLink = (string)instantiationData[0];
        model.fileName = (string)instantiationData[1];
        model.dateOfDownload = (string)instantiationData[2];
        model.size = (string)instantiationData[3];
        path = Path.Combine(Application.persistentDataPath, anch.GetComponentInChildren<ImportManager>().folderName, model.fileName + ".glb");

        ImportModel impModel = anch.GetComponentInChildren<ImportManager>().gameObject.GetComponent<ImportModel>();

        Vector3 buttonPosition = (Vector3)instantiationData[4];
        Quaternion buttonRotation = (Quaternion)instantiationData[5];

        GameObject testModel = impModel.LoadModel(path);
        testModel.name = model.fileName;

        testModel.transform.rotation = buttonRotation;
        testModel.transform.eulerAngles += new Vector3(-90, -180, 0);
        //testModel.transform.position = this.gameObject.transform.position;

        //use the center of the bounding box to set the object
        testModel.transform.position = testModel.transform.position + (buttonPosition - testModel.transform.TransformPoint(testModel.GetComponent<BoxCollider>().center));
        testModel.transform.position = testModel.transform.position - (buttonRotation * Vector3.forward) * 0.1f;

        model.gameObject = testModel;


        refresher.AddItem(model);
    }
}
