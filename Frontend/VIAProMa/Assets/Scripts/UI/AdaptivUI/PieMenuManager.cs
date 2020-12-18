using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class PieMenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject pieMenuPrefab;

    GameObject instantiatedPieMenu;
    IMixedRealityPointer pointer;
    IMixedRealityInputSource invokingSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (instantiatedPieMenu != null)
        {
            Debug.Log(pointer.Position);
        }
    }

    public void MenuOpen(BaseInputEventData eventData)
    {
        //Check, if the Pie Menu was already open by another controller
        if (instantiatedPieMenu == null)
        {
            pointer = eventData.InputSource.Pointers[0];
            invokingSource = eventData.InputSource;
            instantiatedPieMenu = Instantiate(pieMenuPrefab, pointer.Position, pointer.Rotation);
        }
    }

    public void MenuClose(BaseInputEventData eventData)
    {
        //Only the input source that opend the menu can close it again
        if (invokingSource == null || eventData.InputSource == invokingSource)
        {
            Destroy(instantiatedPieMenu);
            invokingSource = null;
        }
    }

    public void TestAct()
    { }
}
