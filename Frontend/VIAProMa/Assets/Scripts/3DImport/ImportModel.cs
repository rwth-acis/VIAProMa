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
using System;
using ExitGames.Client.Photon.StructWrapping;
using i5.VIAProMa.UI;
using static SessionBrowserRefresher;
using TMPro;
using Photon;
using Photon.Pun;

public class ImportModel : MonoBehaviour
{
    public string path;
    public ImportedObject model;

    public GameObject modelWrapper;

    [SerializeField] private Shader GLTFshaderMetallic;
    [SerializeField] private Shader GLTFshaderMetallicTransparent;
    [SerializeField] private Shader GLTFshaderSpecular;
    [SerializeField] private Shader GLTFshaderSpecularTransparent;

    [SerializeField] private Shader shaderMetallic;
    [SerializeField] private Shader shaderMetallicTransparent;
    [SerializeField] private Shader shaderSpecular;
    [SerializeField] private Shader shaderSpecularTransparent;


    void Start()
    {
        if (GetComponent<ImportManager>() == null)
        {
            modelWrapper = GetComponentInParent<ImportManager>().modelWrapper;
        }
    }
    public void LoadModel()
    {
        //was file deleted?
        if (!System.IO.File.Exists(path))
        {
            GetComponentInParent<SearchBrowserRefresher>().SearchChanged(model.webLink);
            GetComponentInParent<SearchBrowserRefresher>().searchBarText.GetComponent<TextMeshPro>().text = model.webLink;
            return;
        }




        GameObject testModel = LoadModel(path);

        testModel.name = model.fileName;

        model.gameObject = testModel;

        this.gameObject.GetComponentInParent<SessionBrowserRefresher>().AddItem(model);

        object[] objs = { testModel, model };
        PhotonNetwork.Instantiate("networkModel", gameObject.transform.position, gameObject.transform.rotation, 0, objs);



    }

    private void UpdateImpObj(ImportedObject impObj)
    {
        impObj.dateOfDownload = System.IO.File.GetCreationTime(path).ToString();
        impObj.size = BytesToNiceString(new System.IO.FileInfo(path).Length);
        //impObj.creator = photonView.Owner.NickName;

        Transform tr = impObj.gameObject.transform;
        impObj.gameObject = LoadModel(path);
        impObj.gameObject.transform.position = tr.position;
        impObj.gameObject.transform.rotation = tr.rotation;
    }


    //function, that only spawns the new GameObject correctly
    public GameObject LoadModel(string path)
    {
        //import into unity scene
        AnimationClip[] animClips;
        GameObject testModel = Importer.LoadFromFile(path, new ImportSettings(), out animClips);
        testModel.transform.SetParent(modelWrapper.transform);
        testModel.transform.position = Vector3.zero;
        testModel.transform.rotation = Quaternion.identity;
        testModel.transform.localScale = Vector3.one;

        //resize object according to mesh bounds
        Renderer[] rr = testModel.GetComponentsInChildren<Renderer>();
        Bounds bounds = rr[0].bounds;
        foreach (Renderer r in rr) { bounds.Encapsulate(r.bounds); }

        //MeshFilter[] ff = testModel.GetComponentsInChildren<MeshFilter>();
        //foreach (MeshFilter f in ff) { bounds.Encapsulate(f.mesh.bounds); }


        //add interactables and collider
        testModel.AddComponent<BoxCollider>();
        testModel.GetComponent<BoxCollider>().size = bounds.size;
        testModel.GetComponent<BoxCollider>().center = bounds.center;


        // Taking only the first clip for now. Should be pretty easy to extend it  to generalize
        if (animClips.Length > 0)
        {
            Animation anim = testModel.AddComponent<Animation>();
            animClips[0].legacy = true;
            animClips[0].wrapMode = WrapMode.Loop;
            anim.AddClip(animClips[0], animClips[0].name);
            anim.Play(animClips[0].name);
        }

        //changing the shader
        foreach (Renderer r in rr)
        {
            if (r.material.shader == GLTFshaderMetallic)
            {
                r.material.shader = shaderMetallic;
            }
            else if (r.material.shader == GLTFshaderMetallicTransparent)
            {
                r.material.shader = shaderMetallicTransparent;
            }
            else if (r.material.shader == GLTFshaderSpecular)
            {
                r.material.shader = shaderSpecular;
            }
            else if (r.material.shader == GLTFshaderSpecularTransparent)
            {
                r.material.shader = shaderSpecularTransparent;
            }
        }

        testModel.transform.localScale = testModel.transform.localScale / (bounds.size.magnitude * 4f);


        testModel.transform.rotation = this.gameObject.transform.rotation;
        testModel.transform.eulerAngles += new Vector3(-90, -180, 0);
        //testModel.transform.position = this.gameObject.transform.position;

        //use the center of the bounding box to set the object
        testModel.transform.position = testModel.transform.position + (this.gameObject.transform.position - testModel.transform.TransformPoint(testModel.GetComponent<BoxCollider>().center));
        testModel.transform.position = testModel.transform.position - this.gameObject.transform.forward * 0.1f;

        testModel.AddComponent<NearInteractionGrabbable>();
        testModel.AddComponent<ObjectManipulator>();
        testModel.GetComponent<ObjectManipulator>().HostTransform = testModel.transform;

        testModel.name = System.IO.Path.GetFileNameWithoutExtension(path);

        //testModel.AddComponent<PhotonView>();
        testModel.AddComponent<PhotonTransformView>();
        //testModel.GetComponent<PhotonTransformView>().m_SynchronizePosition = true;

        return testModel;
    }
    static String BytesToNiceString(long byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
        if (byteCount == 0)
            return "0" + suf[0];
        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return (Math.Sign(byteCount) * num).ToString() + suf[place];
    }
}