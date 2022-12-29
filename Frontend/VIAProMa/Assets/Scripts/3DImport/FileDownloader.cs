using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;
using static System.Net.WebRequestMethods;
using System.Text;

public class FileDownloader : MonoBehaviour {
    [SerializeField] private GameObject boundingBox;
    private string webLink;
    private string fileName;

    void Start () {
        webLink = "http://www.nikita-zaloga.de/undertale-_low_poly.glb";
        fileName = "undertale - _low_poly.glb";
        StartCoroutine(DownloadFile());
    }

    IEnumerator DownloadFile() {
        var uwr = new UnityWebRequest(webLink, UnityWebRequest.kHttpVerbGET);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        uwr.downloadHandler = new DownloadHandlerFile(path);
        yield return uwr.SendWebRequest();
        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(uwr.error);
        }
        else
        {
            Debug.Log("File successfully downloaded and saved to " + path);
            
            //import into unity scene
            GameObject testModel = Importer.LoadFromFile(path);
            
            //GameObject myBoundingBox = Instantiate(boundingBox);
            //myBoundingBox.transform.parent = testModel.transform;

            MeshFilter[] rr = testModel.GetComponentsInChildren<MeshFilter>();
            Bounds bounds = rr[0].mesh.bounds;
            foreach (MeshFilter r in rr) { bounds.Encapsulate(r.mesh.bounds); }


            //myBoundingBox.GetComponent<BoxCollider>().size = bounds.size;

            testModel.transform.localScale = testModel.transform.localScale / bounds.size.magnitude;

        }
    }
}