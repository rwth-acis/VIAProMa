using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SketchfabLinkGenerator : MonoBehaviour
{ 
    public string Uid;
    public string licence;

    /// <summary>
    /// Generates a temporary sketchfab download link out of a given uid, and calls the download function.
    /// </summary>
    public IEnumerator GetDownloadLink(string uid, string licence)
    {
        string uri = string.Format("https://api.sketchfab.com/v3/models/{0}/download", uid);

        using UnityWebRequest webRequest = UnityWebRequest.Get(uri);

        webRequest.SetRequestHeader("Accept", "application/json");
        webRequest.SetRequestHeader("Authorization", "Token 182f243dcffb4e3e830788ceb1467c33");

        yield return webRequest.SendWebRequest();

        string downloadUrl = "";
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            JObject jsonResponse = JObject.Parse(webRequest.downloadHandler.text);
            downloadUrl = (string) jsonResponse["glb"]["url"];
        }
        else
        {
            Debug.Log("Error connecting to sketchfab API: " + webRequest.error);
        }

        if (GetComponent<SearchBrowserRefresher>() == null)
        {
            GetComponentInParent<SearchBrowserRefresher>().SearchChanged("Sketchfab:" + downloadUrl, uid, licence);
        }
        else
        {
            GetComponent<SearchBrowserRefresher>().SearchChanged("Sketchfab:" + downloadUrl, uid, licence);
        }
    }

    /// <summary>
    /// This is called when the "Download" button of a sketchfab object is pressed.
    /// </summary>
    public void GetDownloadLink()
    {
        StartCoroutine(GetDownloadLink(Uid, licence));
    }
}
