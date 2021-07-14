using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MockupEdiorGameObject : MonoBehaviour
{
    public string category;
    public int index;

    public void SetData(string category, int index)
    {
        this.category = category;
        this.index = index;
    }

    public void SpawnChildObject()
    {
        if(transform.childCount == 1)
        {
            MockupEditorList list = (MockupEditorList)AssetDatabase.LoadAssetAtPath(category, typeof(MockupEditorList));
            Instantiate(list.items[index].Prefab, transform);            
        }
    }
}
