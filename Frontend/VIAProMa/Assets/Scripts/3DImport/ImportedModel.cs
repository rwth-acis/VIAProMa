using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportedModel : MonoBehaviour
{
	
	[SerializeField] private string webLink = "";
    public string WebLink {
		get {
			return _webLink;
		}
		set {
			_webLink = value;
			// TODO: Load and show the model
		}
	}
	
}
