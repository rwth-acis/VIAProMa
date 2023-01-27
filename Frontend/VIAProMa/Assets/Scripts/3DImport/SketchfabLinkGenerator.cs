using System.Collections;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Networking;

public class SketchfabLinkGenerator : MonoBehaviour
{ 
    private class Result
    {
        private string DownloadLink { get; set; } 
    }

    public string Uid;

    public IEnumerator GetDownloadLink(string Uid)
    {
        string uri = string.Format("https://api.sketchfab.com/v3/model/{0}/download", Uid);
        //Debug.Log(uri);

        using UnityWebRequest webRequest = UnityWebRequest.Get(uri);

        webRequest.SetRequestHeader("Accept", "application/json");
        //webRequest.SetRequestHeader("Host", "api.sketchfab.com");
        webRequest.SetRequestHeader("Authorization", "Token 182f243dcffb4e3e830788ceb1467c33");

        yield return webRequest.SendWebRequest();
    }
}
