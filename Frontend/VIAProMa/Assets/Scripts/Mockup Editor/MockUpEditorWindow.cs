using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// the main script attached to the Mockup-Editor. Responsible for the button text-preview change and clears the spawn place
/// </summary>
public class MockUpEditorWindow : MonoBehaviour
{
    public Transform spawnPlace;
    [SerializeField] GameObject buttonsTextContent;
    [SerializeField] GameObject buttonsPreviewContent;
    [SerializeField] List<InstantiateButton> buttonsText;
    [SerializeField] List<InstantiateButton> buttonsWithPreview;
    [SerializeField] List<MockupEditorList> categories;
    [SerializeField] bool usePreviewButtons;
    [SerializeField] Sprite textToPreview;
    [SerializeField] Sprite previewToText;
    [SerializeField] SpriteRenderer changeShapeButton;
    MockupEditorList currentList;

    
    /// <summary>
    /// OnClick function to change the shape of the buttons
    /// </summary>
    public void ChangeButtonShape()
    {
        usePreviewButtons = !usePreviewButtons;
        UpdateView();
    }

    /// <summary>
    /// updates all buttons 
    /// </summary>
    void UpdateView()
    {
        //updates the change shape button, depending which mode is active
        changeShapeButton.sprite = usePreviewButtons ? previewToText : textToPreview;

        //choose the buttons that are relevant for this mode
        List<InstantiateButton> buttons = usePreviewButtons ? buttonsWithPreview : buttonsText;
        buttonsPreviewContent.SetActive(usePreviewButtons);
        buttonsTextContent.SetActive(!usePreviewButtons);
       
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < currentList.items.Count)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].list = currentList;
                buttons[i].UpdateButton();
            }
            else
            {
                //deactivate the button if it is out of range
                buttons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Called whenever a new category is selected, called from Toggle Collection
    /// </summary>
    /// <param name="collection">the toggle collection that calls this function</param>
    public void OnCategorySelected(InteractableToggleCollection collection)
    {
        currentList = categories[collection.CurrentIndex];
        UpdateView();
    }

    /// <summary>
    /// clears the spawn place before a new objects spawns on it
    /// </summary>
    public void ClearSpawnPlace()
    {
        GameObject[] mockupItems = GameObject.FindGameObjectsWithTag("MockupItem");
        for(int i = 0; i < mockupItems.Length; i++)
        {
            //clears the spawn place if it is too close to the spawn place
            if(Vector3.Distance(spawnPlace.position, mockupItems[i].transform.position) < 0.06f)
            {
                Destroy(mockupItems[i]);
            }
        }
    }


}
