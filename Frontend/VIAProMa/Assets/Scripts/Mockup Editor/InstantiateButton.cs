using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;
using i5.VIAProMa.ResourceManagagement;

/// <summary>
/// a component for a button. Instantiate a mockup item from the list.
/// </summary>
public class InstantiateButton : MonoBehaviourPun
{
    public MockupEditorList list;
    [SerializeField] int index;
    [SerializeField] TMP_Text label;
    [SerializeField] SpriteRenderer spriteRenderer;
    const byte eventCode = 123;
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
        if(spriteRenderer != null) spriteRenderer.sprite = item.sprite;
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
        Transform spawn;
        MockUpEditorWindow mockupWindow;
        mockupWindow = GetComponentInParent<MockUpEditorWindow>();
        if(mockupWindow != null)
        {
            mockupWindow.ClearSpawnPlace();
            spawn = mockupWindow.spawnPlace;
        } else
        {
            spawn = transform;
        }

        if(PhotonNetwork.InRoom)
        {
            object[] data = new object[]
            {
                list.name,
                index
            };

            ResourceManager.Instance.NetworkInstantiate(list.PrefabBase, spawn.position, spawn.rotation, data);
        } else
        {
            //the base GO which is instantiated every time and "holds" the visual object inside, has the important components (e.g. ownership, network,...)
            GameObject baseGO = Instantiate(list.PrefabBase, spawn.position, spawn.rotation);
            baseGO.GetComponent<MockupEdiorGameObject>().SetData(list.name, index);

        
            //the skin GO which visualizes the object
            GameObject skinGO = Instantiate(item.Prefab, baseGO.transform);

            //if the skinGO or its children have an interactable component, it should be disabled so that it cannot be pressed
            Interactable[] interactables = skinGO.GetComponentsInChildren<Interactable>();
            if(interactables != null && interactables.Length > 0)
            {
                for(int i = 0; i < interactables.Length; i++)
                {
                    interactables[i].enabled = false;
                }
            }
        }
        
    }

}
