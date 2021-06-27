using i5.Toolkit.Core.Utilities;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using SpecialDebugMessages = i5.VIAProMa.Utilities.SpecialDebugMessages;

[RequireComponent(typeof(ObjectManipulator))]
public class CubeToCreate : MonoBehaviour
{

    [SerializeField] private GameObject objectMenu;
    
    // Start is called before the first frame update
    void Start()
    {
        if (objectMenu == null) {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(objectMenu));
        }

    }

    // Update is called once per frame
    void Update() {


    }

    public void OpenObjectMenu() {
        objectMenu.SetActive(true);
        gameObject.GetComponent<PointerHandler>().enabled = false;
    }

    public void CloseObjectMenu() {
        objectMenu.GetComponent<ApplicationObjectMenu>().OnClose();
        objectMenu.SetActive(false);
        gameObject.GetComponent<PointerHandler>().enabled = true;
    }

}


