using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SessionBrowserRefresher;

public class DeleteModel : MonoBehaviour
{
    public GameObject model;

    public void DeleteObject()
    {
        Destroy(model.gameObject.transform);
        ImportedObject deleteThisObj = GetComponentInParent<SessionBrowserRefresher>().importedObjects.Find(e => e.gameObject = model.gameObject);
        GetComponentInParent<SessionBrowserRefresher>().importedObjects.Remove(deleteThisObj);
        GetComponentInParent<SessionBrowserRefresher>().Refresh();
    }
}
