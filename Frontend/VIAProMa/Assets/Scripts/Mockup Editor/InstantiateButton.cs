using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

public class InstantiateButton : MonoBehaviour
{
    [SerializeField] MockupEditorList list;
    [SerializeField] int index;
    [SerializeField] TMP_Text label;

    MockupEditorItem item;

    private void Start()
    {
        item = list.items[index];
        label.text = item.name;
    }

    public void OnClickButton()
    {
        Debug.Log("Instantiate Prefab");
        Spawn();
    }

    void Spawn()
    {
        GameObject baseGO = Instantiate(list.PrefabBase, gameObject.transform.position, Quaternion.identity);
        GameObject skinGO = Instantiate(item.Prefab, baseGO.transform);

        Interactable interactable;
        if (skinGO.TryGetComponent<Interactable>(out interactable)) interactable.enabled = false;
    }
}
