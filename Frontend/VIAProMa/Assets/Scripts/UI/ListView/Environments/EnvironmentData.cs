using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentData : IListViewItemData
{
    public string EnvironmentName { get; private set; }
    public Sprite EnvironmentPreviewImage { get; private set; }
    public Material EnvironmentBackground { get; private set; }
    public GameObject EnvironmentPrefab { get; private set; }
    public string EnvironmentCredit { get; private set; }

    public EnvironmentData(string name, Sprite previewImage, Material background, GameObject prefab, string credit)
    {
        EnvironmentName = name;
        EnvironmentPreviewImage = previewImage;
        EnvironmentBackground = background;
        EnvironmentPrefab = prefab;
        EnvironmentCredit = credit;
    }
}
