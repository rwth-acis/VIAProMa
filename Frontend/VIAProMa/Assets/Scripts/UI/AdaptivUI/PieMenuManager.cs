using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Input;
using HoloToolkit.Unity;



public class PieMenuManager : Singleton<PieMenuManager>
{
    [SerializeField]
    GameObject pieMenuPrefab;

    GameObject instantiatedPieMenu;


    IMixedRealityPointer pointer;
    IMixedRealityInputSource invokingSource;

    [SerializeField]
    public List<MenuEntry> menuEntries;

    [SerializeField]
    GameObject mainCamera;

    public void MenuOpen(BaseInputEventData eventData)
    {
        //Check, if the Pie Menu was already opend by another controller
        if (instantiatedPieMenu == null)
        {
            pointer = eventData.InputSource.Pointers[0];
            invokingSource = eventData.InputSource;
            instantiatedPieMenu = Instantiate(pieMenuPrefab, pointer.Position, Quaternion.identity);
            instantiatedPieMenu.GetComponent<PieMenuRenderer>().constructor(pointer,mainCamera);
            
        }
    }

    public void MenuClose(BaseInputEventData eventData)
    {
        //Only the input source that opend the menu can close it again
        if (eventData.InputSource == invokingSource && instantiatedPieMenu != null)
        {
            VirtualTool virtualTool = eventData.InputSource.Pointers[0].Controller.Visualizer.GameObjectProxy.GetComponentInChildren<VirtualTool>();
            MenuEntry currentEntry = menuEntries[instantiatedPieMenu.GetComponent<PieMenuRenderer>().currentlyHighlighted];
            virtualTool.SetupTool(currentEntry.toolActionOnSelectStart, currentEntry.toolActionOnSelectEnd, currentEntry.toolActionOnToolCreated,
                                  currentEntry.toolActionOnToolDestroyed, currentEntry.InputAction, eventData.InputSource, currentEntry.icon);
            Destroy(instantiatedPieMenu);
            invokingSource = null;
        }
    }
}
