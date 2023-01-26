using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

public class ImportedModel : MonoBehaviour
{
	
	[SerializeField] private string webLink = "";
    public string WebLink {
		get {
			return webLink;
		}
		set {
			webLink = value;

			StartCoroutine(LoadModel());
		}
	}
	public string Owner { get; set; }

	IEnumerator LoadModel() {
		ModelDownloader downloader = Singleton<ModelDownloader>.Instance;
		yield return downloader.Download(webLink);
		ModelDownloader.ModelDownload download = downloader.GetDownload(webLink);
		string path = download.path;

		ModelImporter importer = Singleton<ModelImporter>.Instance;
		GameObject model = importer.InstantiateModel(path);
        GetComponent<BoxCollider>().size = model.GetComponent<BoxCollider>().size;
        GetComponent<BoxCollider>().center = model.GetComponent<BoxCollider>().center;
		model.transform.SetParent(transform);
		model.transform.localPosition = Vector3.zero;

		ImportedModelTracker.LoadedObject(this);

		yield break;
	}

	void OnEnable() {
		ImportedModelTracker.AddObject(this);
	}
	void OnDisable() {
		ImportedModelTracker.RemoveObject(this);
	}
	
}
