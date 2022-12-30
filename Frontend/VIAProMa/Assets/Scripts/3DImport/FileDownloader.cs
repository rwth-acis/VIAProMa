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
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using ExitGames.Client.Photon.StructWrapping;
//using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;

public class FileDownloader : MonoBehaviour
{
    public GameObject wrapper;
    
    //private string webLink;
    //private string fileName;

    void Start ()
    {
        string webLink = "http://www.nikita-zaloga.de/undertale-_low_poly.glb";
        string fileName = System.IO.Path.GetFileName(webLink);
        string folderName = "3Dobjects";

        string path = Path.Combine(Application.persistentDataPath, folderName, fileName);

        if (!System.IO.File.Exists(path))
        {
            StartCoroutine(DownloadFile(path, webLink));
        }
        else
        {
            UnityEngine.Debug.Log("File already saved in " + path + ", not downloading");
            LoadModel(path);
        }

    }

    void LoadModel(String path)
    {
        //import into unity scene

        GameObject testModel = Importer.LoadFromFile(path);
        //Byte[] file = System.IO.File.ReadAllBytes(path);
        //GltfObject obj = GltfUtility.GetGltfObjectFromGlb(file);
        //GameObject testModel = ConstructGltf.ConstructAsync(obj).Result;
        testModel.transform.SetParent(wrapper.transform);

        //resize object according to mesh bounds
        MeshFilter[] rr = testModel.GetComponentsInChildren<MeshFilter>();
        Bounds bounds = rr[0].mesh.bounds;
        foreach (MeshFilter r in rr) { bounds.Encapsulate(r.mesh.bounds); }

        testModel.transform.localScale = testModel.transform.localScale / bounds.size.magnitude;

        //add interactables and collider
        testModel.AddComponent<BoxCollider>();
        testModel.GetComponent<BoxCollider>().size = bounds.size;
        testModel.GetComponent<BoxCollider>().center = bounds.center;
        testModel.AddComponent<NearInteractionGrabbable>();
        testModel.AddComponent<ObjectManipulator>();
        testModel.GetComponent<ObjectManipulator>().HostTransform = testModel.transform;
    }
    IEnumerator DownloadFile(string path, string webLink)
    {
        //downloads file
        var uwr = new UnityWebRequest(webLink, UnityWebRequest.kHttpVerbGET);
        uwr.downloadHandler = new DownloadHandlerFile(path);
        yield return uwr.SendWebRequest();
        while (!uwr.downloadHandler.isDone)
        {
            yield return null;
        }

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.LogError(uwr.error);
        }
        
        uwr.downloadHandler.Dispose();
        UnityEngine.Debug.Log("File successfully downloaded and saved to " + path);

        LoadModel(path);
        yield return null;
    }
}