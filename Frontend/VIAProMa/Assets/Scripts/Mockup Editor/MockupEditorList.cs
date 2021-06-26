using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// the scriptable object to hold all items
/// </summary>
[CreateAssetMenu(menuName = "Mockup Editor/List")]
public class MockupEditorList : ScriptableObject
{
    [Tooltip("The base Prefab which all the functionalities")]
    public GameObject PrefabBase;
    [Tooltip("list of mockup items")]
    public List<MockupEditorItem> items;
}
