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

    IEnumerator LoadModel() {
        GameObject modelWrapper = GameObject.Find("AnchorParent").GetComponentInChildren<ModelImporter>().modelWrapper;

        ModelDownloader downloader = Singleton<ModelDownloader>.Instance;
		yield return downloader.Download(webLink);
		ModelDownloader.ModelDownload download = downloader.GetDownload(webLink);
		string path = download.path;

		ModelImporter importer = Singleton<ModelImporter>.Instance;
		GameObject model = importer.InstantiateModel(path);
        GetComponent<BoxCollider>().size = model.GetComponent<BoxCollider>().size * model.transform.localScale.y;
        GetComponent<BoxCollider>().center = model.GetComponent<BoxCollider>().center * model.transform.localScale.y;
		model.transform.SetParent(transform);
		model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;

        transform.parent = modelWrapper.transform;

		ImportedModelTracker.LoadedObject(this);

        //use the center of the bounding box to set the object
        transform.position = transform.position + (this.gameObject.transform.position - transform.TransformPoint(GetComponent<BoxCollider>().center));
        transform.eulerAngles += new Vector3(-90, -180, 0);

        
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
