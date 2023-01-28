using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using TMPro;
using System.IO.Enumeration;
using Photon.Pun;
using UnityEditor;
using i5.VIAProMa.UI;
using static SessionBrowserRefresher;
using Microsoft.MixedReality.Toolkit;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using WebSocketSharp;
using UnityEditor.Search;
using ExitGames.Client.Photon.StructWrapping;
using static SearchBrowserRefresher;

public class SearchBrowserRefresher : MonoBehaviour
{
    public class SearchResult
    {
        public string Name { get; set;}
        public string Uid { get; set;}
        public string PublishedAt { get; set;}
        public string FileSize { get; set;}
        public string ThumbnailLink { get; set; }
        public string Licence { get; set; }

        public override string ToString()
        {
            return String.Format("Name: {0}, UID: {1}, PublishedAt: {2}, FileSize: {3}, ThumbnailLink: {4}, Licence: {5}", Name, Uid, PublishedAt, FileSize, ThumbnailLink, Licence);
        }
    }

    private GameObject modelWrapper;

    [SerializeField] private GameObject sketchfabItem;
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

    public List<SearchResult> searchedObjects;
    public int head;

    private string sketchfabThumbsFolder;

    private int lastListLength;

    private string searchContentGlobal;

    public Transform mainCamTr;

    void Start()
    {
        modelWrapper = this.gameObject.GetComponent<ImportManager>().modelWrapper;
        sketchfabThumbsFolder = GetComponent<ImportManager>().sketchfabThumbsFolder;

        linkOrFileNameLength = 54;

        itemStartPosition = new Vector3(0, 0.12f, -0.02f);
        itemPositionOffset = new Vector3(0, -0.11f, 0);

        sessionItemStartPosition = new Vector3(0, 0.2f, -0.02f);
        sessionItemPositionOffset = new Vector3(0, -0.11f, 0);

        SearchChanged("", "", "");
        searchedObjects = new List<SearchResult>();
        head = 0;
        lastListLength = 0;
    }

