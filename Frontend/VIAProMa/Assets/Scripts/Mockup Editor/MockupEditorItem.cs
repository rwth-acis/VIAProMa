using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// item class to save the settings for every mockup item
/// </summary>
[System.Serializable]
public class MockupEditorItem
{
    [Tooltip("Name that should be connected to the object")]
    public string name;
    [Tooltip("The Prefab of the item, mostly holds the visual parts")]
    public GameObject Prefab;
    [Tooltip("The Preview Image of the object")]
    public Sprite sprite;
}
