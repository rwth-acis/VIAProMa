using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using HoloToolkit.Unity;



public class PieMenuManager : Singleton<PieMenuManager>
{
    [SerializeField]
    GameObject pieMenuPrefab;
    [SerializeField]
    GameObject mainCamera;

    GameObject instantiatedPieMenu;
    IMixedRealityPointer pointer;
    IMixedRealityInputSource invokingSource;

    [SerializeField]
    public List<MenuEntry> menuEntries;

    void Start()
    {
        
    }


    void Update()
    {
        if (instantiatedPieMenu != null)
        {
            Debug.Log(pointer.Position);
            instantiatedPieMenu.transform.LookAt(mainCamera.transform);
        }
    }

    public void MenuOpen(BaseInputEventData eventData)
    {
        //Check, if the Pie Menu was already open by another controller
        if (instantiatedPieMenu == null)
        {
            pointer = eventData.InputSource.Pointers[0];
            invokingSource = eventData.InputSource;
            instantiatedPieMenu = Instantiate(pieMenuPrefab, pointer.Position, Quaternion.identity);
            instantiatedPieMenu.transform.LookAt(mainCamera.transform);
            menuEntries[0].toolAction.Invoke(null);
        }
    }

    public void MenuClose(BaseInputEventData eventData)
    {
        //Only the input source that opend the menu can close it again
        if ((invokingSource == null || eventData.InputSource == invokingSource) && instantiatedPieMenu != null)
        {
            Destroy(instantiatedPieMenu);
            invokingSource = null;
        }
    }

    public void TestAct()
    { }
}