    public void SearchChanged(string searchContent, string Uid, string licence)
    {
        searchContentGlobal = searchContent;

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
        head = 0;

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
                downloadRoutine = StartCoroutine(DownloadFile(path, searchContent, Uid, licence));
                tempPath = path;
            }
            else
            {
                Debug.Log("File already saved in " + path + ", not downloading");
                RefreshBrowser(path, licence);
            }

        }
        //if sketchfab link
        else if (StringStartsWith(searchContent, "Sketchfab:")) {
            searchContent = searchContent.Remove(0, 10);
            string path = Path.Combine(Application.persistentDataPath, GetComponent<ImportManager>().folderName, Uid + ".glb");
            if (!System.IO.File.Exists(path))
            {
                downloadRoutine = StartCoroutine(DownloadFile(path, searchContent, Uid, licence));
                tempPath = path;
            }
            else
            {
                Debug.Log("File already saved in " + path + ", not downloading");
                RefreshBrowser(path, licence);
            }
        }
        else if (!searchContent.IsNullOrEmpty()) // if not empty, then search for it on sketchfab
        {
            //refresh thumbs folder            
            FileInfo[] imageFiles = new DirectoryInfo(Path.Combine(Application.persistentDataPath, sketchfabThumbsFolder)).GetFiles("*.png");
            foreach (FileInfo imageFile in imageFiles)
            {
                string imagePath = imageFile.FullName;
                File.Delete(imagePath);
            }            

            StartCoroutine(SearchOnSketchfab(searchContent, 10, 0));
        }
    }

        
    private IEnumerator SearchOnSketchfab(string query, int range, int cursor)
    {
        searchedObjects = new List<SearchResult>();

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


        string uri = "https://api.sketchfab.com/v3/search?type=models&count=" + range + "&cursor=" + cursor + "&downloadable=true" + "&q=" + UnityWebRequest.EscapeURL(query);
        //Debug.Log(uri);

        using UnityWebRequest webRequest = UnityWebRequest.Get(uri);

        webRequest.SetRequestHeader("Accept", "application/json");
        //webRequest.SetRequestHeader("Host", "api.sketchfab.com");
        webRequest.SetRequestHeader("Authorization", "Token 182f243dcffb4e3e830788ceb1467c33");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            JObject jsonResponse = JObject.Parse(webRequest.downloadHandler.text);
            List<JToken> results = jsonResponse["results"].Children().ToList();

            foreach(JToken result in results)
            {
                SearchResult searchResult = result.ToObject<SearchResult>();
                List<JToken> thumbs = result["thumbnails"]["images"].Children().ToList();

                if (result["license"] != null)
                {
                    string licenceId = (string)result["license"]["uid"];
                    using UnityWebRequest licenceReq = UnityWebRequest.Get("https://api.sketchfab.com/v3/licenses/" + licenceId);
                    licenceReq.SetRequestHeader("Accept", "application/json");

                    yield return licenceReq.SendWebRequest();

                    if (licenceReq.result == UnityWebRequest.Result.Success)
                    {
                        JObject licenceRes = JObject.Parse(licenceReq.downloadHandler.text);
                        searchResult.Licence = ((string)licenceRes["slug"]).ToUpper();
                    }
                    else
                    {
                        Debug.LogError("Sketchfab License API: " + licenceReq.error);
                    }

                } 
                else
                {
                    searchResult.Licence = "None";
                }
  
                searchResult.ThumbnailLink = (string)thumbs[0]["url"];
                searchResult.FileSize = BytesToNiceString((int)result["archives"]["glb"]["size"]);

                searchedObjects.Add(searchResult);
                Debug.Log(searchResult);
            }

            foreach(SearchResult searchRes in searchedObjects)
            {
                string pathToPNG = Path.Combine(Application.persistentDataPath, sketchfabThumbsFolder, searchRes.Uid + ".png");
                if (!File.Exists(pathToPNG))
                {
                    using UnityWebRequest downloadThumb = UnityWebRequest.Get(searchRes.ThumbnailLink);
                    downloadThumb.downloadHandler = new DownloadHandlerBuffer();

                    yield return downloadThumb.SendWebRequest();

                    if (downloadThumb.result == UnityWebRequest.Result.Success)
                    {
                        byte[] jpegBytes = downloadThumb.downloadHandler.data;

                        Texture2D thumbImg = new Texture2D(256, 256, TextureFormat.RGBA32, false);
                        thumbImg.LoadImage(jpegBytes);
                        thumbImg.Apply();

                        byte[] pngBytes = thumbImg.EncodeToPNG();

                        File.WriteAllBytes(pathToPNG, pngBytes);
                    }
                    else
                    {
                        Debug.Log("Error downloading thumbnail via sketchfab API: " + webRequest.error);
                    }

                    downloadThumb.downloadHandler.Dispose();
                }
            }

            RefreshSearchBrowser(head);
        }
        else
        {
            Debug.Log("Error connecting to sketchfab API: " + webRequest.error);
        }

    }


    public void RefreshSearchBrowser(int headPosition)
    {
        //refresh search browser
        foreach (Transform child in itemWrapper.transform)
        {
            Destroy(child.gameObject);
        }


        if ((searchedObjects.Count() - headPosition < 6) && (lastListLength != searchedObjects.Count()))
        {
            headDownButton.GetComponentInChildren<TextMeshPro>().color = Color.grey;
            headDownButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.grey);
            headDownButton.IsEnabled = false;
            headUpButton.GetComponentInChildren<TextMeshPro>().color = Color.grey;
            headUpButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.grey);
            headUpButton.IsEnabled = false;

            lastListLength = searchedObjects.Count();
            StartCoroutine(SearchOnSketchfab(searchContentGlobal, searchedObjects.Count() + 10, 0));
            return;
        }


        headDownButton.GetComponentInChildren<TextMeshPro>().color = Color.white;
        headDownButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.white);
        headDownButton.IsEnabled = true;
        headUpButton.GetComponentInChildren<TextMeshPro>().color = Color.white;
        headUpButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.white);
        headUpButton.IsEnabled = true;

        //head is at the bottom
        if (searchedObjects.Count() - headPosition - 1 - 3 <= 0)
        {
            headDownButton.GetComponentInChildren<TextMeshPro>().color = Color.grey;
            headDownButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.grey);
            headDownButton.IsEnabled = false;

        }

        //head is at the top
        if (headPosition == 0)
        {
            headUpButton.GetComponentInChildren<TextMeshPro>().color = Color.grey;
            headUpButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.grey);
            headUpButton.IsEnabled = false;

        }


        

        //build current page according to head position
        int j = searchedObjects.Count() - headPosition < 4 ? searchedObjects.Count() - headPosition : 4;
        for (int i = headPosition; i < j + headPosition; i++)
        {
            SearchResult searchObj = searchedObjects[i];

            string truncatedUID = searchObj.Uid.Length > (linkOrFileNameLength - 15) ? (searchObj.Uid.Substring(0, (linkOrFileNameLength - 15) / 2) + "..." + searchObj.Uid.Substring(searchObj.Uid.Length - (linkOrFileNameLength - 15) / 2)) : searchObj.Uid;
            string truncatedFileName = searchObj.Name.Length > linkOrFileNameLength ? (searchObj.Name.Substring(0, linkOrFileNameLength) + "...") : searchObj.Name;
            string truncatedPublisher = searchObj.PublishedAt;

            GameObject searchItem = Instantiate(sketchfabItem);
            searchItem.transform.parent = itemWrapper.transform;
            searchItem.transform.localPosition = itemStartPosition + itemPositionOffset * (i - headPosition);
            searchItem.transform.localRotation = Quaternion.identity;
            
            Renderer thumbRenderer = searchItem.transform.GetChild(0).GetComponentInChildren<Renderer>();
            byte[] bytes = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, sketchfabThumbsFolder, searchObj.Uid + ".png"));
            Texture2D thumbImg = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            thumbImg.LoadImage(bytes);
            thumbImg.Apply();
            thumbRenderer.material.SetTexture("_MainTex", thumbImg);


            thumbRenderer.material.color = Color.white;
            searchItem.GetComponentInChildren<TextMeshPro>().text = "Sketchfab UID: " + truncatedUID + "<br>" + truncatedFileName + "<br>" +
                                                                "Published: " + truncatedPublisher + "<br>" + "Size: " + searchObj.FileSize;
            searchItem.GetComponentInChildren<Animator>().enabled = false;

            searchItem.GetComponentInChildren<SketchfabLinkGenerator>().Uid = searchObj.Uid;
            searchItem.GetComponentInChildren<SketchfabLinkGenerator>().licence = searchObj.Licence;
        }
    }

    public void IncreaseHead()
    {
        head++;
        RefreshSearchBrowser(head);
    }
    public void DecreaseHead()
    {
        head--;
        RefreshSearchBrowser(head);
    }


    IEnumerator DownloadFile(string path, string webLink, string Uid, string licence)
    {
        //refresh session browser
        foreach (Transform child in itemWrapper.transform)
        {
            Destroy(child.gameObject);
        }
        headDownButton.GetComponentInChildren<TextMeshPro>().color = Color.grey;
        headDownButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.grey);
        headDownButton.IsEnabled = false;
        headUpButton.GetComponentInChildren<TextMeshPro>().color = Color.grey;
        headUpButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.grey);
        headUpButton.IsEnabled = false;

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
        if (Uid != "")
        {
            webLink = Uid;
        }
        File.WriteAllLines(pathToTXT, new string[] { webLink, licence });

        //delete img, if already exists
        if (File.Exists(pathToTXT))
        {
            File.Delete(pathToPNG);
        }

        //refresh harddrive browser
        GetComponent<HarddriveBrowserRefresher>().RefreshList();

        //refresh search browser
        RefreshBrowser(path, licence);

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

        impObj.dateOfDownload = System.IO.File.GetCreationTime(path).ToString();
        impObj.size = BytesToNiceString(new System.IO.FileInfo(path).Length);
        //impObj.creator = photonView.Owner.NickName;
        return impObj;
    }

    void RefreshBrowser(string path, string licence)
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
        if (licence != "")
        {
            truncatedWebLink = "Licence: " + licence;
        }
        
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
        item.GetComponentInChildren<ImportModel>().model = new ImportedObject(null, webLink, fileName, dateOfDownload, fileSize, licence);



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