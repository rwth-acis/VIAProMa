using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Input;
using HoloToolkit.Unity;


/// <summary>
/// Manages the input events to open and close the pie menu
/// </summary>
public class PieMenuManager : Singleton<PieMenuManager>
{
    [SerializeField]
    GameObject pieMenuPrefab;

    GameObject instantiatedPieMenu;


    IMixedRealityPointer pointer;
    IMixedRealityInputSource invokingSource;

    [SerializeField]
    public List<MenuEntry> menuEntries;

    /// <summary>
    /// Opens the PieMenu and sets it at the tip of the tool
    /// </summary>
    /// <param name="eventData"></param> The data from the corresponding input event
    public void MenuOpen(BaseInputEventData eventData)
    {
        //Check, if the Pie Menu was already opend by another controller
        if (instantiatedPieMenu == null)
        {
            pointer = eventData.InputSource.Pointers[0];
            invokingSource = eventData.InputSource;
            instantiatedPieMenu = Instantiate(pieMenuPrefab, pointer.Position, Quaternion.identity);
            instantiatedPieMenu.GetComponent<PieMenuRenderer>().Constructor(pointer);
        }
    }

    /// <summary>
    /// Closes the PieMenu and sets the current menu entry of the tool that opend it to the currently selected entry
    /// </summary>
    /// <param name="eventData"></param> The data from the corresponding input event
    public void MenuClose(BaseInputEventData eventData)
    {
        //Only the input source that opend the menu can close it again
        if (eventData.InputSource == invokingSource && instantiatedPieMenu != null)
        {
            ViveWandVirtualTool virtualTool = eventData.InputSource.Pointers[0].Controller.Visualizer.GameObjectProxy.GetComponentInChildren<ViveWandVirtualTool>();
            MenuEntry currentEntry = menuEntries[instantiatedPieMenu.GetComponent<PieMenuRenderer>().currentlyHighlighted];
            virtualTool.SetupTool(currentEntry);
            Destroy(instantiatedPieMenu);
            invokingSource = null;
        }
    }
}
