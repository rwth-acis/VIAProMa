using i5.VIAProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class SessionBrowserRefresher : MonoBehaviour
{
    public struct ImportedObject 
    {
        public ImportedObject(GameObject GameObject, string WebLink, string FileName, string DateOfDownload, string Size/*, string Creator*/)
        {
            gameObject = GameObject;
            webLink = WebLink;
            fileName = FileName;
            dateOfDownload = DateOfDownload;
            size = Size;
            //creator = Creator;
        }

        public GameObject gameObject;
        public string webLink;
        public string fileName;
        public string dateOfDownload;
        public string size;
        //public string creator;     
    }

    public int head;

    [SerializeField] private Interactable headUpButton;
    [SerializeField] private Interactable headDownButton;

    [SerializeField] private GameObject sessionItemWrapper;
    [SerializeField] private GameObject sessionItem;

    private Vector3 sessionItemStartPosition;
    private Vector3 sessionItemPositionOffset;

    private int linkOrFileNameLength;

    [SerializeField] private Texture downloadErrorTex;

    void Start()
    {
        sessionItemStartPosition = new Vector3(0, 0.2f, -0.02f);
        sessionItemPositionOffset = new Vector3(0, -0.11f, 0);

        linkOrFileNameLength = 30;

        head = 0;
    }

	void OnEnable() {
		Refresh(head);

		ImportedModelTracker.OnAdded += ObjectAdded;
		ImportedModelTracker.OnRemove += ObjectRemoved;
	}
	void OnRemove() {
		ImportedModelTracker.OnAdded -= ObjectAdded;
		ImportedModelTracker.OnRemove -= ObjectRemoved;
	}

    public void Refresh(int headPosition)
    {       
		List<ImportedModel> importedModels = ImportedModelTracker.ImportedModels;

		headDownButton.GetComponentInChildren<TextMeshPro>().color = Color.white;
		headDownButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.white);
		headDownButton.IsEnabled = true;
		headUpButton.GetComponentInChildren<TextMeshPro>().color = Color.white;
		headUpButton.GetComponentInChildren<TextMeshPro>().transform.parent.GetChild(1).GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.white);
		headUpButton.IsEnabled = true;        
        
        //head is at the bottom
        if (importedModels.Count() - headPosition - 1 - 4 <= 0)
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
        foreach (Transform child in sessionItemWrapper.transform)
        {
            Destroy(child.gameObject);
        }
        //build current page according to head position
        int j = importedModels.Count() - headPosition < 5 ? importedModels.Count() - headPosition : 5;
        for (int i = headPosition; i < j + headPosition; i++)
        {
            ImportedModel impObj = importedModels[i];

            string truncatedWebLink = impObj.WebLink.Length > linkOrFileNameLength ? (impObj.WebLink.Substring(0, linkOrFileNameLength / 2) + "..." + impObj.WebLink.Substring(impObj.WebLink.Length - linkOrFileNameLength / 2)) : impObj.WebLink;
            // string truncatedFileName = impObj.fileName.Length > linkOrFileNameLength ? (impObj.fileName.Substring(0, linkOrFileNameLength) + "...") : impObj.fileName;
            // string dateOfDownload = impObj.dateOfDownload;
            // string fileSize = impObj.size;
            // string creator = impObj.creator;

            GameObject sessItem = Instantiate(sessionItem);
            sessItem.transform.parent = sessionItemWrapper.transform;
            sessItem.transform.localPosition = sessionItemStartPosition + sessionItemPositionOffset * (i - headPosition);
            sessItem.transform.localRotation = Quaternion.identity;
            // string path = Path.Combine(Application.persistentDataPath, GetComponent<ImportManager>().folderName, impObj.fileName + ".glb");
            Renderer thumbRenderer = sessItem.transform.GetChild(0).GetComponentInChildren<Renderer>();
            
            //broken glbs can break everything, thats why we have to try and catch here
            // try { GetComponent<ThumbnailGenerator>().SetThumbnail(path, thumbRenderer); }
            // catch {
                thumbRenderer.material.color = Color.red;
                sessItem.GetComponentInChildren<TextMeshPro>().text = "ERROR: file cannot be imported";
                sessItem.GetComponentInChildren<Animator>().enabled = false;
                thumbRenderer.material.mainTexture = downloadErrorTex;

                sessItem.GetComponentInChildren<ImportModel>(true).gameObject.SetActive(false);
                sessItem.GetComponentInChildren<HighlightModel>(true).gameObject.SetActive(false);
                sessItem.GetComponentInChildren<DeleteModel>().model = impObj.gameObject;
                sessItem.GetComponentInChildren<DeleteModel>(true).gameObject.SetActive(true);
            	continue;
            // }
            
            thumbRenderer.material.color = Color.white;
            // sessItem.GetComponentInChildren<TextMeshPro>().text = truncatedWebLink + "<br>" + truncatedFileName + "<br>" +
            //                                                     "Downloaded: " + dateOfDownload + "<br>" + fileSize/*+ "<br>" + creator*/;
            sessItem.GetComponentInChildren<Animator>().enabled = false;

            sessItem.GetComponentInChildren<ImportModel>().url = impObj.WebLink;

            sessItem.GetComponentInChildren<HighlightModel>().model = impObj.gameObject;
            if (impObj.gameObject.tag == "Highlighted") { sessItem.GetComponentInChildren<HighlightModel>().HighlightObject(); }
            sessItem.GetComponentInChildren<DeleteModel>().model = impObj.gameObject;
                      
        }
    }

	void ObjectAdded(ImportedModel obj) {
		Refresh(head);
	}
	void ObjectRemoved(ImportedModel obj) {
		Refresh(head);
	}

    public void IncreaseHead()
    {        
        head++;      
        Refresh(head);
    }
    public void DecreaseHead()
    {
        head--;
        Refresh(head);
    }
}
