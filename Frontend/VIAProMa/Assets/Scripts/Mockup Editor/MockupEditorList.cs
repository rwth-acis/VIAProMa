using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mockup Editor/List")]
public class MockupEditorList : ScriptableObject
{
    public GameObject PrefabBase;
    public List<MockupEditorItem> items;
}
