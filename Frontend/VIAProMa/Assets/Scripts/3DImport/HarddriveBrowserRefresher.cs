using i5.VIAProMa.Shelves.Widgets;
using i5.VIAProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using static SessionBrowserRefresher;

public class HarddriveBrowserRefresher : MonoBehaviour
{
    public List<ImportedObject> downloadedObjects;
    public int head;

    [SerializeField] private Interactable headUpButton;
    [SerializeField] private Interactable headDownButton;

    [SerializeField] private GameObject hardItemWrapper;
    [SerializeField] private GameObject hardItem;

    private Vector3 hardItemStartPosition;
    private Vector3 hardItemPositionOffset;

    private int linkOrFileNameLength;

    [SerializeField] private Texture downloadErrorTex;

    void Start()
    {
        hardItemStartPosition = new Vector3(0, 0.2f, -0.02f);
        hardItemPositionOffset = new Vector3(0, -0.11f, 0);

        linkOrFileNameLength = 42;

        downloadedObjects = new List<ImportedObject>();
        RefreshList();        
    }

    public void RefreshList()
    {
        head = 0;
        downloadedObjects.Clear();
        FileInfo[] files = new DirectoryInfo(Path.Combine(Application.persistentDataPath, GetComponent<ImportManager>().folderName)).GetFiles("*.glb").OrderBy(p => p.CreationTime).ToArray();
        foreach (FileInfo file in files)
        {
            string path = file.FullName;
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            string dateOfDownload = System.IO.File.GetCreationTime(path).ToString();
            string fileSize = BytesToNiceString(new System.IO.FileInfo(path).Length);

            ImportedObject dowObj = new ImportedObject(null, path, fileName, dateOfDownload, fileSize/*, creator*/);
            downloadedObjects.Insert(0, dowObj);
            //Debug.Log(dowObj.fileName);
        }
        //Debug.Log(downloadedObjects.Count());
        RefreshBrowser(head);
    }
    public void RefreshBrowser(int headPosition)
    {
        headDownButton.GetComponentInChildren<TextMeshPro>().color = Color.white;
        headDownButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.white);
        headDownButton.IsEnabled = true;
        headUpButton.GetComponentInChildren<TextMeshPro>().color = Color.white;
        headUpButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.white);
        headUpButton.IsEnabled = true;

        //head is at the bottom
        if (downloadedObjects.Count() - headPosition - 1 - 4 <= 0)
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


        //refresh session browser
        foreach (Transform child in hardItemWrapper.transform)
        {
            Destroy(child.gameObject);
        }
        //build current page according to head position
        int j = downloadedObjects.Count() - headPosition < 5 ? downloadedObjects.Count() - headPosition : 5;
        for (int i = headPosition; i < j + headPosition; i++)
        {
            ImportedObject impObj = downloadedObjects[i];

            string truncatedWebLink = impObj.webLink.Length > linkOrFileNameLength ? (impObj.webLink.Substring(0, linkOrFileNameLength / 2) + "..." + impObj.webLink.Substring(impObj.webLink.Length - linkOrFileNameLength / 2)) : impObj.webLink;
            string truncatedFileName = impObj.fileName.Length > linkOrFileNameLength ? (impObj.fileName.Substring(0, linkOrFileNameLength) + "...") : impObj.fileName;
            string dateOfDownload = impObj.dateOfDownload;
            string fileSize = impObj.size;
            //string creator = impObj.creator;

            GameObject sessItem = Instantiate(hardItem);
            sessItem.transform.parent = hardItemWrapper.transform;
            sessItem.transform.localPosition = hardItemStartPosition + hardItemPositionOffset * (i - headPosition);
            sessItem.transform.localRotation = Quaternion.identity;
            string path = impObj.webLink;
            Renderer thumbRenderer = sessItem.transform.GetChild(0).GetComponentInChildren<Renderer>();
            
            //broken glbs can break everything, thats why we have to try and catch here
            try { GetComponent<ThumbnailGenerator>().SetThumbnail(path, thumbRenderer); }
            catch
            {
                thumbRenderer.material.color = Color.red;
                sessItem.GetComponentInChildren<TextMeshPro>().text = "ERROR: file cannot be imported";
                sessItem.GetComponentInChildren<Animator>().enabled = false;
                thumbRenderer.material.mainTexture = downloadErrorTex;

                sessItem.GetComponentInChildren<ImportModel>(true).gameObject.SetActive(false);
                sessItem.GetComponentInChildren<DeleteFile>().path = path;
                sessItem.GetComponentInChildren<DeleteFile>(true).gameObject.SetActive(true);
                continue;
            }

            thumbRenderer.material.color = Color.white;
            sessItem.GetComponentInChildren<TextMeshPro>().text = truncatedWebLink + "<br>" + truncatedFileName + "<br>" +
                                                                "Downloaded: " + dateOfDownload + "<br>" + fileSize/*+ "<br>" + creator*/;
            sessItem.GetComponentInChildren<Animator>().enabled = false;

            sessItem.GetComponentInChildren<DeleteFile>().path = path;

            string txtPath = Path.Combine(Application.persistentDataPath, GetComponentInParent<ImportManager>().folderName, System.IO.Path.GetFileNameWithoutExtension(path) + ".txt");
            string webLink = System.IO.File.ReadAllText(txtPath);
            sessItem.GetComponentInChildren<ImportModel>().path = path;
            sessItem.GetComponentInChildren<ImportModel>().model = new ImportedObject(null, webLink, impObj.fileName, dateOfDownload, fileSize/*, creator*/);

            
        }
    }

    public void IncreaseHead()
    {
        head++;
        RefreshBrowser(head);
    }
    public void DecreaseHead()
    {
        head--;
        RefreshBrowser(head);
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
