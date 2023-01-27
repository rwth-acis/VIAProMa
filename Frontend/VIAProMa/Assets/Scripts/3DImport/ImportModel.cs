#define MRTK_GLTF_IMPORTER_OFF

using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;
using static System.Net.WebRequestMethods;
using System.Text;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using ExitGames.Client.Photon.StructWrapping;
using i5.VIAProMa.UI;
using static SessionBrowserRefresher;
using TMPro;
using Photon.Pun;

public class ImportModel : MonoBehaviour
{
    public string url;

    public void LoadModel()
    {
		object[] InstantiationData = new object[] {(object)url};

		Vector3 position = transform.position - transform.forward * 0.1f;
		PhotonNetwork.Instantiate("ImportedModel", position, transform.rotation, 0, InstantiationData);
    }
}