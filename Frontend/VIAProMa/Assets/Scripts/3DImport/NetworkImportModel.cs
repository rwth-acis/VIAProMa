using i5.VIAProMa.Multiplayer.Common;
using i5.VIAProMa.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using static SessionBrowserRefresher;

public class NetworkImportModel : MonoBehaviour, IPunInstantiateMagicCallback
{
    public string path;
    public ImportedObject model;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        StartCoroutine("InstantiateModel", info);     
    }

    IEnumerator InstantiateModel(PhotonMessageInfo info)
    {
        Debug.Log("Hello! OnPhotonInstantiate");
        GameObject anch = GameObject.Find("AnchorParent");
        SessionBrowserRefresher refresher = anch.GetComponentInChildren<SessionBrowserRefresher>();

        yield return new WaitUntil(() => anch.GetComponentInChildren<ImportManager>().modelWrapper != null);

        GameObject modelWrapper = anch.GetComponentInChildren<ImportManager>().modelWrapper;



        model = new ImportedObject();
        model.gameObject = null;

        object[] instantiationData = info.photonView.InstantiationData;
        model.webLink = (string)instantiationData[0];
        model.fileName = (string)instantiationData[1];
        model.dateOfDownload = (string)instantiationData[2];
        model.size = (string)instantiationData[3];
        model.licence = (string)instantiationData[4];
        path = Path.Combine(Application.persistentDataPath, anch.GetComponentInChildren<ImportManager>().folderName, model.fileName + ".glb");

        ImportModel impModel = anch.GetComponentInChildren<ImportManager>().gameObject.GetComponent<ImportModel>();

        Vector3 buttonPosition = (Vector3)instantiationData[5];
        Quaternion buttonRotation = (Quaternion)instantiationData[6];


        GameObject testModel = impModel.LoadModel(path);
        if (testModel == null)
        {
            //if sketchfab
            if (!model.webLink.EndsWith(".glb"))
            {
                StartCoroutine(anch.GetComponentInChildren<ImportManager>().gameObject.GetComponent<SketchfabLinkGenerator>().GetDownloadLink(model.webLink, model.licence));
            }
            else
            //if direct link
            {
                anch.GetComponentInChildren<SearchBrowserRefresher>().SearchChanged(model.webLink, "", "");
            }
            anch.GetComponentInChildren<SearchBrowserRefresher>().searchBarText.GetComponent<TextMeshPro>().text = model.webLink;
            yield return new WaitUntil(() => System.IO.File.Exists(Path.Combine(Application.persistentDataPath, anch.GetComponentInChildren<ImportManager>().folderName, model.fileName + ".txt")));

            testModel = impModel.LoadModel(path);
        }
        testModel.name = model.fileName;
        testModel.transform.position = Vector3.zero;
        testModel.transform.rotation = Quaternion.identity;

        testModel.GetComponent<NearInteractionGrabbable>().enabled = false;
        testModel.GetComponent<ObjectManipulator>().enabled = false;

        GetComponent<BoxCollider>().size = testModel.GetComponent<BoxCollider>().size;
        GetComponent<BoxCollider>().center = testModel.GetComponent<BoxCollider>().center;
        testModel.GetComponent<BoxCollider>().enabled = false;

        this.gameObject.transform.localScale = testModel.transform.localScale;
        testModel.transform.localScale = Vector3.one;

        this.gameObject.transform.position = buttonPosition;
        this.gameObject.transform.rotation = buttonRotation;
        this.gameObject.transform.eulerAngles += new Vector3(-90, -180, 0);
        //use the center of the bounding box to set the object
        this.gameObject.transform.position = this.gameObject.transform.position + (buttonPosition - this.gameObject.transform.TransformPoint(GetComponent<BoxCollider>().center));
        this.gameObject.transform.position = this.gameObject.transform.position - (buttonRotation * Vector3.forward) * 0.1f;

        testModel.transform.parent = this.gameObject.transform;
        this.gameObject.transform.parent = modelWrapper.transform;

        //this.gameObject.AddComponent<PhotonTransformView>();

        testModel.transform.localPosition = Vector3.zero;
        testModel.transform.localRotation = Quaternion.identity;
        testModel.transform.localScale = Vector3.one;

        model.gameObject = this.gameObject;
        refresher.AddItem(model);
    }

    public void MakeMasterClient()
    {
        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
    }
}
