using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Diagnostics;
using System;
using TMPro;
using System.IO.Enumeration;
using Photon.Pun;
using UnityEditor;
using i5.VIAProMa.UI;
using static SessionBrowserRefresher;
using Microsoft.MixedReality.Toolkit;
using System.Collections.Generic;

public class SearchBrowserRefresher : MonoBehaviour
{
    private GameObject modelWrapper;

    [SerializeField] private GameObject itemWrapper;
    [SerializeField] private GameObject sessionItemWrapper;

    public GameObject searchBarText;

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Texture loadingSymbolTex;
    [SerializeField] private Texture downloadErrorTex;
    [SerializeField] private Texture noThumbTex;

    [SerializeField] private GameObject errorObj;

    private Vector3 itemStartPosition;
    private Vector3 itemPositionOffset;

    private Vector3 sessionItemStartPosition;
    private Vector3 sessionItemPositionOffset;

    private UnityWebRequest uwr;
    private Coroutine downloadRoutine;
    private string tempPath;

    private int linkOrFileNameLength;

    [SerializeField] private Interactable headUpButton;
    [SerializeField] private Interactable headDownButton;

    public Transform mainCamTr;
    void Start()
    {
        modelWrapper = this.gameObject.GetComponent<ImportManager>().modelWrapper;

        linkOrFileNameLength = 54;

        itemStartPosition = new Vector3(0, 0.12f, -0.02f);
        itemPositionOffset = new Vector3(0, -0.09f, 0);

        sessionItemStartPosition = new Vector3(0, 0.2f, -0.02f);
        sessionItemPositionOffset = new Vector3(0, -0.11f, 0);

        SearchChanged("");
    }

