using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using i5.VIAProMa.UI;

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
    public GameObject modelWrapper;

    IEnumerator LoadModel() {
        modelWrapper = GameObject.Find("AnchorParent").GetComponentInChildren<ImportManager>().modelWrapper;

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
        model.transform.localRotation = Quaternion.identity;

        transform.parent = modelWrapper.transform;

		ImportedModelTracker.LoadedObject(this);


        model.transform.eulerAngles += new Vector3(-90, -180, 0);

        //use the center of the bounding box to set the object
        model.transform.position = model.transform.position + (this.gameObject.transform.position - model.transform.TransformPoint(model.GetComponent<BoxCollider>().center));
        //model.transform.position = model.transform.position - this.gameObject.transform.forward * 0.1f;
        yield break;
	}

	void OnEnable() {
		ImportedModelTracker.AddObject(this);
	}
	void OnDisable() {
		ImportedModelTracker.RemoveObject(this);
	}
	



}
