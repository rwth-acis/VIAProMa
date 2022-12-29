using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;

public class FileDownloader : MonoBehaviour {
    //public GameObject testModel; 

    void Start () {
        StartCoroutine(DownloadFile());
    }

    IEnumerator DownloadFile() {
        var uwr = new UnityWebRequest("http://www.nikita-zaloga.de/undertale-_low_poly.glb", UnityWebRequest.kHttpVerbGET);
        string path = Path.Combine(Application.persistentDataPath, "undertale-_low_poly.glb");
        uwr.downloadHandler = new DownloadHandlerFile(path);
        yield return uwr.SendWebRequest();
        if (uwr.result != UnityWebRequest.Result.Success)
            Debug.LogError(uwr.error);
        else
            Debug.Log("File successfully downloaded and saved to " + path);
            GameObject testModel = Importer.LoadFromFile(path);
    }
}