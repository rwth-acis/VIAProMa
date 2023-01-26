using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;

public class ModelDownloader: Singleton<ModelDownloader> {
	
	public enum ModelDownloadState {
		Processing,
		Finished,
		Failed,
	}
	public class ModelDownload {
		public ModelDownloadState state;
		public string path;
		public ModelDownload() {
			state = ModelDownloadState.Processing;
		}
	}

	Dictionary<string, ModelDownload> downloads = new Dictionary<string, ModelDownload>();

	public ModelDownload GetDownload(string url) {
		return downloads[url];
	}

	public IEnumerator Download(string url)
	{
		if (downloads.ContainsKey(url)) {
			yield return AwaitDownload(url);
		} else {
			yield return InternalDownload(url);
		}
	}

	IEnumerator AwaitDownload(string url)
	{
		yield return new WaitUntil(() => downloads[url].state != ModelDownloadState.Processing);
	}

	IEnumerator InternalDownload(string url)
	{
		ModelDownload download = new ModelDownload();
		downloads.Add(url, download);
		
		string path = GetPath(url);
		if (path == null) {
			download.state = ModelDownloadState.Failed;
			yield break;
		}

		UnityWebRequest uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
		uwr.downloadHandler = new DownloadHandlerFile(path);
		yield return uwr.SendWebRequest();
		
        if (uwr.result != UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.LogError(uwr.error);
            uwr.downloadHandler.Dispose();
            File.Delete(path);
			download.state = ModelDownloadState.Failed;
			yield break;
        }
		uwr.downloadHandler.Dispose();

		string pathToTXT = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".txt");
		File.WriteAllText(pathToTXT, url);

		download.path = path;
		download.state = ModelDownloadState.Finished;
	}

	string GetPath(string url)
	{
		if ((url.StartsWith("http://") || url.StartsWith("https://"))
        && url.EndsWith(".glb"))
        {
            string fileName = System.IO.Path.GetFileName(url);
            if (fileName == ".glb")
            {
                return null;
            }
            string path = Path.Combine(Application.persistentDataPath, "3Dobjects", fileName);
			return path;
        }
		return null;
	}
}