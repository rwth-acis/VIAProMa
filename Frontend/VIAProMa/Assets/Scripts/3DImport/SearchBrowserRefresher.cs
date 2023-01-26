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
using HoloToolkit.Unity;

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

    private Coroutine downloadRoutine;

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

        //deactivate up/down buttons
        headDownButton.GetComponentInChildren<TextMeshPro>().color = Color.grey;
        headDownButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.grey);
        headDownButton.IsEnabled = false;
        headUpButton.GetComponentInChildren<TextMeshPro>().color = Color.grey;
        headUpButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.grey);
        headUpButton.IsEnabled = false;

		if ((searchContent.StartsWith("http://") || searchContent.StartsWith("https://"))
        && searchContent.EndsWith(".glb"))
		{
			downloadRoutine = StartCoroutine(DownloadFile(searchContent));
		}
    }

	private IEnumerator DownloadFile(string url)
	{
		RefreshBrowserLoading();

		ModelDownloader downloader = Singleton<ModelDownloader>.Instance;
		yield return downloader.Download(url);
		ModelDownloader.ModelDownload download = downloader.GetDownload(url);
		if (download.state == ModelDownloader.ModelDownloadState.Failed) {
			RefreshBrowserDownloadError();
			yield break;
		}
		RefreshBrowser(download.path);
	}

	private void RefreshBrowserLoading()
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
        string fileSize = ModelDownloader.BytesToNiceString(new System.IO.FileInfo(path).Length);
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
        item.GetComponentInChildren<ImportModel>().url = webLink;
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
}