    public void SearchChanged(string searchContent)
    {
        UnityEngine.Debug.Log(searchContent);
        //refresh search browser
        foreach (Transform child in itemWrapper.transform)
        {
            Destroy(child.gameObject);
        }

        //make sure to stop any downloading models            
        if (downloadRoutine != null) { StopCoroutine(downloadRoutine); }
        if (uwr != null) { uwr.downloadHandler.Dispose(); }
        if (tempPath != null) {
            string temptemp = tempPath;
            tempPath = null;
            GetComponent<SessionBrowserRefresher>().Refresh(GetComponent<SessionBrowserRefresher>().head);
            File.Delete(temptemp);
        }

        //deactivate up/down buttons
        headDownButton.GetComponentInChildren<TextMeshPro>().color = Color.grey;
        headDownButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.grey);
        headDownButton.IsEnabled = false;
        headUpButton.GetComponentInChildren<TextMeshPro>().color = Color.grey;
        headUpButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.grey);
        headUpButton.IsEnabled = false;

        //check, if content is a valid link
        if ((StringStartsWith(searchContent, "http://") || StringStartsWith(searchContent, "https://"))
        && StringEndsWith(searchContent, ".glb"))
        {
            string fileName = System.IO.Path.GetFileName(searchContent);
            if (fileName == ".glb")
            {
                RefreshBrowserDownloadError();
                return;
            }
            string path = Path.Combine(Application.persistentDataPath, GetComponent<ImportManager>().folderName, fileName);
            if (!System.IO.File.Exists(path))
            {
                downloadRoutine = StartCoroutine(DownloadFile(path, searchContent));
                tempPath = path;
            }
            else
            {
                UnityEngine.Debug.Log("File already saved in " + path + ", not downloading");
                RefreshBrowser(path);
            }

        }
    }

        
    

    IEnumerator DownloadFile(string path, string webLink)
    {           
        //spawn loading item
        GameObject item = Instantiate(itemPrefab);
        item.transform.parent = itemWrapper.transform;
        item.transform.localPosition = itemStartPosition;
        item.transform.localRotation = Quaternion.identity;
        Renderer thumbRenderer = item.transform.GetChild(0).GetComponentInChildren<Renderer>();
        thumbRenderer.material.mainTexture = loadingSymbolTex;
        thumbRenderer.material.color = Color.white;
        item.GetComponentInChildren<TextMeshPro>().text = "Loading...";
        item.GetComponentInChildren<Animator>().enabled = true;
        item.GetComponentInChildren<ImportModel>(true).gameObject.SetActive(false);

        //refresh session browser
        foreach (Transform child in sessionItemWrapper.transform)
        {
            Destroy(child.gameObject);
        }

        //spawn loading item for sessionbrowser
        GameObject sessLoadingItem = Instantiate(itemPrefab);
        sessLoadingItem.transform.parent = sessionItemWrapper.transform;
        sessLoadingItem.transform.localPosition = sessionItemStartPosition;
        sessLoadingItem.transform.localRotation = Quaternion.identity;
        Renderer sessThumbRenderer = sessLoadingItem.transform.GetChild(0).GetComponentInChildren<Renderer>();
        sessThumbRenderer.material.mainTexture = loadingSymbolTex;
        sessThumbRenderer.material.color = Color.white;
        sessLoadingItem.GetComponentInChildren<TextMeshPro>().text = "Loading...";
        sessLoadingItem.GetComponentInChildren<Animator>().enabled = true;
        sessLoadingItem.GetComponentInChildren<ImportModel>(true).gameObject.SetActive(false);

        //download file
        uwr = new UnityWebRequest(webLink, UnityWebRequest.kHttpVerbGET);
        uwr.downloadHandler = new DownloadHandlerFile(path);
        yield return uwr.SendWebRequest();
     
        if (uwr.result != UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.LogError(uwr.error);
            uwr.downloadHandler.Dispose();
            File.Delete(path);
            tempPath = null;
            RefreshBrowserDownloadError();
            yield break;
        }
        tempPath = null;
        uwr.downloadHandler.Dispose();
        UnityEngine.Debug.Log("File successfully downloaded and saved to " + path);

        //this might seem stupid but must be done: the webLink is saved in a textfile for every object,
        //so that users can import objects from their harddrive, and see where the object originally was downloaded from
        string pathToTXT = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".txt");
        string pathToPNG = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".png");
        File.WriteAllText(pathToTXT, webLink);

        //delete img, if already exists
        File.Delete(pathToPNG);

        //refresh harddrive browser
        GetComponent<HarddriveBrowserRefresher>().RefreshList();

        //refresh search browser
        RefreshBrowser(path);

        //refresh session browser
        List<ImportedObject> oldList = GetComponent<SessionBrowserRefresher>().importedObjects;
        List<ImportedObject> newList = new List<ImportedObject>();
        foreach (ImportedObject impObj in oldList)
        {
            ImportedObject impObjChanged = impObj;
            if (impObj.webLink == webLink)
            {
                impObjChanged = UpdateImpObj(impObj, path);
            }
            newList.Add(impObjChanged);
        }
        GetComponent<SessionBrowserRefresher>().importedObjects = newList;
        GetComponent<SessionBrowserRefresher>().Refresh(GetComponent<SessionBrowserRefresher>().head);
        yield return null;
    }

    //this script is used when the session items are updated because of a new, conflicting download
    private ImportedObject UpdateImpObj(ImportedObject impObj, string path)
    {
        string tag = impObj.gameObject.tag;
        Transform tr = impObj.gameObject.transform;
        Destroy(impObj.gameObject);
        //this needs to be tried and catched if uuh the link changes to something unloadable
        try {            
            impObj.gameObject = GetComponent<ImportModel>().LoadModel(path);           
        }
        catch {
            impObj.gameObject = errorObj;
            impObj.gameObject.transform.position = tr.position;
            impObj.gameObject.transform.LookAt(mainCamTr); ;
            return impObj; 
        }
        impObj.gameObject.transform.position = tr.position;
        impObj.gameObject.transform.rotation = tr.rotation;
        impObj.gameObject.transform.localScale = tr.localScale;

        impObj.gameObject.tag = tag;

        impObj.dateOfDownload = System.IO.File.GetCreationTime(path).ToString();
        impObj.size = BytesToNiceString(new System.IO.FileInfo(path).Length);
        //impObj.creator = photonView.Owner.NickName;
        return impObj;
    }

    void RefreshBrowser(string path)
    {
        //refresh search browser
        foreach (Transform child in itemWrapper.transform)
        {
            Destroy(child.gameObject);
        }
        
        string webLink = File.ReadAllText(Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".txt"));
        string truncatedWebLink = webLink.Length > linkOrFileNameLength ? (webLink.Substring(0, linkOrFileNameLength / 2) + "..." + webLink.Substring(webLink.Length - linkOrFileNameLength/2)) : webLink;
        string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
        string truncatedFileName = fileName.Length > linkOrFileNameLength ? (fileName.Substring(0, linkOrFileNameLength) + "...") : fileName;
        string dateOfDownload = System.IO.File.GetCreationTime(path).ToString();
        string fileSize = BytesToNiceString(new System.IO.FileInfo(path).Length);
        //string creator = photonView.Owner.NickName;

        
        GameObject item = Instantiate(itemPrefab);
        item.transform.parent = itemWrapper.transform;
        item.transform.localPosition = itemStartPosition;
        item.transform.localRotation = Quaternion.identity;
        Renderer thumbRenderer = item.transform.GetChild(0).GetComponentInChildren<Renderer>();

        //broken glbs can break everything, thats why we have to try and catch here
        try { GetComponent<ThumbnailGenerator>().SetThumbnail(path, thumbRenderer); }
        catch
        {
            thumbRenderer.material.color = Color.red;
            item.GetComponentInChildren<TextMeshPro>().text = "ERROR: file cannot be imported";
            item.GetComponentInChildren<Animator>().enabled = false;
            thumbRenderer.material.mainTexture = downloadErrorTex;

            item.GetComponentInChildren<ImportModel>(true).gameObject.SetActive(false);
            return;
        }        
        
        thumbRenderer.material.color = Color.white;
        item.GetComponentInChildren<TextMeshPro>().text = truncatedWebLink + "<br>" + truncatedFileName + "<br>" +
                                                          "Downloaded: " + dateOfDownload + "<br>" + fileSize/*+ "<br>" + creator*/;
        item.GetComponentInChildren<Animator>().enabled = false;
        item.GetComponentInChildren<ImportModel>(true).gameObject.SetActive(true);
        item.GetComponentInChildren<ImportModel>().path = path;
        item.GetComponentInChildren<ImportModel>().model = new ImportedObject(null, webLink, fileName, dateOfDownload, fileSize/*, creator*/);



    }

    void RefreshBrowserDownloadError()
    {
        //refresh search browser
        foreach (Transform child in itemWrapper.transform)
        {
            Destroy(child.gameObject);
        }

        //spawn error item
        GameObject item = Instantiate(itemPrefab);
        item.transform.parent = itemWrapper.transform;
        item.transform.localPosition = itemStartPosition;
        item.transform.localRotation = Quaternion.identity;
        Renderer thumbRenderer = item.transform.GetChild(0).GetComponentInChildren<Renderer>();
        thumbRenderer.material.mainTexture = downloadErrorTex;
        thumbRenderer.material.color = Color.red;
        item.GetComponentInChildren<TextMeshPro>().text = "ERROR: Download failed";
        item.GetComponentInChildren<Animator>().enabled = false;
        item.GetComponentInChildren<ImportModel>(true).gameObject.SetActive(false);

        GetComponent<SessionBrowserRefresher>().Refresh(GetComponent<SessionBrowserRefresher>().head);
    }

    private static bool StringEndsWith(string a, string b)
    {
        int ap = a.Length - 1;
        int bp = b.Length - 1;

        while (ap >= 0 && bp >= 0 && a[ap] == b[bp])
        {
            ap--;
            bp--;
        }

        return (bp < 0);
    }

    private static bool StringStartsWith(string a, string b)
    {
        int aLen = a.Length;
        int bLen = b.Length;

        int ap = 0; int bp = 0;

        while (ap < aLen && bp < bLen && a[ap] == b[bp])
        {
            ap++;
            bp++;
        }

        return (bp == bLen);
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
