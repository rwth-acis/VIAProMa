using i5.VIAProMa.Shelves.Widgets;
using i5.VIAProMa.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static SessionBrowserRefresher;

public class DeleteFile : MonoBehaviour
{
    public string path;

    public void DeleteObject()
    {
        //Refresh session browser correctly and delete actual file
        //string imgPath = Path.Combine(Application.persistentDataPath, GetComponentInParent<ImportManager>().folderName, System.IO.Path.GetFileNameWithoutExtension(path) + ".png");
        string txtPath = Path.Combine(Application.persistentDataPath, GetComponentInParent<ImportManager>().folderName, System.IO.Path.GetFileNameWithoutExtension(path) + ".txt");
        HarddriveBrowserRefresher refresher = GetComponentInParent<HarddriveBrowserRefresher>();
        ImportedObject deleteThisItem = refresher.downloadedObjects.Find(e => e.webLink == path);
        System.IO.File.Delete(path);
        //System.IO.File.Delete(imgPath);
        System.IO.File.Delete(txtPath);
        refresher.downloadedObjects.Remove(deleteThisItem);
        refresher.RefreshBrowser(refresher.head);
    }

}
