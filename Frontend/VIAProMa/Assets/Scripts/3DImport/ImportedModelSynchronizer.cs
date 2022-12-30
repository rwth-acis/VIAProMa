using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.Multiplayer.Common;
using Photon.Pun;

[RequireComponent(typeof(ImportedModel))]
public class ImportedModelSynchronizer : TransformSynchronizer
{
	private ImportedModel ImportedModel;

	private void Awake()
	{
		ImportedModel = GetComponent<ImportedModel>();
	}

    void Start()
    {
		ImportedModel.WebLink = (string)photonView.InstantiationData[0];
		ImportedModel.Owner = photonView.Owner.NickName;
    }
}
