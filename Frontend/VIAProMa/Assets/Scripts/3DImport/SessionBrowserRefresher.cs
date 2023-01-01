using i5.VIAProMa.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

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

    public List<ImportedObject> importedObjects;
    private int head;

    public GameObject sessionItemWrapper;

    public GameObject sessionItem;
    [SerializeField] private Sprite loadingSymbol;
    [SerializeField] private Sprite downloadErrorSprite;
    [SerializeField] private Sprite noThumbSprite;

    private Vector3 sessionItemStartPosition;
    private Vector3 sessionItemPositionOffset;

    void Start()
    {
        sessionItemStartPosition = new Vector3(0, 0.2f, -0.02f);
        sessionItemPositionOffset = new Vector3(0, -0.09f, 0);

        importedObjects = new List<ImportedObject>();
        head = 0;
        //clear session browser
        foreach (Transform child in sessionItemWrapper.transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    public void AddItem(ImportedObject obj)
    {
        importedObjects.Insert(0, obj);

        head = 0;
        //refresh session browser
        foreach (Transform child in sessionItemWrapper.transform)
        {
            Destroy(child.gameObject);
        }
        //build first page
        int j = importedObjects.Count() < 5 ? importedObjects.Count() : 5;
        for (int i = 0; i < j; i++)
        {
            ImportedObject impObj = importedObjects[i];

            string truncatedWebLink = impObj.webLink.Length > 20 ? (impObj.webLink.Substring(0, 20) + "...") : impObj.webLink;
            string truncatedFileName = impObj.fileName.Length > 20 ? (impObj.fileName.Substring(0, 20) + "...") : impObj.fileName;
            string dateOfDownload = impObj.dateOfDownload;
            string fileSize = impObj.size;
            //string creator = impObj.creator;

            GameObject sessItem = Instantiate(sessionItem);
            sessItem.transform.parent = sessionItemWrapper.transform;
            sessItem.transform.localPosition = sessionItemStartPosition + sessionItemPositionOffset*i;
            sessItem.transform.localRotation = Quaternion.identity;
            sessItem.GetComponentInChildren<SpriteRenderer>().sprite = noThumbSprite;
            sessItem.GetComponentInChildren<SpriteRenderer>().color = Color.green;
            sessItem.GetComponentInChildren<TextMeshPro>().text = truncatedWebLink + "<br>" + truncatedFileName + "<br>" +
                                                              dateOfDownload + "<br>" + fileSize/*+ "<br>" + creator*/;
            sessItem.GetComponentInChildren<Animator>().enabled = false;
            
            //sessItem.GetComponentInChildren<ImportModel>(true).gameObject.SetActive(true);
        }
    }

    public void IncreaseHead() { head++; }
    public void DecreaseHead() { head--; }
}
