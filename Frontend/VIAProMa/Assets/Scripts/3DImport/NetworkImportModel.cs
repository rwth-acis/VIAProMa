using i5.VIAProMa.Multiplayer.Common;
using i5.VIAProMa.SaveLoadSystem.Core;
using i5.VIAProMa.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using TMPro;
using UnityEditor;
using UnityEngine;
using static SessionBrowserRefresher;

public class NetworkImportModel : MonoBehaviour, IPunInstantiateMagicCallback, i5.VIAProMa.SaveLoadSystem.Core.ISerializable
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

    IEnumerator InstantiateAfterLoad()
    {
        Debug.Log("Hello! OnPhotonInstantiateLOAD");
        GameObject anch = GameObject.Find("AnchorParent");
        SessionBrowserRefresher refresher = anch.GetComponentInChildren<SessionBrowserRefresher>();

        yield return new WaitUntil(() => anch.GetComponentInChildren<ImportManager>().modelWrapper != null);

        GameObject modelWrapper = anch.GetComponentInChildren<ImportManager>().modelWrapper;

        model.gameObject = null;

        path = Path.Combine(Application.persistentDataPath, anch.GetComponentInChildren<ImportManager>().folderName, model.fileName + ".glb");

        ImportModel impModel = anch.GetComponentInChildren<ImportManager>().gameObject.GetComponent<ImportModel>();

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

        //this.gameObject.transform.localScale = testModel.transform.localScale;
        testModel.transform.localScale = Vector3.one;

        //this.gameObject.transform.position = buttonPosition;
        //this.gameObject.transform.rotation = buttonRotation;
        //this.gameObject.transform.eulerAngles += new Vector3(-90, -180, 0);
        //use the center of the bounding box to set the object
        //this.gameObject.transform.position = this.gameObject.transform.position + (buttonPosition - this.gameObject.transform.TransformPoint(GetComponent<BoxCollider>().center));
        //this.gameObject.transform.position = this.gameObject.transform.position - (buttonRotation * Vector3.forward) * 0.1f;

        testModel.transform.parent = this.gameObject.transform;
        this.gameObject.transform.parent = modelWrapper.transform;

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

    public i5.VIAProMa.SaveLoadSystem.Core.SerializedObject Serialize()
    {
        i5.VIAProMa.SaveLoadSystem.Core.SerializedObject serializedObject = new i5.VIAProMa.SaveLoadSystem.Core.SerializedObject();


        serializedObject.Strings.Add("importedModel_webLinkKey", model.webLink);
        serializedObject.Strings.Add("importedModel_fileNameKey", model.fileName);
        serializedObject.Strings.Add("importedModel_dateKey", model.dateOfDownload);
        serializedObject.Strings.Add("importedModel_sizeKey", model.size);
        serializedObject.Strings.Add("importedModel_licenceKey", model.licence);
        /*
        serializedObject.Vector3s.Add("importedModel_positionKey", model.gameObject.transform.position);
        serializedObject.Quaternions.Add("importedModel_rotationKey", model.gameObject.transform.rotation);
        */

        return serializedObject;
    }

    public void Deserialize(i5.VIAProMa.SaveLoadSystem.Core.SerializedObject serializedObject)
    {
        model = new ImportedObject();
        string web_link = i5.VIAProMa.SaveLoadSystem.Core.SerializedObject.TryGet("importedModel_webLinkKey", serializedObject.Strings, gameObject, out bool found_web_link);
        if (found_web_link)
        {
            model.webLink = web_link;
        }
        string file_name = i5.VIAProMa.SaveLoadSystem.Core.SerializedObject.TryGet("importedModel_fileNameKey", serializedObject.Strings, gameObject, out bool found_file_name);
        if (found_file_name)
        {
            model.fileName = file_name;
        }
        string date = i5.VIAProMa.SaveLoadSystem.Core.SerializedObject.TryGet("importedModel_dateKey", serializedObject.Strings, gameObject, out bool found_date);
        if (found_date)
        {
            model.dateOfDownload = date;
        }
        string size = i5.VIAProMa.SaveLoadSystem.Core.SerializedObject.TryGet("importedModel_sizeKey", serializedObject.Strings, gameObject, out bool found_size);
        if (found_size)
        {
            model.size = size;
        }
        string licence = i5.VIAProMa.SaveLoadSystem.Core.SerializedObject.TryGet("importedModel_licenceKey", serializedObject.Strings, gameObject, out bool found_licence);
        if (found_licence)
        {
            model.licence = licence;
        }
        /*
        Vector3 position = i5.VIAProMa.SaveLoadSystem.Core.SerializedObject.TryGet("importedModel_positionKey", serializedObject.Vector3s, gameObject, out bool found_position);
        if (found_position)
        {
            model.gameObject.transform.position = position;
        }
        Quaternion rotation = i5.VIAProMa.SaveLoadSystem.Core.SerializedObject.TryGet("importedModel_rotationKey", serializedObject.Quaternions, gameObject, out bool found_rotation);
        if (found_rotation)
        {
            model.gameObject.transform.rotation = rotation;
        }
        */

        StopAllCoroutines();
        StartCoroutine("InstantiateAfterLoad");
    }

 
}
