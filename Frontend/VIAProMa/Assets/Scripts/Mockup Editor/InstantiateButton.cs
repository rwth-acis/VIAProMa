using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEditor;

/// <summary>
/// a component for a button. Instantiate a mockup item from the list.
/// </summary>
public class InstantiateButton : MonoBehaviour
{
    public MockupEditorList list;
    [SerializeField] int index;
    [SerializeField] TMP_Text label;

    MockupEditorItem item;

    private void Start()
    {
        UpdateButton();
    }

    /// <summary>
    /// updates the button so that the label and the corresponding item is up to date
    /// </summary>
    public void UpdateButton()
    {
        item = list.items[index];
        label.text = item.name;
    }

    /// <summary>
    /// called by the OnClick method of the button
    /// </summary>
    public void OnClickButton()
    {
        Spawn();
    }

    /// <summary>
    /// spawns the gameobject
    /// </summary>
    void Spawn()
    {
        GetComponentInParent<MockUpEditorWindow>().ClearSpawnPlace();

        Vector3 spawnPosition = GetComponentInParent<MockUpEditorWindow>().spawnPlace.position;
        //the base GO which is instantiated every time and "holds" the visual object inside, has the important components (e.g. ownership, network,...)
        GameObject baseGO = Instantiate(list.PrefabBase, spawnPosition, Quaternion.identity);
        baseGO.GetComponent<MockupEdiorGameObject>().SetData(AssetDatabase.GetAssetPath(list), index);
        //the skin GO which visualizes the object
        GameObject skinGO = Instantiate(item.Prefab, baseGO.transform);

        //if the skinGO has an interactable component, it should be disabled so that it cannot be pressed
        Interactable interactable;
        if (skinGO.TryGetComponent<Interactable>(out interactable)) interactable.enabled = false;
    }
}
