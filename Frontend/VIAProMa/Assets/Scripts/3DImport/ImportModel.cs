#define MRTK_GLTF_IMPORTER_OFF

using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;
using static System.Net.WebRequestMethods;
using System.Text;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Diagnostics;
using System;
using ExitGames.Client.Photon.StructWrapping;
using i5.VIAProMa.UI;
using static SessionBrowserRefresher;

public class ImportModel : MonoBehaviour
{
    public string path;
    public ImportedObject model;

    private GameObject modelWrapper;

    void Start()
    {
        modelWrapper = this.gameObject.GetComponentInParent<ImportManager>().modelWrapper;
    }
    public void LoadModel()
    {
        //UnityEngine.Debug.Log("LoadModelFunctionTriggered");
        //import into unity scene
        GameObject testModel = Importer.LoadFromFile(path);
        testModel.transform.SetParent(modelWrapper.transform);
        testModel.transform.position = this.gameObject.transform.position
                                       - this.gameObject.transform.forward * 0.1f
                                       - this.gameObject.transform.up * 0.05f;
        testModel.transform.rotation = this.gameObject.transform.rotation;
        testModel.transform.eulerAngles += new Vector3(-90, -180, 0);

        //resize object according to mesh bounds
        MeshFilter[] rr = testModel.GetComponentsInChildren<MeshFilter>();
        Bounds bounds = rr[0].mesh.bounds;
        foreach (MeshFilter r in rr) { bounds.Encapsulate(r.mesh.bounds); }

        testModel.transform.localScale = testModel.transform.localScale / (bounds.size.magnitude * 3.5f);

        //add interactables and collider
        testModel.AddComponent<BoxCollider>();
        testModel.GetComponent<BoxCollider>().size = bounds.size;
        testModel.GetComponent<BoxCollider>().center = bounds.center;
        testModel.AddComponent<NearInteractionGrabbable>();
        testModel.AddComponent<ObjectManipulator>();
        testModel.GetComponent<ObjectManipulator>().HostTransform = testModel.transform;

        testModel.name = model.fileName;

        model.gameObject = testModel;

        this.gameObject.GetComponentInParent<SessionBrowserRefresher>().AddItem(model);
    }

    
}
