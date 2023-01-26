using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;

public class ModelDownloader: Singleton<ModelDownloader> {

	public delegate void ModelDownloadedAction(string url);
	public event ModelDownloadedAction OnModelDownloaded;
	public delegate void ModelDeletedAction(string url);
	public event ModelDeletedAction OnModelDeleted;
	
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

	string downloadDirectory = "3Dobjects";
	Dictionary<string, ModelDownload> downloads = new Dictionary<string, ModelDownload>();
	System.Random random = new System.Random();

	public void Start() {
		FileInfo[] files = new DirectoryInfo(Path.Combine(Application.persistentDataPath, downloadDirectory))
			.GetFiles("*.txt");
        foreach (FileInfo file in files)
        {
            string txtPath = file.FullName;
			string glbPath = Path.Combine(Path.GetDirectoryName(txtPath), Path.GetFileNameWithoutExtension(txtPath) + ".glb");
			string url = File.ReadAllText(txtPath);
			ModelDownload download = new ModelDownload();
			download.path = glbPath;
			download.state = ModelDownloadState.Finished;
			downloads.Add(url, download);
        }
	}

	public ModelDownload GetDownload(string url) {
		return downloads[url];
	}
	public Dictionary<string, ModelDownload> GetDownloads() {
		return downloads;
	}

	public void Yeet(string url) {
		ModelDownload download = downloads[url];
		if (download.state == ModelDownloadState.Finished) {
			downloads.Remove(url);
			System.IO.File.Delete(download.path);
			System.IO.File.Delete(Path.Combine(Path.GetDirectoryName(download.path), Path.GetFileNameWithoutExtension(download.path) + ".png"));
			System.IO.File.Delete(Path.Combine(Path.GetDirectoryName(download.path), Path.GetFileNameWithoutExtension(download.path) + ".txt"));
		}
		OnModelDeleted(url);
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
		
		string path = GetPath();
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
		OnModelDownloaded(url);
	}

	string GetPath()
	{
		return Path.Combine(Application.persistentDataPath, downloadDirectory, random.Next().ToString() + ".glb");
	}
}