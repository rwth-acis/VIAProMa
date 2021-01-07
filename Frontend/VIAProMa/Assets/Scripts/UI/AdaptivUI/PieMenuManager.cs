using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
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

    //Dictionary<IMixedRealityInputSource, VirtualTool> virtualTools;
    VirtualTool virtualTool;
    [SerializeField]
    GameObject virtualToolPrefab;

    void Start()
    {

    }


    void Update()
    {
    }



    public void MenuOpen(BaseInputEventData eventData)
    {
        //Check, if the Pie Menu was already open by another controller
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
            if (virtualTool != null)
            {
                //Needs to be invoked here and not in an actual OnDestroy, because otherwise the OnToolCreated Method of the new one can get executed before the OnToolDestroyed of the old one
                virtualTool.OnToolDestroyed.Invoke(null);
                Destroy(virtualTool);
            }

            virtualTool = Instantiate(virtualToolPrefab).GetComponent<VirtualTool>();

            virtualTool.inputSource = invokingSource;
            MenuEntry currentEntry = menuEntries[instantiatedPieMenu.GetComponent<PieMenuRenderer>().currentlyHighlighted];
            virtualTool.InputAction = currentEntry.InputAction;
            virtualTool.OnInputActionStarted = currentEntry.toolActionOnSelectStart;
            virtualTool.OnInputActionEnded = currentEntry.toolActionOnSelectEnd;
            virtualTool.OnToolCreated = currentEntry.toolActionOnToolCreated;
            virtualTool.OnToolDestroyed = currentEntry.toolActionOnToolDestroyed;

            Destroy(instantiatedPieMenu);
            invokingSource = null;
        }
    }
}
