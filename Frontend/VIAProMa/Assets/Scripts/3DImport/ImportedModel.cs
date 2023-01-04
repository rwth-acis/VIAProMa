using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportedModel : MonoBehaviour
{
	
	[SerializeField] private string webLink = "";
    public string WebLink {
		get {
			return webLink;
		}
		set {
			webLink = value;
			// TODO: Load and show the model
		}
	}
	public string Owner { get; set; }
	
}
