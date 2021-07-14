using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockUpEditorWindow : MonoBehaviour
{
    public Transform spawnPlace;
    [SerializeField] List<InstantiateButton> buttons;
    [SerializeField] List<MockupEditorList> categories;

    /// <summary>
    /// Called whenever a new category is selected, called from Toggle Collection
    /// </summary>
    /// <param name="collection">the toggle collection that calls this function</param>
    public void OnCategorySelected(InteractableToggleCollection collection)
    {
        MockupEditorList currentList = categories[collection.CurrentIndex];
        for(int i = 0; i < buttons.Count; i++)
        {
            if(i < currentList.items.Count)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].list = currentList;
                buttons[i].UpdateButton();
            } else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// clears the spawn place before a new objects spawns on it
    /// </summary>
    public void ClearSpawnPlace()
    {
        GameObject[] mockupItems = GameObject.FindGameObjectsWithTag("MockupItem");
        for(int i = 0; i < mockupItems.Length; i++)
        {
            if(Vector3.Distance(spawnPlace.position, mockupItems[i].transform.position) < 0.06f)
            {
                Destroy(mockupItems[i]);
            }
        }
    }


}
