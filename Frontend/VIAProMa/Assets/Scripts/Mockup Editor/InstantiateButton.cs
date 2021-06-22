using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.UI;

public class InstantiateButton : MonoBehaviour
{
    [SerializeField] MockupEditorList list;
    [SerializeField] int index;
    [SerializeField] ButtonConfigHelper buttonConfig;

    MockupEditorItem item;

    private void Start()
    {
        item = list.items[index];
        buttonConfig.MainLabelText = item.name;
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
