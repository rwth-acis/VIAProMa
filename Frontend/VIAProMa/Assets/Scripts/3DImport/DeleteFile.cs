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

    /// <summary>
    /// Deletes the glb and txt - however not the png file,
    /// to be able to have the object still in the session with a thumbnail.
    /// </summary>
    public void DeleteObject()
    {
        //Refresh session browser correctly and delete actual file, txt - however, not the img
        string txtPath = Path.Combine(Application.persistentDataPath, GetComponentInParent<ImportManager>().folderName, System.IO.Path.GetFileNameWithoutExtension(path) + ".txt");
        HarddriveBrowserRefresher refresher = GetComponentInParent<HarddriveBrowserRefresher>();
        SessionBrowserRefresher refresherSess = GetComponentInParent<SessionBrowserRefresher>();
        ImportedObject deleteThisItem = refresher.downloadedObjects.Find(e => e.webLink == path);
        System.IO.File.Delete(path);
        System.IO.File.Delete(txtPath);
        refresher.downloadedObjects.Remove(deleteThisItem);
        refresher.RefreshBrowser(refresher.head);
        refresherSess.Refresh(refresherSess.head);
    }

}
