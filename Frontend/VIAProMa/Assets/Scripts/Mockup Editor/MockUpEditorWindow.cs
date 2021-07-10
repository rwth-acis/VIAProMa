using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockUpEditorWindow : MonoBehaviour
{
    public Transform spawnPlace;
    [SerializeField] List<InstantiateButton> buttons;
    [SerializeField] List<MockupEditorList> categories;

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
}
