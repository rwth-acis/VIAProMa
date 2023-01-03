using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SessionBrowserRefresher;

public class DeleteModel : MonoBehaviour
{
    public GameObject model;

    public void DeleteObject()
    {
        //Refresh session browser correctly and delete actual GameObject
        SessionBrowserRefresher refresher = GetComponentInParent<SessionBrowserRefresher>();
        ImportedObject deleteThisItem = refresher.importedObjects.Find(e => e.gameObject == model);
        Destroy(model);
        refresher.importedObjects.Remove(deleteThisItem);
        refresher.Refresh(refresher.head);
    }
}
